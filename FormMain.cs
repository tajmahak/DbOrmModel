using DbOrmModel.Properties;
using FirebirdSql.Data.FirebirdClient;
using MyLibrary.DataBase;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DbOrmModel
{
    public partial class FormMain : Form
    {
        private string MetaFilePath
        {
            get
            {
                if (_dbFilePath == null)
                {
                    return null;
                }

                return _dbFilePath + ".meta.txt";
            }
        }

        private string _dbFilePath;
        private readonly string[] _args;
        private OrmModelTextBuilder _builder;
        private readonly MetaManager _metaManager = new MetaManager();
        private readonly List<string> _recentList;
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
        private void Form_Shown(object sender, EventArgs e)
        {
            if (_args.Length > 0 && File.Exists(_args[0]))
            {
                Open(_args[0]);
            }
        }
        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            var path = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (path.Length == 0)
            {
                return;
            }

            Open(path[0]);
        }

        private void UpdateInfo()
        {
            if (_dbFilePath == null)
            {
                WriteStatus("База данных не открыта", true);
                return;
            }
            Open(_dbFilePath);
        }

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Файлы базы данных (*.FDB)|*.FDB"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Open(dialog.FileName);
            }
        }
        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateInfo();
        }
        private void recentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var recentItem = (ToolStripMenuItem)sender;
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
            UpdateInfo();
        }
        private void использоватьПользовательскиеИменаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateInfo();
        }
        private void использоватьОтладочнуюИнформациюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateInfo();
        }
        private void создатьобновитьФайлМетаданныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_dbFilePath == null)
            {
                WriteStatus("База данных не открыта", true);
                return;
            }

            if (File.Exists(MetaFilePath))
            {
                File.Delete(MetaFilePath);
            }

            var content = _metaManager.UploadInfo(_builder.Model);
            File.WriteAllLines(MetaFilePath, content, Encoding.UTF8);

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
            {
                return;
            }

            WriteStatus("Загрузка базы данных...", false);

            try
            {
                databasePath = Path.GetFullPath(databasePath);
                if (databasePath != _dbFilePath)
                {
                    _builder = InitializeBuilder(databasePath);
                }
                _dbFilePath = databasePath;
                AddToRecentList(_dbFilePath);

                _metaManager.Clear();
                _metaManager.UseComments = использоватьКомментарииToolStripMenuItem.Checked;
                _metaManager.UseUserNames = использоватьПользовательскиеИменаToolStripMenuItem.Checked;
                _metaManager.UseToString = использоватьОтладочнуюИнформациюToolStripMenuItem.Checked;

                if (File.Exists(MetaFilePath))
                {
                    var content = File.ReadAllLines(MetaFilePath);
                    _metaManager.LoadInfo(_builder.Model, content);
                }
                else
                {
                    _metaManager.LoadInfo(_builder.Model, null);
                }

                var dbText = _builder.CreateDbText(_metaManager);
                var ormText = _builder.CreateOrmText(_metaManager);

                textBox1.Text = dbText;
                textBox2.Text = ormText;
                textBox3.Text = dbText + Environment.NewLine + ormText;

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
            for (var i = 2; i < dropDownItems.Count; i++)
            {
                dropDownItems.RemoveAt(i);
                i--;
            }

            if (File.Exists(_recentFilePath))
            {
                foreach (var item in File.ReadAllLines(_recentFilePath, Encoding.UTF8))
                {
                    if (!File.Exists(item))
                    {
                        continue;
                    }

                    _recentList.Add(item);

                    var recentItem = new ToolStripMenuItem
                    {
                        Text = item
                    };
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
                {
                    connection.Dispose();
                }
            }
        }
        private DbConnection CreateDataBaseConnection(string path)
        {
            var conBuilder = new FbConnectionStringBuilder
            {
                Dialect = 3,
                UserID = "SYSDBA",
                Password = "masterkey",
                Charset = "WIN1251",
                Database = path
            };

            if (Settings.Default.UseEmbeddedServer == 0)
            {
                conBuilder.ServerType = FbServerType.Default;
                conBuilder.DataSource = "127.0.0.1";
            }
            else
            {
                conBuilder.ServerType = FbServerType.Embedded;
                conBuilder.ClientLibrary = Path.GetFullPath("fbclient\\fbembed.dll");
            }

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
            {
                return;
            }

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
