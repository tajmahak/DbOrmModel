using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using MyLibrary.DataBase;

namespace DbOrmModel
{
    public partial class Form1 : Form
    {
        private string CommentPath
        {
            get
            {
                if (_dbPath == null)
                    return null;
                return _dbPath + ".comment.txt";
            }
        }
        private string UserNamesPath
        {
            get
            {
                if (_dbPath == null)
                    return null;
                return _dbPath + ".usernames.txt";
            }
        }



        private DBModelBase _currentModel;
        private string _dbPath;
        private string[] _args;

        public Form1(string[] args)
        {
            _args = args;
            InitializeComponent();
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            if (_args.Length > 0 && File.Exists(_args[0]))
            {
                Open(_args[0]);
            }
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
        private void buttonCopyDb_Click(object sender, EventArgs e)
        {
            CopyToClipboard(textBox1.Text);
        }
        private void buttonCopyOrm_Click(object sender, EventArgs e)
        {
            CopyToClipboard(textBox2.Text);
        }
        private void buttonCopyDbOrm_Click(object sender, EventArgs e)
        {
            CopyToClipboard(textBox3.Text);
        }

        // комментарии
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
            //!!!
            //PrepareCommentDictionary();

            //string comment;

            //var text = new StringBuilder();
            //foreach (var table in _currentModel.Tables)
            //{
            //    text.AppendLine();
            //    text.Append(table.Name + "\t");
            //    if (_commentDictionary.TryGetValue(table.Name, out comment))
            //    {
            //        text.Append(comment);
            //    }
            //    text.AppendLine();

            //    foreach (var column in table.Columns)
            //    {
            //        var columnName = table.Name + "." + column.Name;
            //        text.Append(columnName + "\t");
            //        if (_commentDictionary.TryGetValue(columnName, out comment))
            //        {
            //            text.Append(comment);
            //        }
            //        text.AppendLine();
            //    }
            //}
            //text.Remove(0, 2); // убирает первую пустую строку

            //File.Delete(CommentPath);
            //File.WriteAllText(CommentPath, text.ToString(), Encoding.UTF8);
        }
        private void PrepareCommentDictionary()
        {
            //!!!
            //_commentDictionary.Clear();
            //if (!File.Exists(CommentPath))
            //    return;

            //foreach (var line in File.ReadAllLines(CommentPath))
            //{
            //    string[] split = line.Split('\t');
            //    if (split.Length != 2)
            //        continue;

            //    var text1 = split[0].Trim();
            //    var text2 = split[1].Trim();
            //    if (text2.Length == 0)
            //        continue;

            //    _commentDictionary.Add(text1, text2);
            //}
        }

        // польз. имена
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
            //!!!
            //PrepareUserNamesDictionary();

            //string userName;

            //var text = new StringBuilder();
            //foreach (var table in _currentModel.Tables)
            //{
            //    text.AppendLine();
            //    text.Append(table.Name + "\t");

            //    if (_userNamesDictionary.TryGetValue(table.Name, out userName))
            //    {
            //        text.Append(userName);
            //    }
            //    else
            //    {
            //        text.Append(table.Name);
            //    }

            //    text.AppendLine();

            //    foreach (var column in table.Columns)
            //    {
            //        var columnName = table.Name + "." + column.Name;
            //        text.Append(columnName + "\t");

            //        var fieldName = column.Name;
            //        if (fieldName.StartsWith(table.Name))
            //            fieldName = fieldName.Remove(0, table.Name.Length + 1);

            //        if (_userNamesDictionary.TryGetValue(columnName, out userName))
            //        {
            //            text.Append(userName);
            //        }
            //        else
            //        {
            //            text.Append(fieldName);
            //        }
            //        text.AppendLine();
            //    }
            //}
            //text.Remove(0, 2); // убирает первую пустую строку

            //File.Delete(UserNamesPath);
            //File.WriteAllText(UserNamesPath, text.ToString(), Encoding.UTF8);
        }
        private void PrepareUserNamesDictionary()
        {
            //!!!
            //_userNamesDictionary.Clear();
            //if (!File.Exists(UserNamesPath))
            //    return;

            //foreach (var line in File.ReadAllLines(UserNamesPath))
            //{
            //    string[] split = line.Split('\t');
            //    if (split.Length != 2)
            //        continue;

            //    var text1 = split[0].Trim();
            //    var text2 = split[1].Trim();
            //    if (text2.Length == 0)
            //        continue;

            //    _userNamesDictionary.Add(text1, text2);
            //}
        }

        private void CreateText(DBModelBase model, bool useComment, bool useUserName)
        {
            //!!!
            //textBox1.Clear();
            //textBox2.Clear();
            //textBox3.Clear();

            //textBox1.Text = strDB.ToString();

            //var str = new StringBuilder();
            //str.Append(strORM.ToString());
            //textBox2.Text = str.ToString();

            //str = new StringBuilder();
            //str.AppendLine(strDB.ToString());
            //str.Append(strORM.ToString());
            //textBox3.Text = str.ToString();
        }
        private void Open(string path)
        {
            _dbPath = path;
            DbConnection connection = null;
            try
            {
                path = path.Trim('\"');
                connection = CreateDataBaseConnection(path);
                _currentModel = new FireBirdDBModel();
                _currentModel.Initialize(connection);
                connection.Dispose();

                //!!!
                //if (checkBoxUseComments.Checked)
                //{
                //    PrepareCommentDictionary();
                //}
                //if (checkBoxUseUserNames.Checked)
                //{
                //    PrepareUserNamesDictionary();
                //}
                //CreateText(_currentModel, checkBoxUseComments.Checked, checkBoxUseUserNames.Checked);
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
        private void CopyToClipboard(string text)
        {
            if (text.Length == 0)
                return;
            Clipboard.SetText(text, TextDataFormat.UnicodeText);
        }
    
    }
}
