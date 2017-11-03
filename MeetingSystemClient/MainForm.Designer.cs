namespace MeetingSystemClient
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exitClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.meetingTopic = new System.Windows.Forms.TextBox();
            this.meetingDepart = new System.Windows.Forms.TextBox();
            this.exitSystemBtn = new System.Windows.Forms.PictureBox();
            this.label3 = new System.Windows.Forms.Label();
            this.creater = new System.Windows.Forms.TextBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.localSpace = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.uploadBtn = new System.Windows.Forms.ToolStripButton();
            this.clearLocalSpaceBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.detailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backUpBtn = new System.Windows.Forms.ToolStripButton();
            this.detailListView = new System.Windows.Forms.ListView();
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.largeImageList = new System.Windows.Forms.ImageList(this.components);
            this.smallImageList = new System.Windows.Forms.ImageList(this.components);
            this.clientSpaceWatcher = new System.IO.FileSystemWatcher();
            this.contextMenuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.exitSystemBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clientSpaceWatcher)).BeginInit();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifyIcon1.BalloonTipText = "已最小化到托盘";
            this.notifyIcon1.BalloonTipTitle = "提示";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "会议系统客户端";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitClientToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(113, 56);
            // 
            // exitClientToolStripMenuItem
            // 
            this.exitClientToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("exitClientToolStripMenuItem.Image")));
            this.exitClientToolStripMenuItem.Name = "exitClientToolStripMenuItem";
            this.exitClientToolStripMenuItem.Size = new System.Drawing.Size(112, 26);
            this.exitClientToolStripMenuItem.Text = "退出";
            this.exitClientToolStripMenuItem.Click += new System.EventHandler(this.exitClientToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(112, 26);
            this.aboutToolStripMenuItem.Text = "关于";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(908, 640);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(902, 53);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "会议基本信息";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 109F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.meetingTopic, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.meetingDepart, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.exitSystemBtn, 6, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.creater, 5, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 21);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(896, 29);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "会议主题：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(396, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "办会部门：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // meetingTopic
            // 
            this.meetingTopic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meetingTopic.Enabled = false;
            this.meetingTopic.Location = new System.Drawing.Point(112, 3);
            this.meetingTopic.Name = "meetingTopic";
            this.meetingTopic.Size = new System.Drawing.Size(278, 25);
            this.meetingTopic.TabIndex = 2;
            // 
            // meetingDepart
            // 
            this.meetingDepart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meetingDepart.Enabled = false;
            this.meetingDepart.Location = new System.Drawing.Point(497, 3);
            this.meetingDepart.Name = "meetingDepart";
            this.meetingDepart.Size = new System.Drawing.Size(136, 25);
            this.meetingDepart.TabIndex = 3;
            // 
            // exitSystemBtn
            // 
            this.exitSystemBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("exitSystemBtn.BackgroundImage")));
            this.exitSystemBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.exitSystemBtn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.exitSystemBtn.Location = new System.Drawing.Point(868, 3);
            this.exitSystemBtn.Name = "exitSystemBtn";
            this.exitSystemBtn.Size = new System.Drawing.Size(25, 23);
            this.exitSystemBtn.TabIndex = 4;
            this.exitSystemBtn.TabStop = false;
            this.exitSystemBtn.Click += new System.EventHandler(this.exitSystemBtn_Click);
            this.exitSystemBtn.MouseEnter += new System.EventHandler(this.exitSystemBtn_MouseEnter);
            this.exitSystemBtn.MouseLeave += new System.EventHandler(this.exitSystemBtn_MouseLeave);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(639, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "办会人：";
            // 
            // creater
            // 
            this.creater.Dock = System.Windows.Forms.DockStyle.Fill;
            this.creater.Enabled = false;
            this.creater.Location = new System.Drawing.Point(726, 3);
            this.creater.Name = "creater";
            this.creater.Size = new System.Drawing.Size(136, 25);
            this.creater.TabIndex = 6;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 62);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.localSpace);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel3);
            this.splitContainer1.Size = new System.Drawing.Size(902, 575);
            this.splitContainer1.SplitterDistance = 227;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 1;
            // 
            // localSpace
            // 
            this.localSpace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.localSpace.Font = new System.Drawing.Font("宋体", 12F);
            this.localSpace.ImageIndex = 0;
            this.localSpace.ImageList = this.imageList1;
            this.localSpace.Location = new System.Drawing.Point(0, 0);
            this.localSpace.Name = "localSpace";
            this.localSpace.SelectedImageIndex = 0;
            this.localSpace.ShowLines = false;
            this.localSpace.Size = new System.Drawing.Size(227, 575);
            this.localSpace.TabIndex = 2;
            this.localSpace.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.localSpace_AfterSelect);
            this.localSpace.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.localSpace_NodeMouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.png");
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.toolStrip1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.detailListView, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(667, 575);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(200)))), ((int)(((byte)(210)))));
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uploadBtn,
            this.clearLocalSpaceBtn,
            this.toolStripSeparator2,
            this.toolStripDropDownButton1,
            this.backUpBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(667, 31);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // uploadBtn
            // 
            this.uploadBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.uploadBtn.Image = ((System.Drawing.Image)(resources.GetObject("uploadBtn.Image")));
            this.uploadBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.uploadBtn.Name = "uploadBtn";
            this.uploadBtn.Size = new System.Drawing.Size(24, 28);
            this.uploadBtn.Text = "数据回传";
            this.uploadBtn.Click += new System.EventHandler(this.uploadBtn_Click);
            // 
            // clearLocalSpaceBtn
            // 
            this.clearLocalSpaceBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearLocalSpaceBtn.Image = ((System.Drawing.Image)(resources.GetObject("clearLocalSpaceBtn.Image")));
            this.clearLocalSpaceBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearLocalSpaceBtn.Name = "clearLocalSpaceBtn";
            this.clearLocalSpaceBtn.Size = new System.Drawing.Size(24, 28);
            this.clearLocalSpaceBtn.Text = "安全清除会议空间";
            this.clearLocalSpaceBtn.Click += new System.EventHandler(this.clearLocalSpaceBtn_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailToolStripMenuItem,
            this.LagerToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(34, 28);
            this.toolStripDropDownButton1.Text = "查看";
            // 
            // detailToolStripMenuItem
            // 
            this.detailToolStripMenuItem.Name = "detailToolStripMenuItem";
            this.detailToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.detailToolStripMenuItem.Text = "详细视图";
            this.detailToolStripMenuItem.Click += new System.EventHandler(this.detailToolStripMenuItem_Click);
            // 
            // LagerToolStripMenuItem
            // 
            this.LagerToolStripMenuItem.Name = "LagerToolStripMenuItem";
            this.LagerToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.LagerToolStripMenuItem.Text = "大图标";
            this.LagerToolStripMenuItem.Click += new System.EventHandler(this.LagerToolStripMenuItem_Click);
            // 
            // backUpBtn
            // 
            this.backUpBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backUpBtn.Image = ((System.Drawing.Image)(resources.GetObject("backUpBtn.Image")));
            this.backUpBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backUpBtn.Name = "backUpBtn";
            this.backUpBtn.Size = new System.Drawing.Size(24, 28);
            this.backUpBtn.Text = "返回上一级";
            this.backUpBtn.Click += new System.EventHandler(this.backUpBtn_Click);
            // 
            // detailListView
            // 
            this.detailListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.date,
            this.size});
            this.detailListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailListView.LargeImageList = this.largeImageList;
            this.detailListView.Location = new System.Drawing.Point(3, 34);
            this.detailListView.Name = "detailListView";
            this.detailListView.Size = new System.Drawing.Size(661, 538);
            this.detailListView.SmallImageList = this.smallImageList;
            this.detailListView.TabIndex = 3;
            this.detailListView.UseCompatibleStateImageBehavior = false;
            this.detailListView.View = System.Windows.Forms.View.Details;
            this.detailListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.detailListView_MouseDoubleClick);
            // 
            // name
            // 
            this.name.Text = "名称";
            this.name.Width = 150;
            // 
            // date
            // 
            this.date.Text = "日期";
            this.date.Width = 150;
            // 
            // size
            // 
            this.size.Text = "大小";
            this.size.Width = 122;
            // 
            // largeImageList
            // 
            this.largeImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("largeImageList.ImageStream")));
            this.largeImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.largeImageList.Images.SetKeyName(0, "folder.png");
            this.largeImageList.Images.SetKeyName(1, "rar.png");
            this.largeImageList.Images.SetKeyName(2, "word.png");
            this.largeImageList.Images.SetKeyName(3, "excel.png");
            this.largeImageList.Images.SetKeyName(4, "ppt.png");
            this.largeImageList.Images.SetKeyName(5, "pdf.png");
            this.largeImageList.Images.SetKeyName(6, "png.png");
            this.largeImageList.Images.SetKeyName(7, "ext.png");
            this.largeImageList.Images.SetKeyName(8, "unkown.png");
            // 
            // smallImageList
            // 
            this.smallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("smallImageList.ImageStream")));
            this.smallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.smallImageList.Images.SetKeyName(0, "folder.png");
            this.smallImageList.Images.SetKeyName(1, "rar.png");
            this.smallImageList.Images.SetKeyName(2, "word.png");
            this.smallImageList.Images.SetKeyName(3, "excel.png");
            this.smallImageList.Images.SetKeyName(4, "ppt.png");
            this.smallImageList.Images.SetKeyName(5, "pdf.png");
            this.smallImageList.Images.SetKeyName(6, "png.png");
            this.smallImageList.Images.SetKeyName(7, "ext.png");
            this.smallImageList.Images.SetKeyName(8, "unkown.png");
            // 
            // clientSpaceWatcher
            // 
            this.clientSpaceWatcher.EnableRaisingEvents = true;
            this.clientSpaceWatcher.IncludeSubdirectories = true;
            this.clientSpaceWatcher.SynchronizingObject = this;
            this.clientSpaceWatcher.Created += new System.IO.FileSystemEventHandler(this.clientSpaceWatcher_Created);
            this.clientSpaceWatcher.Deleted += new System.IO.FileSystemEventHandler(this.clientSpaceWatcher_Deleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(908, 640);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "会议系统客户端";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.exitSystemBtn)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clientSpaceWatcher)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exitClientToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox meetingTopic;
        private System.Windows.Forms.TextBox meetingDepart;
        private System.Windows.Forms.PictureBox exitSystemBtn;
        private System.IO.FileSystemWatcher clientSpaceWatcher;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView localSpace;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton uploadBtn;
        private System.Windows.Forms.ToolStripButton clearLocalSpaceBtn;
        private System.Windows.Forms.ListView detailListView;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem detailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LagerToolStripMenuItem;
        private System.Windows.Forms.ImageList largeImageList;
        private System.Windows.Forms.ImageList smallImageList;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader date;
        private System.Windows.Forms.ColumnHeader size;
        private System.Windows.Forms.ToolStripButton backUpBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox creater;
    }
}

