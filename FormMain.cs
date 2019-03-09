using System;
using System.Collections.Generic;
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
        private string CommentFilePath
        {
            get
            {
                if (_dbFilePath == null)
                    return null;
                return _dbFilePath + ".comment.txt";
            }
        }
        private string UserNamesFilePath
        {
            get
            {
                if (_dbFilePath == null)
                    return null;
                return _dbFilePath + ".usernames.txt";
            }
        }

        private string _dbFilePath;
        private string[] _args;
        private OrmModelTextBuilder _builder;
        private List<string> _recentList;
        private const string _recentFilePath = "recent.txt";
        private const int _recentFileCount = 10;

        public FormMain(string[] args)
        {
            _recentList = new List<string>();
            _args = args;
            InitializeComponent();

            UpdateRecentList();
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

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Файлы базы данных (*.FDB)|*.FDB";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Open(dialog.FileName);
            }
        }
        private void recentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem recentItem = (ToolStripMenuItem)sender;
            var filePath = recentItem.Text;

            Open(filePath);
        }
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void очиститьСписокToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.Delete(_recentFilePath);
            UpdateRecentList();
        }
        private void использоватьКомментарииToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open(_dbFilePath);
        }
        private void использоватьПользовательскиеИменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Open(_dbFilePath);
        }
        private void создатьобновитьФайлыМетаданныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dbFilePath == null)
            {
                WriteStatus("База данных не открыта", true);
                return;
            }

            string[] commentContent = null;
            string[] userNamesContent = null;
            if (File.Exists(CommentFilePath))
            {
                commentContent = File.ReadAllLines(CommentFilePath);
            }
            if (File.Exists(UserNamesFilePath))
            {
                userNamesContent = File.ReadAllLines(UserNamesFilePath);
            }

            commentContent = _builder.UpdateCommentContent(commentContent);
            userNamesContent = _builder.UpdateUserNamesContent(userNamesContent);

            if (File.Exists(CommentFilePath))
            {
                File.Delete(CommentFilePath);
            }
            if (File.Exists(UserNamesFilePath))
            {
                File.Delete(UserNamesFilePath);
            }

            File.WriteAllLines(CommentFilePath, commentContent, Encoding.UTF8);
            File.WriteAllLines(UserNamesFilePath, userNamesContent, Encoding.UTF8);

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

            WriteStatus("Загрузка базы данных...", false);

            try
            {
                databasePath = Path.GetFullPath(databasePath);
                if (databasePath != _dbFilePath)
                {
                    _builder = InitializeBuilder(databasePath);
                }
                _dbFilePath = databasePath;


                string[] commentContent = null;
                string[] userNamesContent = null;
                if (File.Exists(CommentFilePath))
                {
                    commentContent = File.ReadAllLines(CommentFilePath);
                }
                if (File.Exists(UserNamesFilePath))
                {
                    userNamesContent = File.ReadAllLines(UserNamesFilePath);
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

                AddToRecentList(_dbFilePath);
                WriteStatus(_dbFilePath, false);
            }
            catch (Exception ex)
            {
                WriteStatus(ex.Message, true);
            }
        }
        private void UpdateRecentList()
        {
            _recentList.Clear();

            var dropDownItems = недавниеФайлыToolStripMenuItem.DropDownItems;
            for (int i = 2; i < dropDownItems.Count; i++)
            {
                dropDownItems.RemoveAt(i);
                i--;
            }

            if (File.Exists(_recentFilePath))
            {
                foreach (var item in File.ReadAllLines(_recentFilePath, Encoding.UTF8))
                {
                    if (!File.Exists(item))
                        continue;

                    _recentList.Add(item);

                    var recentItem = new ToolStripMenuItem();
                    recentItem.Text = item;
                    recentItem.Click += new EventHandler(recentToolStripMenuItem_Click);
                    dropDownItems.Add(recentItem);
                }
            }
        }
        private void AddToRecentList(string path)
        {
            var index = _recentList.FindIndex(x => x == path);
            if (index != -1)
            {
                _recentList.RemoveAt(index);
            }

            if (_recentList.Count < _recentFileCount)
            {
                _recentList.Insert(0, path);
            }

            File.Delete(_recentFilePath);
            File.WriteAllLines(_recentFilePath, _recentList.ToArray(), Encoding.UTF8);

            UpdateRecentList();
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
            Application.DoEvents();
        }
    }
}
