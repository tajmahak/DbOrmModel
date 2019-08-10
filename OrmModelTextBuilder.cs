using MyLibrary.DataBase;
using System.Text;

namespace DbOrmModel
{
    internal class OrmModelTextBuilder
    {
        public OrmModelTextBuilder(DBModelBase model)
        {
            Model = model;
        }

        public DBModelBase Model { get; private set; }

        public string CreateDbText(MetaManager meta)
        {
            var str = new StringBuilder();

            str.Line(0, "namespace DB");
            str.AppendLine("{");

            for (var i = 0; i < Model.Tables.Count; i++)
            {
                var table = Model.Tables[i];

                var tableName = table.Name;
                if (meta.ContainsUserName(table.Name))
                {
                    tableName = meta.GetUserName(table.Name);
                }

                str.Line(1, $"#region {tableName}");

                if (meta.ContainsComment(table.Name))
                {
                    str.LineComment(1, meta.GetComment(table.Name));
                }

                str.Line(1, $"public static class {tableName}");
                str.Line(1, "{");

                str.Line(2, $"public const string _ = \"{table.Name}\";");
                for (var j = 0; j < table.Columns.Count; j++)
                {
                    var column = table.Columns[j];

                    var fieldName = column.Name;
                    if (meta.ContainsUserName(table.Name + "." + column.Name))
                    {
                        fieldName = meta.GetUserName(table.Name + "." + column.Name);
                    }
                    else
                    {
                        if (fieldName.StartsWith(table.Name))
                        {
                            fieldName = fieldName.Remove(0, table.Name.Length + 1);
                        }
                    }

                    InsertComment(meta, str, 2, column, true);
                    str.Line(2, $"public const string {fieldName} = \"{table.Name}.{column.Name}\";");
                }
                str.Line(1, "}");
                str.Line(1, "#endregion");
            }
            str.Line(0, "}");

            return str.ToString();
        }
        public string CreateOrmText(MetaManager meta)
        {
            var str = new StringBuilder();

            str.Line(0, "namespace ORM");
            str.AppendLine("{");

            str.Line(1, "using MyLibrary.DataBase;");
            str.Line(1, "using System;");
            str.AppendLine();

            for (var i = 0; i < Model.Tables.Count; i++)
            {
                var table = Model.Tables[i];

                var tableName = table.Name;
                if (meta.ContainsUserName(table.Name))
                {
                    tableName = meta.GetUserName(table.Name);
                }

                str.Line(1, $"#region {tableName}");
                str.AppendLine();

                if (meta.ContainsComment(table.Name))
                {
                    str.LineComment(1, meta.GetComment(table.Name));
                }

                str.Line(1, $"[DBOrmTable(DB.{tableName}._)]");
                str.Line(1, $"public class {tableName}: DBOrmTableBase");
                str.Line(1, "{");

                str.LineProperty(2, "public string _", "Row.Table.Name;", null);

                for (var j = 0; j < table.Columns.Count; j++)
                {
                    var column = table.Columns[j];

                    var fieldName = column.Name;
                    if (meta.ContainsUserName(table.Name + "." + column.Name))
                    {
                        fieldName = meta.GetUserName(table.Name + "." + column.Name);
                    }
                    else
                    {
                        if (fieldName.StartsWith(table.Name))
                        {
                            fieldName = fieldName.Remove(0, table.Name.Length + 1);
                        }
                    }

                    var constName = $"DB.{tableName}.{fieldName}";

                    str.Line(2, $"#region {fieldName}");
                    str.AppendLine();

                    InsertComment(meta, str, 2, column, true);
                    var typeName = GetTypeName(column, meta);

                    var attrAllowDbNull = string.Empty;
                    #region
                    if (column.NotNull)
                    {
                        attrAllowDbNull = ", NotNull: true";
                    }
                    #endregion
                    var attrIsPrimaryKey = string.Empty;
                    #region
                    if (column.IsPrimary)
                    {
                        attrIsPrimaryKey = ", PrimaryKey: true";
                    }
                    #endregion
                    var attrForeignKey = string.Empty;
                    #region
                    if (meta.ContainsForeignKey(table.Name + "." + column.Name))
                    {
                        var foreignKey = meta.GetForeignKeyInfo(table.Name + "." + column.Name);
                        var split = foreignKey.Split('.');

                        if (meta.ContainsUserName(split[0] + "." + split[1]))
                        {
                            split[1] = meta.GetUserName(split[0] + "." + split[1]);
                        }
                        if (meta.ContainsUserName(split[0]))
                        {
                            split[0] = meta.GetUserName(split[0]);
                        }

                        attrForeignKey = $", ForeignKey: DB.{split[0]}.{split[1]}";
                    }
                    #endregion

                    str.Line(2, $"[DBOrmColumn({constName}{attrAllowDbNull}{attrIsPrimaryKey}{attrForeignKey})]");

                    var propertyText = $"public {typeName} {fieldName}";
                    var getText = $"Row.Get<{typeName}>({constName});";
                    var setText = $"Row[{constName}] = value;";
                    str.LineProperty(2, propertyText, getText, setText);

                    InsertComment(meta, str, 2, column, true);
                    propertyText = $"public object _{fieldName}";
                    getText = $"Row[{constName}];";
                    setText = $"Row[{constName}] = value;";
                    str.LineProperty(2, propertyText, getText, setText);

                    str.AppendLine();
                    str.Line(2, "#endregion");
                }

                str.AppendLine();

                str.Line(2, $"public {tableName}(DBRow row) : base(row) {{ }}");

                if (meta.ContainsToString(table.Name))
                {
                    str.AppendLine();
                    str.Line(2, "public override string ToString()");
                    str.Line(2, "{");
                    str.Line(3, $"return {meta.GetToString(table.Name)};");
                    str.Line(2, "}");
                }

                str.Line(1, "}");

                str.AppendLine();
                str.Line(1, "#endregion");
            }
            str.Line(0, "}");

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
