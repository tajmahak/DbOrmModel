using System;
using System.Data.Common;
using System.Drawing;
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
        private void richTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
            {
                e.Handled = e.SuppressKeyPress = true;
                CopyToClipboard(((RichTextBox)sender).Text);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CopyToClipboard(richTextBox1.Text);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            CopyToClipboard(richTextBox2.Text);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            CopyToClipboard(richTextBox3.Text);
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
            var strUsing = new StringBuilder();
            #region

            strUsing.Line(0, "using System;");
            strUsing.Line(0, "using DBSetExtension;");

            #endregion

            var strDB = new StringBuilder();
            #region текст 1

            strDB.Line(0, "namespace DB");
            strDB.AppendLine("{");

            for (int i = 0; i < model.Tables.Length; i++)
            {
                var table = model.Tables[i];
                strDB.Line(1, "#region " + table.Name);
                strDB.Line(1, "public static class " + table.Name);
                strDB.Line(1, "{");

                strDB.Line(2, "public const string _ = \"{0}\";", table.Name);
                for (int j = 0; j < table.Columns.Length; j++)
                {
                    var column = table.Columns[j];

                    var fieldName = column.Name;
                    if (fieldName.StartsWith(table.Name))
                        fieldName = fieldName.Remove(0, table.Name.Length + 1);

                    strDB.Line(2, @"public const string {0} = ""{1}.{2}"";", fieldName, table.Name, column.Name);
                }
                strDB.Line(1, "}");
                strDB.Line(1, "#endregion");
            }
            strDB.Line(0, "}");

            #endregion

            var strORM = new StringBuilder();
            #region текст 2

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

                    var constName = "_" + fieldName.ToLower();

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

                    strORM.Line(2, @"private const string {0} = ""{1}.{2}"";", constName, table.Name, column.Name);

                    string propertyText = "public " + objectType + " " + fieldName;
                    string getText = "return Row.Get<" + objectType + ">(" + constName + ");";
                    string setText = "Row.SetNotNull(" + constName + ", value);";
                    strORM.LineProperty(2, propertyText, getText, setText);

                    propertyText = "public object " + fieldName + "_Obj";
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

            richTextBox1.Clear();
            richTextBox2.Clear();
            richTextBox3.Clear();

            richTextBox1.Text = strDB.ToString();
            SetColor(richTextBox1);

            var str = new StringBuilder();
            str.AppendLine(strUsing.ToString());
            str.AppendLine(strORM.ToString());
            richTextBox2.Text = str.ToString();
            SetColor(richTextBox2);

            str = new StringBuilder();
            str.AppendLine(strUsing.ToString());
            str.AppendLine(strDB.ToString());
            str.AppendLine(strORM.ToString());
            richTextBox3.Text = str.ToString();
            SetColor(richTextBox3);

            #endregion
        }
        private void CopyToClipboard(string text)
        {
            text = text.Replace("\n", "\r\n");
            if (text.Length == 0)
                return;
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }

        #region Изменение цвета

        private void SetColor(RichTextBox richTextBox)
        {
            ChangeColor_Name(richTextBox, "using", color1);
            ChangeColor_Name(richTextBox, "#region", color1);
            ChangeColor_Name(richTextBox, "#endregion", color1);
            ChangeColor_Name(richTextBox, "public", color1);
            ChangeColor_Name(richTextBox, "private", color1);
            ChangeColor_Name(richTextBox, "const", color1);
            ChangeColor_Name(richTextBox, "object", color1);
            ChangeColor_Name(richTextBox, "string", color1);
            ChangeColor_Name(richTextBox, "byte", color1);
            ChangeColor_Name(richTextBox, "get", color1);
            ChangeColor_Name(richTextBox, "set", color1);
            ChangeColor_Name(richTextBox, "static", color1);
            ChangeColor_Name(richTextBox, "class", color1);
            ChangeColor_Name(richTextBox, "namespace", color1);
            ChangeColor_Name(richTextBox, "value", color1);

            ChangeColor_Name(richTextBox, "Nullable", color2);
            ChangeColor_Name(richTextBox, typeof(IOrmTable).Name, color2);
            ChangeColor_Name(richTextBox, typeof(string).Name, color2);

            ChangeColor_Name(richTextBox, typeof(bool).Name, color2);
            ChangeColor_Name(richTextBox, typeof(byte).Name, color2);
            ChangeColor_Name(richTextBox, typeof(byte).Name, color2);
            ChangeColor_Name(richTextBox, typeof(short).Name, color2);
            ChangeColor_Name(richTextBox, typeof(ushort).Name, color2);
            ChangeColor_Name(richTextBox, typeof(int).Name, color2);
            ChangeColor_Name(richTextBox, typeof(uint).Name, color2);
            ChangeColor_Name(richTextBox, typeof(long).Name, color2);
            ChangeColor_Name(richTextBox, typeof(ulong).Name, color2);

            ChangeColor_Name(richTextBox, typeof(float).Name, color2);
            ChangeColor_Name(richTextBox, typeof(double).Name, color2);
            ChangeColor_Name(richTextBox, typeof(decimal).Name, color2);

            ChangeColor_Name(richTextBox, typeof(DateTime).Name, color2);
            ChangeColor_Name(richTextBox, typeof(TimeSpan).Name, color2);

            ChangeColor_String(richTextBox, color3);
            ChangeColor_ClassName(richTextBox, color2);
        }

        private void ChangeColor_Name(RichTextBox richTextBox, string name, Color color)
        {
            ProcessChangeColor(richTextBox, color, (text, startIndex) =>
            {
                int index = text.IndexOf(name, startIndex);
                int length = name.Length;
                return new object[] { index, length };
            });
        }
        private void ChangeColor_String(RichTextBox richTextBox, Color color)
        {
            ProcessChangeColor(richTextBox, color, (text, startIndex) =>
            {
                int index1 = text.IndexOf("\"", startIndex);
                if (index1 == -1)
                    return new object[] { index1 };

                int index2 = text.IndexOf("\"", index1 + 1);
                return new object[] { index1, index2 - index1 + 1 };
            });
        }
        private void ChangeColor_ClassName(RichTextBox richTextBox, Color color)
        {
            ProcessChangeColor(richTextBox, color, (text, startIndex) =>
            {
                int index1 = text.IndexOf("class ", startIndex);
                if (index1 == -1)
                    return new object[] { index1 };

                int index2 = text.IndexOf(" ", index1 + 6);
                int index2_1 = text.IndexOf(":", index1 + 6);
                if (index2_1 != -1 && index2_1 < index2)
                    index2 = index2_1 + 1;
                index2--;

                return new object[] { index1 + 6, index2 - index1 - 6 };
            });
        }

        private void ProcessChangeColor(RichTextBox richTextBox, Color color, Func<string, int, object[]> func)
        {
            string text = richTextBox.Text.Replace("\r\n", "\n");
            int startIndex = 0;
            while (true)
            {
                object[] result = func(text, startIndex);
                int index = (int)result[0];

                if (index == -1)
                    break;

                int length = (int)result[1];
                startIndex = index + length;

                richTextBox.Select(index, length);
                richTextBox.SelectionColor = color;
            }
            richTextBox.Select(0, 0);
        }

        private Color color1 = Color.FromArgb(0, 0, 255);
        private Color color2 = Color.FromArgb(43, 171, 212);
        private Color color3 = Color.FromArgb(163, 21, 21);

        #endregion
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
