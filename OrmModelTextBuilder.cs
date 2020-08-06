using MyLibrary.DataBase;
using System.Text;

namespace DbOrmModel
{
    internal class OrmModelTextBuilder
    {
        public DBProvider Provider { get; private set; }

        public OrmModelTextBuilder(DBProvider provider)
        {
            Provider = provider;
        }
        public string CreateText(MetaManager meta)
        {
            StringBuilder str = new StringBuilder();
            str.AddLine(0, "using MyLibrary.DataBase;");
            str.AddLine(0, "using System;");
            str.AddLine(0, "using System.Data.Common;");
            str.AddLine();
            str.AddLine(0, $"public class DB : DBContext");
            str.AddLine(0, "{");
            #region
            foreach (DBTable table in Provider.Tables)
            {
                string originalTableName = table.Name;
                string customTableName = meta.ContainsUserName(originalTableName) ? meta.GetUserName(originalTableName) : originalTableName;
                string tableComment = meta.ContainsComment(originalTableName) ? meta.GetComment(originalTableName) : string.Empty;
                string customTableListName = meta.ContainsUserName_TableList(originalTableName) ? meta.GetUserName_TableList(originalTableName) : customTableName + "s";

                str.AddLine(1, $"#region {customTableName}");
                #region
                str.AddLine();

                if (meta.UseComments)
                {
                    str.AddComment(1, tableComment);
                }
                str.AddLine(1, $"public const string {customTableName}TableName = \"{originalTableName}\";");
                str.AddLine();


                if (meta.UseComments)
                {
                    str.AddComment(1, tableComment);
                }
                str.AddLine(1, $"public DBQuery<{customTableName}Row> {customTableListName} => Select<{customTableName}Row>();");
                str.AddLine();

                if (meta.UseComments)
                {
                    str.AddComment(1, tableComment);
                }
                str.AddLine(1, $"internal static class {customTableName}");
                str.AddLine(1, "{");
                #region

                foreach (DBColumn column in table.Columns)
                {
                    string originalColumnName = column.Name;
                    string customColumnName = meta.ContainsUserName(originalTableName + "." + originalColumnName) ? meta.GetUserName(originalTableName + "." + originalColumnName) : originalColumnName;

                    InsertComment(meta, str, 2, column, true);
                    str.AddLine(2, $"public const string {customColumnName} = \"{originalTableName}.{originalColumnName}\";");
                    str.AddLine();
                }

                #endregion
                str.AddLine(1, "}");
                str.AddLine();

                if (meta.UseComments)
                {
                    str.AddComment(1, tableComment);
                }
                str.AddLine(1, $"[DBOrmTable({customTableName}TableName)]");
                str.AddLine(1, $"public class {customTableName}Row : DBOrmRow<{customTableName}Row>");
                str.AddLine(1, "{");
                #region

                foreach (DBColumn column in table.Columns)
                {
                    string originalColumnName = column.Name;
                    string customColumnName = meta.ContainsUserName(originalTableName + "." + originalColumnName) ? meta.GetUserName(originalTableName + "." + originalColumnName) : originalColumnName;
                    string columnTypeName = GetTypeName(column, meta);

                    string notNullAttribute = "";
                    string primaryKeyAttribute = "";
                    string foreignKeyAttribute = "";
                    #region Получение информации об атрибутах

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

                    #endregion

                    str.AddLine(2, $"#region {customColumnName}");
                    str.AddLine();

                    InsertComment(meta, str, 2, column, false);
                    str.AddLine(2, $"[DBOrmColumn({customTableName}.{customColumnName}{notNullAttribute}{primaryKeyAttribute}{foreignKeyAttribute})]");
                    str.AddProperty(2, $"public {columnTypeName} {customColumnName}",
                        $"Row.GetValue<{columnTypeName}>({customTableName}.{customColumnName});",
                        $"Row[{customTableName}.{customColumnName}] = value;");

                    str.AddLine();
                    InsertComment(meta, str, 2, column, true);
                    str.AddLine(2, $"public void Set{customColumnName}(object value)");
                    str.AddLine(2, "{");
                    str.AddLine(3, $"Row[{customTableName}.{customColumnName}] = value;");
                    str.AddLine(2, "}");

                    str.AddLine();
                    str.AddLine(2, "#endregion");
                    str.AddLine();
                }

                str.AddLine(2, $"public {customTableName}Row(DBRow row) : base(row) {{ }}");

                #endregion
                str.AddLine(1, "}");

                str.AddLine();
                #endregion
                str.AddLine(1, "#endregion");
                str.AddLine();
            }
            #endregion

            str.AddLine(1, "public DB(DBProvider provider, DbConnection connection) : base(provider, connection)");
            str.AddLine(1, "{");
            str.AddLine(1, "}");

            str.AddLine(0, "}");

            return str.ToString();
        }
        public string CreateText_Mode1(MetaManager meta)
        {
            StringBuilder str = new StringBuilder();

            foreach (DBTable table in Provider.Tables)
            {
                string originalTableName = table.Name;
                string customTableName = meta.ContainsUserName(originalTableName) ? meta.GetUserName(originalTableName) : originalTableName;
                string tableComment = meta.ContainsComment(originalTableName) ? meta.GetComment(originalTableName) : string.Empty;

                str.AddLine(0, $"#region {customTableName}");
                #region
                str.AddLine();

                if (meta.UseComments)
                {
                    str.AddComment(0, tableComment);
                }
                str.AddLine(0, $"public class {customTableName}Item");
                str.AddLine(0, "{");
                #region

                str.AddLine(1, $"public DB.{customTableName}Row Row {{ get; private set; }}");

                str.AddLine(1, $"public {customTableName}Item(DB.{customTableName}Row row)");
                str.AddLine(1, "{");
                str.AddLine(2, "Row = row;");
                str.AddLine(1, "}");
                str.AddLine();

                foreach (DBColumn column in table.Columns)
                {
                    string originalColumnName = column.Name;
                    string customColumnName = meta.ContainsUserName(originalTableName + "." + originalColumnName) ? meta.GetUserName(originalTableName + "." + originalColumnName) : originalColumnName;
                    string columnTypeName = GetTypeName(column, meta);

                    InsertComment(meta, str, 1, column, false);
                    str.AddProperty(1, $"public {columnTypeName} {customColumnName}",
                        $"Row.{customColumnName};", null);
                }
                str.AddLine();
                foreach (DBColumn column in table.Columns)
                {
                    string originalColumnName = column.Name;
                    string customColumnName = meta.ContainsUserName(originalTableName + "." + originalColumnName) ? meta.GetUserName(originalTableName + "." + originalColumnName) : originalColumnName;
                    string columnTypeName = GetTypeName(column, meta);

                    InsertComment(meta, str, 1, column, false);
                    str.AddLine(1, $"public bool Set{customColumnName}(object value)");
                    str.AddLine(1, "{");
                    str.AddLine(2, $"Row.Set{customColumnName}(value);");
                    str.AddLine(2, "return true;");
                    str.AddLine(1, "}");
                }

                str.AddLine();

                #endregion
                str.AddLine(0, "}");

                str.AddLine();
                #endregion
                str.AddLine(0, "#endregion");
                str.AddLine();
            }

            return str.ToString();
        }

        private void InsertComment(MetaManager meta, StringBuilder str, int level, DBColumn column, bool insertTypeName)
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
                    comment += $" [{GetCommentObjectType(column, meta)}]";
                }
                if (comment.Length > 0)
                {
                    str.AddComment(level, comment);
                }
            }
        }
        private string GetCommentObjectType(DBColumn column, MetaManager meta)
        {
            string typeName = GetTypeName(column, meta);
            if (!column.NotNull && column.DataType.IsClass)
            {
                typeName += "?";
            }
            return typeName;
        }
        private string GetTypeName(DBColumn column, MetaManager meta)
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
