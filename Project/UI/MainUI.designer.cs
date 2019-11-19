using System;
using System.Windows.Forms;

namespace Window
{
    partial class MainUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainUI));
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.timerSimulate = new System.Windows.Forms.Timer(this.components);
            this.panelAll = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.工程ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.新建工程ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.打开工程ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.编辑工程设置ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.menuStrip3 = new System.Windows.Forms.MenuStrip();
            this.播放ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.menu_Start = new System.Windows.Forms.ToolStripMenuItem();
            this.停止ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.timerProgressBar1 = new System.Windows.Forms.Timer(this.components);
            this.BottomToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.TopToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.RightToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.LeftToolStripPanel = new System.Windows.Forms.ToolStripPanel();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.doubleBufferPanel1 = new Windows.DoubleBufferPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.SidePanel = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.cb_drawHeatmap = new System.Windows.Forms.CheckBox();
            this.checkBox_outputVRS = new System.Windows.Forms.CheckBox();
            this.cB_ShowExits = new System.Windows.Forms.CheckBox();
            this.cB_DrawColorRule = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelPanelInfo = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.BackGroundPanel = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.chartOuts = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.groupBoxDebug = new System.Windows.Forms.GroupBox();
            this.labelDebugInfo = new System.Windows.Forms.Label();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.groupBox_ChangeArea = new System.Windows.Forms.GroupBox();
            this.cB_AreasList = new System.Windows.Forms.ComboBox();
            this.btn_SaveMesh = new System.Windows.Forms.Button();
            this.groupBox_Weight = new System.Windows.Forms.GroupBox();
            this.tB_Weight = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rB_corwds = new System.Windows.Forms.RadioButton();
            this.rB_heatmap = new System.Windows.Forms.RadioButton();
            this.groupBox_paramenter = new System.Windows.Forms.GroupBox();
            this.label_panelChangeParamenter = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cB_showGoalNow = new System.Windows.Forms.CheckBox();
            this.chBoxChart = new System.Windows.Forms.CheckBox();
            this.cB_ShowGird = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rB_Border = new System.Windows.Forms.RadioButton();
            this.rB_AreaSet = new System.Windows.Forms.RadioButton();
            this.rB_AreaPartition = new System.Windows.Forms.RadioButton();
            this.rB_Mesh = new System.Windows.Forms.RadioButton();
            this.panelReadPlay = new System.Windows.Forms.TableLayoutPanel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.trackBar_speed = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox_Stop = new System.Windows.Forms.PictureBox();
            this.pictureBox_Play = new System.Windows.Forms.PictureBox();
            this.label_FrameMax = new System.Windows.Forms.Label();
            this.trackBar_replay = new System.Windows.Forms.TrackBar();
            this.label_frameMin = new System.Windows.Forms.Label();
            this.panelAll.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel8.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.menuStrip3.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.doubleBufferPanel1.SuspendLayout();
            this.SidePanel.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.BackGroundPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartOuts)).BeginInit();
            this.groupBoxDebug.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.groupBox_ChangeArea.SuspendLayout();
            this.groupBox_Weight.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox_paramenter.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panelReadPlay.SuspendLayout();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_speed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Stop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Play)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_replay)).BeginInit();
            this.SuspendLayout();
            // 
            // timerSimulate
            // 
            this.timerSimulate.Tick += new System.EventHandler(this.timerSimulate_Tick);
            // 
            // panelAll
            // 
            this.panelAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.panelAll.Controls.Add(this.doubleBufferPanel1);
            this.panelAll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAll.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panelAll.Location = new System.Drawing.Point(0, 0);
            this.panelAll.Name = "panelAll";
            this.panelAll.Size = new System.Drawing.Size(1229, 841);
            this.panelAll.TabIndex = 22;
            this.panelAll.Text = "0";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(95)))), ((int)(((byte)(111)))));
            this.panel2.Controls.Add(this.panel8);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1229, 61);
            this.panel2.TabIndex = 45;
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.menuStrip1);
            this.panel8.Controls.Add(this.panel4);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel8.Location = new System.Drawing.Point(0, 0);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(445, 61);
            this.panel8.TabIndex = 49;
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.工程ToolStripMenuItem,
            this.编辑ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(9, 5);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(170, 51);
            this.menuStrip1.TabIndex = 42;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 工程ToolStripMenuItem
            // 
            this.工程ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.新建工程ToolStripMenuItem,
            this.打开工程ToolStripMenuItem,
            this.编辑ToolStripMenuItem1});
            this.工程ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.工程ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.工程ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("工程ToolStripMenuItem.Image")));
            this.工程ToolStripMenuItem.Name = "工程ToolStripMenuItem";
            this.工程ToolStripMenuItem.Size = new System.Drawing.Size(80, 47);
            this.工程ToolStripMenuItem.Text = "工程  ";
            // 
            // 新建工程ToolStripMenuItem
            // 
            this.新建工程ToolStripMenuItem.Image = global::test.Properties.Resources.newProject;
            this.新建工程ToolStripMenuItem.Name = "新建工程ToolStripMenuItem";
            this.新建工程ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.新建工程ToolStripMenuItem.Text = "新建工程";
            this.新建工程ToolStripMenuItem.Click += new System.EventHandler(this.新建工程ToolStripMenuItem_Click);
            // 
            // 打开工程ToolStripMenuItem
            // 
            this.打开工程ToolStripMenuItem.Name = "打开工程ToolStripMenuItem";
            this.打开工程ToolStripMenuItem.Size = new System.Drawing.Size(144, 26);
            this.打开工程ToolStripMenuItem.Text = "打开工程";
            this.打开工程ToolStripMenuItem.Click += new System.EventHandler(this.打开工程ToolStripMenuItem_Click);
            // 
            // 编辑ToolStripMenuItem1
            // 
            this.编辑ToolStripMenuItem1.Name = "编辑ToolStripMenuItem1";
            this.编辑ToolStripMenuItem1.Size = new System.Drawing.Size(144, 26);
            this.编辑ToolStripMenuItem1.Text = "另存为";
            this.编辑ToolStripMenuItem1.Click += new System.EventHandler(this.编辑ToolStripMenuItem1_Click);
            // 
            // 编辑ToolStripMenuItem
            // 
            this.编辑ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.编辑工程设置ToolStripMenuItem1});
            this.编辑ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.编辑ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.编辑ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("编辑ToolStripMenuItem.Image")));
            this.编辑ToolStripMenuItem.Name = "编辑ToolStripMenuItem";
            this.编辑ToolStripMenuItem.Size = new System.Drawing.Size(80, 47);
            this.编辑ToolStripMenuItem.Text = "编辑  ";
            this.编辑ToolStripMenuItem.Click += new System.EventHandler(this.编辑ToolStripMenuItem_Click);
            // 
            // 编辑工程设置ToolStripMenuItem1
            // 
            this.编辑工程设置ToolStripMenuItem1.Image = ((System.Drawing.Image)(resources.GetObject("编辑工程设置ToolStripMenuItem1.Image")));
            this.编辑工程设置ToolStripMenuItem1.Name = "编辑工程设置ToolStripMenuItem1";
            this.编辑工程设置ToolStripMenuItem1.Size = new System.Drawing.Size(144, 26);
            this.编辑工程设置ToolStripMenuItem1.Text = "工程设置";
            this.编辑工程设置ToolStripMenuItem1.Click += new System.EventHandler(this.编辑工程设置ToolStripMenuItem1_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel4.BackgroundImage")));
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Controls.Add(this.label6);
            this.panel4.Location = new System.Drawing.Point(194, -7);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(253, 55);
            this.panel4.TabIndex = 44;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.BackColor = System.Drawing.Color.Transparent;
            this.label6.Font = new System.Drawing.Font("Microsoft YaHei", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.label6.Location = new System.Drawing.Point(59, 18);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 27);
            this.label6.TabIndex = 0;
            this.label6.Text = "人群疏散系统";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.progressBar1);
            this.panel3.Controls.Add(this.menuStrip3);
            this.panel3.Controls.Add(this.menuStrip2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel3.Location = new System.Drawing.Point(647, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(582, 61);
            this.panel3.TabIndex = 48;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.BackColor = System.Drawing.Color.Red;
            this.progressBar1.ForeColor = System.Drawing.Color.Black;
            this.progressBar1.Location = new System.Drawing.Point(323, 16);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(250, 30);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TabIndex = 46;
            this.progressBar1.Visible = false;
            // 
            // menuStrip3
            // 
            this.menuStrip3.AllowDrop = true;
            this.menuStrip3.AutoSize = false;
            this.menuStrip3.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip3.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.播放ToolStripMenuItem});
            this.menuStrip3.Location = new System.Drawing.Point(227, 5);
            this.menuStrip3.Name = "menuStrip3";
            this.menuStrip3.Size = new System.Drawing.Size(104, 51);
            this.menuStrip3.TabIndex = 47;
            this.menuStrip3.Text = "menuStrip3";
            // 
            // 播放ToolStripMenuItem
            // 
            this.播放ToolStripMenuItem.Enabled = false;
            this.播放ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.播放ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.播放ToolStripMenuItem.Image = global::test.Properties.Resources.play;
            this.播放ToolStripMenuItem.Name = "播放ToolStripMenuItem";
            this.播放ToolStripMenuItem.Size = new System.Drawing.Size(96, 47);
            this.播放ToolStripMenuItem.Text = " 播放  ";
            this.播放ToolStripMenuItem.Click += new System.EventHandler(this.播放ToolStripMenuItem_Click);
            // 
            // menuStrip2
            // 
            this.menuStrip2.AllowDrop = true;
            this.menuStrip2.AutoSize = false;
            this.menuStrip2.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_Start,
            this.停止ToolStripMenuItem});
            this.menuStrip2.Location = new System.Drawing.Point(43, 5);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(193, 51);
            this.menuStrip2.TabIndex = 43;
            this.menuStrip2.Text = "menuStrip2";
            this.menuStrip2.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MenuStrip2_ItemClicked);
            // 
            // menu_Start
            // 
            this.menu_Start.AutoSize = false;
            this.menu_Start.Enabled = false;
            this.menu_Start.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.menu_Start.ForeColor = System.Drawing.Color.White;
            this.menu_Start.Image = global::test.Properties.Resources.simulate_play;
            this.menu_Start.Name = "menu_Start";
            this.menu_Start.Size = new System.Drawing.Size(90, 47);
            this.menu_Start.Text = " 仿真  ";
            this.menu_Start.Click += new System.EventHandler(this.menu_Start_Click);
            // 
            // 停止ToolStripMenuItem
            // 
            this.停止ToolStripMenuItem.Enabled = false;
            this.停止ToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.停止ToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.停止ToolStripMenuItem.Image = global::test.Properties.Resources.simulate_stop;
            this.停止ToolStripMenuItem.Name = "停止ToolStripMenuItem";
            this.停止ToolStripMenuItem.Size = new System.Drawing.Size(90, 47);
            this.停止ToolStripMenuItem.Text = " 停止 ";
            this.停止ToolStripMenuItem.Click += new System.EventHandler(this.停止ToolStripMenuItem_Click);
            // 
            // timerProgressBar1
            // 
            this.timerProgressBar1.Tick += new System.EventHandler(this.timer_Progress_Tick);
            // 
            // BottomToolStripPanel
            // 
            this.BottomToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.BottomToolStripPanel.Name = "BottomToolStripPanel";
            this.BottomToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.BottomToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.BottomToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // TopToolStripPanel
            // 
            this.TopToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.TopToolStripPanel.Name = "TopToolStripPanel";
            this.TopToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.TopToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.TopToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // RightToolStripPanel
            // 
            this.RightToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.RightToolStripPanel.Name = "RightToolStripPanel";
            this.RightToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.RightToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.RightToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // LeftToolStripPanel
            // 
            this.LeftToolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftToolStripPanel.Name = "LeftToolStripPanel";
            this.LeftToolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.LeftToolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.LeftToolStripPanel.Size = new System.Drawing.Size(0, 0);
            // 
            // ContentPanel
            // 
            this.ContentPanel.Size = new System.Drawing.Size(527, 18);
            // 
            // doubleBufferPanel1
            // 
            this.doubleBufferPanel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(88)))), ((int)(((byte)(88)))));
            this.doubleBufferPanel1.Controls.Add(this.label5);
            this.doubleBufferPanel1.Controls.Add(this.SidePanel);
            this.doubleBufferPanel1.Controls.Add(this.groupBoxDebug);
            this.doubleBufferPanel1.Controls.Add(this.panelReadPlay);
            this.doubleBufferPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.doubleBufferPanel1.Location = new System.Drawing.Point(0, 0);
            this.doubleBufferPanel1.Name = "doubleBufferPanel1";
            this.doubleBufferPanel1.Size = new System.Drawing.Size(1229, 841);
            this.doubleBufferPanel1.TabIndex = 2;
            this.doubleBufferPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.doubleBufferPanel1_Paint);
            this.doubleBufferPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseDown);
            this.doubleBufferPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseMove);
            this.doubleBufferPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseUp);
            this.doubleBufferPanel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.doubleBufferPanel1_MouseWheel);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(752, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 48;
            this.label5.Text = "label5";
            this.label5.Visible = false;
            // 
            // SidePanel
            // 
            this.SidePanel.BackColor = System.Drawing.Color.Transparent;
            this.SidePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("SidePanel.BackgroundImage")));
            this.SidePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.SidePanel.Controls.Add(this.panel6);
            this.SidePanel.Controls.Add(this.panel5);
            this.SidePanel.Controls.Add(this.label4);
            this.SidePanel.Controls.Add(this.BackGroundPanel);
            this.SidePanel.Location = new System.Drawing.Point(-7, 67);
            this.SidePanel.Name = "SidePanel";
            this.SidePanel.Size = new System.Drawing.Size(420, 506);
            this.SidePanel.TabIndex = 45;
            this.SidePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.SidePanel_Paint);
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(90)))), ((int)(((byte)(96)))));
            this.panel6.Controls.Add(this.cb_drawHeatmap);
            this.panel6.Controls.Add(this.checkBox_outputVRS);
            this.panel6.Controls.Add(this.cB_ShowExits);
            this.panel6.Controls.Add(this.cB_DrawColorRule);
            this.panel6.Location = new System.Drawing.Point(37, 454);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(335, 34);
            this.panel6.TabIndex = 46;
            // 
            // cb_drawHeatmap
            // 
            this.cb_drawHeatmap.AutoSize = true;
            this.cb_drawHeatmap.BackColor = System.Drawing.Color.Transparent;
            this.cb_drawHeatmap.Checked = true;
            this.cb_drawHeatmap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb_drawHeatmap.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cb_drawHeatmap.ForeColor = System.Drawing.Color.White;
            this.cb_drawHeatmap.Location = new System.Drawing.Point(194, 8);
            this.cb_drawHeatmap.Name = "cb_drawHeatmap";
            this.cb_drawHeatmap.Size = new System.Drawing.Size(87, 21);
            this.cb_drawHeatmap.TabIndex = 31;
            this.cb_drawHeatmap.Text = "显示热力图";
            this.cb_drawHeatmap.UseVisualStyleBackColor = false;
            this.cb_drawHeatmap.Visible = false;
            this.cb_drawHeatmap.CheckedChanged += new System.EventHandler(this.Cb_drawHeatmap_CheckedChanged);
            // 
            // checkBox_outputVRS
            // 
            this.checkBox_outputVRS.AutoSize = true;
            this.checkBox_outputVRS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(90)))), ((int)(((byte)(96)))));
            this.checkBox_outputVRS.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.checkBox_outputVRS.ForeColor = System.Drawing.Color.White;
            this.checkBox_outputVRS.ImageKey = "(无)";
            this.checkBox_outputVRS.Location = new System.Drawing.Point(194, 35);
            this.checkBox_outputVRS.Name = "checkBox_outputVRS";
            this.checkBox_outputVRS.Size = new System.Drawing.Size(124, 21);
            this.checkBox_outputVRS.TabIndex = 30;
            this.checkBox_outputVRS.Text = "输出vrs文件(测试)";
            this.checkBox_outputVRS.UseVisualStyleBackColor = false;
            this.checkBox_outputVRS.Visible = false;
            // 
            // cB_ShowExits
            // 
            this.cB_ShowExits.AutoSize = true;
            this.cB_ShowExits.BackColor = System.Drawing.Color.Transparent;
            this.cB_ShowExits.Checked = true;
            this.cB_ShowExits.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_ShowExits.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cB_ShowExits.ForeColor = System.Drawing.Color.White;
            this.cB_ShowExits.ImageKey = "(无)";
            this.cB_ShowExits.Location = new System.Drawing.Point(15, 8);
            this.cB_ShowExits.Name = "cB_ShowExits";
            this.cB_ShowExits.Size = new System.Drawing.Size(99, 21);
            this.cB_ShowExits.TabIndex = 27;
            this.cB_ShowExits.Text = "显示出口标识";
            this.cB_ShowExits.UseVisualStyleBackColor = false;
            this.cB_ShowExits.CheckedChanged += new System.EventHandler(this.CB_ShowExits_CheckedChanged);
            // 
            // cB_DrawColorRule
            // 
            this.cB_DrawColorRule.AutoSize = true;
            this.cB_DrawColorRule.BackColor = System.Drawing.Color.Transparent;
            this.cB_DrawColorRule.Checked = true;
            this.cB_DrawColorRule.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cB_DrawColorRule.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.cB_DrawColorRule.ForeColor = System.Drawing.Color.White;
            this.cB_DrawColorRule.Location = new System.Drawing.Point(15, 35);
            this.cB_DrawColorRule.Name = "cB_DrawColorRule";
            this.cB_DrawColorRule.Size = new System.Drawing.Size(123, 21);
            this.cB_DrawColorRule.TabIndex = 29;
            this.cB_DrawColorRule.Text = "显示密度颜色参考";
            this.cB_DrawColorRule.UseVisualStyleBackColor = false;
            this.cB_DrawColorRule.Visible = false;
            this.cB_DrawColorRule.CheckedChanged += new System.EventHandler(this.CB_DrawColorRule_CheckedChanged);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(90)))), ((int)(((byte)(96)))));
            this.panel5.Controls.Add(this.label9);
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.labelPanelInfo);
            this.panel5.Location = new System.Drawing.Point(37, 21);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(335, 142);
            this.panel5.TabIndex = 45;
            this.panel5.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel5_Paint);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(4, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 29;
            this.label9.Text = "label9";
            this.label9.Visible = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(3, 112);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "label8";
            this.label8.Visible = false;
            // 
            // labelPanelInfo
            // 
            this.labelPanelInfo.AutoSize = true;
            this.labelPanelInfo.Font = new System.Drawing.Font("Microsoft YaHei", 10F);
            this.labelPanelInfo.ForeColor = System.Drawing.Color.White;
            this.labelPanelInfo.Location = new System.Drawing.Point(3, 6);
            this.labelPanelInfo.Name = "labelPanelInfo";
            this.labelPanelInfo.Size = new System.Drawing.Size(84, 20);
            this.labelPanelInfo.TabIndex = 27;
            this.labelPanelInfo.Text = "  XXXXXXX ";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("SimSun", 7.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(376, 235);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 35);
            this.label4.TabIndex = 44;
            this.label4.Text = "<<";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label4.Click += new System.EventHandler(this.labelDrowBack_Click);
            // 
            // BackGroundPanel
            // 
            this.BackGroundPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(90)))), ((int)(((byte)(96)))));
            this.BackGroundPanel.Controls.Add(this.label7);
            this.BackGroundPanel.Controls.Add(this.chartOuts);
            this.BackGroundPanel.Location = new System.Drawing.Point(37, 179);
            this.BackGroundPanel.Name = "BackGroundPanel";
            this.BackGroundPanel.Size = new System.Drawing.Size(335, 260);
            this.BackGroundPanel.TabIndex = 44;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Microsoft YaHei", 10F);
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(12, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 20);
            this.label7.TabIndex = 46;
            this.label7.Text = "人数";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // chartOuts
            // 
            this.chartOuts.BackColor = System.Drawing.Color.Transparent;
            chartArea1.AxisX.IntervalOffset = 1D;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelAutoFitStyle = ((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles)((((System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.IncreaseFont | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.DecreaseFont) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.StaggeredLabels) 
            | System.Windows.Forms.DataVisualization.Charting.LabelAutoFitStyles.WordWrap)));
            chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorGrid.Enabled = false;
            chartArea1.AxisX.MajorTickMark.Enabled = false;
            chartArea1.AxisX.Maximum = 13.5D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisX.ScaleBreakStyle.Enabled = true;
            chartArea1.AxisX.Title = "各出口已疏散人数";
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.IsLabelAutoFit = false;
            chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.MajorTickMark.Enabled = false;
            chartArea1.BackColor = System.Drawing.Color.Transparent;
            chartArea1.Name = "ChartArea1";
            this.chartOuts.ChartAreas.Add(chartArea1);
            this.chartOuts.Location = new System.Drawing.Point(0, 27);
            this.chartOuts.Margin = new System.Windows.Forms.Padding(0);
            this.chartOuts.Name = "chartOuts";
            this.chartOuts.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.EarthTones;
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.YValuesPerPoint = 6;
            this.chartOuts.Series.Add(series1);
            this.chartOuts.Size = new System.Drawing.Size(335, 230);
            this.chartOuts.TabIndex = 1;
            this.chartOuts.Click += new System.EventHandler(this.chartOuts_Click);
            // 
            // groupBoxDebug
            // 
            this.groupBoxDebug.Controls.Add(this.labelDebugInfo);
            this.groupBoxDebug.Controls.Add(this.trackBar2);
            this.groupBoxDebug.Controls.Add(this.groupBox_ChangeArea);
            this.groupBoxDebug.Controls.Add(this.groupBox_Weight);
            this.groupBoxDebug.Controls.Add(this.panel1);
            this.groupBoxDebug.Controls.Add(this.groupBox_paramenter);
            this.groupBoxDebug.Controls.Add(this.groupBox2);
            this.groupBoxDebug.Controls.Add(this.groupBox1);
            this.groupBoxDebug.ForeColor = System.Drawing.Color.White;
            this.groupBoxDebug.Location = new System.Drawing.Point(806, 94);
            this.groupBoxDebug.Name = "groupBoxDebug";
            this.groupBoxDebug.Size = new System.Drawing.Size(556, 350);
            this.groupBoxDebug.TabIndex = 47;
            this.groupBoxDebug.TabStop = false;
            this.groupBoxDebug.Text = "自己调试用";
            this.groupBoxDebug.Visible = false;
            // 
            // labelDebugInfo
            // 
            this.labelDebugInfo.AutoSize = true;
            this.labelDebugInfo.Location = new System.Drawing.Point(6, 19);
            this.labelDebugInfo.Name = "labelDebugInfo";
            this.labelDebugInfo.Size = new System.Drawing.Size(31, 13);
            this.labelDebugInfo.TabIndex = 3;
            this.labelDebugInfo.Text = "信息";
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(375, 278);
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(151, 45);
            this.trackBar2.TabIndex = 46;
            // 
            // groupBox_ChangeArea
            // 
            this.groupBox_ChangeArea.Controls.Add(this.cB_AreasList);
            this.groupBox_ChangeArea.Controls.Add(this.btn_SaveMesh);
            this.groupBox_ChangeArea.Location = new System.Drawing.Point(375, 24);
            this.groupBox_ChangeArea.Name = "groupBox_ChangeArea";
            this.groupBox_ChangeArea.Size = new System.Drawing.Size(151, 87);
            this.groupBox_ChangeArea.TabIndex = 7;
            this.groupBox_ChangeArea.TabStop = false;
            this.groupBox_ChangeArea.Text = "更改区域";
            // 
            // cB_AreasList
            // 
            this.cB_AreasList.FormattingEnabled = true;
            this.cB_AreasList.Location = new System.Drawing.Point(17, 22);
            this.cB_AreasList.Name = "cB_AreasList";
            this.cB_AreasList.Size = new System.Drawing.Size(121, 21);
            this.cB_AreasList.TabIndex = 2;
            this.cB_AreasList.SelectedIndexChanged += new System.EventHandler(this.cB_AreasList_SelectedIndexChanged);
            // 
            // btn_SaveMesh
            // 
            this.btn_SaveMesh.Location = new System.Drawing.Point(17, 50);
            this.btn_SaveMesh.Name = "btn_SaveMesh";
            this.btn_SaveMesh.Size = new System.Drawing.Size(121, 25);
            this.btn_SaveMesh.TabIndex = 0;
            this.btn_SaveMesh.Text = "保存";
            this.btn_SaveMesh.UseVisualStyleBackColor = true;
            this.btn_SaveMesh.Click += new System.EventHandler(this.btn_SaveMesh_Click);
            // 
            // groupBox_Weight
            // 
            this.groupBox_Weight.Controls.Add(this.tB_Weight);
            this.groupBox_Weight.Controls.Add(this.button2);
            this.groupBox_Weight.Controls.Add(this.label3);
            this.groupBox_Weight.Location = new System.Drawing.Point(9, 247);
            this.groupBox_Weight.Name = "groupBox_Weight";
            this.groupBox_Weight.Size = new System.Drawing.Size(157, 87);
            this.groupBox_Weight.TabIndex = 8;
            this.groupBox_Weight.TabStop = false;
            this.groupBox_Weight.Text = "更改权重";
            this.groupBox_Weight.Visible = false;
            // 
            // tB_Weight
            // 
            this.tB_Weight.Location = new System.Drawing.Point(57, 22);
            this.tB_Weight.Name = "tB_Weight";
            this.tB_Weight.Size = new System.Drawing.Size(81, 20);
            this.tB_Weight.TabIndex = 4;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(17, 51);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(121, 25);
            this.button2.TabIndex = 0;
            this.button2.Text = "保存";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "权重：";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rB_corwds);
            this.panel1.Controls.Add(this.rB_heatmap);
            this.panel1.Location = new System.Drawing.Point(536, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(74, 52);
            this.panel1.TabIndex = 22;
            this.panel1.Tag = "是对人赋予颜色，还是用只一个热力图";
            // 
            // rB_corwds
            // 
            this.rB_corwds.AutoSize = true;
            this.rB_corwds.Checked = true;
            this.rB_corwds.Location = new System.Drawing.Point(3, 3);
            this.rB_corwds.Name = "rB_corwds";
            this.rB_corwds.Size = new System.Drawing.Size(37, 17);
            this.rB_corwds.TabIndex = 20;
            this.rB_corwds.TabStop = true;
            this.rB_corwds.Text = "人";
            this.rB_corwds.UseVisualStyleBackColor = true;
            // 
            // rB_heatmap
            // 
            this.rB_heatmap.AutoSize = true;
            this.rB_heatmap.Location = new System.Drawing.Point(3, 26);
            this.rB_heatmap.Name = "rB_heatmap";
            this.rB_heatmap.Size = new System.Drawing.Size(61, 17);
            this.rB_heatmap.TabIndex = 21;
            this.rB_heatmap.Text = "热力图";
            this.rB_heatmap.UseVisualStyleBackColor = true;
            this.rB_heatmap.CheckedChanged += new System.EventHandler(this.rB_heatmap_CheckedChanged);
            // 
            // groupBox_paramenter
            // 
            this.groupBox_paramenter.Controls.Add(this.label_panelChangeParamenter);
            this.groupBox_paramenter.Controls.Add(this.button3);
            this.groupBox_paramenter.Location = new System.Drawing.Point(172, 173);
            this.groupBox_paramenter.Name = "groupBox_paramenter";
            this.groupBox_paramenter.Size = new System.Drawing.Size(197, 161);
            this.groupBox_paramenter.TabIndex = 9;
            this.groupBox_paramenter.TabStop = false;
            this.groupBox_paramenter.Text = "更改参数";
            // 
            // label_panelChangeParamenter
            // 
            this.label_panelChangeParamenter.AutoSize = true;
            this.label_panelChangeParamenter.Location = new System.Drawing.Point(15, 26);
            this.label_panelChangeParamenter.Name = "label_panelChangeParamenter";
            this.label_panelChangeParamenter.Size = new System.Drawing.Size(35, 13);
            this.label_panelChangeParamenter.TabIndex = 1;
            this.label_panelChangeParamenter.Text = "label4";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(17, 117);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(121, 25);
            this.button3.TabIndex = 0;
            this.button3.Text = "保存";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cB_showGoalNow);
            this.groupBox2.Controls.Add(this.chBoxChart);
            this.groupBox2.Controls.Add(this.cB_ShowGird);
            this.groupBox2.Location = new System.Drawing.Point(375, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(151, 148);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "显示选项";
            this.groupBox2.Enter += new System.EventHandler(this.GroupBox2_Enter);
            // 
            // cB_showGoalNow
            // 
            this.cB_showGoalNow.AutoSize = true;
            this.cB_showGoalNow.Location = new System.Drawing.Point(16, 28);
            this.cB_showGoalNow.Name = "cB_showGoalNow";
            this.cB_showGoalNow.Size = new System.Drawing.Size(98, 17);
            this.cB_showGoalNow.TabIndex = 9;
            this.cB_showGoalNow.Text = "显示当前目标";
            this.cB_showGoalNow.UseVisualStyleBackColor = true;
            // 
            // chBoxChart
            // 
            this.chBoxChart.AutoSize = true;
            this.chBoxChart.Checked = true;
            this.chBoxChart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chBoxChart.Location = new System.Drawing.Point(16, 76);
            this.chBoxChart.Name = "chBoxChart";
            this.chBoxChart.Size = new System.Drawing.Size(98, 17);
            this.chBoxChart.TabIndex = 13;
            this.chBoxChart.Text = "显示实时信息";
            this.chBoxChart.UseVisualStyleBackColor = true;
            // 
            // cB_ShowGird
            // 
            this.cB_ShowGird.AutoSize = true;
            this.cB_ShowGird.Location = new System.Drawing.Point(16, 52);
            this.cB_ShowGird.Name = "cB_ShowGird";
            this.cB_ShowGird.Size = new System.Drawing.Size(74, 17);
            this.cB_ShowGird.TabIndex = 11;
            this.cB_ShowGird.Text = "显示网格";
            this.cB_ShowGird.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rB_Border);
            this.groupBox1.Controls.Add(this.rB_AreaSet);
            this.groupBox1.Controls.Add(this.rB_AreaPartition);
            this.groupBox1.Controls.Add(this.rB_Mesh);
            this.groupBox1.Location = new System.Drawing.Point(249, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(120, 148);
            this.groupBox1.TabIndex = 42;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "显示模式";
            // 
            // rB_Border
            // 
            this.rB_Border.AutoSize = true;
            this.rB_Border.Location = new System.Drawing.Point(11, 27);
            this.rB_Border.Name = "rB_Border";
            this.rB_Border.Size = new System.Drawing.Size(73, 17);
            this.rB_Border.TabIndex = 4;
            this.rB_Border.Text = "道路边界";
            this.rB_Border.UseVisualStyleBackColor = true;
            // 
            // rB_AreaSet
            // 
            this.rB_AreaSet.AutoSize = true;
            this.rB_AreaSet.Location = new System.Drawing.Point(13, 99);
            this.rB_AreaSet.Name = "rB_AreaSet";
            this.rB_AreaSet.Size = new System.Drawing.Size(97, 17);
            this.rB_AreaSet.TabIndex = 10;
            this.rB_AreaSet.Text = "分区参数设置";
            this.rB_AreaSet.UseVisualStyleBackColor = true;
            this.rB_AreaSet.CheckedChanged += new System.EventHandler(this.rB_AreaSet_CheckedChanged);
            // 
            // rB_AreaPartition
            // 
            this.rB_AreaPartition.AutoSize = true;
            this.rB_AreaPartition.Location = new System.Drawing.Point(11, 75);
            this.rB_AreaPartition.Name = "rB_AreaPartition";
            this.rB_AreaPartition.Size = new System.Drawing.Size(73, 17);
            this.rB_AreaPartition.TabIndex = 6;
            this.rB_AreaPartition.Text = "分区更改";
            this.rB_AreaPartition.UseVisualStyleBackColor = true;
            this.rB_AreaPartition.CheckedChanged += new System.EventHandler(this.rB_AreaPartition_CheckedChanged);
            // 
            // rB_Mesh
            // 
            this.rB_Mesh.AutoSize = true;
            this.rB_Mesh.Checked = true;
            this.rB_Mesh.Location = new System.Drawing.Point(11, 51);
            this.rB_Mesh.Name = "rB_Mesh";
            this.rB_Mesh.Size = new System.Drawing.Size(73, 17);
            this.rB_Mesh.TabIndex = 5;
            this.rB_Mesh.TabStop = true;
            this.rB_Mesh.Text = "三角网格";
            this.rB_Mesh.UseVisualStyleBackColor = true;
            // 
            // panelReadPlay
            // 
            this.panelReadPlay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(93)))), ((int)(((byte)(103)))));
            this.panelReadPlay.ColumnCount = 3;
            this.panelReadPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelReadPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelReadPlay.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.panelReadPlay.Controls.Add(this.panel7, 1, 0);
            this.panelReadPlay.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelReadPlay.Location = new System.Drawing.Point(0, 786);
            this.panelReadPlay.Name = "panelReadPlay";
            this.panelReadPlay.RowCount = 1;
            this.panelReadPlay.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelReadPlay.Size = new System.Drawing.Size(1229, 55);
            this.panelReadPlay.TabIndex = 46;
            this.panelReadPlay.Visible = false;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.label2);
            this.panel7.Controls.Add(this.trackBar_speed);
            this.panel7.Controls.Add(this.label1);
            this.panel7.Controls.Add(this.pictureBox_Stop);
            this.panel7.Controls.Add(this.pictureBox_Play);
            this.panel7.Controls.Add(this.label_FrameMax);
            this.panel7.Controls.Add(this.trackBar_replay);
            this.panel7.Controls.Add(this.label_frameMin);
            this.panel7.Location = new System.Drawing.Point(67, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1094, 49);
            this.panel7.TabIndex = 46;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(103, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 18);
            this.label2.TabIndex = 49;
            this.label2.Text = "×1";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Visible = false;
            // 
            // trackBar_speed
            // 
            this.trackBar_speed.Location = new System.Drawing.Point(75, 0);
            this.trackBar_speed.Maximum = 4;
            this.trackBar_speed.Name = "trackBar_speed";
            this.trackBar_speed.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_speed.Size = new System.Drawing.Size(45, 49);
            this.trackBar_speed.TabIndex = 48;
            this.trackBar_speed.Scroll += new System.EventHandler(this.TrackBar_speed_Scroll);
            this.trackBar_speed.ValueChanged += new System.EventHandler(this.trackBar_speed_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(11, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 44;
            this.label1.Text = "播放速度:";
            // 
            // pictureBox_Stop
            // 
            this.pictureBox_Stop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_Stop.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_Stop.Image")));
            this.pictureBox_Stop.Location = new System.Drawing.Point(1042, 0);
            this.pictureBox_Stop.Name = "pictureBox_Stop";
            this.pictureBox_Stop.Size = new System.Drawing.Size(48, 54);
            this.pictureBox_Stop.TabIndex = 47;
            this.pictureBox_Stop.TabStop = false;
            this.pictureBox_Stop.Click += new System.EventHandler(this.pictureBox_Stop_Click);
            // 
            // pictureBox_Play
            // 
            this.pictureBox_Play.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox_Play.Image = global::test.Properties.Resources.video_play;
            this.pictureBox_Play.Location = new System.Drawing.Point(978, 0);
            this.pictureBox_Play.Name = "pictureBox_Play";
            this.pictureBox_Play.Size = new System.Drawing.Size(48, 54);
            this.pictureBox_Play.TabIndex = 46;
            this.pictureBox_Play.TabStop = false;
            this.pictureBox_Play.Click += new System.EventHandler(this.pictureBox_Play_Click);
            // 
            // label_FrameMax
            // 
            this.label_FrameMax.Location = new System.Drawing.Point(952, 15);
            this.label_FrameMax.Name = "label_FrameMax";
            this.label_FrameMax.Size = new System.Drawing.Size(13, 11);
            this.label_FrameMax.TabIndex = 42;
            this.label_FrameMax.Text = "0";
            this.label_FrameMax.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label_FrameMax.Visible = false;
            // 
            // trackBar_replay
            // 
            this.trackBar_replay.AutoSize = false;
            this.trackBar_replay.BackColor = System.Drawing.SystemColors.Control;
            this.trackBar_replay.Location = new System.Drawing.Point(149, 7);
            this.trackBar_replay.Maximum = 2200;
            this.trackBar_replay.Minimum = 1;
            this.trackBar_replay.Name = "trackBar_replay";
            this.trackBar_replay.Size = new System.Drawing.Size(796, 39);
            this.trackBar_replay.TabIndex = 38;
            this.trackBar_replay.Value = 1;
            this.trackBar_replay.ValueChanged += new System.EventHandler(this.trackBar_replay_ValueChanged);
            this.trackBar_replay.MouseDown += new System.Windows.Forms.MouseEventHandler(this.trackBar_replay_MouseDown);
            this.trackBar_replay.MouseUp += new System.Windows.Forms.MouseEventHandler(this.trackBar_replay_MouseUp);
            // 
            // label_frameMin
            // 
            this.label_frameMin.ForeColor = System.Drawing.Color.White;
            this.label_frameMin.Location = new System.Drawing.Point(105, 15);
            this.label_frameMin.Name = "label_frameMin";
            this.label_frameMin.Size = new System.Drawing.Size(43, 12);
            this.label_frameMin.TabIndex = 39;
            this.label_frameMin.Text = "0";
            this.label_frameMin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label_frameMin.Visible = false;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1229, 841);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelAll);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip2;
            this.MinimumSize = new System.Drawing.Size(990, 600);
            this.Name = "MainUI";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "东门步行街人群疏散系统-空项目";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.newUI_FormClosed);
            this.Load += new System.EventHandler(this.newUI_Load);
            this.panelAll.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel8.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.menuStrip3.ResumeLayout(false);
            this.menuStrip3.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.doubleBufferPanel1.ResumeLayout(false);
            this.doubleBufferPanel1.PerformLayout();
            this.SidePanel.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.BackGroundPanel.ResumeLayout(false);
            this.BackGroundPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chartOuts)).EndInit();
            this.groupBoxDebug.ResumeLayout(false);
            this.groupBoxDebug.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.groupBox_ChangeArea.ResumeLayout(false);
            this.groupBox_Weight.ResumeLayout(false);
            this.groupBox_Weight.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox_paramenter.ResumeLayout(false);
            this.groupBox_paramenter.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelReadPlay.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_speed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Stop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_Play)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_replay)).EndInit();
            this.ResumeLayout(false);

        }

        

        #endregion
        private Timer timerSimulate;
        private Label labelDebugInfo;
        private Panel panelAll;
        private GroupBox groupBox_Weight;
        private TextBox tB_Weight;
        private Button button2;
        private Label label3;
        private ProgressBar progressBar1;
        private Timer timerProgressBar1;
        private ToolStripMenuItem 播放ToolStripMenuItem;
        private MenuStrip menuStrip3;
        private ToolStripMenuItem menu_Start;
        private ToolStripMenuItem 停止ToolStripMenuItem;
        private MenuStrip menuStrip2;
        private ToolStripMenuItem 工程ToolStripMenuItem;
        private ToolStripMenuItem 新建工程ToolStripMenuItem;
        private ToolStripMenuItem 打开工程ToolStripMenuItem;
        private ToolStripMenuItem 编辑ToolStripMenuItem1;
        private ToolStripMenuItem 编辑ToolStripMenuItem;
        private ToolStripMenuItem 编辑工程设置ToolStripMenuItem1;
        private MenuStrip menuStrip1;
        private Panel panel2;
        private Panel panel4;
        private Label label6;
        private ToolStripPanel BottomToolStripPanel;
        private ToolStripPanel TopToolStripPanel;
        private ToolStripPanel RightToolStripPanel;
        private ToolStripPanel LeftToolStripPanel;
        private ToolStripContentPanel ContentPanel;
        private FontDialog fontDialog1;
        public Windows.DoubleBufferPanel doubleBufferPanel1;
        private GroupBox groupBoxDebug;
        private TrackBar trackBar2;
        private GroupBox groupBox_ChangeArea;
        private ComboBox cB_AreasList;
        private Button btn_SaveMesh;
        private Panel panel1;
        private RadioButton rB_corwds;
        private RadioButton rB_heatmap;
        private GroupBox groupBox_paramenter;
        private Label label_panelChangeParamenter;
        private Button button3;
        private GroupBox groupBox2;
        private CheckBox cB_showGoalNow;
        public CheckBox chBoxChart;
        private CheckBox cB_ShowGird;
        private GroupBox groupBox1;
        private RadioButton rB_Border;
        private RadioButton rB_AreaSet;
        private RadioButton rB_AreaPartition;
        private RadioButton rB_Mesh;
        private Panel SidePanel;
        private Panel panel6;
        public CheckBox checkBox_outputVRS;
        public CheckBox cB_ShowExits;
        public CheckBox cB_DrawColorRule;
        private Panel panel5;
        private Label labelPanelInfo;
        private Label label4;
        private Panel BackGroundPanel;
        private Label label7;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartOuts;
        private TableLayoutPanel panelReadPlay;
        private Panel panel7;
        private Label label1;
        private PictureBox pictureBox_Stop;
        private PictureBox pictureBox_Play;
        private Label label_FrameMax;
        public TrackBar trackBar_replay;
        private Label label_frameMin;
        private TrackBar trackBar_speed;
        private Panel panel3;
        private Panel panel8;
        private Label label2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        public CheckBox cb_drawHeatmap;
        private Label label5;
        private Label label8;
        private Label label9;
    }
}