namespace MeetingSystemServer
{
    partial class HistoryMeeting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryMeeting));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AllExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectExportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayRange = new System.Windows.Forms.ToolStripComboBox();
            this.displayMode = new System.Windows.Forms.ToolStripComboBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.historyTree = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.backUpLevelBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.detailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bigViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailListView = new System.Windows.Forms.ListView();
            this.name = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.date = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.size = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.largeImageList = new System.Windows.Forms.ImageList(this.components);
            this.smallImageList = new System.Windows.Forms.ImageList(this.components);
            this.gorupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.topic = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.statrttime = new System.Windows.Forms.TextBox();
            this.endtime = new System.Windows.Forms.TextBox();
            this.department = new System.Windows.Forms.TextBox();
            this.creater = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.gorupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Silver;
            this.tableLayoutPanel3.SetColumnSpan(this.menuStrip1, 2);
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.displayRange,
            this.displayMode});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1095, 39);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Enabled = false;
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(71, 35);
            this.clearToolStripMenuItem.Text = "清除(&C)";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(65, 35);
            this.importToolStripMenuItem.Text = "导入(&I)";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AllExportToolStripMenuItem,
            this.selectExportToolStripMenuItem});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(69, 35);
            this.exportToolStripMenuItem.Text = "导出(&E)";
            // 
            // AllExportToolStripMenuItem
            // 
            this.AllExportToolStripMenuItem.Name = "AllExportToolStripMenuItem";
            this.AllExportToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.AllExportToolStripMenuItem.Text = "全部导出";
            this.AllExportToolStripMenuItem.Click += new System.EventHandler(this.AllExportToolStripMenuItem_Click);
            // 
            // selectExportToolStripMenuItem
            // 
            this.selectExportToolStripMenuItem.Name = "selectExportToolStripMenuItem";
            this.selectExportToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.selectExportToolStripMenuItem.Text = "选择导出";
            this.selectExportToolStripMenuItem.Click += new System.EventHandler(this.selectExportToolStripMenuItem_Click);
            // 
            // displayRange
            // 
            this.displayRange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayRange.Items.AddRange(new object[] {
            "最近一月",
            "最近三月",
            "最近半年",
            "最近一年",
            "全部"});
            this.displayRange.Name = "displayRange";
            this.displayRange.Size = new System.Drawing.Size(150, 35);
            this.displayRange.SelectedIndexChanged += new System.EventHandler(this.displayRange_SelectedIndexChanged);
            // 
            // displayMode
            // 
            this.displayMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.displayMode.Items.AddRange(new object[] {
            "按时间顺序",
            "按时间倒序",
            "按会议空间大小顺序",
            "按会议空间大小倒序"});
            this.displayMode.Name = "displayMode";
            this.displayMode.Size = new System.Drawing.Size(180, 35);
            this.displayMode.SelectedIndexChanged += new System.EventHandler(this.displayMode_SelectedIndexChanged);
            // 
            // splitContainer1
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.splitContainer1, 2);
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 42);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.historyTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
            this.splitContainer1.Size = new System.Drawing.Size(1089, 694);
            this.splitContainer1.SplitterDistance = 281;
            this.splitContainer1.TabIndex = 1;
            // 
            // historyTree
            // 
            this.historyTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyTree.Font = new System.Drawing.Font("宋体", 11F);
            this.historyTree.ImageIndex = 0;
            this.historyTree.ImageList = this.imageList1;
            this.historyTree.Location = new System.Drawing.Point(0, 0);
            this.historyTree.Name = "historyTree";
            this.historyTree.SelectedImageIndex = 0;
            this.historyTree.Size = new System.Drawing.Size(281, 694);
            this.historyTree.TabIndex = 0;
            this.historyTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.historyTree_AfterSelect);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "gray.png");
            this.imageList1.Images.SetKeyName(1, "light.png");
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.toolStrip1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.detailListView, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.gorupBox1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 98F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(804, 694);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.backUpLevelBtn,
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 98);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(804, 44);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // backUpLevelBtn
            // 
            this.backUpLevelBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.backUpLevelBtn.Image = ((System.Drawing.Image)(resources.GetObject("backUpLevelBtn.Image")));
            this.backUpLevelBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.backUpLevelBtn.Name = "backUpLevelBtn";
            this.backUpLevelBtn.Size = new System.Drawing.Size(24, 41);
            this.backUpLevelBtn.Text = "返回上一级";
            this.backUpLevelBtn.Click += new System.EventHandler(this.backUpLevelBtn_Click);
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailToolStripMenuItem,
            this.bigViewToolStripMenuItem});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(34, 41);
            this.toolStripDropDownButton1.Text = "查看";
            // 
            // detailToolStripMenuItem
            // 
            this.detailToolStripMenuItem.Name = "detailToolStripMenuItem";
            this.detailToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.detailToolStripMenuItem.Text = "详细视图";
            this.detailToolStripMenuItem.Click += new System.EventHandler(this.detailToolStripMenuItem_Click);
            // 
            // bigViewToolStripMenuItem
            // 
            this.bigViewToolStripMenuItem.Name = "bigViewToolStripMenuItem";
            this.bigViewToolStripMenuItem.Size = new System.Drawing.Size(159, 26);
            this.bigViewToolStripMenuItem.Text = "大图标视图";
            this.bigViewToolStripMenuItem.Click += new System.EventHandler(this.bigViewToolStripMenuItem_Click);
            // 
            // detailListView
            // 
            this.detailListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.name,
            this.date,
            this.size});
            this.detailListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailListView.LargeImageList = this.largeImageList;
            this.detailListView.Location = new System.Drawing.Point(3, 145);
            this.detailListView.Name = "detailListView";
            this.detailListView.Size = new System.Drawing.Size(798, 546);
            this.detailListView.SmallImageList = this.smallImageList;
            this.detailListView.TabIndex = 1;
            this.detailListView.UseCompatibleStateImageBehavior = false;
            this.detailListView.View = System.Windows.Forms.View.Details;
            this.detailListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.detailListView_MouseDoubleClick);
            // 
            // name
            // 
            this.name.Text = "名称";
            this.name.Width = 431;
            // 
            // date
            // 
            this.date.Text = "日期";
            this.date.Width = 111;
            // 
            // size
            // 
            this.size.Text = "大小";
            this.size.Width = 108;
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
            // gorupBox1
            // 
            this.gorupBox1.Controls.Add(this.tableLayoutPanel2);
            this.gorupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gorupBox1.Location = new System.Drawing.Point(3, 3);
            this.gorupBox1.Name = "gorupBox1";
            this.gorupBox1.Size = new System.Drawing.Size(798, 92);
            this.gorupBox1.TabIndex = 2;
            this.gorupBox1.TabStop = false;
            this.gorupBox1.Text = "会议基本信息:";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 6;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 94F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.87316F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.56342F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 104F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 31.56342F));
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.topic, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.label2, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label5, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.statrttime, 5, 0);
            this.tableLayoutPanel2.Controls.Add(this.endtime, 5, 1);
            this.tableLayoutPanel2.Controls.Add(this.department, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.creater, 3, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 21);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(792, 68);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "会议主题：";
            // 
            // topic
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.topic, 3);
            this.topic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topic.Location = new System.Drawing.Point(97, 3);
            this.topic.Name = "topic";
            this.topic.Size = new System.Drawing.Size(431, 25);
            this.topic.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(534, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "开始时间：";
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(534, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "结束时间：";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "办会部门：";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(278, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 15);
            this.label5.TabIndex = 5;
            this.label5.Text = "承办人：";
            // 
            // statrttime
            // 
            this.statrttime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statrttime.Location = new System.Drawing.Point(638, 3);
            this.statrttime.Name = "statrttime";
            this.statrttime.Size = new System.Drawing.Size(151, 25);
            this.statrttime.TabIndex = 6;
            // 
            // endtime
            // 
            this.endtime.Dock = System.Windows.Forms.DockStyle.Fill;
            this.endtime.Location = new System.Drawing.Point(638, 37);
            this.endtime.Name = "endtime";
            this.endtime.Size = new System.Drawing.Size(151, 25);
            this.endtime.TabIndex = 7;
            // 
            // department
            // 
            this.department.Dock = System.Windows.Forms.DockStyle.Fill;
            this.department.Location = new System.Drawing.Point(97, 37);
            this.department.Name = "department";
            this.department.Size = new System.Drawing.Size(175, 25);
            this.department.TabIndex = 8;
            // 
            // creater
            // 
            this.creater.Dock = System.Windows.Forms.DockStyle.Fill;
            this.creater.Location = new System.Drawing.Point(379, 37);
            this.creater.Name = "creater";
            this.creater.Size = new System.Drawing.Size(149, 25);
            this.creater.TabIndex = 9;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 831F));
            this.tableLayoutPanel3.Controls.Add(this.splitContainer1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.menuStrip1, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1095, 739);
            this.tableLayoutPanel3.TabIndex = 2;
            // 
            // HistoryMeeting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1095, 739);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "HistoryMeeting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "历史会议";
            this.Load += new System.EventHandler(this.HistoryMeeting_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.gorupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AllExportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectExportToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView historyTree;
        private System.Windows.Forms.ListView detailListView;
        private System.Windows.Forms.ColumnHeader name;
        private System.Windows.Forms.ColumnHeader date;
        private System.Windows.Forms.ColumnHeader size;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton backUpLevelBtn;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem detailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bigViewToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox gorupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox topic;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox statrttime;
        private System.Windows.Forms.TextBox endtime;
        private System.Windows.Forms.TextBox department;
        private System.Windows.Forms.TextBox creater;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ImageList smallImageList;
        private System.Windows.Forms.ImageList largeImageList;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ToolStripComboBox displayRange;
        private System.Windows.Forms.ToolStripComboBox displayMode;
    }
}