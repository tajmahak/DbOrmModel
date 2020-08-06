using MyLibrary.DataBase;
using System;
using System.Data.Common;
using System.IO;
using System.Text;

namespace DbOrmModel
{
    internal class OrmModelProject
    {
        private const string pad = "    ";
        private readonly ProgramModel programModel;
        public OrmModelProjectInfo ProjectInfo { get; private set; }
        private DBProvider dbProvider;
        private readonly MetaManager meta;

        public OrmModelProject(ProgramModel programModel, OrmModelProjectInfo projectInfo)
        {
            this.programModel = programModel;
            ProjectInfo = projectInfo;

            if (string.IsNullOrEmpty(projectInfo.DBPath))
            {
                throw new Exception("Не указан путь к БД");
            }

            meta = new MetaManager();

            LoadDBProvider();

            LoadMetaData();

            //!!!
            meta.UseComments = true;
            meta.UseUserNames = true;
        }

        private string GetMetafilePath()
        {
            return ProjectInfo.DBPath + ".meta.txt";
        }

        private void LoadDBProvider()
        {
            string dbPath = Path.GetFullPath(ProjectInfo.DBPath);
            DbConnection dbConnection = null;
            try
            {
                dbConnection = programModel.OpenDataBaseConnection(dbPath);
                dbProvider = programModel.CreateDBProvider(dbConnection);
            }
            finally
            {
                if (dbConnection != null)
                {
                    dbConnection.Close();
                    dbConnection.Dispose();
                }
            }



            //FireBirdProvider provider = new FireBirdProvider();
            //provider.Initialize(connection);
            //return new OrmModelTextBuilder(provider);
        }

        private void LoadMetaData()
        {
            string mfPath = GetMetafilePath();
            string[] content = null;
            if (File.Exists(mfPath))
            {
                content = File.ReadAllLines(mfPath, Encoding.UTF8);
            }
            meta.LoadInfo(dbProvider, content);
        }



        public string GetMainDBNamespace(int level)
        {
            StringBuilder str = new StringBuilder();

            str.AppendLine(GetUsingDeclaration(level));
            str.AppendLine();

            str.AddLine(level, "namespace " + GetNamespaceName());
            str.AddLine(level, "{");
            str.AppendLine(GetMainDBClass(level + 1));
            str.AddLine(level, "}");
            str.RemoveLastLine();

            return str.ToString();
        }

        public string GetUsingDeclaration(int level)
        {
            StringBuilder str = new StringBuilder();
            str.AddLine(level, "using MyLibrary.DataBase;");
            str.AddLine(level, "using System;");
            str.AddLine(level, "using System.Data.Common;");
            str.RemoveLastLine();
            return str.ToString();
        }

        public string GetMainDBClass(int level)
        {
            StringBuilder str = new StringBuilder();
            str.AddLine(level, "public class DB : DBContext");
            str.AddLine(level, "{");

            foreach (DBTable table in dbProvider.Tables)
            {
                string tableComment = GetTableComment(table);
                string originalTableName = GetOriginalTableName(table);
                string customTableName = GetCustomTableName(table);
                string customTableListName = GetCustomTableListName(table);

                str.AddLine(level + 1, "#region " + customTableName);
                str.AddLine();

                str.AddComment(level + 1, tableComment);
                str.AddLine(level + 1, $"public const string {customTableName}TableName = \"{originalTableName}\";");
                str.AddLine();

                str.AddComment(level + 1, tableComment);
                str.AddLine(level + 1, $"public DBQuery<{customTableName}Row> {customTableListName} => Select<{customTableName}Row>();");
                str.AddLine();

                str.AppendLine(GetStaticTableClass(level + 1, table));

                str.AppendLine(GetTableRowClass(level + 1, table));

                str.AddLine();
                str.AddLine(level + 1, "#endregion");
                str.AddLine();
            }

            str.AddLine(level + 1, @"public DB(DBProvider provider, DbConnection connection) : base(provider, connection)");
            str.AddLine(level + 1, "{");
            str.AddLine(level + 1, "}");

            str.AddLine(level, "}");

            str.RemoveLastLine();
            return str.ToString();
        }

        public string GetStaticTableClass(int level, DBTable table)
        {
            StringBuilder str = new StringBuilder();

            string originalTableName = GetOriginalTableName(table);
            string customTableName = GetCustomTableName(table);

            str.AddLine(level, $"internal static class {customTableName}");
            str.AddLine(level, "{");

            foreach (DBColumn column in table.Columns)
            {
                string originalColumnName = GetOriginalColumnName(column);
                string customColumnName = GetCustomColumnName(column);

                AddColumnComment(level + 1, str, column, true);
                str.AddLine(level + 1, $"public const string {customColumnName} = \"{originalTableName}.{originalColumnName}\";");
                str.AddLine();
            }

            str.RemoveLastLine();
            str.AddLine(level, "}");

            return str.ToString();
        }

        public string GetTableRowClass(int level, DBTable table)
        {
            StringBuilder str = new StringBuilder();

            string tableComment = GetTableComment(table);
            string originalTableName = GetOriginalTableName(table);
            string customTableName = GetCustomTableName(table);

            str.AddComment(level, tableComment);
            str.AddLine(level, $"[DBOrmTable({customTableName}TableName)]");
            str.AddLine(level, $"public class {customTableName}Row : DBOrmRow<{customTableName}Row>");
            str.AddLine(level, "{");

            foreach (DBColumn column in table.Columns)
            {
                string originalColumnName = GetOriginalColumnName(column);
                string customColumnName = GetCustomColumnName(column);
                string columnTypeName = GetColumnTypeName(column);

                string notNullAttribute = "";
                string primaryKeyAttribute = "";
                string foreignKeyAttribute = "";

                if (column.NotNull)
                {
                    notNullAttribute = ", NotNull: true";
                }

                if (column.IsPrimary)
                {
                    primaryKeyAttribute = ", PrimaryKey: true";
                }

                if (meta.ContainsForeignKey(originalTableName + "." + originalColumnName))
                {
                    string foreignKey = meta.GetForeignKeyInfo(originalTableName + "." + originalColumnName);
                    string[] split = foreignKey.Split('.');

                    if (meta.ContainsUserName(split[0] + "." + split[1]))
                    {
                        split[1] = meta.GetUserName(split[0] + "." + split[1]);
                    }
                    if (meta.ContainsUserName(split[0]))
                    {
                        split[0] = meta.GetUserName(split[0]);
                    }

                    foreignKeyAttribute = $", ForeignKey: {split[0]}.{split[1]}";
                }


                str.AddLine(level + 1, $"#region {customColumnName}");
                str.AddLine();

                AddColumnComment(level + 1, str, column, false);
                str.AddLine(level + 1, $"[DBOrmColumn({customTableName}.{customColumnName}{notNullAttribute}{primaryKeyAttribute}{foreignKeyAttribute})]");
                str.AddProperty(level + 1, $"public {columnTypeName} {customColumnName}",
                    $"Row.GetValue<{columnTypeName}>({customTableName}.{customColumnName});",
                    $"Row[{customTableName}.{customColumnName}] = value;");

                str.AddLine();
                AddColumnComment(level + 1, str, column, true);
                str.AddLine(level + 1, $"public void Set{customColumnName}(object value)");
                str.AddLine(level + 1, "{");
                str.AddLine(level + 2, $"Row[{customTableName}.{customColumnName}] = value;");
                str.AddLine(level + 1, "}");

                str.AddLine();
                str.AddLine(level + 1, "#endregion");
                str.AddLine();
            }

            str.AddLine(level + 1, $"public {customTableName}Row(DBRow row) : base(row) {{ }}");

            str.AddLine(level, "}");
            str.RemoveLastLine();

            return str.ToString();
        }



        private string GetNamespaceName()
        {
            string namespaceName = string.IsNullOrEmpty(ProjectInfo.NameSpace) ? "UnknownNamespace" : ProjectInfo.NameSpace;
            return namespaceName;
        }

        private string GetOriginalTableName(DBTable table)
        {
            string originalTableName = table.Name;
            return originalTableName;
        }

        private string GetCustomTableName(DBTable table)
        {
            string originalTableName = GetOriginalTableName(table);
            string customTableName = meta.ContainsUserName(originalTableName) ? meta.GetUserName(originalTableName) : originalTableName;
            return customTableName;
        }

        private string GetTableComment(DBTable table)
        {
            string originalTableName = GetOriginalTableName(table);
            string tableComment = meta.ContainsComment(originalTableName) ? meta.GetComment(originalTableName) : string.Empty;
            return tableComment;
        }

        private string GetCustomTableListName(DBTable table)
        {
            string originalTableName = GetOriginalTableName(table);
            string customTableListName;
            if (meta.ContainsUserName_TableList(originalTableName))
            {
                customTableListName = meta.GetUserName_TableList(originalTableName);
            }
            else
            {
                string customTableName = GetCustomTableName(table);
                customTableListName = customTableName + "s";
            }
            return customTableListName;
        }

        private string GetOriginalColumnName(DBColumn column)
        {
            return column.Name;
        }

        private string GetCustomColumnName(DBColumn column)
        {
            string originalColumnName = GetOriginalColumnName(column);
            string originalTableName = GetOriginalTableName(column.Table);
            string customColumnName = meta.ContainsUserName(originalTableName + "." + originalColumnName) ? meta.GetUserName(originalTableName + "." + originalColumnName) : originalColumnName;
            return customColumnName;
        }



        private void AddColumnComment(int level, StringBuilder str, DBColumn column, bool insertTypeName)
        {
            if (meta.UseComments)
            {
                string columnName = column.Table.Name + "." + column.Name;

                string comment = string.Empty;
                if (meta.ContainsComment(columnName))
                {
                    comment += meta.GetComment(columnName);
                }
                if (insertTypeName)
                {
                    comment += $" [{GetCommentObjectType(column)}]";
                }
                if (comment.Length > 0)
                {
                    str.AddComment(level, comment);
                }
            }
        }

        private string GetCommentObjectType(DBColumn column)
        {
            string typeName = GetColumnTypeName(column);
            if (!column.NotNull && column.DataType.IsClass)
            {
                typeName += "?";
            }
            return typeName;
        }

        private string GetColumnTypeName(DBColumn column)
        {
            System.Type type = column.DataType;

            string typeName;
            if (meta.ContainsDataType(column.Table.Name + "." + column.Name))
            {
                typeName = meta.GetDataType(column.Table.Name + "." + column.Name);
            }

            else if (type == typeof(bool))
            {
                typeName = "bool";
            }
            else if (type == typeof(byte))
            {
                typeName = "byte";
            }
            else if (type == typeof(char))
            {
                typeName = "char";
            }
            else if (type == typeof(decimal))
            {
                typeName = "decimal";
            }
            else if (type == typeof(double))
            {
                typeName = "double";
            }
            else if (type == typeof(float))
            {
                typeName = "float";
            }
            else if (type == typeof(int))
            {
                typeName = "int";
            }
            else if (type == typeof(long))
            {
                typeName = "long";
            }
            else if (type == typeof(sbyte))
            {
                typeName = "sbyte";
            }
            else if (type == typeof(short))
            {
                typeName = "short";
            }
            else if (type == typeof(string))
            {
                typeName = "string";
            }
            else if (type == typeof(uint))
            {
                typeName = "uint";
            }
            else if (type == typeof(ulong))
            {
                typeName = "ulong";
            }
            else if (type == typeof(ushort))
            {
                typeName = "ushort";
            }
            else
            {
                typeName = type.Name;
            }

            if (!column.NotNull && !column.DataType.IsClass)
            {
                typeName += "?";
            }
            return typeName;
        }
    }
}
