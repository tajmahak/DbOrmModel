using MyLibrary.DataBase;
using System.Text;

namespace DbOrmModel
{
    class OrmModelTextBuilder
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

            for (int i = 0; i < Model.Tables.Length; i++)
            {
                var table = Model.Tables[i];

                var tableName = table.Name;
                if (meta.ContainsUserName(table.Name))
                {
                    tableName = meta.GetUserName(table.Name);
                }

                str.Line(1, "#region " + tableName);

                if (meta.ContainsComment(table.Name))
                {
                    str.LineComment(1, meta.GetComment(table.Name));
                }

                str.Line(1, "public static class " + tableName);
                str.Line(1, "{");

                str.Line(2, "public const string _ = \"{0}\";", table.Name);
                for (int j = 0; j < table.Columns.Length; j++)
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
                    str.Line(2, "public const string {0} = \"{1}.{2}\";", fieldName, table.Name, column.Name);
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
            str.Line(1, "using MyLibrary.DataBase.Orm;");
            str.Line(1, "using System;");
            str.AppendLine();

            for (int i = 0; i < Model.Tables.Length; i++)
            {
                var table = Model.Tables[i];

                var tableName = table.Name;
                if (meta.ContainsUserName(table.Name))
                {
                    tableName = meta.GetUserName(table.Name);
                }

                str.Line(1, "#region " + tableName);
                str.AppendLine();

                if (meta.ContainsComment(table.Name))
                {
                    str.LineComment(1, meta.GetComment(table.Name));
                }

                str.Line(1, "[DBOrmTable(\"" + table.Name + "\")]");
                str.Line(1, "public class " + tableName + ": DBOrmTableBase");
                str.Line(1, "{");

                str.LineProperty(2, "public string _", "return Row.Table.Name;", null);

                for (int j = 0; j < table.Columns.Length; j++)
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

                    var constName = "DB." + tableName + "." + fieldName;

                    str.Line(2, "#region " + fieldName);
                    str.AppendLine();

                    InsertComment(meta, str, 2, column, false);
                    string objectType = GetObjectType(column);


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
                        str.Line(2, "[DBOrmColumn(" + constName + ", DB." + split[0] + "." + split[1] + ")]");
                    }
                    else
                    {
                        str.Line(2, "[DBOrmColumn(" + constName + ")]");
                    }

                    string propertyText = "public " + objectType + " " + fieldName;
                    string getText = "return Row.Get<" + objectType + ">(" + constName + ");";
                    string setText = "Row[" + constName + "] = value;";
                    str.LineProperty(2, propertyText, getText, setText);

                    InsertComment(meta, str, 2, column, false);
                    propertyText = "public object _" + fieldName;
                    getText = "return Row[" + constName + "];";
                    setText = "Row[" + constName + "] = value;";
                    str.LineProperty(2, propertyText, getText, setText);

                    str.AppendLine();
                    str.Line(2, "#endregion");
                }

                str.AppendLine();

                str.Line(2, "public " + tableName + "(DBRow row) : base(row) { }");

                if (meta.ContainsDebugInfo(table.Name))
                {
                    str.AppendLine();
                    str.Line(2, "public override string ToString()");
                    str.Line(2, "{");
                    str.Line(3, "return " + meta.GetDebugInfo(table.Name) + "" + ";");
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
            if (!meta.UseComments)
                return;

            var columnName = column.Table.Name + "." + column.Name;

            string comment = string.Empty;
            if (meta.ContainsComment(columnName))
            {
                comment += meta.GetComment(columnName);
            }
            if (insertTypeName)
            {
                comment += " [" + GetCommentObjectType(column) + "]";
            }
            if (comment.Length > 0)
            {
                str.LineComment(level, comment);
            }
        }
        private string GetCommentObjectType(DBColumn column)
        {
            string objectType;
            if (column.Comment != null)
            {
                if (column.Comment == "BOOLEAN")
                {
                    objectType = typeof(bool).Name;
                }
                else
                {
                    objectType = column.Comment;
                }
            }
            else
            {
                objectType = column.DataType.Name;
            }

            switch (objectType)
            {
                case "Boolean": objectType = "bool"; break;
                case "Char": objectType = "char"; break;
                case "String": objectType = "string"; break;

                case "Byte": objectType = "byte"; break;
                case "SByte": objectType = "sbyte"; break;
                case "Int16": objectType = "short"; break;
                case "UInt16": objectType = "ushort"; break;
                case "Int32": objectType = "int"; break;
                case "UInt32": objectType = "uint"; break;
                case "Int64": objectType = "long"; break;
                case "UInt64": objectType = "ulong"; break;

                case "Single": objectType = "float"; break;
                case "Double": objectType = "double"; break;
                case "Decimal": objectType = "decimal"; break;
            }

            if (column.AllowDBNull)
            {
                objectType += "?";
            }
            return objectType;
        }
        private string GetObjectType(DBColumn column)
        {
            string objectType;
            if (column.Comment != null)
            {
                if (column.Comment == "BOOLEAN")
                {
                    objectType = typeof(bool).Name;
                }
                else
                {
                    objectType = column.Comment;
                }
            }
            else
            {
                objectType = column.DataType.Name;
            }
            if (column.AllowDBNull && !column.DataType.IsClass)
            {
                objectType = "Nullable<" + objectType + ">";
            }
            return objectType;
        }
    }
}
