using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DBSetExtension;
using FirebirdSql.Data.FirebirdClient;

namespace DbOrmModel
{
    public partial class Form1 : Form
    {
        private DBModelBase _currentModel;
        private string _dbPath;

        public Form1()
        {
            _commentDictionary = new Dictionary<string, string>();
            _userNamesDictionary = new Dictionary<string, string>();
            InitializeComponent();

            Open(@"C:\Users\Admin\Desktop\garage_conv\bin\Debug\db\GARAGE_EMPTY.FDB");
        }
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var path = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (path.Length == 0)
                return;
            Open(path[0]);
        }
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                e.Handled = e.SuppressKeyPress = true;
                CopyToClipboard(((TextBox)sender).Text);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CopyToClipboard(textBox1.Text);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            CopyToClipboard(textBox2.Text);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            CopyToClipboard(textBox3.Text);
        }

        // комментарии
        private Dictionary<string, string> _commentDictionary;
        private string CommentPath
        {
            get
            {
                if (_dbPath == null)
                    return null;
                return _dbPath + ".comment.txt";
            }
        }
        private void checkBoxUseComments_CheckedChanged(object sender, EventArgs e)
        {
            if (_dbPath != null)
                Open(_dbPath);
        }
        private void labelComment_Click(object sender, EventArgs e)
        {
            PrepareCommentFile();
            MessageBox.Show("Готово.");
        }
        private void PrepareCommentFile()
        {
            PrepareCommentDictionary();

            string comment;

            var text = new StringBuilder();
            foreach (var table in _currentModel.Tables)
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

            File.Delete(CommentPath);
            File.WriteAllText(CommentPath, text.ToString(), Encoding.UTF8);
        }
        private void PrepareCommentDictionary()
        {
            _commentDictionary.Clear();
            if (!File.Exists(CommentPath))
                return;

            foreach (var line in File.ReadAllLines(CommentPath))
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

        // польз. имена
        private Dictionary<string, string> _userNamesDictionary;
        private string UserNamesPath
        {
            get
            {
                if (_dbPath == null)
                    return null;
                return _dbPath + ".usernames.txt";
            }
        }
        private void checkBoxUseUserNames_CheckedChanged(object sender, EventArgs e)
        {
            if (_dbPath != null)
                Open(_dbPath);
        }
        private void labelUserName_Click(object sender, EventArgs e)
        {
            PrepareUserNamesFile();
            MessageBox.Show("Готово.");
        }
        private void PrepareUserNamesFile()
        {
            PrepareUserNamesDictionary();

            string userName;

            var text = new StringBuilder();
            foreach (var table in _currentModel.Tables)
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

            File.Delete(UserNamesPath);
            File.WriteAllText(UserNamesPath, text.ToString(), Encoding.UTF8);
        }
        private void PrepareUserNamesDictionary()
        {
            _userNamesDictionary.Clear();
            if (!File.Exists(UserNamesPath))
                return;

            foreach (var line in File.ReadAllLines(UserNamesPath))
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

        private void Open(string path)
        {
            _dbPath = path;
            DbConnection connection = null;
            try
            {
                path = path.Trim('\"');
                connection = CreateDataBaseConnection(path);
                _currentModel = new DBModelFireBird();
                _currentModel.Initialize(connection);
                connection.Dispose();

                if (checkBoxUseComments.Checked)
                {
                    PrepareCommentDictionary();
                }
                if (checkBoxUseUserNames.Checked)
                {
                    PrepareUserNamesDictionary();
                }
                CreateText(_currentModel, checkBoxUseComments.Checked, checkBoxUseUserNames.Checked);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }
        private DbConnection CreateDataBaseConnection(string path)
        {
            var conBuilder = new FbConnectionStringBuilder();
            conBuilder.Dialect = 3;
            conBuilder.UserID = "SYSDBA";
            conBuilder.Password = "masterkey";
            conBuilder.Charset = "WIN1251";

            conBuilder.ServerType = FbServerType.Default;
            conBuilder.DataSource = "127.0.0.1";
            conBuilder.Database = path;
            conBuilder.Port = 3050;

            var connection = new FbConnection(conBuilder.ToString());
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка подключения к БД: " + ex.Message, ex);
            }
            return connection;
        }
        private void CreateText(DBModelBase model, bool useComment, bool useUserName)
        {
            string s1 = "", s2 = ""; // сепараторы названий сущности таблицы (убраны для совместимости с другими БД)
            string comment;

            var strDB = new StringBuilder();
            #region

            strDB.Line(0, "namespace DB");
            strDB.AppendLine("{");

            for (int i = 0; i < model.Tables.Length; i++)
            {
                var table = model.Tables[i];
                strDB.Line(1, "#region " + table.Name);

                if (useComment && _commentDictionary.TryGetValue(table.Name, out comment))
                {
                    strDB.LineComment(1, comment);
                }

                var tableName = table.Name;
                if (useUserName && _userNamesDictionary.ContainsKey(tableName))
                    tableName = _userNamesDictionary[tableName];

                strDB.Line(1, "public static class " + tableName);
                strDB.Line(1, "{");

                strDB.Line(2, "public const string _ = \"{1}{0}{2}\";", table.Name, s1, s2);
                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];

                    var fieldName = column.Name;
                    if (_userNamesDictionary.ContainsKey(table.Name + "." + column.Name))
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

                    InsertComment(strDB, 2, column, true);
                    strDB.Line(2, "public const string {0} = \"{3}{1}{4}.{3}{2}{4}\";", fieldName, table.Name, column.Name, s1, s2);
                }
                strDB.Line(1, "}");
                strDB.Line(1, "#endregion");
            }
            strDB.Line(0, "}");

            #endregion

            var strORM = new StringBuilder();
            #region

            strORM.Line(0, "namespace ORM");
            strORM.AppendLine("{");

            strORM.Line(1, "using System;");
            strORM.Line(1, "using MyLibrary.DataBase;");
            strORM.Line(1, "using MyLibrary.DataBase.Orm;");
            strORM.AppendLine();

            for (int i = 0; i < model.Tables.Length; i++)
            {
                var table = model.Tables[i];
                strORM.Line(1, "#region " + table.Name);
                if (useComment && _commentDictionary.TryGetValue(table.Name, out comment))
                {
                    strORM.LineComment(1, comment);
                }

                var tableName = table.Name;
                if (useUserName && _userNamesDictionary.ContainsKey(tableName))
                    tableName = _userNamesDictionary[tableName];

                strORM.Line(1, "public class " + tableName + ": DBOrmTableBase");
                strORM.Line(1, "{");

                strORM.LineProperty(2, "public string _", "return Row.Table.Name;", null);

                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];

                    var fieldName = column.Name;
                    if (_userNamesDictionary.ContainsKey(table.Name + "." + column.Name))
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

                    strORM.AppendLine();

                    strORM.Line(2, "private const string {0} = \"{3}{1}{4}.{3}{2}{4}\";", constName, table.Name, column.Name, s1, s2);

                    InsertComment(strORM, 2, column, false);
                    string objectType = GetObjectType(column, true);
                    
                    strORM.Line(2, "[DBOrmColumn(" + constName + ")]");
                   
                    string propertyText = "public " + objectType + " " + fieldName;
                    string getText = "return Row.Get<" + objectType + ">(" + constName + ");";
                    string setText = "Row.SetNotNull(" + constName + ", value);";
                    strORM.LineProperty(2, propertyText, getText, setText);

                    InsertComment(strORM, 2, column, false);
                    propertyText = "public object _" + fieldName;
                    getText = "return Row[" + constName + "];";
                    setText = "Row.SetNotNull(" + constName + ", value);";
                    strORM.LineProperty(2, propertyText, getText, setText);
                }

                strORM.AppendLine();

                strORM.Line(2, "public " + tableName + "(DBRow row)");
                strORM.Line(2, "{");
                strORM.Line(3, "Row = row;");
                strORM.Line(2, "}");

                strORM.Line(1, "}");
                strORM.Line(1, "#endregion");
            }
            strORM.Line(0, "}");

            #endregion

            #region Заполнение текстбоксов

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();

            textBox1.Text = strDB.ToString();

            var str = new StringBuilder();
            str.Append(strORM.ToString());
            textBox2.Text = str.ToString();

            str = new StringBuilder();
            str.AppendLine(strDB.ToString());
            str.Append(strORM.ToString());
            textBox3.Text = str.ToString();

            #endregion
        }
        private void CopyToClipboard(string text)
        {
            if (text.Length == 0)
                return;
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }
        private string GetObjectType(DBColumn column, bool fullType)
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
                if (fullType)
                {
                    objectType = "Nullable<" + objectType + ">";
                }
                else
                {
                    objectType += "?";
                }
            }
            return objectType;
        }
        private void InsertComment(StringBuilder str, int level, DBColumn column, bool insertTypeName)
        {
            if (!checkBoxUseComments.Checked)
                return;
            var columnName = column.Table.Name + "." + column.Name;

            string comment = string.Empty;
            if (_commentDictionary.ContainsKey(columnName))
            {
                comment += _commentDictionary[columnName];
            }
            if (insertTypeName)
            {
                comment += " [" + GetObjectType(column, false) + "]";
            }
            if (comment.Length > 0)
            {
                str.LineComment(level, comment);
            }
        }
    }
    public static class StringBuilderExtension
    {
        public static void Line(this StringBuilder str, int level, string text, params object[] values)
        {
            string padding = string.Empty;
            if (level > 0)
                padding = new string(' ', level * 4);
            if (values.Length == 0)
                str.AppendLine(padding + text);
            else str.AppendLine(padding + string.Format(text, values));
        }
        public static void LineProperty(this StringBuilder str, int level, string text, string getText, string setText)
        {
            str.Line(level, text);
            str.Line(level, "{");

            str.Line(level + 1, "get");
            str.Line(level + 1, "{");
            str.Line(level + 2, getText);
            str.Line(level + 1, "}");

            if (setText != null)
            {
                str.Line(level + 1, "set");
                str.Line(level + 1, "{");
                str.Line(level + 2, setText);
                str.Line(level + 1, "}");
            }
            str.Line(level, "}");
        }
        public static void LineComment(this StringBuilder str, int level, string text)
        {
            str.Line(level, "/// <summary>");
            str.Line(level, "/// " + text);
            str.Line(level, "/// </summary>");
        }
    }
}
