using System;
using System.Collections.Generic;
using System.Text;
using MyLibrary.DataBase;

namespace DbOrmModel
{
    class OrmModelTextBuilder
    {
        public OrmModelTextBuilder(DBModelBase model)
        {
            _commentDictionary = new Dictionary<string, string>();
            _userNamesDictionary = new Dictionary<string, string>();
            _model = model;
        }

        public bool UseComments { get; set; }
        public bool UseUserNames { get; set; }

        private DBModelBase _model;
        private Dictionary<string, string> _commentDictionary;
        private Dictionary<string, string> _userNamesDictionary;

        public string CreateDbText()
        {
            string comment;
            var str = new StringBuilder();

            str.Line(0, "namespace DB");
            str.AppendLine("{");

            for (int i = 0; i < _model.Tables.Length; i++)
            {
                var table = _model.Tables[i];

                var tableName = table.Name;
                if (UseUserNames && _userNamesDictionary.ContainsKey(tableName))
                    tableName = _userNamesDictionary[tableName];

                str.Line(1, "#region " + tableName);

                if (UseComments && _commentDictionary.TryGetValue(table.Name, out comment))
                {
                    str.LineComment(1, comment);
                }

                str.Line(1, "public static class " + tableName);
                str.Line(1, "{");

                str.Line(2, "public const string _ = \"{0}\";", table.Name);
                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];

                    var fieldName = column.Name;
                    if (UseUserNames && _userNamesDictionary.ContainsKey(table.Name + "." + column.Name))
                    {
                        fieldName = _userNamesDictionary[table.Name + "." + column.Name];
                    }
                    else
                    {
                        if (fieldName.StartsWith(table.Name))
                        {
                            fieldName = fieldName.Remove(0, table.Name.Length + 1);
                        }
                    }

                    InsertComment(str, 2, column, true);
                    str.Line(2, "public const string {0} = \"{1}.{2}\";", fieldName, table.Name, column.Name);
                }
                str.Line(1, "}");
                str.Line(1, "#endregion");
            }
            str.Line(0, "}");

            return str.ToString();
        }
        public string CreateOrmText()
        {
            string comment;
            var str = new StringBuilder();

            str.Line(0, "namespace ORM");
            str.AppendLine("{");

            str.Line(1, "using System;");
            str.Line(1, "using MyLibrary.DataBase;");
            str.Line(1, "using MyLibrary.DataBase.Orm;");
            str.AppendLine();

            for (int i = 0; i < _model.Tables.Length; i++)
            {
                var table = _model.Tables[i];

                var tableName = table.Name;
                if (UseUserNames && _userNamesDictionary.ContainsKey(tableName))
                    tableName = _userNamesDictionary[tableName];

                str.Line(1, "#region " + tableName);
                if (UseComments && _commentDictionary.TryGetValue(table.Name, out comment))
                {
                    str.LineComment(1, comment);
                }

                str.Line(1, "[DBOrmTable(\"" + table.Name + "\")]");
                str.Line(1, "public class " + tableName + ": DBOrmTableBase");
                str.Line(1, "{");

                str.LineProperty(2, "public string _", "return Row.Table.Name;", null);

                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];

                    var fieldName = column.Name;
                    if (UseUserNames && _userNamesDictionary.ContainsKey(table.Name + "." + column.Name))
                    {
                        fieldName = _userNamesDictionary[table.Name + "." + column.Name];
                    }
                    else
                    {
                        if (fieldName.StartsWith(table.Name))
                        {
                            fieldName = fieldName.Remove(0, table.Name.Length + 1);
                        }
                    }

                    var constName = "__" + fieldName.ToLower();

                    str.AppendLine();

                    str.Line(2, "private const string {0} = \"{1}.{2}\";", constName, table.Name, column.Name);

                    InsertComment(str, 2, column, false);
                    string objectType = GetObjectType(column);

                    str.Line(2, "[DBOrmColumn(" + constName + ")]");

                    string propertyText = "public " + objectType + " " + fieldName;
                    string getText = "return Row.Get<" + objectType + ">(" + constName + ");";
                    string setText = "Row.SetNotNull(" + constName + ", value);";
                    str.LineProperty(2, propertyText, getText, setText);

                    InsertComment(str, 2, column, false);
                    propertyText = "public object _" + fieldName;
                    getText = "return Row[" + constName + "];";
                    setText = "Row.SetNotNull(" + constName + ", value);";
                    str.LineProperty(2, propertyText, getText, setText);
                }

                str.AppendLine();

                str.Line(2, "public " + tableName + "(DBRow row)");
                str.Line(2, "{");
                str.Line(3, "Row = row;");
                str.Line(2, "}");

                str.Line(1, "}");
                str.Line(1, "#endregion");
            }
            str.Line(0, "}");

            return str.ToString();
        }
        public void PrepareCommentDictionary(string[] content)
        {
            _commentDictionary.Clear();

            if (content == null)
                return;

            foreach (var line in content)
            {
                string[] split = line.Split('\t');
                if (split.Length != 2)
                    continue;

                var text1 = split[0].Trim();
                var text2 = split[1].Trim();
                if (text2.Length == 0)
                    continue;

                _commentDictionary.Add(text1, text2);
            }
        }
        public void PrepareUserNamesDictionary(string[] content)
        {
            _userNamesDictionary.Clear();

            if (content == null)
                return;

            foreach (var line in content)
            {
                string[] split = line.Split('\t');
                if (split.Length != 2)
                    continue;

                var text1 = split[0].Trim();
                var text2 = split[1].Trim();
                if (text2.Length == 0)
                    continue;

                _userNamesDictionary.Add(text1, text2);
            }
        }
        public string[] UpdateCommentContent(string[] content)
        {
            PrepareCommentDictionary(content);

            string comment;

            var text = new StringBuilder();
            foreach (var table in _model.Tables)
            {
                text.AppendLine();
                text.Append(table.Name + "\t");
                if (_commentDictionary.TryGetValue(table.Name, out comment))
                {
                    text.Append(comment);
                }
                text.AppendLine();

                foreach (var column in table.Columns)
                {
                    var columnName = table.Name + "." + column.Name;
                    text.Append(columnName + "\t");
                    if (_commentDictionary.TryGetValue(columnName, out comment))
                    {
                        text.Append(comment);
                    }
                    text.AppendLine();
                }
            }
            text.Remove(0, 2); // убирает первую пустую строку

            // преобразование текста в массив строк
            return text.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }
        public string[] UpdateUserNamesContent(string[] content)
        {
            PrepareUserNamesDictionary(content);

            string userName;

            var text = new StringBuilder();
            foreach (var table in _model.Tables)
            {
                text.AppendLine();
                text.Append(table.Name + "\t");

                if (_userNamesDictionary.TryGetValue(table.Name, out userName))
                {
                    text.Append(userName);
                }
                else
                {
                    text.Append(table.Name);
                }

                text.AppendLine();

                foreach (var column in table.Columns)
                {
                    var columnName = table.Name + "." + column.Name;
                    text.Append(columnName + "\t");

                    var fieldName = column.Name;
                    if (fieldName.StartsWith(table.Name))
                        fieldName = fieldName.Remove(0, table.Name.Length + 1);

                    if (_userNamesDictionary.TryGetValue(columnName, out userName))
                    {
                        text.Append(userName);
                    }
                    else
                    {
                        text.Append(fieldName);
                    }
                    text.AppendLine();
                }
            }
            text.Remove(0, 2); // убирает первую пустую строку

            // преобразование текста в массив строк
            return text.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
        }

        private void InsertComment(StringBuilder str, int level, DBColumn column, bool insertTypeName)
        {
            if (!UseComments)
                return;

            var columnName = column.Table.Name + "." + column.Name;

            string comment = string.Empty;
            if (_commentDictionary.ContainsKey(columnName))
            {
                comment += _commentDictionary[columnName];
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
