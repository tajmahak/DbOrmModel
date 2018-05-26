using System;
using System.Data.Common;
using System.Text;
using System.Windows.Forms;
using DBSetExtension;
using FirebirdSql.Data.FirebirdClient;

namespace DbOrmModel
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
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

        private void Open(string path)
        {
            DbConnection connection = null;
            try
            {
                path = path.Trim('\"');
                connection = CreateDataBaseConnection(path);
                var model = new DBModelFireBird();
                model.Initialize(connection);
                connection.Dispose();
                CreateText(model);
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
        private void CreateText(DBModelBase model)
        {
            string s1 = "\\\"", s2 = "\\\"";

            var strUsing = new StringBuilder();
            #region

            strUsing.Line(0, "using System;");
            strUsing.Line(0, "using MyLibrary.DataBase;");

            #endregion

            var strDB = new StringBuilder();
            #region

            strDB.Line(0, "namespace DB");
            strDB.AppendLine("{");

            for (int i = 0; i < model.Tables.Length; i++)
            {
                var table = model.Tables[i];
                strDB.Line(1, "#region " + table.Name);
                strDB.Line(1, "public static class " + table.Name);
                strDB.Line(1, "{");

                strDB.Line(2, "public const string _ = \"{1}{0}{2}\";", table.Name, s1, s2);
                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];

                    var fieldName = column.Name;
                    if (fieldName.StartsWith(table.Name))
                        fieldName = fieldName.Remove(0, table.Name.Length + 1);

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

            for (int i = 0; i < model.Tables.Length; i++)
            {
                var table = model.Tables[i];
                strORM.Line(1, "#region " + table.Name);
                strORM.Line(1, "public class " + table.Name + ": IOrmTable");
                strORM.Line(1, "{");

                strORM.LineProperty(2, "public string _", "return Row.Table.Name;", null);

                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];

                    var fieldName = column.Name;
                    if (fieldName.StartsWith(table.Name))
                        fieldName = fieldName.Remove(0, table.Name.Length + 1);

                    var constName = "__" + fieldName.ToLower();

                    string objectType;
                    #region выбор типа объекта

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

                    #endregion

                    strORM.AppendLine();

                    strORM.Line(2, "private const string {0} = \"{3}{1}{4}.{3}{2}{4}\";", constName, table.Name, column.Name, s1, s2);

                    string propertyText = "public " + objectType + " " + fieldName;
                    string getText = "return Row.Get<" + objectType + ">(" + constName + ");";
                    string setText = "Row.SetNotNull(" + constName + ", value);";
                    strORM.LineProperty(2, propertyText, getText, setText);

                    propertyText = "public object _" + fieldName;
                    getText = "return Row[" + constName + "];";
                    setText = "Row.SetNotNull(" + constName + ", value);";
                    strORM.LineProperty(2, propertyText, getText, setText);
                }


                strORM.AppendLine();
                strORM.Line(2, "public DBRow Row { get; set; }");
                strORM.Line(2, "public " + table.Name + "(DBRow row)");
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
            str.AppendLine(strUsing.ToString());
            str.AppendLine(strORM.ToString());
            textBox2.Text = str.ToString();

            str = new StringBuilder();
            str.AppendLine(strUsing.ToString());
            str.AppendLine(strDB.ToString());
            str.AppendLine(strORM.ToString());
            textBox3.Text = str.ToString();

            #endregion
        }
        private void CopyToClipboard(string text)
        {
            if (text.Length == 0)
                return;
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
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
    }
}
