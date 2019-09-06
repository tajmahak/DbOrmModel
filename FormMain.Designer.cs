namespace DbOrmModel
{
    partial class FormMain
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this._text = new MyLibrary.WinForms.Controls.MyTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this._status = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._openFile = new System.Windows.Forms.ToolStripMenuItem();
            this._updateFile = new System.Windows.Forms.ToolStripMenuItem();
            this.недавниеФайлыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._clearRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.дополнительноToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._useComments = new System.Windows.Forms.ToolStripMenuItem();
            this._useUserNames = new System.Windows.Forms.ToolStripMenuItem();
            this._updateMeta = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._mode1 = new System.Windows.Forms.ToolStripMenuItem();
            this._copy = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _text
            // 
            this._text.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._text.BackColor = System.Drawing.SystemColors.Control;
            this._text.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._text.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._text.Location = new System.Drawing.Point(0, 27);
            this._text.Multiline = true;
            this._text.Name = "_text";
            this._text.ReadOnly = true;
            this._text.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._text.Size = new System.Drawing.Size(868, 488);
            this._text.TabIndex = 9;
            this._text.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 518);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(868, 22);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // _status
            // 
            this._status.Name = "_status";
            this._status.Size = new System.Drawing.Size(118, 17);
            this._status.Text = "toolStripStatusLabel1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.дополнительноToolStripMenuItem,
            this._copy});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(868, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._openFile,
            this._updateFile,
            this.недавниеФайлыToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // _openFile
            // 
            this._openFile.Name = "_openFile";
            this._openFile.Size = new System.Drawing.Size(168, 22);
            this._openFile.Text = "Открыть...";
            this._openFile.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // _updateFile
            // 
            this._updateFile.Name = "_updateFile";
            this._updateFile.Size = new System.Drawing.Size(168, 22);
            this._updateFile.Text = "Обновить";
            this._updateFile.Click += new System.EventHandler(this.UpdateFile_Click);
            // 
            // недавниеФайлыToolStripMenuItem
            // 
            this.недавниеФайлыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._clearRecent,
            this.toolStripSeparator1});
            this.недавниеФайлыToolStripMenuItem.Name = "недавниеФайлыToolStripMenuItem";
            this.недавниеФайлыToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.недавниеФайлыToolStripMenuItem.Text = "Недавние файлы";
            // 
            // _clearRecent
            // 
            this._clearRecent.Name = "_clearRecent";
            this._clearRecent.Size = new System.Drawing.Size(168, 22);
            this._clearRecent.Text = "Очистить список";
            this._clearRecent.Click += new System.EventHandler(this.ClearRecent_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // дополнительноToolStripMenuItem
            // 
            this.дополнительноToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._useComments,
            this._useUserNames,
            this._updateMeta,
            this.toolStripSeparator2,
            this._mode1});
            this.дополнительноToolStripMenuItem.Name = "дополнительноToolStripMenuItem";
            this.дополнительноToolStripMenuItem.Size = new System.Drawing.Size(107, 20);
            this.дополнительноToolStripMenuItem.Text = "Дополнительно";
            // 
            // _useComments
            // 
            this._useComments.Checked = true;
            this._useComments.CheckOnClick = true;
            this._useComments.CheckState = System.Windows.Forms.CheckState.Checked;
            this._useComments.Name = "_useComments";
            this._useComments.Size = new System.Drawing.Size(292, 22);
            this._useComments.Text = "Использовать комментарии";
            this._useComments.Click += new System.EventHandler(this.UseComments_Click);
            // 
            // _useUserNames
            // 
            this._useUserNames.Checked = true;
            this._useUserNames.CheckOnClick = true;
            this._useUserNames.CheckState = System.Windows.Forms.CheckState.Checked;
            this._useUserNames.Name = "_useUserNames";
            this._useUserNames.Size = new System.Drawing.Size(292, 22);
            this._useUserNames.Text = "Использовать пользовательские имена";
            this._useUserNames.Click += new System.EventHandler(this.UseUserNames_Click);
            // 
            // _updateMeta
            // 
            this._updateMeta.Name = "_updateMeta";
            this._updateMeta.Size = new System.Drawing.Size(292, 22);
            this._updateMeta.Text = "Создать/обновить файл метаданных";
            this._updateMeta.Click += new System.EventHandler(this.UpdateMeta_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(289, 6);
            // 
            // _mode1
            // 
            this._mode1.CheckOnClick = true;
            this._mode1.Name = "_mode1";
            this._mode1.Size = new System.Drawing.Size(292, 22);
            this._mode1.Text = "Режим 1";
            this._mode1.Click += new System.EventHandler(this.Mode1_Click);
            // 
            // _copy
            // 
            this._copy.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this._copy.Name = "_copy";
            this._copy.Size = new System.Drawing.Size(88, 20);
            this._copy.Text = "Копировать";
            this._copy.Click += new System.EventHandler(this.Copy_Click);
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 540);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this._text);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Генератор ORM-модели";
            this.Shown += new System.EventHandler(this.Form_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form_DragEnter);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MyLibrary.WinForms.Controls.MyTextBox _text;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel _status;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem дополнительноToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _useComments;
        private System.Windows.Forms.ToolStripMenuItem _useUserNames;
        private System.Windows.Forms.ToolStripMenuItem _updateMeta;
        private System.Windows.Forms.ToolStripMenuItem недавниеФайлыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _clearRecent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _openFile;
        private System.Windows.Forms.ToolStripMenuItem _updateFile;
        private System.Windows.Forms.ToolStripMenuItem _copy;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem _mode1;
    }
}

