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
            System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
            this.textBox = new MyLibrary.Win32.Controls.MyTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.status = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateFileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._clearRecent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.дополнительноToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useCommentsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useUserNamesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateMetaMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mode1MenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox.BackColor = System.Drawing.SystemColors.Control;
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox.Location = new System.Drawing.Point(0, 27);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ReadOnly = true;
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox.Size = new System.Drawing.Size(868, 488);
            this.textBox.TabIndex = 9;
            this.textBox.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.status,
            toolStripStatusLabel1});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 520);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(868, 20);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // status
            // 
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(118, 15);
            this.status.Text = "toolStripStatusLabel1";
            this.status.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.дополнительноToolStripMenuItem,
            this.copyMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(868, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openFileMenuItem,
            this.updateFileMenuItem,
            this.recentMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // openFileMenuItem
            // 
            this.openFileMenuItem.Name = "openFileMenuItem";
            this.openFileMenuItem.Size = new System.Drawing.Size(168, 22);
            this.openFileMenuItem.Text = "Открыть...";
            this.openFileMenuItem.Click += new System.EventHandler(this.OpenFile_Click);
            // 
            // updateFileMenuItem
            // 
            this.updateFileMenuItem.Name = "updateFileMenuItem";
            this.updateFileMenuItem.Size = new System.Drawing.Size(168, 22);
            this.updateFileMenuItem.Text = "Обновить";
            this.updateFileMenuItem.Click += new System.EventHandler(this.Update_Click);
            // 
            // recentMenuItem
            // 
            this.recentMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._clearRecent,
            this.toolStripSeparator1});
            this.recentMenuItem.Name = "recentMenuItem";
            this.recentMenuItem.Size = new System.Drawing.Size(168, 22);
            this.recentMenuItem.Text = "Недавние файлы";
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
            this.useCommentsMenuItem,
            this.useUserNamesMenuItem,
            this.updateMetaMenuItem,
            this.toolStripSeparator2,
            this.mode1MenuItem});
            this.дополнительноToolStripMenuItem.Name = "дополнительноToolStripMenuItem";
            this.дополнительноToolStripMenuItem.Size = new System.Drawing.Size(107, 20);
            this.дополнительноToolStripMenuItem.Text = "Дополнительно";
            // 
            // useCommentsMenuItem
            // 
            this.useCommentsMenuItem.Checked = true;
            this.useCommentsMenuItem.CheckOnClick = true;
            this.useCommentsMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useCommentsMenuItem.Name = "useCommentsMenuItem";
            this.useCommentsMenuItem.Size = new System.Drawing.Size(292, 22);
            this.useCommentsMenuItem.Text = "Использовать комментарии";
            this.useCommentsMenuItem.Click += new System.EventHandler(this.UseComments_Click);
            // 
            // useUserNamesMenuItem
            // 
            this.useUserNamesMenuItem.Checked = true;
            this.useUserNamesMenuItem.CheckOnClick = true;
            this.useUserNamesMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useUserNamesMenuItem.Name = "useUserNamesMenuItem";
            this.useUserNamesMenuItem.Size = new System.Drawing.Size(292, 22);
            this.useUserNamesMenuItem.Text = "Использовать пользовательские имена";
            this.useUserNamesMenuItem.Click += new System.EventHandler(this.UseCustomNames_Click);
            // 
            // updateMetaMenuItem
            // 
            this.updateMetaMenuItem.Name = "updateMetaMenuItem";
            this.updateMetaMenuItem.Size = new System.Drawing.Size(292, 22);
            this.updateMetaMenuItem.Text = "Создать/обновить файл метаданных";
            this.updateMetaMenuItem.Click += new System.EventHandler(this.UpdateMeta_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(289, 6);
            // 
            // mode1MenuItem
            // 
            this.mode1MenuItem.CheckOnClick = true;
            this.mode1MenuItem.Name = "mode1MenuItem";
            this.mode1MenuItem.Size = new System.Drawing.Size(292, 22);
            this.mode1MenuItem.Text = "Режим 1";
            this.mode1MenuItem.Click += new System.EventHandler(this.Mode1_Click);
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.copyMenuItem.Name = "copyMenuItem";
            this.copyMenuItem.Size = new System.Drawing.Size(88, 20);
            this.copyMenuItem.Text = "Копировать";
            this.copyMenuItem.Click += new System.EventHandler(this.Copy_Click);
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new System.Drawing.Size(10, 15);
            toolStripStatusLabel1.Text = " ";
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 540);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.textBox);
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
        private MyLibrary.Win32.Controls.MyTextBox textBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel status;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem дополнительноToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useCommentsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useUserNamesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateMetaMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _clearRecent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem openFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem updateFileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mode1MenuItem;
    }
}

