using System;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FirebirdSql.Data.FirebirdClient;
using MyLibrary.DataBase;

namespace DbOrmModel
{
    public partial class FormMain : Form
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

        private string _dbPath;
        private string[] _args;
        private OrmModelTextBuilder _builder;

        public FormMain(string[] args)
        {
            _args = args;
            InitializeComponent();

            WriteStatus(string.Empty, false);
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

        private void использоватьКомментарииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open(_dbPath);
        }
        private void использоватьПользовательскиеИменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open(_dbPath);
        }
        private void создатьобновитьФайлыМетаданныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string[] commentContent = null;
            string[] userNamesContent = null;
            if (File.Exists(CommentPath))
            {
                commentContent = File.ReadAllLines(CommentPath);
            }
            if (File.Exists(UserNamesPath))
            {
                userNamesContent = File.ReadAllLines(UserNamesPath);
            }

            commentContent = _builder.UpdateCommentContent(commentContent);
            userNamesContent = _builder.UpdateUserNamesContent(userNamesContent);

            if (File.Exists(CommentPath))
            {
                File.Delete(CommentPath);
            }
            if (File.Exists(UserNamesPath))
            {
                File.Delete(UserNamesPath);
            }

            File.WriteAllLines(CommentPath, commentContent, Encoding.UTF8);
            File.WriteAllLines(UserNamesPath, userNamesContent, Encoding.UTF8);

            WriteStatus("Создание/обновление файлов метаданных выполнено", false);
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

        private void Open(string databasePath)
        {
            if (!File.Exists(databasePath))
                return;

            try
            {
                databasePath = Path.GetFullPath(databasePath);
                if (databasePath != _dbPath)
                {
                    _builder = InitializeBuilder(databasePath);
                }
                _dbPath = databasePath;


                string[] commentContent = null;
                string[] userNamesContent = null;
                if (File.Exists(CommentPath))
                {
                    commentContent = File.ReadAllLines(CommentPath);
                }
                if (File.Exists(UserNamesPath))
                {
                    userNamesContent = File.ReadAllLines(UserNamesPath);
                }
                _builder.PrepareCommentDictionary(commentContent);
                _builder.PrepareUserNamesDictionary(userNamesContent);


                _builder.UseComments = использоватьКомментарииToolStripMenuItem.Checked;
                _builder.UseUserNames = использоватьПользовательскиеИменаToolStripMenuItem.Checked;

                var dbText = _builder.CreateDbText();
                var ormText = _builder.CreateOrmText();

                textBox1.Text = dbText;
                textBox2.Text = ormText;
                textBox3.Text = dbText + Environment.NewLine + ormText;

                WriteStatus(string.Empty, false);
            }
            catch (Exception ex)
            {
                WriteStatus(ex.Message, true);
            }
        }
        private OrmModelTextBuilder InitializeBuilder(string databasePath)
        {
            DbConnection connection = null;
            try
            {
                connection = CreateDataBaseConnection(databasePath);
                var model = new FireBirdDBModel();
                model.Initialize(connection);
                return new OrmModelTextBuilder(model);
            }
            catch
            {
                throw;
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
        private void WriteStatus(string text, bool error)
        {
            toolStripStatusLabel1.ForeColor = error ? Color.DarkRed : Color.Black;
            toolStripStatusLabel1.Text = text;
        }
    }
}
