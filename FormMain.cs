using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DbOrmModel
{
    public partial class FormMain : Form
    {
        private OrmModelProject currentProject;
        private ProgramModel programModel => Program.ProgramModel;
        private readonly string[] args;

        public FormMain(string[] args)
        {
            this.args = args;
            InitializeComponent();
            SetStatus(string.Empty);
            RefreshRecentList();
        }

        private void Form_Shown(object sender, EventArgs e)
        {
            if (args.Length > 0)
            {
                OpenFile(args[0]);
            }
        }

        private void Form_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void Form_DragDrop(object sender, DragEventArgs e)
        {
            string[] dropList = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (dropList.Length > 0)
            {
                OpenFile(dropList[0]);
            }
        }


        private void OpenFile(string path)
        {
            TryOperation(() =>
            {
                SetStatus("Загрузка...");
                currentProject = programModel.CreateProject(path);
                RefreshRecentList();
                GetUpdatedDataFromProject();
                SetStatus(string.Empty);
            });
        }

        private void GetUpdatedDataFromProject()
        {
            currentProject.UpdateMetaData();
            currentProject.UseComments = useCommentsMenuItem.Checked;
            currentProject.UseCustomNames = useUserNamesMenuItem.Checked;
            if (mode1MenuItem.Checked)
            {
                textBox.Text = currentProject.GetTableItemsNamespace(0);
            }
            else
            {
                textBox.Text = currentProject.GetMainDBNamespace(0);
            }
        }

        private void RefreshRecentList()
        {
            ToolStripItemCollection dropDownItems = recentMenuItem.DropDownItems;
            for (int i = 2; i < dropDownItems.Count; i++)
            {
                dropDownItems.RemoveAt(i);
                i--;
            }

            foreach (string recentFile in programModel.RecentList)
            {
                ToolStripMenuItem recentItem = new ToolStripMenuItem
                {
                    Text = recentFile
                };
                recentItem.Click += new EventHandler(OpenRecent_Click);
                dropDownItems.Add(recentItem);
            }
        }

        private void SetStatus(string text, bool isError = false)
        {
            status.ForeColor = isError ? Color.DarkRed : Color.Black;
            status.Text = text;
            Application.DoEvents();
        }

        private bool TryOperation(Action action)
        {
            try
            {
                action.Invoke();
                Application.DoEvents();
                return true;
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
                return false;
            }
        }

        private void CheckOpenProject()
        {
            if (currentProject == null)
            {
                throw new Exception("Файл/проект не открыт.");
            }
        }



        private void OpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    OpenFile(dialog.FileName);
                }
            }
        }

        private void Update_Click(object sender, EventArgs e)
        {
            TryOperation(() =>
            {
                CheckOpenProject();
                GetUpdatedDataFromProject();
            });
        }

        private void OpenRecent_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem recentItem = (ToolStripMenuItem)sender;
            string filePath = recentItem.Text;

            OpenFile(filePath);
        }

        private void ClearRecent_Click(object sender, EventArgs e)
        {
            TryOperation(() =>
            {
                programModel.CrearRecentList();
                RefreshRecentList();
            });
        }

        private void UseComments_Click(object sender, EventArgs e)
        {
            if (currentProject != null)
            {
                GetUpdatedDataFromProject();
            }
        }

        private void UseCustomNames_Click(object sender, EventArgs e)
        {
            if (currentProject != null)
            {
                GetUpdatedDataFromProject();
            }
        }

        private void UpdateMeta_Click(object sender, EventArgs e)
        {
            TryOperation(() =>
            {
                CheckOpenProject();
                currentProject.UpdateMetaData();
                string[] content = currentProject.UploadMetaData();

                string metafilePath = currentProject.GetMetafilePath();
                File.Delete(metafilePath);
                File.WriteAllLines(metafilePath, content, Encoding.UTF8);
                SetStatus("Файл метаданных обновлён");
            });
        }

        private void Mode1_Click(object sender, EventArgs e)
        {
            if (currentProject != null)
            {
                GetUpdatedDataFromProject();
            }
        }

        private void Copy_Click(object sender, EventArgs e)
        {
            string text = textBox.Text;
            if (text.Length > 0)
            {
                Clipboard.SetText(text, TextDataFormat.UnicodeText);
            }
        }
    }
}
