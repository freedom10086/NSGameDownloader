namespace NSGameDownloader
{
    partial class MainWindow
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchButton = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_update_game = new System.Windows.Forms.ToolStripMenuItem();
            this.updateCnExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.查看帮助ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.发送反馈ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.关于ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar_download = new System.Windows.Forms.ToolStripProgressBar();
            this.label_progress = new System.Windows.Forms.ToolStripStatusLabel();
            this.gameCountLabel = new System.Windows.Forms.Label();
            this.gameSizeLabel = new System.Windows.Forms.Label();
            this.gameLansLabel = new System.Windows.Forms.Label();
            this.localDirLabel = new System.Windows.Forms.LinkLabel();
            this.gameTypeLabel = new System.Windows.Forms.Label();
            this.chCheckBox = new System.Windows.Forms.CheckBox();
            this.gameNameLabel = new System.Windows.Forms.Label();
            this.GamePublisherLabel = new System.Windows.Forms.Label();
            this.downloadCheckBox = new System.Windows.Forms.CheckBox();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gameList = new System.Windows.Forms.ListView();
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gameImageBox = new System.Windows.Forms.PictureBox();
            this.localGameListbox = new System.Windows.Forms.ListBox();
            this.收藏 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gameImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // searchTextBox
            // 
            this.searchTextBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.searchTextBox.Location = new System.Drawing.Point(20, 56);
            this.searchTextBox.Margin = new System.Windows.Forms.Padding(6);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(500, 39);
            this.searchTextBox.TabIndex = 99;
            this.searchTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GameNameInputChange);
            // 
            // searchButton
            // 
            this.searchButton.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.searchButton.Location = new System.Drawing.Point(532, 53);
            this.searchButton.Margin = new System.Windows.Forms.Padding(6);
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(126, 44);
            this.searchButton.TabIndex = 1;
            this.searchButton.Text = "搜索";
            this.searchButton.UseVisualStyleBackColor = true;
            this.searchButton.Click += new System.EventHandler(this.SearchButtonClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.文件ToolStripMenuItem,
            this.帮助ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(9, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(1174, 48);
            this.menuStrip1.TabIndex = 100;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 文件ToolStripMenuItem
            // 
            this.文件ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_update_game,
            this.updateCnExcel});
            this.文件ToolStripMenuItem.Name = "文件ToolStripMenuItem";
            this.文件ToolStripMenuItem.Size = new System.Drawing.Size(82, 42);
            this.文件ToolStripMenuItem.Text = "文件";
            // 
            // menu_update_game
            // 
            this.menu_update_game.Name = "menu_update_game";
            this.menu_update_game.Size = new System.Drawing.Size(324, 44);
            this.menu_update_game.Text = "更新游戏数据库";
            this.menu_update_game.Click += new System.EventHandler(this.UpdateGameDbClick);
            // 
            // updateCnExcel
            // 
            this.updateCnExcel.Name = "updateCnExcel";
            this.updateCnExcel.Size = new System.Drawing.Size(324, 44);
            this.updateCnExcel.Text = "从Excel导入数据";
            this.updateCnExcel.Click += new System.EventHandler(this.UpdateCnExcelClick);
            // 
            // 帮助ToolStripMenuItem
            // 
            this.帮助ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.查看帮助ToolStripMenuItem,
            this.toolStripSeparator1,
            this.发送反馈ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.关于ToolStripMenuItem});
            this.帮助ToolStripMenuItem.Name = "帮助ToolStripMenuItem";
            this.帮助ToolStripMenuItem.Size = new System.Drawing.Size(82, 42);
            this.帮助ToolStripMenuItem.Text = "帮助";
            // 
            // 查看帮助ToolStripMenuItem
            // 
            this.查看帮助ToolStripMenuItem.Name = "查看帮助ToolStripMenuItem";
            this.查看帮助ToolStripMenuItem.Size = new System.Drawing.Size(243, 44);
            this.查看帮助ToolStripMenuItem.Text = "查看帮助";
            this.查看帮助ToolStripMenuItem.Click += new System.EventHandler(this.ViewHelpClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(240, 6);
            // 
            // 发送反馈ToolStripMenuItem
            // 
            this.发送反馈ToolStripMenuItem.Name = "发送反馈ToolStripMenuItem";
            this.发送反馈ToolStripMenuItem.Size = new System.Drawing.Size(243, 44);
            this.发送反馈ToolStripMenuItem.Text = "发送反馈";
            this.发送反馈ToolStripMenuItem.Click += new System.EventHandler(this.SendFeedBackClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(240, 6);
            // 
            // 关于ToolStripMenuItem
            // 
            this.关于ToolStripMenuItem.Name = "关于ToolStripMenuItem";
            this.关于ToolStripMenuItem.Size = new System.Drawing.Size(243, 44);
            this.关于ToolStripMenuItem.Text = "关于";
            this.关于ToolStripMenuItem.Click += new System.EventHandler(this.AboutProgramClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar_download,
            this.label_progress});
            this.statusStrip1.Location = new System.Drawing.Point(0, 1100);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 28, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1174, 22);
            this.statusStrip1.TabIndex = 101;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar_download
            // 
            this.toolStripProgressBar_download.Maximum = 5000;
            this.toolStripProgressBar_download.Name = "toolStripProgressBar_download";
            this.toolStripProgressBar_download.Size = new System.Drawing.Size(200, 29);
            this.toolStripProgressBar_download.Visible = false;
            // 
            // label_progress
            // 
            this.label_progress.ForeColor = System.Drawing.Color.DimGray;
            this.label_progress.LinkColor = System.Drawing.Color.DimGray;
            this.label_progress.Name = "label_progress";
            this.label_progress.Size = new System.Drawing.Size(146, 31);
            this.label_progress.Text = "正在下载xxx";
            this.label_progress.Visible = false;
            // 
            // gameCountLabel
            // 
            this.gameCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.gameCountLabel.BackColor = System.Drawing.Color.Transparent;
            this.gameCountLabel.Font = new System.Drawing.Font("微软雅黑", 7.125F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gameCountLabel.ForeColor = System.Drawing.Color.DimGray;
            this.gameCountLabel.Location = new System.Drawing.Point(974, 1094);
            this.gameCountLabel.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.gameCountLabel.Name = "gameCountLabel";
            this.gameCountLabel.Size = new System.Drawing.Size(176, 22);
            this.gameCountLabel.TabIndex = 103;
            this.gameCountLabel.Text = "总数: 计算中";
            this.gameCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gameSizeLabel
            // 
            this.gameSizeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gameSizeLabel.AutoSize = true;
            this.gameSizeLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gameSizeLabel.Location = new System.Drawing.Point(285, 874);
            this.gameSizeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.gameSizeLabel.Name = "gameSizeLabel";
            this.gameSizeLabel.Size = new System.Drawing.Size(86, 31);
            this.gameSizeLabel.TabIndex = 106;
            this.gameSizeLabel.Text = "大小：";
            // 
            // gameLansLabel
            // 
            this.gameLansLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gameLansLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gameLansLabel.Location = new System.Drawing.Point(285, 998);
            this.gameLansLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.gameLansLabel.Name = "gameLansLabel";
            this.gameLansLabel.Size = new System.Drawing.Size(888, 66);
            this.gameLansLabel.TabIndex = 108;
            this.gameLansLabel.Text = "支持语言：";
            // 
            // localDirLabel
            // 
            this.localDirLabel.ActiveLinkColor = System.Drawing.Color.Black;
            this.localDirLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.localDirLabel.AutoSize = true;
            this.localDirLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.localDirLabel.LinkColor = System.Drawing.Color.Black;
            this.localDirLabel.Location = new System.Drawing.Point(14, 787);
            this.localDirLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.localDirLabel.Name = "localDirLabel";
            this.localDirLabel.Size = new System.Drawing.Size(110, 31);
            this.localDirLabel.TabIndex = 110;
            this.localDirLabel.TabStop = true;
            this.localDirLabel.Text = "本地目录";
            this.localDirLabel.Visible = false;
            this.localDirLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.LocalDirLabelClicked);
            // 
            // gameTypeLabel
            // 
            this.gameTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gameTypeLabel.AutoSize = true;
            this.gameTypeLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gameTypeLabel.Location = new System.Drawing.Point(285, 918);
            this.gameTypeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.gameTypeLabel.Name = "gameTypeLabel";
            this.gameTypeLabel.Size = new System.Drawing.Size(86, 31);
            this.gameTypeLabel.TabIndex = 111;
            this.gameTypeLabel.Text = "类型：";
            // 
            // chCheckBox
            // 
            this.chCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chCheckBox.AutoSize = true;
            this.chCheckBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.chCheckBox.Location = new System.Drawing.Point(1059, 58);
            this.chCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.chCheckBox.Name = "chCheckBox";
            this.chCheckBox.Size = new System.Drawing.Size(94, 35);
            this.chCheckBox.TabIndex = 112;
            this.chCheckBox.Text = "中文";
            this.chCheckBox.UseVisualStyleBackColor = true;
            this.chCheckBox.CheckedChanged += new System.EventHandler(this.HaveCnCheckedChanged);
            // 
            // gameNameLabel
            // 
            this.gameNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gameNameLabel.AutoSize = true;
            this.gameNameLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Bold);
            this.gameNameLabel.Location = new System.Drawing.Point(285, 838);
            this.gameNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.gameNameLabel.Name = "gameNameLabel";
            this.gameNameLabel.Size = new System.Drawing.Size(110, 31);
            this.gameNameLabel.TabIndex = 113;
            this.gameNameLabel.Text = "游戏名：";
            // 
            // GamePublisherLabel
            // 
            this.GamePublisherLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.GamePublisherLabel.AutoSize = true;
            this.GamePublisherLabel.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.GamePublisherLabel.Location = new System.Drawing.Point(285, 958);
            this.GamePublisherLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.GamePublisherLabel.Name = "GamePublisherLabel";
            this.GamePublisherLabel.Size = new System.Drawing.Size(110, 31);
            this.GamePublisherLabel.TabIndex = 114;
            this.GamePublisherLabel.Text = "发行商：";
            // 
            // downloadCheckBox
            // 
            this.downloadCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.downloadCheckBox.AutoSize = true;
            this.downloadCheckBox.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.downloadCheckBox.Location = new System.Drawing.Point(933, 58);
            this.downloadCheckBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.downloadCheckBox.Name = "downloadCheckBox";
            this.downloadCheckBox.Size = new System.Drawing.Size(118, 35);
            this.downloadCheckBox.TabIndex = 115;
            this.downloadCheckBox.Text = "已下载";
            this.downloadCheckBox.UseVisualStyleBackColor = true;
            this.downloadCheckBox.Visible = false;
            this.downloadCheckBox.CheckedChanged += new System.EventHandler(this.DownloadCheckedChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "TID";
            this.columnHeader1.Width = 240;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Tag = "NAME";
            this.columnHeader2.Text = "NAME";
            this.columnHeader2.Width = 340;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "中文";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader7.Width = 70;
            // 
            // gameList
            // 
            this.gameList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gameList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader5,
            this.收藏});
            this.gameList.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gameList.FullRowSelect = true;
            this.gameList.GridLines = true;
            this.gameList.HideSelection = false;
            this.gameList.LabelEdit = true;
            this.gameList.Location = new System.Drawing.Point(20, 110);
            this.gameList.Margin = new System.Windows.Forms.Padding(6);
            this.gameList.Name = "gameList";
            this.gameList.Size = new System.Drawing.Size(1133, 672);
            this.gameList.TabIndex = 0;
            this.gameList.UseCompatibleStateImageBehavior = false;
            this.gameList.View = System.Windows.Forms.View.Details;
            this.gameList.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.GameListColumnClick);
            this.gameList.SelectedIndexChanged += new System.EventHandler(this.GameListSelectedIndexChanged);
            this.gameList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GameListMouseClick);
            this.gameList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.GameListMouseDoubleClick);
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "发行商";
            this.columnHeader8.Width = 180;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "发行日期";
            this.columnHeader5.Width = 140;
            // 
            // gameImageBox
            // 
            this.gameImageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.gameImageBox.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.gameImageBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.gameImageBox.ImageLocation = "";
            this.gameImageBox.Location = new System.Drawing.Point(15, 836);
            this.gameImageBox.Margin = new System.Windows.Forms.Padding(6);
            this.gameImageBox.Name = "gameImageBox";
            this.gameImageBox.Size = new System.Drawing.Size(250, 250);
            this.gameImageBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.gameImageBox.TabIndex = 7;
            this.gameImageBox.TabStop = false;
            this.gameImageBox.Click += new System.EventHandler(this.GameImageClick);
            // 
            // localGameListbox
            // 
            this.localGameListbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.localGameListbox.FormattingEnabled = true;
            this.localGameListbox.ItemHeight = 24;
            this.localGameListbox.Location = new System.Drawing.Point(741, 793);
            this.localGameListbox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.localGameListbox.Name = "localGameListbox";
            this.localGameListbox.Size = new System.Drawing.Size(409, 148);
            this.localGameListbox.TabIndex = 117;
            this.localGameListbox.Visible = false;
            this.localGameListbox.SelectedIndexChanged += new System.EventHandler(this.LocalGameListSelectedIndexChanged);
            // 
            // 收藏
            // 
            this.收藏.Text = "收藏";
            this.收藏.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.收藏.Width = 70;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1174, 1122);
            this.Controls.Add(this.localGameListbox);
            this.Controls.Add(this.downloadCheckBox);
            this.Controls.Add(this.GamePublisherLabel);
            this.Controls.Add(this.gameNameLabel);
            this.Controls.Add(this.chCheckBox);
            this.Controls.Add(this.gameTypeLabel);
            this.Controls.Add(this.localDirLabel);
            this.Controls.Add(this.gameLansLabel);
            this.Controls.Add(this.gameSizeLabel);
            this.Controls.Add(this.gameCountLabel);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.gameImageBox);
            this.Controls.Add(this.gameList);
            this.Controls.Add(this.searchButton);
            this.Controls.Add(this.searchTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MinimumSize = new System.Drawing.Size(1100, 800);
            this.Name = "MainWindow";
            this.Text = "NSGameManager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWindowClosing);
            this.Load += new System.EventHandler(this.FormLoad);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gameImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Button searchButton;
        private System.Windows.Forms.PictureBox gameImageBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem menu_update_game;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar_download;
        private System.Windows.Forms.ToolStripMenuItem 帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 查看帮助ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 发送反馈ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 关于ToolStripMenuItem;
        private System.Windows.Forms.Label gameCountLabel;
        private System.Windows.Forms.Label gameSizeLabel;
        private System.Windows.Forms.Label gameLansLabel;
        private System.Windows.Forms.LinkLabel localDirLabel;
        private System.Windows.Forms.Label gameTypeLabel;
        private System.Windows.Forms.CheckBox chCheckBox;
        private System.Windows.Forms.Label gameNameLabel;
        private System.Windows.Forms.Label GamePublisherLabel;
        private System.Windows.Forms.CheckBox downloadCheckBox;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ListView gameList;
        private System.Windows.Forms.ToolStripStatusLabel label_progress;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ToolStripMenuItem updateCnExcel;
        private System.Windows.Forms.ListBox localGameListbox;
        private System.Windows.Forms.ColumnHeader 收藏;
    }
}

