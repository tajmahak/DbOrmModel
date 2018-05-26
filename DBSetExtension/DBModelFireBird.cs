using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using FirebirdSql.Data.FirebirdClient;

namespace DBSetExtension
{
    public class DBModelFireBird : DBModelBase
    {
        public override void Initialize(DbConnection connection)
        {
            InitializeDBModel((FbConnection)connection);
            InitializeDefaultCommands();
            base.IsInitialized = true;
        }
        public override object ExecuteInsertCommand(DbCommand command)
        {
            return command.ExecuteScalar();
        }
        public override void AddParameter(DbCommand command, string name, object value)
        {
            ((FbCommand)command).Parameters.AddWithValue(name, value);
        }
        public override DbCommand BuildCommand(DbConnection connection, DBCommand command)
        {
            var cmd = (FbCommand)connection.CreateCommand();
            int paramCounter = 0;
            cmd.CommandText = BuildCommandInternal(cmd, command, ref paramCounter);
            return cmd;
        }

        private void InitializeDBModel(FbConnection connection)
        {
            List<string> tableNames = new List<string>();
            #region Получение названий таблиц

            var dataTables = connection.GetSchema("Tables");
            foreach (DataRow table in dataTables.Rows)
                if ((short)table["IS_SYSTEM_TABLE"] == 0)
                    tableNames.Add((string)table["TABLE_NAME"]);
            dataTables.Clear();
            dataTables.Dispose();

            #endregion

            DataSet ds = new DataSet();
            #region Подготовка ДатаСета
            foreach (var tableName in tableNames)
            {
                FbDataAdapter dataAdapter = new FbDataAdapter(string.Format("SELECT FIRST 0 * FROM \"{0}\"", tableName), connection);
                dataAdapter.Fill(ds, 0, 0, tableName);
                dataAdapter.Dispose();
            }
            #endregion

            DBTable[] tables = new DBTable[tableNames.Count];
            #region Добавление информации для таблиц

            var columnsInfo = connection.GetSchema("Columns");
            var primaryKeysInfo = connection.GetSchema("PrimaryKeys");
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                DataTable dataTable = ds.Tables[i];
                DBTable table = new DBTable(this, dataTable.TableName);
                DBColumn[] columns = new DBColumn[dataTable.Columns.Count];
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    DataColumn dataColumn = dataTable.Columns[j];
                    DBColumn column = new DBColumn(table);
                    #region Добавление информации для столбцов

                    var columnInfo = columnsInfo.Select("TABLE_NAME = '" + dataTable.TableName + "' AND COLUMN_NAME = '" + dataColumn.ColumnName + "'")[0];
                    var primaryKeyInfo = primaryKeysInfo.Select("TABLE_NAME = '" + dataTable.TableName + "' AND COLUMN_NAME = '" + dataColumn.ColumnName + "'");

                    column.Name = dataColumn.ColumnName;
                    column.DataType = dataColumn.DataType;
                    column.AllowDBNull = (bool)columnInfo["IS_NULLABLE"];

                    var columnDescription = columnInfo["DESCRIPTION"];
                    if (columnDescription != DBNull.Value)
                        column.Comment = (string)columnDescription;

                    if (primaryKeyInfo.Length > 0)
                        column.IsPrimary = true;

                    var defaultValue = columnInfo["COLUMN_DEFAULT"].ToString();
                    if (defaultValue.Length > 0)
                    {
                        defaultValue = defaultValue.Remove(0, 8);
                        column.DefaultValue = Convert.ChangeType(defaultValue, column.DataType);
                    }
                    else column.DefaultValue = DBNull.Value;

                    if (column.DataType == typeof(string))
                        column.MaxTextLength = (int)columnInfo["COLUMN_SIZE"];

                    #endregion
                    columns[j] = column;

                    columnsInfo.Dispose();
                    primaryKeysInfo.Dispose();
                }
                table.AddColumns(columns);
                tables[i] = table;
            }

            #endregion

            ds.Dispose();

            #region Подготовка значений

            base.Tables = tables;
            for (int i = 0; i < tables.Length; i++)
            {
                var table = tables[i];
                base.TablesDict.Add(table.Name, table);

                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];
                    string longName = string.Concat(table.Name, '.', column.Name);
                    base.ColumnsDict.Add(longName, column);
                }
            }

            #endregion
        }
        private void InitializeDefaultCommands()
        {
            #region SELECT
            for (int i = 0; i < Tables.Length; i++)
            {
                var table = Tables[i];
                var str = new StringBuilder();
                str.Append("SELECT ");
                str.Append(GetName(table.Name));
                str.Append(".* FROM ");
                str.Append(GetName(table.Name));

                DefaultSelectCommandsDict.Add(table, str.ToString());
            }
            #endregion
            #region INSERT
            for (int i = 0; i < Tables.Length; i++)
            {
                var table = Tables[i];
                var str = new StringBuilder();
                str.Append("INSERT INTO ");
                str.Append(GetName(table.Name));
                str.Append(" VALUES(");

                int index = 0;
                for (int j = 0; j < table.Columns.Length; j++)
                {
                    if (j > 0)
                        str.Append(',');

                    if (table.Columns[j].IsPrimary)
                        str.Append("NULL");
                    else
                    {
                        str.Append(String.Concat("@p", index));
                        index++;
                    }
                }

                str.Append(") RETURNING ");
                str.Append(GetName(table.Columns[table.PrimaryKeyIndex].Name));

                DefaultInsertCommandsDict.Add(table, str.ToString());
            }
            #endregion
            #region UPDATE
            for (int i = 0; i < Tables.Length; i++)
            {
                var table = Tables[i];
                var str = new StringBuilder();

                str.Append("UPDATE ");
                str.Append(GetName(table.Name));
                str.Append(" SET ");
                int index = 0;
                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];
                    if (column.IsPrimary)
                        continue;

                    if (index != 0)
                        str.Append(",");

                    str.Append(GetName(column.Name));
                    str.Append("=@p");
                    str.Append(index++);
                }
                str.Append(" WHERE ");
                str.Append(GetName(table.Columns[table.PrimaryKeyIndex].Name));
                str.Append("=@id");
                DefaultUpdateCommandsDict.Add(table, str.ToString());
            }
            #endregion
            #region DELETE
            for (int i = 0; i < Tables.Length; i++)
            {
                var table = Tables[i];
                var str = new StringBuilder();

                str.Append("DELETE FROM ");
                str.Append(GetName(table.Name));
                str.Append(" WHERE ");
                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];
                    if (column.IsPrimary)
                    {
                        str.Append(GetName(column.Name));
                        break;
                    }
                }
                str.Append("=@id");

                DefaultDeleteCommandsDict.Add(table, str.ToString());
            }
            #endregion
        }
        private string BuildCommandInternal(FbCommand cmd, DBCommand command, ref int paramCounter)
        {
            var str = new StringBuilder();

            List<object[]> blockList;
            object[] block;
            string[] paramCols;
            int index = 0;

            if (command.CommandType == DBCommand.DBCommandTypeEnum.Select)
            {
                #region SELECT [...] ... FROM ...

                blockList = FindBlockList(command, x => x.StartsWith("Select"));
                if (blockList.Count == 0)
                {
                    str.Append(DefaultSelectCommandsDict[command.Table]);
                }
                else
                {
                    str.Append("SELECT ");
                    #region

                    index = 0;
                    for (int i = 0; i < blockList.Count; i++)
                    {
                        block = blockList[i];
                        switch ((string)block[0])
                        {
                            case "Select":
                                #region
                                paramCols = (string[])block[1];
                                if (paramCols.Length == 0)
                                {
                                    if (index > 0)
                                        str.Append(',');
                                    str.Append('*');
                                    index++;
                                }
                                else
                                {
                                    for (int j = 0; j < paramCols.Length; j++)
                                    {
                                        if (index > 0)
                                            str.Append(',');

                                        var paramCol = (string)paramCols[j];
                                        if (paramCol.Contains("."))
                                        {
                                            // Столбец
                                            str.Append(GetFullName(paramCol));
                                        }
                                        else
                                        {
                                            // Таблица
                                            str.Append(GetName(paramCol));
                                            str.Append(".*");
                                        }
                                        index++;
                                    }
                                }
                                break;
                                #endregion
                            case "SelectAs":
                                #region
                                if (index > 0)
                                    str.Append(',');
                                str.Append(GetName(block[1]));
                                str.Append(".");
                                str.Append(GetColumnName(block[2]));
                                index++;
                                break;
                                #endregion
                            case "SelectSum":
                                #region
                                paramCols = (string[])block[1];
                                for (int j = 0; j < paramCols.Length; j++)
                                {
                                    if (index > 0)
                                        str.Append(',');
                                    str.Append("SUM(");
                                    str.Append(GetFullName(paramCols[j]));
                                    str.Append(')');
                                    index++;
                                }
                                break;
                                #endregion
                            case "SelectSumAs":
                                #region
                                paramCols = (string[])block[1];
                                for (int j = 0; j < paramCols.Length; j += 2)
                                {
                                    if (index > 0)
                                        str.Append(',');
                                    str.Append("SUM(");
                                    str.Append(GetFullName(paramCols[j]));
                                    str.Append(") AS ");
                                    str.Append(GetName(paramCols[j + 1]));
                                    index++;
                                }
                                break;
                                #endregion
                            case "SelectCount":
                                #region
                                paramCols = (string[])block[1];
                                if (paramCols.Length == 0)
                                {
                                    if (index > 0)
                                        str.Append(',');
                                    str.Append("COUNT(*)");
                                    index++;
                                }
                                else
                                {
                                    for (int j = 0; j < paramCols.Length; j++)
                                    {
                                        if (index > 0)
                                            str.Append(',');
                                        str.Append("COUNT(");
                                        str.Append(GetFullName(paramCols[j]));
                                        str.Append(')');
                                        index++;
                                    }
                                }
                                break;
                                #endregion
                        }
                    }

                    #endregion
                    str.Append(" FROM ");
                    str.Append(GetName(command.Table.Name));
                }

                block = FindBlock(command, x => x == "Distinct");
                if (block != null)
                    str.Insert(6, " DISTINCT");

                block = FindBlock(command, x => x == "Skip");
                if (block != null)
                    str.Insert(6, string.Concat(" SKIP ", block[1]));

                block = FindBlock(command, x => x == "First");
                if (block != null)
                    str.Insert(6, string.Concat(" FIRST ", block[1]));

                #endregion
                #region JOIN

                foreach (var item in command.Stack)
                {
                    switch ((string)item[0])
                    {
                        case "InnerJoin":
                            #region
                            str.Append(" INNER JOIN ");
                            str.Append(GetName(item[1]));
                            str.Append(" ON ");
                            str.Append(GetFullName(item[1]));
                            str.Append('=');
                            str.Append(GetFullName(item[2]));
                            break;
                            #endregion
                        case "LeftOuterJoin":
                            #region
                            str.Append(" LEFT OUTER JOIN ");
                            str.Append(GetName(item[1]));
                            str.Append(" ON ");
                            str.Append(GetFullName(item[1]));
                            str.Append('=');
                            str.Append(GetFullName(item[2]));
                            break;
                            #endregion
                        case "RightOuterJoin":
                            #region
                            str.Append(" RIGHT OUTER JOIN ");
                            str.Append(GetName(item[1]));
                            str.Append(" ON ");
                            str.Append(GetFullName(item[1]));
                            str.Append('=');
                            str.Append(GetFullName(item[2]));
                            break;
                            #endregion
                        case "FullOuterJoin":
                            #region
                            str.Append(" FULL OUTER JOIN ");
                            str.Append(GetName(item[1]));
                            str.Append(" ON ");
                            str.Append(GetFullName(item[1]));
                            str.Append('=');
                            str.Append(GetFullName(item[2]));
                            break;
                            #endregion
                        case "InnerJoinAs":
                            #region
                            str.Append(" INNER JOIN ");
                            str.Append(GetName(item[2]));
                            str.Append(" AS ");
                            str.Append(GetName(item[1]));
                            str.Append(" ON ");
                            str.Append(GetName(item[1]));
                            str.Append(".");
                            str.Append(GetColumnName(item[2]));
                            str.Append('=');
                            str.Append(GetFullName(item[3]));
                            break;
                            #endregion
                        case "LeftOuterJoinAs":
                            #region
                            str.Append(" LEFT OUTER JOIN ");
                            str.Append(GetName(item[2]));
                            str.Append(" AS ");
                            str.Append(GetName(item[1]));
                            str.Append(" ON ");
                            str.Append(GetName(item[1]));
                            str.Append(".");
                            str.Append(GetColumnName(item[2]));
                            str.Append('=');
                            str.Append(GetFullName(item[3]));
                            break;
                            #endregion
                        case "RightOuterJoinAs":
                            #region
                            str.Append(" RIGHT OUTER JOIN ");
                            str.Append(GetName(item[2]));
                            str.Append(" AS ");
                            str.Append(GetName(item[1]));
                            str.Append(" ON ");
                            str.Append(GetName(item[1]));
                            str.Append(".");
                            str.Append(GetColumnName(item[2]));
                            str.Append('=');
                            str.Append(GetFullName(item[3]));
                            break;
                            #endregion
                        case "FullOuterJoinAs":
                            #region
                            str.Append(" FULL OUTER JOIN ");
                            str.Append(GetName(item[2]));
                            str.Append(" AS ");
                            str.Append(GetName(item[1]));
                            str.Append(" ON ");
                            str.Append(GetName(item[1]));
                            str.Append(".");
                            str.Append(GetColumnName(item[2]));
                            str.Append('=');
                            str.Append(GetFullName(item[3]));
                            break;
                            #endregion
                    }
                }

                #endregion
            }
            else if (command.CommandType == DBCommand.DBCommandTypeEnum.Update)
            {
                #region UPDATE ... SET ...

                str.Append("UPDATE ");
                str.Append(GetName(command.Table.Name));
                str.Append(" SET ");

                blockList = command.Stack.FindAll(x => ((string)x[0]) == "UpdateSet");
                if (blockList.Count == 0)
                    throw DBSetException.InadequateUpdateCommand();
                for (int i = 0; i < blockList.Count; i++)
                {
                    if (i > 0)
                        str.Append(',');
                    str.Append(GetFullName(blockList[i][1]));
                    str.Append('=');
                    str.Append(AddParam(cmd, paramCounter++, blockList[i][2]));
                }

                #endregion
            }
            else if (command.CommandType == DBCommand.DBCommandTypeEnum.Delete)
            {
                #region DELETE FROM ...

                str.Append("DELETE FROM ");
                str.Append(GetName(command.Table.Name));

                #endregion
            }

            #region WHERE ...

            blockList = FindBlockList(command, x => x.Contains("Where") || x == "Or");
            bool needPastePredicate = false;
            if (blockList.Count > 0)
            {
                str.Append(" WHERE");
                for (int i = 0; i < blockList.Count; i++)
                {
                    block = blockList[i];
                    var blockName = (string)block[0];

                    #region AND/OR
                    if (needPastePredicate)
                    {
                        if (blockName == "Or")
                        {
                            str.Append(" OR");
                            needPastePredicate = false;
                            continue;
                        }
                        else
                        {
                            str.Append(" AND");
                        }
                    }
                    #endregion
                    switch (blockName)
                    {
                        case "Where":
                            #region
                            block[3] = block[3] ?? DBNull.Value;

                            str.Append(' ');
                            str.Append(GetFullName(block[1]));

                            if ((block[3] is DBNull) && ((string)block[2]) == "=")
                                str.Append(" IS NULL");
                            else if (block[3] is DBNull && ((string)block[2]) == "<>")
                                str.Append(" IS NOT NULL");
                            else
                            {
                                str.Append(block[2]); // оператор сравнения
                                str.Append(AddParam(cmd, paramCounter++, block[3]));
                            }
                            break;
                            #endregion
                        case "WhereBetween":
                            #region
                            str.Append(' ');
                            str.Append(GetFullName(block[1]));
                            str.Append(" BETWEEN ");
                            str.Append(AddParam(cmd, paramCounter++, block[2]));
                            str.Append(" AND ");
                            str.Append(AddParam(cmd, paramCounter++, block[3]));
                            break;
                            #endregion
                        case "WhereUpper":
                            #region
                            str.Append(" UPPER(");
                            str.Append(GetFullName(block[1]));
                            str.Append(")=");
                            str.Append(AddParam(cmd, paramCounter++, block[2]));
                            break;
                            #endregion
                        case "WhereContaining":
                            #region
                            str.Append(' ');
                            str.Append(GetFullName(block[1]));
                            str.Append(" CONTAINING ");
                            str.Append(AddParam(cmd, paramCounter++, block[2]));
                            break;
                            #endregion
                        case "WhereContainingUpper":
                            #region
                            str.Append(" UPPER(");
                            str.Append(GetFullName(block[1]));
                            str.Append(") CONTAINING ");
                            str.Append(AddParam(cmd, paramCounter++, block[2]));
                            break;
                            #endregion
                        case "WhereLike":
                            #region
                            str.Append(' ');
                            str.Append(GetFullName(block[1]));
                            str.Append(" LIKE \'");
                            str.Append(block[2]);
                            str.Append('\'');
                            break;
                            #endregion
                        case "WhereLikeUpper":
                            #region
                            str.Append(" UPPER(");
                            str.Append(GetFullName(block[1]));
                            str.Append(") LIKE '");
                            str.Append(block[2]);
                            str.Append('\'');
                            break;
                            #endregion
                        case "WhereIn_command":
                            #region
                            str.Append(' ');
                            str.Append(GetFullName(block[1]));
                            str.Append(" IN (");
                            str.Append(BuildCommandInternal(cmd, (DBCommand)block[2], ref paramCounter));
                            str.Append(')');
                            break;
                            #endregion
                        case "WhereIn_values":
                            #region
                            str.Append(' ');
                            str.Append(GetFullName(block[1]));
                            str.Append(" IN (");
                            #region Добавление списка значений

                            var values = (object[])block[2];
                            for (int j = 0; j < values.Length; j++)
                            {
                                if (j > 0)
                                    str.Append(',');

                                var value = values[j];
                                if (value.GetType().IsPrimitive)
                                {
                                    str.Append(value);
                                }
                                else
                                {
                                    throw new NotImplementedException();
                                }
                            }

                            #endregion
                            str.Append(')');
                            break;
                            #endregion
                    }
                    needPastePredicate = true;
                }
            }

            #endregion
            if (command.CommandType == DBCommand.DBCommandTypeEnum.Select)
            {
                #region GROUP BY ...

                blockList = FindBlockList(command, x => x == "GroupBy");
                if (blockList.Count > 0)
                {
                    str.Append(" GROUP BY ");
                    index = 0;
                    for (int i = 0; i < blockList.Count; i++)
                    {
                        block = blockList[i];
                        paramCols = (string[])block[1];
                        for (int j = 0; j < paramCols.Length; j++)
                        {
                            if (index > 0)
                                str.Append(',');
                            str.Append(GetFullName(paramCols[j]));
                            index++;
                        }
                    }
                }

                #endregion
                #region ORDER BY ...

                blockList = FindBlockList(command, x => x.StartsWith("OrderBy"));
                if (blockList.Count > 0)
                {
                    str.Append(" ORDER BY ");
                    index = 0;
                    for (int i = 0; i < blockList.Count; i++)
                    {
                        block = blockList[i];
                        paramCols = (string[])block[1];
                        switch ((string)block[0])
                        {
                            case "OrderBy":
                                #region
                                for (int j = 0; j < paramCols.Length; j++)
                                {
                                    if (index > 0)
                                        str.Append(',');
                                    str.Append(GetFullName(paramCols[j]));
                                    index++;
                                }
                                break;
                                #endregion
                            case "OrderByDesc":
                                #region
                                for (int j = 0; j < paramCols.Length; j++)
                                {
                                    if (index > 0)
                                        str.Append(',');
                                    str.Append(GetFullName(paramCols[j]));
                                    str.Append(" DESC");
                                    index++;
                                }
                                break;
                                #endregion
                            case "OrderByUpper":
                                #region
                                for (int j = 0; j < paramCols.Length; j++)
                                {
                                    if (index > 0)
                                        str.Append(',');
                                    str.Append("UPPER(");
                                    str.Append(GetFullName(paramCols[j]));
                                    str.Append(")");
                                    index++;
                                }
                                break;
                                #endregion
                            case "OrderByUpperDesc":
                                #region
                                for (int j = 0; j < paramCols.Length; j++)
                                {
                                    if (index > 0)
                                        str.Append(',');
                                    str.Append("UPPER(");
                                    str.Append(GetFullName(block[1]));
                                    str.Append(") DESC");
                                    index++;
                                }
                                break;
                                #endregion
                        }
                    }
                }

                #endregion
            }

            return str.ToString();
        }
        private string AddParam(FbCommand command, int counter, object value)
        {
            value = value ?? DBNull.Value;

            if (value is string && ColumnsDict.ContainsKey((string)value))
                return GetFullName((string)value);

            var type = value.GetType();
            if (type.BaseType == typeof(Enum))
                value = Convert.ChangeType(value, Enum.GetUnderlyingType(type));

            var paramName = string.Concat("@p", counter);
            AddParameter(command, paramName, value);
            return paramName;
        }
        private string GetFullName(object value)
        {
            string[] split = ((string)value).Split('.');
            return string.Concat('\"', split[0], "\".\"", split[1], '\"');
        }
        private string GetName(object value)
        {
            string[] split = ((string)value).Split('.');
            return string.Concat('\"', split[0], '\"');
        }
        private string GetColumnName(object value)
        {
            string[] split = ((string)value).Split('.');
            return string.Concat('\"', split[1], '\"');
        }
        private List<object[]> FindBlockList(DBCommand command, Predicate<string> predicate)
        {
            return command.Stack.FindAll(block =>
                predicate((string)block[0]));
        }
        private object[] FindBlock(DBCommand command, Predicate<string> predicate)
        {
            return command.Stack.Find(block =>
                predicate((string)block[0]));
        }
    }
}
