using MyLibrary.DataBase;
using System.Text;

namespace DbOrmModel
{
    internal class OrmModelTextBuilder
    {
        public DBModelBase Model { get; private set; }

        public OrmModelTextBuilder(DBModelBase model)
        {
            Model = model;
        }
        public string CreateText(MetaManager meta)
        {
            var str = new StringBuilder();
            str.Line(0, "using MyLibrary.DataBase;");
            str.Line(0, "using System;");
            str.Line();
            str.Line(0, $"namespace DB");
            str.Line(0, "{");
            #region
            foreach (var table in Model.Tables)
            {
                var originalTableName = table.Name;
                var customTableName = meta.ContainsUserName(originalTableName) ? meta.GetUserName(originalTableName) : originalTableName;
                var tableComment = meta.ContainsComment(originalTableName) ? meta.GetComment(originalTableName) : string.Empty;

                str.Line(1, $"#region {customTableName}");
                #region
                str.Line();

                if (meta.UseComments)
                {
                    str.LineComment(1, tableComment);
                }
                str.Line(1, $"internal static class {customTableName}Table");
                str.Line(1, "{");
                #region

                str.Line(2, $"public const string _ = \"{originalTableName}\";");

                foreach (var column in table.Columns)
                {
                    var originalColumnName = column.Name;
                    var customColumnName = meta.ContainsUserName(originalTableName + "." + originalColumnName) ? meta.GetUserName(originalTableName + "." + originalColumnName) : originalColumnName;

                    str.Line();
                    InsertComment(meta, str, 2, column, true);
                    str.Line(2, $"public const string {customColumnName} = \"{originalTableName}.{originalColumnName}\";");
                }

                #endregion
                str.Line(1, "}");
                str.Line();

                if (meta.UseComments)
                {
                    str.LineComment(1, tableComment);
                }
                str.Line(1, $"[DBOrmTable({customTableName}Table._)]");
                str.Line(1, $"internal class {customTableName}Row : DBOrmRowBase<{customTableName}Row>");
                str.Line(1, "{");
                #region

                foreach (var column in table.Columns)
                {
                    var originalColumnName = column.Name;
                    var customColumnName = meta.ContainsUserName(originalTableName + "." + originalColumnName) ? meta.GetUserName(originalTableName + "." + originalColumnName) : originalColumnName;
                    var columnTypeName = GetTypeName(column, meta);

                    var notNullAttribute = "";
                    var primaryKeyAttribute = "";
                    var foreignKeyAttribute = "";
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
                        var foreignKey = meta.GetForeignKeyInfo(originalTableName + "." + originalColumnName);
                        var split = foreignKey.Split('.');

                        if (meta.ContainsUserName(split[0] + "." + split[1]))
                        {
                            split[1] = meta.GetUserName(split[0] + "." + split[1]);
                        }
                        if (meta.ContainsUserName(split[0]))
                        {
                            split[0] = meta.GetUserName(split[0]);
                        }

                        foreignKeyAttribute = $", ForeignKey: {split[0]}Table.{split[1]}";
                    }

                    #endregion

                    str.Line(2, $"#region {customColumnName}");
                    str.Line();

                    InsertComment(meta, str, 2, column, false);
                    str.Line(2, $"[DBOrmColumn({customTableName}Table.{customColumnName}{notNullAttribute}{primaryKeyAttribute}{foreignKeyAttribute})]");
                    str.LineProperty(2, $"public {columnTypeName} {customColumnName}",
                        $"Row.Get<{columnTypeName}>({customTableName}Table.{customColumnName});",
                        $"Row[{customTableName}Table.{customColumnName}] = value;");

                    str.Line();
                    InsertComment(meta, str, 2, column, true);
                    str.Line(2, $"public void Set{customColumnName}(object value)");
                    str.Line(2, "{");
                    str.Line(3, $"Row[{customTableName}Table.{customColumnName}] = value;");
                    str.Line(2, "}");

                    str.Line();
                    str.Line(2, "#endregion");
                    str.Line();
                }

                str.Line(2, $"public {customTableName}Row(DBRow row) : base(row) {{ }}");

                #endregion
                str.Line(1, "}");

                str.Line();
                #endregion
                str.Line(1, "#endregion");
                str.Line();
            }
            #endregion
            str.Remove(str.Length - 2, 2);
            str.Line(0, "}");

            return str.ToString();
        }
        public string CreateText_Mode1(MetaManager meta)
        {
            var str = new StringBuilder();

            foreach (var table in Model.Tables)
            {
                var originalTableName = table.Name;
                var customTableName = meta.ContainsUserName(originalTableName) ? meta.GetUserName(originalTableName) : originalTableName;
                var tableComment = meta.ContainsComment(originalTableName) ? meta.GetComment(originalTableName) : string.Empty;

                str.Line(0, $"#region {customTableName}");
                #region
                str.Line();

                if (meta.UseComments)
                {
                    str.LineComment(0, tableComment);
                }
                str.Line(0, $"internal class {customTableName}Item");
                str.Line(0, "{");
                #region

                str.Line(1, $"public DB.{customTableName}Row Row {{ get; private set; }}");

                str.Line(1, $"public {customTableName}Item(DB.{customTableName}Row row)");
                str.Line(1, "{");
                str.Line(2, "Row = row;");
                str.Line(1, "}");
                str.Line();

                foreach (var column in table.Columns)
                {
                    var originalColumnName = column.Name;
                    var customColumnName = meta.ContainsUserName(originalTableName + "." + originalColumnName) ? meta.GetUserName(originalTableName + "." + originalColumnName) : originalColumnName;
                    var columnTypeName = GetTypeName(column, meta);

                    InsertComment(meta, str, 1, column, false);
                    str.LineProperty(1, $"public {columnTypeName} {customColumnName}",
                        $"Row.{customColumnName};", null);
                }
                str.Line();
                foreach (var column in table.Columns)
                {
                    var originalColumnName = column.Name;
                    var customColumnName = meta.ContainsUserName(originalTableName + "." + originalColumnName) ? meta.GetUserName(originalTableName + "." + originalColumnName) : originalColumnName;
                    var columnTypeName = GetTypeName(column, meta);

                    InsertComment(meta, str, 1, column, false);
                    str.Line(1, $"public bool Set{customColumnName}(object value)");
                    str.Line(1, "{");
                    str.Line(2, $"Row.Set{customColumnName}(value);");
                    str.Line(2, "return true;");
                    str.Line(1, "}");
                }

                str.Line();

                #endregion
                str.Line(0, "}");

                str.Line();
                #endregion
                str.Line(0, "#endregion");
                str.Line();
            }

            return str.ToString();
        }

        private void InsertComment(MetaManager meta, StringBuilder str, int level, DBColumn column, bool insertTypeName)
        {
            if (meta.UseComments)
            {
                var columnName = column.Table.Name + "." + column.Name;

                var comment = string.Empty;
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
                    str.LineComment(level, comment);
                }
            }
        }
        private string GetCommentObjectType(DBColumn column, MetaManager meta)
        {
            var typeName = GetTypeName(column, meta);
            if (!column.NotNull && column.DataType.IsClass)
            {
                typeName += "?";
            }
            return typeName;
        }
        private string GetTypeName(DBColumn column, MetaManager meta)
        {
            var type = column.DataType;

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
