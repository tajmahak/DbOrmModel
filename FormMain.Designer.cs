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
            this.buttonCopyDb = new System.Windows.Forms.Button();
            this.buttonCopyOrm = new System.Windows.Forms.Button();
            this.buttonCopyDbOrm = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.недавниеФайлыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.очиститьСписокToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.дополнительноToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.использоватьКомментарииToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.использоватьПользовательскиеИменаToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.создатьобновитьФайлыМетаданныхToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.использоватьОтладочнуюИнформациюToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCopyDb
            // 
            this.buttonCopyDb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonCopyDb.Location = new System.Drawing.Point(3, 3);
            this.buttonCopyDb.Name = "buttonCopyDb";
            this.buttonCopyDb.Size = new System.Drawing.Size(283, 39);
            this.buttonCopyDb.TabIndex = 5;
            this.buttonCopyDb.Text = "Копировать\r\nDB";
            this.buttonCopyDb.UseVisualStyleBackColor = true;
            this.buttonCopyDb.Click += new System.EventHandler(this.buttonCopyDb_Click);
            // 
            // buttonCopyOrm
            // 
            this.buttonCopyOrm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonCopyOrm.Location = new System.Drawing.Point(292, 3);
            this.buttonCopyOrm.Name = "buttonCopyOrm";
            this.buttonCopyOrm.Size = new System.Drawing.Size(283, 39);
            this.buttonCopyOrm.TabIndex = 6;
            this.buttonCopyOrm.Text = "Копировать\r\nORM";
            this.buttonCopyOrm.UseVisualStyleBackColor = true;
            this.buttonCopyOrm.Click += new System.EventHandler(this.buttonCopyOrm_Click);
            // 
            // buttonCopyDbOrm
            // 
            this.buttonCopyDbOrm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonCopyDbOrm.Location = new System.Drawing.Point(581, 3);
            this.buttonCopyDbOrm.Name = "buttonCopyDbOrm";
            this.buttonCopyDbOrm.Size = new System.Drawing.Size(284, 39);
            this.buttonCopyDbOrm.TabIndex = 8;
            this.buttonCopyDbOrm.Text = "Копировать\r\nDB+ORM";
            this.buttonCopyDbOrm.UseVisualStyleBackColor = true;
            this.buttonCopyDbOrm.Click += new System.EventHandler(this.buttonCopyDbOrm_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33332F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.Controls.Add(this.buttonCopyDb, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonCopyOrm, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.buttonCopyDbOrm, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.textBox3, 2, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 27);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(868, 488);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.White;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox1.Location = new System.Drawing.Point(3, 48);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(283, 437);
            this.textBox1.TabIndex = 9;
            this.textBox1.WordWrap = false;
            // 
            // textBox2
            // 
            this.textBox2.BackColor = System.Drawing.Color.White;
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox2.Location = new System.Drawing.Point(292, 48);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox2.Size = new System.Drawing.Size(283, 437);
            this.textBox2.TabIndex = 10;
            this.textBox2.WordWrap = false;
            // 
            // textBox3
            // 
            this.textBox3.BackColor = System.Drawing.Color.White;
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox3.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBox3.Location = new System.Drawing.Point(581, 48);
            this.textBox3.Multiline = true;
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox3.Size = new System.Drawing.Size(284, 437);
            this.textBox3.TabIndex = 11;
            this.textBox3.WordWrap = false;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.White;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 518);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(868, 22);
            this.statusStrip1.TabIndex = 16;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.White;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.дополнительноToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(868, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.открытьToolStripMenuItem,
            this.недавниеФайлыToolStripMenuItem,
            this.toolStripSeparator2,
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // открытьToolStripMenuItem
            // 
            this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            this.открытьToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.открытьToolStripMenuItem.Text = "Открыть...";
            this.открытьToolStripMenuItem.Click += new System.EventHandler(this.открытьToolStripMenuItem_Click);
            // 
            // недавниеФайлыToolStripMenuItem
            // 
            this.недавниеФайлыToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.очиститьСписокToolStripMenuItem,
            this.toolStripSeparator1});
            this.недавниеФайлыToolStripMenuItem.Name = "недавниеФайлыToolStripMenuItem";
            this.недавниеФайлыToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.недавниеФайлыToolStripMenuItem.Text = "Недавние файлы";
            // 
            // очиститьСписокToolStripMenuItem
            // 
            this.очиститьСписокToolStripMenuItem.Name = "очиститьСписокToolStripMenuItem";
            this.очиститьСписокToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.очиститьСписокToolStripMenuItem.Text = "Очистить список";
            this.очиститьСписокToolStripMenuItem.Click += new System.EventHandler(this.очиститьСписокToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(165, 6);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(165, 6);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(168, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // дополнительноToolStripMenuItem
            // 
            this.дополнительноToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.использоватьКомментарииToolStripMenuItem,
            this.использоватьПользовательскиеИменаToolStripMenuItem,
            this.использоватьОтладочнуюИнформациюToolStripMenuItem,
            this.создатьобновитьФайлыМетаданныхToolStripMenuItem});
            this.дополнительноToolStripMenuItem.Name = "дополнительноToolStripMenuItem";
            this.дополнительноToolStripMenuItem.Size = new System.Drawing.Size(107, 20);
            this.дополнительноToolStripMenuItem.Text = "Дополнительно";
            // 
            // использоватьКомментарииToolStripMenuItem
            // 
            this.использоватьКомментарииToolStripMenuItem.Checked = true;
            this.использоватьКомментарииToolStripMenuItem.CheckOnClick = true;
            this.использоватьКомментарииToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.использоватьКомментарииToolStripMenuItem.Name = "использоватьКомментарииToolStripMenuItem";
            this.использоватьКомментарииToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.использоватьКомментарииToolStripMenuItem.Text = "Использовать комментарии";
            this.использоватьКомментарииToolStripMenuItem.Click += new System.EventHandler(this.использоватьКомментарииToolStripMenuItem_Click);
            // 
            // использоватьПользовательскиеИменаToolStripMenuItem
            // 
            this.использоватьПользовательскиеИменаToolStripMenuItem.Checked = true;
            this.использоватьПользовательскиеИменаToolStripMenuItem.CheckOnClick = true;
            this.использоватьПользовательскиеИменаToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.использоватьПользовательскиеИменаToolStripMenuItem.Name = "использоватьПользовательскиеИменаToolStripMenuItem";
            this.использоватьПользовательскиеИменаToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.использоватьПользовательскиеИменаToolStripMenuItem.Text = "Использовать пользовательские имена";
            this.использоватьПользовательскиеИменаToolStripMenuItem.Click += new System.EventHandler(this.использоватьПользовательскиеИменаToolStripMenuItem_Click);
            // 
            // создатьобновитьФайлыМетаданныхToolStripMenuItem
            // 
            this.создатьобновитьФайлыМетаданныхToolStripMenuItem.Name = "создатьобновитьФайлыМетаданныхToolStripMenuItem";
            this.создатьобновитьФайлыМетаданныхToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.создатьобновитьФайлыМетаданныхToolStripMenuItem.Text = "Создать/обновить файлы метаданных";
            this.создатьобновитьФайлыМетаданныхToolStripMenuItem.Click += new System.EventHandler(this.создатьобновитьФайлыМетаданныхToolStripMenuItem_Click);
            // 
            // использоватьОтладочнуюИнформациюToolStripMenuItem
            // 
            this.использоватьОтладочнуюИнформациюToolStripMenuItem.CheckOnClick = true;
            this.использоватьОтладочнуюИнформациюToolStripMenuItem.Name = "использоватьОтладочнуюИнформациюToolStripMenuItem";
            this.использоватьОтладочнуюИнформациюToolStripMenuItem.Size = new System.Drawing.Size(301, 22);
            this.использоватьОтладочнуюИнформациюToolStripMenuItem.Text = "Использовать отладочную информацию";
            this.использоватьОтладочнуюИнформациюToolStripMenuItem.Click += new System.EventHandler(this.использоватьОтладочнуюИнформациюToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 540);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Генератор ORM-модели";
            this.Shown += new System.EventHandler(this.Form1_Shown);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCopyDb;
        private System.Windows.Forms.Button buttonCopyOrm;
        private System.Windows.Forms.Button buttonCopyDbOrm;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem дополнительноToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem использоватьКомментарииToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem использоватьПользовательскиеИменаToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem создатьобновитьФайлыМетаданныхToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem недавниеФайлыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem очиститьСписокToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem использоватьОтладочнуюИнформациюToolStripMenuItem;
    }
}

