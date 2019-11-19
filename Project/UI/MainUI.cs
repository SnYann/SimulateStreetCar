using SharpNav;
using SharpNav.IO.Json;
using SharpNav.Pathfinding;
using Simulate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using test.Scripts;
using test.UI;
using System.Diagnostics;
using Control = test.Scripts.Control;

namespace Window
{

    
    public partial class MainUI : Form
    {
        
        /// <summary>
        /// 绘图panel的gc
        /// </summary>
        Graphics gc;

        int stepCounts;//用于判断是否有新的步骤变化
        System.Windows.Forms.Timer timerRead = new System.Windows.Forms.Timer();
        //System.Timers.Timer timerRead = new System.Timers.Timer();

        //线程控制
        bool heatmapThreadControl = false;

        /// <summary>
        /// 控制一开始在屏幕中央
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void newUI_Load(object sender, EventArgs e)
        {
            
            this.StartPosition = FormStartPosition.CenterScreen; //窗体的位置由Location属性决定
            //this.Location = (Point)new Size(600, 15); //窗体的起始位置为0,0 
            labelPanelInfo.Text = "";
            timerforHeatmap.Elapsed += new System.Timers.ElapsedEventHandler(timerforHeatmap_Tick);
            timerforHeatmap.Enabled = false;
            timerforHeatmap.AutoReset = true;

            this.MouseWheel+=new System.Windows.Forms.MouseEventHandler(this.Form_MouseWheel);

        }
        private void newUI_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        public MainUI()
        {
            InitializeComponent();
            menuStrip1.Renderer = new MyRenderer();
            HeatMap.heatmapControl = cb_drawHeatmap.Checked;
            CheckForIllegalCrossThreadCalls = false;

            //采用双缓冲技术的控件必需的设置
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);

            test.Scripts.Control.SampleInit();
            tile = test.Scripts.Control.songnav.tiledNavMesh.GetTileAt(0, 0, 0);
            test.Scripts.Control.UpdateUIpanelInfo += UpdateUIpanelInfoDeligate;
            test.Scripts.Control.UpdateSimulateStatus += UpdateSimulateStatusDeligate;

            //设置timerRead
            timerRead.Interval = 1000;
            //timerRead.Elapsed += new System.Timers.ElapsedEventHandler(timerRead_Tick);
            timerRead.Tick += new System.EventHandler(timerRead_Tick);


            //int i = 100;
            //while(i>-100)
            //{
            //    forfun.Add(new Vector2(i,i));
            //    i -= 2;
            //}
            //for (int i = 0; i < 400; i++)
            //{
            //    forfun.Add(new forfunstruct(Simulate.MathHelper.RandomUniform(-150, 150) + 400, Simulate.MathHelper.RandomUniform(-150, 150) + 400));

            //}
        }

        //public class forfunstruct
        //{
        //    public float x;
        //    public float y;
        //    public float xd= Simulate.MathHelper.RandomUniform(-2, 2) > 0 ? 0.1f : -0.1f;
        //    public float yd= Simulate.MathHelper.RandomUniform(-2, 2) > 0 ? 0.1f : -0.1f;
        //    public Color c = Color.Red;
        //    public forfunstruct(float a,float b)
        //    {
        //        x = a;
        //        y = b;
        //    }
        //}
        //public List<forfunstruct> forfun=new List<forfunstruct>();


        #region 显示所需要的部分变量与复写的一些函数
        //位置偏移
        private PointF _gridLeftTop = new PointF(820,450);

        //是否鼠标按下
        private bool _leftButtonPress = false;

        //鼠标位置
        private PointF _mousePosition = new PointF(0, 0);

        //缩放控制
        private float _zoomOld = 1.0f;
        private float _zoom = 2f;
        private float _zoomMin = 1f;
        private float _zoomMax = 100f;

        //单元格大小
        private int _cellWidth_px = 30;
        private int _cellHeight_px = 30;

        //是否可初始化
        public bool isInitializeAvailable = false;
        //是否可运行
        public bool isRuningAvailable = false;
        //是否可查看结果
        public bool isPlayingAvailable = false;

        //编辑mesh所用的    
        int AreaIdSelected = 0xfe;//代表没有被选择
        float distance = 0;

        float mouseRealX;
        float mouseRealY;
        private void GetRealMouse()
        {
            mouseRealX = _mousePosition.X / _zoom - _gridLeftTop.X / _zoom;
            mouseRealY = _mousePosition.Y / _zoom - _gridLeftTop.Y / _zoom;
        }

        private void doubleBufferPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            var offsetX = e.X - _mousePosition.X;
            var offsetY = e.Y - _mousePosition.Y;
            if (_leftButtonPress)
            {
                _gridLeftTop.X += offsetX;
                _gridLeftTop.Y += offsetY;

                _mousePosition.X = e.X;
                _mousePosition.Y = e.Y;

                this.Refresh();
            }
        }

        private void doubleBufferPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            _leftButtonPress = false;
            this.Cursor = Cursors.Default;
        }

        Vector2 mouseLast = new Vector2(0,0);
        private void doubleBufferPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _mousePosition.X = e.X;
                _mousePosition.Y = e.Y;

                _leftButtonPress = true;
                this.Cursor = Cursors.Hand;


                GetRealMouse();

                SearchAgent(mouseRealX, mouseRealY);
                Console.WriteLine("鼠标坐标：" + mouseRealX + "," + mouseRealY);
                Vector2 mouseNow = new Vector2(mouseRealX, mouseRealY);
                Console.WriteLine("距离上次：" + Simulate.MathHelper.abs(mouseLast - mouseNow));
                mouseLast = new Vector2(mouseRealX, mouseRealY);

                var v1 = new Vector2(24, 13.8f);
                var v2 = new Vector2(mouseRealX, mouseRealY);


                //var agent = Sample._instance[1].getNewAgent(v2, new Vector2(-100, 0), AgentStates.Evacuating, 1, 1, 1);
                //agent.navPoints.Add(new Vector2(-100, 0));
                //Sample._instance[1]._agents.Add(agent);
                //Sample._instance[1].RVOInstance.setneedRefresh();


                distance = Simulate.MathHelper.abs(v1 - v2);
                NavPoint np = test.Scripts.Control.songnav.navMeshQuery.FindNearestPoly(new SharpNav.Geometry.Vector3(mouseRealX, 0, mouseRealY), new SharpNav.Geometry.Vector3(20, 20, 20));

                if (Control.agentsAll!=null)//原来的
                {
                    if (Control.agentsAll.Count == 0) return;
                    //设置poly被选中
                    var tile = test.Scripts.Control.songnav.tiledNavMesh.GetTileAt(0, 0, 0);

                    //当前选择的polyid
                    if (np.Polygon.Id != 0)
                    {
                        int id = test.Scripts.Control.songnav.navMeshQuery.nav.IdManager.DecodePolyIndex(ref np.Polygon);
                        //在选择的多边形不是空的情况下
                        //根据当前功能设置
                        //Console.WriteLine(tile.Polys[id].Area.Id);
                        if (rB_AreaPartition.Checked)//分区更改
                        {
                            if (cB_AreasList.Text == "未选择区域")//未选择区域只能选择，不能取消
                            {
                                tile.Polys[id].Area = Area.None;
                                tB_Weight.Text = tile.Polys[id].Area.Id.ToString();
                                tile.Polys[id].Selected = true;
                            }
                            else
                            {
                                if (tile.Polys[id].Area == GetAreabyName(cB_AreasList.Text))
                                {
                                    tile.Polys[id].Area = Area.None;
                                    tile.Polys[id].Selected = false;
                                }
                                else
                                {
                                    tile.Polys[id].Area = GetAreabyName(cB_AreasList.Text);
                                    tB_Weight.Text = tile.Polys[id].Area.Id.ToString();
                                    tile.Polys[id].Selected = true;
                                    label_panelChangeParamenter.Text = tB_Weight.Text;
                                }
                            }

                        }
                        else if (rB_AreaSet.Checked)//区域参数设置
                        {
                            //tile.Polys[id].Selected = true;
                            AreaIdSelected = tile.Polys[id].Area.Id;
                            foreach (var area in test.Scripts.Control._areas)
                            {
                                if (AreaIdSelected == area.Id)
                                {
                                    //显示一些参数到panel
                                    label_panelChangeParamenter.Text = area.AreaName + "\r\n\r\n";
                                    label_panelChangeParamenter.Text += test.Scripts.Control.getAreaAcreage(tile, AreaIdSelected) + "\r\n\r\n";
                                    label_panelChangeParamenter.Text += "人员数量：" + area.headMaxcount + "\r\n\r\n";

                                    int c = 0;
                                    for (int i = 0; i < test.Scripts.Control._areas.Count; i++)
                                    {
                                        c += test.Scripts.Control._areas[i].headMaxcount;
                                    }
                                    label_panelChangeParamenter.Text += "人员数量：" + c + "\r\n\r\n";
                                    break;
                                }
                            }
                        }
                    }
                    else AreaIdSelected = 0xfe;//代表没选择
                }
                else
                {
                    //设置poly被选中
                    var tile = test.Scripts.Control.songnav.tiledNavMesh.GetTileAt(0, 0, 0);

                    //当前选择的polyid
                    if (np.Polygon.Id != 0)
                    {
                        int id = test.Scripts.Control.songnav.navMeshQuery.nav.IdManager.DecodePolyIndex(ref np.Polygon);
                        //在选择的多边形不是空的情况下
                        //根据当前功能设置
                        //Console.WriteLine(tile.Polys[id].Area.Id);
                        if (rB_AreaPartition.Checked)//分区更改
                        {
                            if (cB_AreasList.Text == "未选择区域")//未选择区域只能选择，不能取消
                            {
                                tile.Polys[id].Area = Area.None;
                                tB_Weight.Text = tile.Polys[id].Area.Id.ToString();
                                tile.Polys[id].Selected = true;
                            }
                            else
                            {
                                if (tile.Polys[id].Area == GetAreabyName(cB_AreasList.Text))
                                {
                                    tile.Polys[id].Area = Area.None;
                                    tile.Polys[id].Selected = false;
                                }
                                else
                                {
                                    tile.Polys[id].Area = GetAreabyName(cB_AreasList.Text);
                                    tB_Weight.Text = tile.Polys[id].Area.Id.ToString();
                                    tile.Polys[id].Selected = true;
                                    label_panelChangeParamenter.Text = tB_Weight.Text;
                                }
                            }

                        }
                        else if (rB_AreaSet.Checked)//区域参数设置
                        {
                            //tile.Polys[id].Selected = true;
                            AreaIdSelected = tile.Polys[id].Area.Id;
                            foreach (var area in test.Scripts.Control._areas)
                            {
                                if (AreaIdSelected == area.Id)
                                {
                                    //显示一些参数到panel
                                    label_panelChangeParamenter.Text = area.AreaName + "\r\n\r\n";
                                    label_panelChangeParamenter.Text += test.Scripts.Control.getAreaAcreage(tile, AreaIdSelected) + "\r\n\r\n";
                                    label_panelChangeParamenter.Text += "人员数量：" + area.headMaxcount + "\r\n\r\n";

                                    int c = 0;
                                    for (int i = 0; i < test.Scripts.Control._areas.Count; i++)
                                    {
                                        c += test.Scripts.Control._areas[i].headMaxcount;
                                    }
                                    label_panelChangeParamenter.Text += "人员数量：" + c + "\r\n\r\n";
                                    break;
                                }
                            }
                        }
                    }
                    else AreaIdSelected = 0xfe;//代表没选择
                }
            }
            doubleBufferPanel1.Refresh();
        }
        private void SearchAgent(float x, float y)
        {
            if (Control.agentsAll == null) return;
            Vector2 p = new Vector2(x, y);
            for (int i = 0; i < test.Scripts.Control.agentsAll.Count; i++)
            {
                if (Simulate.MathHelper.abs(test.Scripts.Control.agentsAll[i].positionNow - p) < 1)
                {
                    test.Scripts.Control.agentsAll[i].color = Color.Red;
                    agentForNav = test.Scripts.Control.agentsAll[i];
                    return;
                }
            }
        }
        private void doubleBufferPanel1_MouseWheel(object sender, MouseEventArgs e)
        {
            return;
            var delta = e.Delta;
            //if (Math.Abs(delta) < 10)
            //{
            //    return;
            //}
            var mousePosition = new PointF();
            mousePosition.X = e.X;
            mousePosition.Y = e.Y;
            _zoomOld = _zoom;

            if (delta < 0)
            {
                _zoom -= _zoom/4;
            }
            else if (delta > 0)
            {
                _zoom += _zoom/4;
            }
            if (_zoom < _zoomMin)
            {
                _zoom = _zoomMin;
            }
            else if (_zoom > _zoomMax)
            {
                _zoom = _zoomMax;
            }

            var zoomNew = _zoom;
            var zoomOld = _zoomOld;
            var deltaZoomNewToOld = zoomNew / zoomOld;

            var zero = _gridLeftTop;
            zero.X = mousePosition.X - (mousePosition.X - zero.X) * deltaZoomNewToOld;
            zero.Y = mousePosition.Y - (mousePosition.Y - zero.Y) * deltaZoomNewToOld;
            //zero.X = mousePosition.X * (1 - _zoom)-zero.X;
            //zero.Y = mousePosition.Y * (1 - _zoom)-zero.Y;
            _gridLeftTop = zero;

            doubleBufferPanel1.Refresh();
        }

        private void Form_MouseWheel(object sender, MouseEventArgs e)
        {
            
            var delta = e.Delta;
            //if (Math.Abs(delta) < 10)
            //{
            //    return;
            //}
            var mousePosition = new PointF();
            mousePosition.X = e.X;
            mousePosition.Y = e.Y;
            _zoomOld = _zoom;

            if (delta < 0)
            {
                _zoom -= _zoom / 4;
            }
            else if (delta > 0)
            {
                _zoom += _zoom / 4;
            }
            if (_zoom < _zoomMin)
            {
                _zoom = _zoomMin;
            }
            else if (_zoom > _zoomMax)
            {
                _zoom = _zoomMax;
            }

            var zoomNew = _zoom;
            var zoomOld = _zoomOld;
            var deltaZoomNewToOld = zoomNew / zoomOld;

            var zero = _gridLeftTop;
            zero.X = mousePosition.X - (mousePosition.X - zero.X) * deltaZoomNewToOld;
            zero.Y = mousePosition.Y - (mousePosition.Y - zero.Y) * deltaZoomNewToOld;
            //zero.X = mousePosition.X * (1 - _zoom)-zero.X;
            //zero.Y = mousePosition.Y * (1 - _zoom)-zero.Y;
            _gridLeftTop = zero;

            doubleBufferPanel1.Refresh();
        }



        //鼠标点击时候用的，通过名字得到Area
        private Area GetAreabyName(string text)
        {
            if (Control._areas == null) return Area.None;
            for (int i = 0; i < test.Scripts.Control._areas.Count; i++)
            {
                if (test.Scripts.Control._areas[i].AreaName == text)
                {
                    return test.Scripts.Control._areas[i];
                }
            }
            return Area.None;
        }

        #endregion


        /// <summary>
        /// panel的绘制, 在refresh时自动调用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        Stopwatch sw = new Stopwatch();//用来帮助计算程序耗时
        private void doubleBufferPanel1_Paint(object sender, PaintEventArgs e)
        {
           
            if(sw.ElapsedMilliseconds>0) label8.Text =(1000/(float)sw.ElapsedMilliseconds).ToString("f2");
            sw.Reset();
            sw.Start();
            //label9.Text = Instance.readswCishu;

            gc = e.Graphics;
            gc.SmoothingMode = SmoothingMode.HighQuality;
            gc.ScaleTransform(_zoom, _zoom);
            gc.TranslateTransform(_gridLeftTop.X / _zoom, _gridLeftTop.Y / _zoom);

            //drawFun();

            DrawForCrows();

        }
      


        #region 仿真相关
        //仿真需要的
        //public List<Instance> Sample._instance;
        NavTile tile;
        Bitmap meshMap;
        internal void UISimulateInit()
        {
            //到这里,就已经改过control的文件目录文件名了,所以以后再解耦
            char[] lineSplitChars = { '.' };
            this.Text = "东门步行街人群疏散系统 - " + test.Scripts.Control.projectName.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries)[0];

            test.Scripts.Control.Clear();
            UpdateUIpanelInfoDeligate("可点击仿真按钮进行仿真");
            UpdateSimulateStatus(SimulateStates.SimulatInited);

            //对chart清空
            chartOuts.Series[0].Points.Clear();
        }

        /// <summary>
        /// 读取设置文件中的颜色设置
        /// </summary>
        /// <param name="path"></param>
        public void DensityColorReadInit(string path)
        {
            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlElement settings = xmlDocument.DocumentElement;
                XmlNodeList colorDensity = settings.GetElementsByTagName("ColorDensity");
                var maxDens=((XmlElement)colorDensity[0]).GetAttribute("maxDens");
                HeatMap.calculateColorFactor(float.Parse(maxDens));
            }
            catch
            {
            }

        }

        #endregion

        #region 读取相关
        private int speedMulti = 1;
        int timerReadInterval = 1000;
        InstanceRead instanceRead = new InstanceRead();
        bool ReadInit(string readingpath)
        {
            //可能配置文件出错,可能仿真结果没有
            if (File.Exists(readingpath))
            {
                instanceRead.InitRead(readingpath,Control.设置.outids.Count);
                
                var ReadInterval = instanceRead.readdeltT;//这个对reader new了，所以放最后，上面都是读取后关掉
                timerReadInterval = (int)(1000 * ReadInterval);
                timerRead.Interval = timerReadInterval;
                labelPanelInfo.Text = ("结果间隔为" + Math.Round(ReadInterval) + ",请稍等...");

                //根据时间间隔调整一下速度，免得间隔太长时等太久
                var roundInterval = Math.Round(ReadInterval);
                trackBar_speed.Value = 0;//不要去掉，防止点击停止播放，然后点击播放后，速度不对
                if (roundInterval < 5) trackBar_speed.Value = 0;
                else if (roundInterval == 5) trackBar_speed.Value = 2;
                else if (roundInterval <= 10) trackBar_speed.Value = 3;
                else if (roundInterval <= 20) trackBar_speed.Value = 4;
                else if (roundInterval > 20) trackBar_speed.Value = 4;

                //出口数统计最大值
                var chartMax = instanceRead.GetOutMaxCount();
                if (chartMax > 0) chartOuts.ChartAreas[0].AxisY.Maximum = chartMax;


                //进度条最大值
                this.trackBar_replay.Maximum = instanceRead.GetFrameCount();
                Console.WriteLine("trackBar_replay.Maximum:" + trackBar_replay.Maximum);

                this.label_FrameMax.Text = trackBar_replay.Maximum.ToString();

                HeatMap.HeatMapInit();//读取文件时也要对热力图初始化
                UpdateSimulateStatus(SimulateStates.Reading);
                return true;
            }
            else
            {
                UpdateSimulateStatus(SimulateStates.SimulatInited);
                MessageBox.Show("当前项目没有仿真结果文件, 需要先进行仿真", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            
        }
        #endregion

        #region 绘制相关
        public static Bitmap crowdImage;
        public static Bitmap heatImage;

        /// <summary>
        /// 主要绘制人群和人群所需要的地图等，song
        /// </summary>
        void DrawForCrows()
        {
            if (rB_Mesh.Checked) DrawNavMesh();

            if (cB_ShowExits.Checked) DrawExits(gc);
            DrawBuildingName(gc);
            //if (test.Scripts.Control.agentsAll == null)
            //    return;

            if (rB_Border.Checked) DrawBoudary();
            else if (rB_AreaPartition.Checked) DrawMeshforPartition();
            else if (rB_AreaSet.Checked) DrawMeshforSet();
            if (cB_ShowGird.Checked) DrawGrid(gc);
            //if (rB_corwds.Checked) DrawAgents();
            //else if (rB_heatmap.Checked) gc.DrawImage(heatmap, -HeatMap.xOffset, -HeatMap.xOffset);//gc.DrawImage(heatmap, -206, -1980); //-trackBar3.Value,-trackBar_repaly.Value);

            //显示颜色密度比例尺
            //if (cB_DrawColorRule.Checked) DrawColorRule(gc);
         
            if(cb_drawHeatmap.Checked)
            {
                if (HeatMap.bmp != null)
                {
                    //gc.DrawImage(HeatMap.bmp, -250, -250);

                    //gc.DrawImage(HeatMap.bmp, 100, 100);
                    //gc.DrawImage(HeatMap.bmp, new Rectangle(-(int)(_gridLeftTop.X * int.Parse(textBox1.Text)), -(int)(_gridLeftTop.Y * int.Parse(textBox1.Text)), HeatMap.bmp.Width * 15, HeatMap.bmp.Height * 15), 0, 0, HeatMap.bmp.Width, HeatMap.bmp.Height, GraphicsUnit.Pixel);
                    if(Control.state!= SimulateStates.ReadingEnd) gc.DrawImage(HeatMap.bmp, new Rectangle(-260, -210, HeatMap.bmp.Width, HeatMap.bmp.Height), 0, 0, HeatMap.bmp.Width, HeatMap.bmp.Height, GraphicsUnit.Pixel);
                    DrawColorRule(gc);
                }
            }
            else
            {
                DrawAgentsFormRead();
            }
#if DEBUG
            drawPoints();
#endif

        }

        int getcolor()
        {
            return (int)Simulate.MathHelper.RandomUniform(0, 255);
        }

        //private void drawFun()
        //{
            
        //    //var brush = new SolidBrush(Color.Red);
        //    for (int i=2;i<forfun.Count;i++)
        //    {
        //        var pen = new Pen(forfun[i].c, 1f);
        //        var brush = new SolidBrush(forfun[i].c);
        //        //gc.FillEllipse(brush, forfun[i].x, forfun[i].y,2,2);
        //        //gc.DrawLine(pen, forfun[i-1].x, forfun[i - 1].y, forfun[i].x, forfun[i].y);
        //        PointF[] points = new PointF[3];
        //        points[0] = new PointF(forfun[i - 2].x, forfun[i - 2].y);
        //        points[1] = new PointF(forfun[i - 1].x, forfun[i - 1].y);
        //        points[2] = new PointF(forfun[i].x, forfun[i].y);
        //        gc.FillPolygon(brush, points);
        //    }
        //    for(int i = 0; i < forfun.Count; i++)
        //    {
        //        if(forfun[i].x<-150+400)
        //        {
        //            forfun[i].xd = 1;
        //            forfun[i].c=Color.FromArgb(getcolor(), getcolor(), getcolor());
        //        }
        //        else if(forfun[i].x > 150 + 400)
        //        {
        //            forfun[i].xd = -1;
        //            forfun[i].c = Color.FromArgb(getcolor(), getcolor(), getcolor());
        //        }

        //        if (forfun[i].y < -150 + 400)
        //        {
        //            forfun[i].yd = 1;
        //            forfun[i].c = Color.FromArgb(getcolor(), getcolor(), getcolor());
        //        }
                    
        //        else if (forfun[i].y > 150 + 400)
        //        {
        //            forfun[i].yd = -1;
        //            forfun[i].c = Color.FromArgb(getcolor(), getcolor(), getcolor());
        //        }
                    

        //        forfun[i].x += forfun[i].xd*Simulate.MathHelper.RandomUniform(0,1);
        //        forfun[i].y += forfun[i].yd * Simulate.MathHelper.RandomUniform(0, 1);

        //    }
           
        //}

        //鼠标点击，搜寻最近agent，绘制他的导航点
        AgentClass agentForNav;
        /// <summary>
        /// 鼠标点击，搜寻最近agent，绘制他的导航点
        /// </summary>
        private void drawPoints()
        {
            //return;
            Font font = new Font(labelDebugInfo.Font.FontFamily, 2, FontStyle.Bold);
            try
            {
                
                //return;//暂时不用显示
                if (agentForNav == null) return;
                var pen = new Pen(Color.Violet, 0.2f);
                Vector2 last = agentForNav.positionNow;
                for (int n = 0; n < agentForNav.navPoints.Count; n++)
                {
                    gc.FillEllipse(Brushes.Red, agentForNav.navPoints[n].x_-1, agentForNav.navPoints[n].y_-1, 2, 2);
                    gc.DrawLine(pen, last.x_, last.y_, agentForNav.navPoints[n].x_, agentForNav.navPoints[n].y_);
                    last = agentForNav.navPoints[n];
                    gc.DrawString(n.ToString(),font, Brushes.Green, agentForNav.navPoints[n].x_, agentForNav.navPoints[n].y_);
                }
                labelDebugInfo.Text += "\n 速度：" + agentForNav.speedNow;
            }
            catch
            {

            }

            try
            {
                if (agentForNav == null) return;
                var pen = new Pen(Color.Honeydew, 0.2f);
                Vector2 last = agentForNav.positionNow;
                for (int n = 0; n < agentForNav.navPointstemp.Count; n++)
                {
                    gc.FillEllipse(Brushes.Honeydew, agentForNav.navPointstemp[n].x_-1, agentForNav.navPointstemp[n].y_-1, 2, 2);
                    gc.DrawLine(pen, last.x_, last.y_, agentForNav.navPointstemp[n].x_, agentForNav.navPointstemp[n].y_);
                    last = agentForNav.navPointstemp[n];
                    gc.DrawString(n.ToString(), font, Brushes.Green, agentForNav.navPointstemp[n].x_, agentForNav.navPointstemp[n].y_);
                }
            }
            catch
            {

            }
        }



        private void DrawBoudary()
        {
            //Color color = Color.Red;
            //float max = 0;

            //for (int j = 0; j < test.Scripts.Control.agentsAll.Count; j++)
            //{
            //    switch (j)
            //    {
            //        case 0:
            //            color = Color.Red;
            //            break;
            //        case 1:
            //            color = Color.Green;
            //            break;
            //        case 2:
            //            color = Color.Blue;
            //            break;
            //        case 3:
            //            color = Color.Purple;
            //            break;
            //    }

            //    Pen pen = new Pen(color, 0.1f);

            //    for (int i = 0; i < test.Scripts.Control._instance[j].obsTest.Count - 1; i += 2)
            //    {
            //        var v0 = test.Scripts.Control._instance[j].obsTest[i];
            //        var v1 = test.Scripts.Control._instance[j].obsTest[i + 1];

            //        if (v0.Z > max) max = v0.Z;
            //        if (v1.Z > max) max = v1.Z;
            //        if (max >= 209)
            //        {
            //            Console.WriteLine(max);
            //        }

            //        PointF[] points = new PointF[2];
            //        points[0] = new PointF(v0.X, v0.Z);
            //        points[1] = new PointF(v1.X, v1.Z);

            //        gc.DrawPolygon(pen, points);
            //    }
            //}

        }
  
        private void DrawNavMesh()
        {
            if (test.Scripts.Control.songnav.tiledNavMesh == null)
                return;
            var tile = test.Scripts.Control.songnav.tiledNavMesh.GetTileAt(0, 0, 0);
            
            Color color = Color.Purple;
            Pen pen = new Pen(Color.FromArgb(212, 212, 214),0.1f);
            Brush brush = new SolidBrush(Color.FromArgb(212, 212, 214));
            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;
                    
                //Brush brush = new SolidBrush(Color.FromArgb(212, 212, 214));
                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    //if (color.R < 1) color.set += 0.1f;
                    //if (color.G < 1) color.G += 0.1f;
                    //if (color.R >= 1) color.R = 0.1f;
                    //if (color.G >= 1) color.G = 0.1f;
                    //GL.Color4(color);

                    //Brush brush = new SolidBrush(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);
                    

                    //GL.Color4(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);//
                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];


                    var v0 = tile.Verts[vertIndex0];

                    var v1 = tile.Verts[vertIndex1];

                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X, v0.Z);
                    points[1] = new PointF(v1.X, v1.Z);
                    points[2] = new PointF(v2.X, v2.Z);
                    //gc.DrawPolygon(pen, points);
                    gc.FillPolygon(brush, points);
                    gc.DrawPolygon(pen, points);

                    //PointF[] points4 = new PointF[4];
                    //points4[0] = new PointF(0, 0);
                    //points4[1] = new PointF(1000, 0);
                    //points4[2] = new PointF(1000, 1000);
                    //points4[3] = new PointF(0, 1000);
                    //gc.DrawPolygon(pen,points4);
                }

            }
        }

        private void DrawNavMeshWithColor()
        {
            //Graphics gc = panel1.CreateGraphics();


            if (test.Scripts.Control.songnav.tiledNavMesh == null)
                return;

            var tile = test.Scripts.Control.songnav.tiledNavMesh.GetTileAt(0, 0, 0);


            Color color = Color.Purple;

            //Pen pen = new Pen(color, 1);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;

                //PointF[] points = new PointF[tile.Polys[i].VertCount];
                //for(int v=0;v<tile.Polys[i].VertCount;v++)
                //{
                //    var vert = tile.Verts[v];
                //    points[v]=new PointF(vert.X,vert.Z);
                //}
                //gc.DrawPolygon(pen,points);

                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    //if (color.R < 1) color.set += 0.1f;
                    //if (color.G < 1) color.G += 0.1f;
                    //if (color.R >= 1) color.R = 0.1f;
                    //if (color.G >= 1) color.G = 0.1f;
                    //GL.Color4(color);

                    Brush brush = new SolidBrush(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);

                    //GL.Color4(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);//
                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];


                    var v0 = tile.Verts[vertIndex0];

                    var v1 = tile.Verts[vertIndex1];

                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X, v0.Z);
                    points[1] = new PointF(v1.X, v1.Z);
                    points[2] = new PointF(v2.X, v2.Z);
                    //gc.DrawPolygon(pen, points);
                    gc.FillPolygon(brush, points);
                }

            }

            //neighbor edges
            //for (int i = 0; i < tile.Polys.Length; i++)
            //{
            //    for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            //    {
            //        if (tile.Polys[i].Verts[j] == 0)
            //            break;
            //        if (PolyMesh.IsBoundaryEdge(tile.Polys[i].Neis[j]))
            //            continue;

            //        int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

            //        int vertIndex0 = tile.Polys[i].Verts[j];
            //        int vertIndex1 = tile.Polys[i].Verts[nj];

            //        var v = tile.Verts[vertIndex0];
            //        GL.Vertex3(v.X, v.Y, v.Z);

            //        v = tile.Verts[vertIndex1];
            //        GL.Vertex3(v.X, v.Y, v.Z);
            //    }
            //}
            //GL.End();

            ////boundary edges
            //GL.Color4(Color4.Yellow);
            //GL.LineWidth(4f);
            //GL.Begin(BeginMode.Lines);
            //for (int i = 0; i < tile.Polys.Length; i++)
            //{
            //    for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            //    {
            //        if (tile.Polys[i].Verts[j] == 0)
            //            break;

            //        if (PolyMesh.IsInteriorEdge(tile.Polys[i].Neis[j]))
            //            continue;

            //        int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

            //        int vertIndex0 = tile.Polys[i].Verts[j];
            //        int vertIndex1 = tile.Polys[i].Verts[nj];

            //        var v = tile.Verts[vertIndex0];
            //        GL.Vertex3(v.X, v.Y, v.Z);

            //        v = tile.Verts[vertIndex1];
            //        GL.Vertex3(v.X, v.Y, v.Z);
            //    }
            //}

            //pen.Dispose();
        }

        private void DrawMeshforPartition()
        {
            //没有被选择的区域呈现红色
            //当前被选择的区域呈现
            if (test.Scripts.Control.songnav.tiledNavMesh == null)
                return;

            var tile = test.Scripts.Control.songnav.tiledNavMesh.GetTileAt(0, 0, 0);

            Color color = Color.Purple;
            Color colorSelected = Color.Purple;

            Pen pen = new Pen(color, 1);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                //if (!tile.Polys[i].Area.IsWalkable)
                //    continue;

                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    //if (color.R < 1) color.set += 0.1f;
                    //if (color.G < 1) color.G += 0.1f;
                    //if (color.R >= 1) color.R = 0.1f;
                    //if (color.G >= 1) color.G = 0.1f;
                    //GL.Color4(color);


                    if (tile.Polys[i].Area.Id == Area.None)
                    {
                        color = Color.FromArgb(150, tile.Polys[i].ColorOriginal.R, tile.Polys[i].ColorOriginal.G, tile.Polys[i].ColorOriginal.B);
                    }
                    else
                    {
                        //不同区域不同颜色
                        switch (tile.Polys[i].Area.Id)
                        {
                            case 0: color = Color.Purple; break;
                            case 1: color = Color.Salmon; break;
                            case 2: color = Color.Chocolate; break;
                            case 3: color = Color.Red; break;
                            case 4: color = Color.MidnightBlue; break;
                            case 5: color = Color.SlateBlue; break;
                            case 6: color = Color.DodgerBlue; break;
                            case 7: color = Color.SkyBlue; break;
                            case 8: color = Color.Cyan; break;
                            case 9: color = Color.DarkGreen; break;
                            case 10: color = Color.Purple; break;
                            case 11: color = Color.RosyBrown; break;
                            case 12: color = Color.Gold; break;
                            case 13: color = Color.IndianRed; break;
                            case 14: color = Color.Salmon; break;
                            case 15: color = Color.Orange; break;
                            case 16: color = Color.HotPink; break;
                            case 17: color = Color.Maroon; break;
                            case 18: color = Color.DarkViolet; break;
                            case 19: color = Color.MediumSeaGreen; break;
                            case 20: color = Color.GreenYellow; break;
                            case 21: color = Color.PaleGreen; break;
                            case 22: color = Color.Wheat; break;
                            case 23: color = Color.Azure; break;
                            case 24: color = Color.OldLace; break;
                            case 38: color = Color.Aqua; break;
                            case 0xfe: color = Color.Green; break;
                            default:
                                color = Color.White;//白色代表没有定义
                                break;

                        }
                        color = Color.FromArgb(100, color.R, color.G, color.B);
                        colorSelected = Color.FromArgb(255, color.R, color.G, color.B);
                    }


                    Brush brush;
                    if (tile.Polys[i].Selected)
                    {
                        brush = new SolidBrush(colorSelected);
                    }
                    else
                    {
                        brush = new SolidBrush(color);
                    }
                    //Brush brush = new SolidBrush(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : color);


                    //GL.Color4(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);//
                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];


                    var v0 = tile.Verts[vertIndex0];

                    var v1 = tile.Verts[vertIndex1];

                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X, v0.Z);
                    points[1] = new PointF(v1.X, v1.Z);
                    points[2] = new PointF(v2.X, v2.Z);
                    //gc.DrawPolygon(pen, points);
                    gc.FillPolygon(brush, points);
                }
            }
        }

        private void DrawMeshforSet()
        {
            //没有被选择的区域呈现红色
            //当前被选择的区域呈现
            if (test.Scripts.Control.songnav.tiledNavMesh == null)
                return;

            var tile = test.Scripts.Control.songnav.tiledNavMesh.GetTileAt(0, 0, 0);

            Color color = Color.Purple;

            //Pen pen = new Pen(color, 1);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;
                //Brush brush = new SolidBrush(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);

                //不同区域不同颜色
                switch (tile.Polys[i].Area.Id)
                {
                    case 0: color = Color.Purple; break;
                    case 1: color = Color.Salmon; break;
                    case 2: color = Color.Chocolate; break;
                    case 3: color = Color.Red; break;
                    case 4: color = Color.MidnightBlue; break;
                    case 5: color = Color.SlateBlue; break;
                    case 6: color = Color.DodgerBlue; break;
                    case 7: color = Color.SkyBlue; break;
                    case 8: color = Color.Cyan; break;
                    case 9: color = Color.DarkGreen; break;
                    case 10: color = Color.Purple; break;
                    case 11: color = Color.RosyBrown; break;
                    case 12: color = Color.Gold; break;
                    case 13: color = Color.IndianRed; break;
                    case 14: color = Color.Salmon; break;
                    case 15: color = Color.Orange; break;
                    case 16: color = Color.HotPink; break;
                    case 17: color = Color.Maroon; break;
                    case 18: color = Color.DarkViolet; break;
                    case 19: color = Color.MediumSeaGreen; break;
                    case 20: color = Color.GreenYellow; break;
                    case 21: color = Color.PaleGreen; break;
                    case 22: color = Color.Wheat; break;
                    case 23: color = Color.Azure; break;
                    case 24: color = Color.OldLace; break;
                    case 28: color = Color.Aqua; break;
                    case 0xfe: color = Color.Green; break;

                    default:
                        color = Color.Black;
                        break;
                }

                if (tile.Polys[i].Area.Id != AreaIdSelected) color = Color.FromArgb(60, color.R, color.G, color.B);
                Brush brush = new SolidBrush(color);

                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    if (tile.Polys[i].Verts[j] == 0)
                        break;
                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];

                    var v0 = tile.Verts[vertIndex0];
                    var v1 = tile.Verts[vertIndex1];
                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X, v0.Z);
                    points[1] = new PointF(v1.X, v1.Z);
                    points[2] = new PointF(v2.X, v2.Z);
                    gc.FillPolygon(brush, points);
                }
            }
        }

        public void DrawAgentsFormRead()
        {
            DrawCrowd(instanceRead._agents);
            
        }

        public void DrawCrowd(List<AgentClass> agents)
        {
            Pen pen = new Pen(Color.Red, 0.1f);
            for (int i = 0; i < agents.Count; i++)
            {
                ////song 画导航点
                //GL.Begin(BeginMode.Points);
                //GL.Color4(Color4.Green);
                ////GL.Color4(new Color4((float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()),0.5f));
                //for (int j = 0; j < Sample._instance._agents[i].navPoints.Count - 1; j++)
                //{
                //    SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(Sample._instance._agents[i].navPoints[j].x_, 1f, Sample._instance._agents[i].navPoints[j].y_);
                //    GL.Vertex3(p.X, p.Y, p.Z);
                //}
                //GL.End();

                //画导航线 song
                //if(i%100==0)
                //{
                //    try
                //    {
                //        int c = instance._agents[i].navPoints.Count - 1;
                //        //if (c > 2000)
                //        {
                //            //GL.Color4(new Color4((float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()), (float)(Simulate.MathHelper.random.NextDouble()), 0.5f));
                //            for (int j = 1; j < c; j++)
                //            {
                //                SharpNav.Geometry.Vector3 p1 = new SharpNav.Geometry.Vector3(instance._agents[i].navPoints[j].x_, 1f, instance._agents[i].navPoints[j].y_);

                //                SharpNav.Geometry.Vector3 p2 = new SharpNav.Geometry.Vector3(instance._agents[i].navPoints[j - 1].x_, 1f, instance._agents[i].navPoints[j - 1].y_);

                //                gc.DrawLine(pen, p1.X, p1.Z, p2.X, p2.Z);
                //            }
                //        }
                //    }
                //    catch
                //    {
                //    }
                //}
                

                if (cB_showGoalNow.Checked)
                {
                    //画当前目标线 song
                    try
                    {
                        if (agents[i].navPoints.Count > 1)
                        {
                            //SharpNav.Geometry.Vector3 p1;
                            //SharpNav.Geometry.Vector3 p2;
                            //p1 = new SharpNav.Geometry.Vector3(instance._agents[i].positionNow.x_, 1.01f, instance._agents[i].positionNow.y_);

                            //p2 = new SharpNav.Geometry.Vector3(instance._agents[i].navPoints[0].x_, 1.01f, instance._agents[i].navPoints[0].y_);


                            gc.DrawLine(pen, agents[i].positionNow.x_, agents[i].positionNow.y_ , agents[i].navPoints[0].x_, agents[i].navPoints[0].y_);
                            

                            //float pp = p1.X - p2.X + p1.Z - p2.Z;
                            //if (pp > 20)
                            //{
                            //    Console.WriteLine(p2);
                            //}
                        }
                    }
                    catch
                    {

                    }
                }
            }

            Brush brush = new SolidBrush(Color.Blue);//填充的颜色

            //if (agentCylinder != null)
            {
                lock (agents)
                {

                    for (int i = 0; i < agents.Count; i++)
                    {
                        try
                        {
                            //if(Sample.readMode)brush = new SolidBrush(instance._agents[i].color);//暂时注释,不用更新区域颜色,都是绿色就可以

                            ///////////源于颜色设置
                            ///方法1，根据本来颜色，如果运行了heatmap就是根据密度设置的来操作
                            //brush = new SolidBrush(instance._agents[i].color);
                            ///方法2，
                            //if (instance._agents[i].state == AgentStates.Evacuating)
                            //    brush = new SolidBrush(Color.Red);
                            //else
                            //    brush = new SolidBrush(Color.Green);
                            ///方法3
                            //if (instance._agents[i].state == AgentStates.Evacuating)
                            //    brush = new SolidBrush(instance._agents[i].color);
                            //else
                            //    brush = new SolidBrush(Color.Black);

#if DEBUG
                            /// 原来的根据本身颜色设置进行绘制
                            if (agents[i].navPoints.Count > 0 && agents[i].navPoints[0].x_ == 0)
                                brush = new SolidBrush(Color.Black);
                            else
                                brush = new SolidBrush(agents[i].color);
#endif

                            //if (instance._agents[i].haveReplaned) brush = new SolidBrush(Color.Black);

                            SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(agents[i].positionNow.X(), 0, agents[i].positionNow.Y());
                            //agentCylinder.Draw(new OpenTK.Vector3(p.X, p.Y, p.Z), instance._agents[i].color);
                            var r = agents[i].agentRadius;
                            gc.FillEllipse(brush, p.X-r, p.Z-r, r, r);
                            //#if outfile    
                            //                        FileHelper.Write(Program._agents[i].positionNow.x_ + " " + Program._agents[i].positionNow.y_ + " ");
                            //#endif
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("多线程对_agents.count访问出错 - 绘制");
                            Console.WriteLine(e);
                        }

                    }
                }
            }
        }

        private void DrawGrid(Graphics g)
        {
            float cellWidth = _cellWidth_px;
            float cellHeight = _cellHeight_px;

            //单元格的宽和高最小为1像素
            cellWidth = cellWidth < 1 ? 1 : cellWidth;
            cellHeight = cellHeight < 1 ? 1 : cellHeight;


            int rowCount = 10;
            int columnCount = 10;
            var gridHeight = rowCount * cellHeight;
            var gridWidth = columnCount * cellWidth;

            Pen pen = new Pen(Color.Red, 1f);
            var p1 = new PointF();
            var p2 = new PointF();




            //绘制横线
            for (int r = 0; r <= rowCount; r++)
            {
                p1.X = -gridWidth / 2;
                p1.Y = r * cellHeight - gridWidth / 2;

                p2.X = p1.X + gridWidth;
                p2.Y = p1.Y;

                g.DrawLine(pen, p1, p2);
            }

            //绘制竖线
            for (int c = 0; c <= columnCount; c++)
            {
                p1.X = c * cellHeight - gridHeight / 2;
                p1.Y = -gridHeight / 2;

                p2.X = p1.X;
                p2.Y = p1.Y + gridHeight;

                g.DrawLine(pen, p1, p2);
            }

            g.DrawLine(pen, -2400,2000,-2400,-2000);
            g.DrawLine(pen, 2000, 2000, 2000, -2000);


            //绘制比例
            p1.X = 0;
            p1.Y = 0;

            g.DrawString($"{_zoom * 100}%", SystemFonts.DefaultFont, Brushes.Gray, p1);
            pen = new Pen(Color.Red, 20f);
            p1.X = gridWidth / 2;
            p1.Y = gridHeight / 2;

            g.DrawString(gridWidth + " x " + gridHeight, SystemFonts.DefaultFont, Brushes.Red, p1);
        }

        //地图上绘制数据信息
        private void DrawExits(Graphics g)
        {
            var p1 = new PointF();
            Font font = new Font(labelDebugInfo.Font.FontFamily, 6, FontStyle.Bold);
            for (int i = 0; i < test.Scripts.Control._out.Length; i++)
            {
                p1.X = test.Scripts.Control._out[i].position.X;
                p1.Y = test.Scripts.Control._out[i].position.Z;
                //g.FillEllipse(Brushes.Green, p1.X, p1.Y, 10, 10);
                //p1.X += 10;

                ////数字位置细节调整
                if (i == 5 )
                {
                    p1.Y += 3;
                }
                else if(i == 6)
                {
                    p1.Y += 5;
                }
                else if (i == 1)
                {
                    p1.Y -= 8;
                }

                p1.X -= 3;
                p1.Y -= 3;

                if (i < 9)
                    g.FillEllipse(Brushes.DarkOrange, p1.X - 3, p1.Y - 2, 12, 12);
                else
                    g.FillEllipse(Brushes.DarkOrange, p1.X - 0.5f, p1.Y - 2, 12, 12);
                g.DrawString((i + 1).ToString(), font, Brushes.White, p1);
                //g.DrawString(i + "人数: " + Instance._outAgentCount[i] + " " + Instance._outAgentCount[i] * 100 / (Sample.numsPeople - agentsOutCount) + "% " + Instance._outAgentCount[i] * 100 / Sample.numsPeople + "%", font, Brushes.Red, p1);
            }
        }

        private void DrawBuildingName(Graphics g)
        {
            //var p1 = new PointF();
            Font font = new Font(labelDebugInfo.Font.FontFamily, 6, FontStyle.Bold);
            Brush brush = Brushes.LightGray;
            g.DrawString("太阳广场", font, brush, -100, 31);
            g.DrawString("大世界商城", font, brush, 105, 55);
            g.DrawString("天龙商业城", font, brush, 41, -150);
            g.DrawString("宝华楼", font, brush, -70, -63);
            g.DrawString("东门鞋城", font, brush, 35, 42);
            g.DrawString("金世界", font, brush, -195, 42);
            g.DrawString("新白马中心城", font, brush, 40f, -38f);

        }


        /// <summary>
        /// 用来生成bmp图像，还没有写完，会太卡
        /// </summary>
        /// <returns></returns>
        private Bitmap getNavMeshBmp()
        {
            //Graphics gc = panel1.CreateGraphics();

            Bitmap b = new Bitmap(1000, 1000);
            Graphics bmpG = Graphics.FromImage(b);//图像画布添加绘图

            if (test.Scripts.Control.songnav.tiledNavMesh == null)
                return null;

            var tile = test.Scripts.Control.songnav.tiledNavMesh.GetTileAt(0, 0, 0);


            Color color = Color.Purple;

            Pen pen = new Pen(color, 1);

            for (int i = 0; i < tile.Polys.Length; i++)
            {
                if (!tile.Polys[i].Area.IsWalkable)
                    continue;

                //PointF[] points = new PointF[tile.Polys[i].VertCount];
                //for(int v=0;v<tile.Polys[i].VertCount;v++)
                //{
                //    var vert = tile.Verts[v];
                //    points[v]=new PointF(vert.X,vert.Z);
                //}
                //gc.DrawPolygon(pen,points);

                for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
                {
                    //if (color.R < 1) color.set += 0.1f;
                    //if (color.G < 1) color.G += 0.1f;
                    //if (color.R >= 1) color.R = 0.1f;
                    //if (color.G >= 1) color.G = 0.1f;
                    //GL.Color4(color);


                    //GL.Color4(tile.Polys[i].Selected ? tile.Polys[i].ColorSelected : tile.Polys[i].ColorOriginal);//
                    if (tile.Polys[i].Verts[j] == 0)
                        break;

                    int vertIndex0 = tile.Polys[i].Verts[0];
                    int vertIndex1 = tile.Polys[i].Verts[j - 1];
                    int vertIndex2 = tile.Polys[i].Verts[j];


                    var v0 = tile.Verts[vertIndex0];

                    var v1 = tile.Verts[vertIndex1];

                    var v2 = tile.Verts[vertIndex2];

                    PointF[] points = new PointF[3];
                    points[0] = new PointF(v0.X, v0.Z);
                    points[1] = new PointF(v1.X, v1.Z);
                    points[2] = new PointF(v2.X, v2.Z);
                    bmpG.DrawPolygon(pen, points);
                }

            }
            //for (int i = 0; i < tile.Polys.Length; i++)
            //{
            //    for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            //    {
            //        if (tile.Polys[i].Verts[j] == 0)
            //            break;
            //        if (PolyMesh.IsBoundaryEdge(tile.Polys[i].Neis[j]))
            //            continue;

            //        int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

            //        int vertIndex0 = tile.Polys[i].Verts[j];
            //        int vertIndex1 = tile.Polys[i].Verts[nj];

            //        var v = tile.Verts[vertIndex0];
            //        GL.Vertex3(v.X, v.Y, v.Z);

            //        v = tile.Verts[vertIndex1];
            //        GL.Vertex3(v.X, v.Y, v.Z);
            //    }
            //}
            //GL.End();

            ////boundary edges
            //GL.Color4(Color4.Yellow);
            //GL.LineWidth(4f);
            //GL.Begin(BeginMode.Lines);
            //for (int i = 0; i < tile.Polys.Length; i++)
            //{
            //    for (int j = 0; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            //    {
            //        if (tile.Polys[i].Verts[j] == 0)
            //            break;

            //        if (PolyMesh.IsInteriorEdge(tile.Polys[i].Neis[j]))
            //            continue;

            //        int nj = (j + 1 >= PathfindingCommon.VERTS_PER_POLYGON || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

            //        int vertIndex0 = tile.Polys[i].Verts[j];
            //        int vertIndex1 = tile.Polys[i].Verts[nj];

            //        var v = tile.Verts[vertIndex0];
            //        GL.Vertex3(v.X, v.Y, v.Z);

            //        v = tile.Verts[vertIndex1];
            //        GL.Vertex3(v.X, v.Y, v.Z);
            //    }
            //}

            pen.Dispose();
            bmpG.Dispose();

            return b;
        }

        private void DrawColorRule(Graphics g)
        {
            if (HeatMap.maxColorDensity < 1) return;

            int x = 180;
            int y = -200;

            //DrawRect(g,0.44f, ref x, ref y);
            //DrawRect(g, 1.32f, ref x, ref y);
            //DrawRect(g, 2.2f, ref x, ref y);
            //DrawRect(g, 2.64f, ref x, ref y);
            //DrawRect(g, 3.52f, ref x, ref y);
            //DrawRect(g, 4f, ref x, ref y);

            DrawRect(g, 0, ref x, ref y);
            DrawRect(g, HeatMap.maxColorDensity / 5 * 1, ref x, ref y);
            DrawRect(g, HeatMap.maxColorDensity / 5 * 2, ref x, ref y);
            DrawRect(g, HeatMap.maxColorDensity / 5 * 3.5f, ref x, ref y);
            DrawRect(g, HeatMap.maxColorDensity / 5 * 4, ref x, ref y);
            //最后一个单独弄
            {
                Font font = new Font(labelDebugInfo.Font.FontFamily, 4, FontStyle.Bold);
                Color c = HeatMap.GetColor(HeatMap.maxColorDensity + 0.02f);
                Brush b = new SolidBrush(c);
                g.FillRectangle(b, x, y, 10, 8);
                g.DrawString(">" + HeatMap.maxColorDensity.ToString("0.00"), font, Brushes.White, x + 11, y+1);
                y += 9;

                //最后一行加个单位
                g.DrawString("人/平米", font, Brushes.White, x + 6, y+2);
            }
            


            //for (int i = 0; i < HeatMap.maxNum; i += 2)
            //{
            //    Color color = HeatMap.GetColor(i, 1, HeatMap.maxNum);
            //    Brush b = new SolidBrush(color);
            //    g.FillRectangle(b, x, y, 60, 60);
            //    //if(i%2==0)gc.DrawString(i.ToString(), font, brush, 160, y);
            //    if (i == 44) g.DrawString(">" + ((float)i / 54).ToString("0.00"), font, brush, x + 60, y);
            //    else
            //    {
            //        if (i % 4 == 0) g.DrawString(((float)i / 54).ToString("0.00"), font, brush, x + 60, y);
            //    }
            //    y += 50;
            //}
            //g.DrawString("人/平米", font, brush, x - 10, y + 50);
        }
        private void DrawRect(Graphics g, float value, ref int x, ref int y)
        {
            Font font = new Font(labelDebugInfo.Font.FontFamily, 4, FontStyle.Bold);
            Color c = HeatMap.GetColor(value);
            Brush b = new SolidBrush(c);
            g.FillRectangle(b, x, y, 10, 8);
            g.DrawString(">" + value.ToString("0.00"), font, Brushes.White, x + 11, y+1 );
            y+= 9;
        }
        #endregion
        
        #region 热力图相关 暂时没有用
        Bitmap heatmap = new Bitmap(1, 1);
        public int mapCount = 0;
        private void CalculateMap()
        {
            //while (true)
            //{
            //    if (heatmapThreadControl)
            //    {
            //        List<List<AgentClass>> _agents = new List<List<AgentClass>>();
            //        for (int i = 0; i < Sample._instance.Count; i++)
            //        {
            //            _agents.Add(Sample._instance[i]._agents);
            //        }
            //        heatmap = HeatMap.CalculateMap(ref _agents);
            //        mapCount++;
            //    }
            //}
        }
        #endregion
       
        #region 顶部菜单相关
        public class MyRenderer : ToolStripProfessionalRenderer
        {
            public MyRenderer() : base(new MyColors()) { }
        }


        private void 新建工程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (test.Scripts.Control.mainDirectory == null)
            {
                ProjectPanel projectPanel = new ProjectPanel();
                projectPanel.Show(this);
            }
            else
            {
                if (test.Scripts.Control.stepscontrolThread != null)
                {
                    //先将所有的仿真暂停
                    if (test.Scripts.Control.stepscontrolThread.IsAlive) test.Scripts.Control.stepscontrolThread.Suspend();
                    if (MessageBox.Show("确定要关闭当前工程吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                    {
                        //if (Sample.stepscontrolThread.IsAlive) Sample.stepscontrolThread.Resume();//防止出错
                        test.Scripts.Control.StopSimulate();
                        ProjectPanel projectPanel = new ProjectPanel();
                        projectPanel.Show(this);
                    }
                    else
                    {
                        if (test.Scripts.Control.stepscontrolThread.IsAlive) test.Scripts.Control.stepscontrolThread.Resume();
                    }
                }
                else //上次正在读取文件
                {
                    timerRead.Enabled = false;
                    test.Scripts.Control.Clear();
                    ProjectPanel projectPanel = new ProjectPanel();
                    projectPanel.Show(this);
                }
            }
        }
        
        public void ShowMessagePanel(string filename, int command)
        {
            SettingPanel messagePanel = new SettingPanel(command, filename);
            messagePanel.Show(this);

        }
        private void 编辑工程设置ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (test.Scripts.Control.mainDirectory == null)
            {
                MessageBox.Show("没有创建工程，不存在工程文件，请先创建工程", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!File.Exists(test.Scripts.Control.mainDirectory + test.Scripts.Control.projectName))
            {
                MessageBox.Show("当前工程还没有工程文件，请先新建工程文件", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            ShowMessagePanel(test.Scripts.Control.mainDirectory + test.Scripts.Control.projectName, 2);
        }

        /// <summary>
        /// 控制右上方进度条，进度条加载期间是每个agent在寻径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Progress_Tick(object sender, EventArgs e)
        {
            progressBar1.Value = Control.pathFindedNums;
        }


        private void 打开工程ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //1. 对control进行初始化
            //2. 对读取进行初始化

            OpenFileDialog f = new OpenFileDialog();
            //默认所有工程都在project文件夹中
            f.InitialDirectory = Directory.GetCurrentDirectory();
            f.Multiselect = false;
            f.Title = "请选择工程文件";
            f.Filter = "工程文件(*.sim)|*.sim";
            if (f.ShowDialog() == DialogResult.OK)
            {
                string 主目录 = System.IO.Path.GetDirectoryName(f.FileName) + "\\";
                string 项目名 = System.IO.Path.GetFileName(f.FileName);
                this.Text = "东门步行街人群疏散系统 - " + System.IO.Path.GetFileNameWithoutExtension(f.FileName); ;
                //控制区读取
                string readingPath = 主目录 + 项目名;
                Control.ControlInit(主目录, 项目名);
                DensityColorReadInit(readingPath);//位置可以去个更好的
                ReadInit(主目录+ "output0");
            }


            //FolderBrowserDialog f = new FolderBrowserDialog();
            //f.SelectedPath = Directory.GetCurrentDirectory() + "\\project";
            //if (f.ShowDialog() == DialogResult.OK)
            //{
            //    if (File.Exists(f.SelectedPath + "" +))
            //    {
            //        Sample.Clear();
            //        Sample.mainDirectory = f.SelectedPath;
            //        //控制区读取
            //        ReadInit();
            //    }
            //    else
            //    {
            //        MessageBox.Show("错误，这不是一个工程", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}
        }

        private void 编辑ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (test.Scripts.Control.mainDirectory == null)
            {
                MessageBox.Show("没有创建工程，请先创建工程", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (test.Scripts.Control.state == SimulateStates.Reading)
            {

            }
            else if (test.Scripts.Control.state == SimulateStates.SimulatingLocalCPU)
            {
                if (test.Scripts.Control.stepscontrolThread != null)
                {
                    MessageBox.Show("另存为文件需要先结束仿真", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            ProjectPanel p = new ProjectPanel();
            p.ChangeTitle();
            p.Show(this);
        }

        private void 停止ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Control.state == SimulateStates.SimulatingLocalCPU)
            {
                //先将所有的仿真暂停
                if (test.Scripts.Control.stepscontrolThread.IsAlive) test.Scripts.Control.stepscontrolThread.Suspend();
                if (MessageBox.Show("确定要停止此次仿真吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    //Sample.stepscontrolThread.Resume();//防止出错
                    menu_Start.Text = "仿真";
                    test.Scripts.Control.StopSimulate();
                }
                else
                {
                    test.Scripts.Control.stepscontrolThread.Resume();
                }
            }

            if(test.Scripts.Control.state == SimulateStates.SimulatingLocalGPU)
            {
                //Sample.process.Close();
                test.Scripts.Control.process.Kill();
                return;
            }
            if(Control.state == SimulateStates.SendingAgentsToRemote)//如果是正在发送agent文件
            {
                SocketClient.Close();
            }
            if(test.Scripts.Control.state == SimulateStates.SimulatingRemote)
            {
                SocketClient.Close();
                try
                {
                    test.Scripts.Control.PathFindandSimulateThread.Abort();
                    //if(Sample.CombineFiles())
                    //{
                    //    UpdateUIpanelInfoDeligate("已停止仿真，可以点击播放观看结果");
                    //    UpdateSimulateStatus(SimulateStates.SimulateEnd);
                    //}
                    //else
                    //{
                    //    UpdateUIpanelInfoDeligate("可点击仿真按钮进行仿真");
                    //    UpdateSimulateStatus(SimulateStates.SimulatInited);
                    //}
                }
                catch
                {

                }
               
                return;
            }

           
        }


        delegate void DeligateForInfo(string s);//委托
        /// <summary>
        /// 更新左侧状态栏的部分参数
        /// </summary>
        /// <param name="s"></param>
        private void UpdateUIpanelInfoDeligate(string s)
        {
            if(InvokeRequired)
            {
                this.Invoke(new DeligateForInfo(delegate (string state)
                {
                    labelPanelInfo.Text = s;
                    labelDebugInfo.Text = s;
                }), s);
            }
            else
            {
                labelPanelInfo.Text = s;
                labelDebugInfo.Text = s;
            }
           
        }
        private void menu_Start_Click(object sender, EventArgs e)
        {
            if (test.Scripts.Control.state == SimulateStates.SimulatInited || test.Scripts.Control.state == SimulateStates.Reading || test.Scripts.Control.state == SimulateStates.ReadingEnd || test.Scripts.Control.state == SimulateStates.SimulateEnd) //显示
            {
                //先判断有没有相应的仿真结果文件,如果有就提示, 如果没有就仿真
                if (test.Scripts.Control.ExitsOutput())
                {
                    if (MessageBox.Show("有仿真文件存在, 确定要替换掉他们吗?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                ////如果reader存在，就把reader关掉
                //for (int i = 0; i < test.Scripts.Control._instance.Count; i++)
                //{
                //    if (test.Scripts.Control._instance[i].reader != null) test.Scripts.Control._instance[i].reader.Close();
                //    try
                //    {
                //        if (File.Exists(test.Scripts.Control.mainDirectory+ test.Scripts.Control._instance[i].instanceID)) File.Delete(test.Scripts.Control.mainDirectory + test.Scripts.Control._instance[i].instanceID);
                //    }
                //    catch
                //    {
                //        Console.WriteLine("删除出错");
                //    }
                //}
                //然后删除文件

                //图例清理一下
                chartOuts.Series[0].Points.Clear();

                if (Control.SimulateInit())//初始化人
                {
                    //进度条最大值
                    progressBar1.Maximum = test.Scripts.Control.设置.总人数;
                    //设置图标最大值
                    chartOuts.ChartAreas[0].AxisY.Maximum = test.Scripts.Control.出口人数统计.Max();

                    //开始寻径+仿真 寻径结束就开始仿真
                    UpdateSimulateStatus(SimulateStates.SimulatPathfinding);
                    Control.PathFindandSimulate();
                }
                else
                {
                    MessageBox.Show("初始化失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                
            }
            else if (test.Scripts.Control.state == SimulateStates.SimulatingLocalCPU)
            {
                if (test.Scripts.Control.stepscontrolThread.IsAlive) test.Scripts.Control.stepscontrolThread.Suspend();
                UpdateSimulateStatus(SimulateStates.SimulatPaused);
            }
            else if (test.Scripts.Control.state == SimulateStates.SimulatPaused)
            {
                if (test.Scripts.Control.stepscontrolThread.IsAlive) test.Scripts.Control.stepscontrolThread.Resume();
                UpdateSimulateStatus(SimulateStates.SimulatingLocalCPU);
            }
        }



        

        private void 播放ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (Sample.state == SimulateStates.SimulateEnd)
            {
                Control.Clear();
                //Sample.mainDirectory = f.SelectedPath;
                //控制区读取
                ReadInit(Control.mainDirectory+"output0");
            }
        }

        public class MyColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return Color.FromArgb(90, 194, 231); }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return Color.FromArgb(90, 194, 231); }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return Color.FromArgb(90, 194, 231); }
            }

            public override Color MenuItemPressedGradientMiddle
            {
                get { return Color.FromArgb(90, 194, 231); }
            }

            public override Color MenuItemPressedGradientBegin
            {
                get { return Color.FromArgb(90, 194, 231); }
            }

            public override Color MenuItemPressedGradientEnd
            {
                get { return Color.FromArgb(90, 194, 231); }
            }
        }

        #endregion

        #region 右侧按钮，主要是测试用

        private void rB_AreaPartition_CheckedChanged(object sender, EventArgs e)
        {
            //等待修改
            //////////////if (rB_AreaPartition.Checked)
            //////////////{
            //////////////    groupBox_ChangeArea.Visible = true;
            //////////////    cB_AreasList.Items.Clear();
            //////////////    cB_AreasList.Items.Add("未选择区域");
            //////////////    List<Area> _areas;
            //////////////    Sample.GetAreaNames(out _areas);
            //////////////    等待修改
            //////////////    Sample._areas = _areas;//临时
            //////////////    if (_areas != null)
            //////////////    {
            //////////////        for (int i = 0; i < _areas.Count; i++)
            //////////////        {
            //////////////            for (int j = 0; j < _areas[i].Count; j++)
            //////////////            {
            //////////////                cB_AreasList.Items.Add(_areas[i][j].AreaName);
            //////////////            }
            //////////////        }
            //////////////        cB_AreasList.SelectedIndex = 0;
            //////////////    }
            //////////////}
            //////////////else groupBox_ChangeArea.Visible = false;
            //////////////doubleBufferPanel1.Refresh();
        }

        private void cB_AreasList_SelectedIndexChanged(object sender, EventArgs e)
        {

            int areaId = GetAreabyName(cB_AreasList.Text).Id;//是0代表空！是0xff代表未选择
            if (areaId > 0)
            {
                for (int i = 0; i < tile.Polys.Length; i++)
                {
                    if (tile.Polys[i].Area.Id == areaId)
                    {
                        tile.Polys[i].Selected = true;
                    }
                    else
                    {
                        tile.Polys[i].Selected = false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < tile.Polys.Length; i++)
                {
                    tile.Polys[i].Selected = false;
                }
            }
            doubleBufferPanel1.Refresh();
        }
        private void rB_AreaSet_CheckedChanged(object sender, EventArgs e)
        {
            groupBox_paramenter.Visible = rB_AreaSet.Checked;
        }

        private void SaveNavMeshToFile(string path)
        {
            try
            {
                new NavMeshJsonSerializer().Serialize(path, test.Scripts.Control.songnav.tiledNavMesh);
            }
            catch (Exception e)
            {
                Console.WriteLine("Navmesh saving failed with exception:" + Environment.NewLine + e.ToString());
                return;
            }
            Console.WriteLine("Saved to file!");
        }

        private void btn_SaveMesh_Click(object sender, EventArgs e)
        {
            //Gwen.Platform.Neutral.FileSave("Save NavMesh to file", ".", "All SharpNav Files(.snb, .snx, .snj)|*.snb;*.snx;*.snj|SharpNav Binary(.snb)|*.snb|SharpNav XML(.snx)|*.snx|SharpNav JSON(.snj)|*.snj", SaveNavMeshToFile);
            SaveNavMeshToFile("../../Meshes/" + test.Scripts.Control.snbName);
        }

        //用来控制，是对人赋予颜色，还是生成位图热力图
        private void rB_heatmap_CheckedChanged(object sender, EventArgs e)
        {
            if (rB_heatmap.Checked) heatmapThreadControl = true;
            else heatmapThreadControl = false;
        }

        #endregion
        
        #region 左侧界面
        public System.Threading.Timer timerSlide = null;
        private void labelDrowBack_Click(object sender, EventArgs e)
        {
            if (SidePanel.Left > -10)
            {
                timerSlide = new System.Threading.Timer(SlideToLeft, this, 0, 30);
                label4.Text = ">>";
            }
            else
            {
                timerSlide = new System.Threading.Timer(SlideToRight, this, 0, 30);
                label4.Text = "<<";
            }
        }


        public void SlideToRight(Object obj)
        {
            SidePanel.Left += 30;
            if (SidePanel.Left >= -10)
            {
                SidePanel.Left = -8;

                timerSlide.Dispose();
            }
        }

        public void SlideToLeft(Object obj)
        {
            SidePanel.Left -= 30;
            if (SidePanel.Left <= -360)
            {
                SidePanel.Left = -382;
                timerSlide.Dispose();
            }
        }
        #endregion

        #region 播放控件相关
        
        //背景
        private void panelReadPlay_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, panelReadPlay.ClientRectangle, Color.FromArgb(64, 128, 185), ButtonBorderStyle.Solid);
        }

        //控制条
        private void trackBar_replay_MouseDown(object sender, MouseEventArgs e)
        {
            timerRead.Enabled = false;
        }
        private void trackBar_replay_ValueChanged(object sender, EventArgs e)
        {
            this.label_frameMin.Text = trackBar_replay.Value.ToString();
        }

        private void trackBar_replay_MouseUp(object sender, MouseEventArgs e)
        {
            Console.WriteLine(trackBar_replay.Value);
            int linesIndex = trackBar_replay.Value;
            pictureBox_Play.Image = test.Properties.Resources.video_pause;//用鼠标移动后，图片显示为暂停
            timerRead.Enabled = false;

            instanceRead.ReadLineNoThread(linesIndex, speedMulti);
            

            timerRead.Enabled = true;
        }

        //两个按钮,播放停止
        private void pictureBox_Play_Click(object sender, EventArgs e)
        {
            if (timerRead.Enabled == true)
            {
                timerRead.Enabled = false;
                pictureBox_Play.Image = test.Properties.Resources.video_play;
            }
            else
            {
                //if(trackBar_replay.Value== trackBar_replay.Maximum|| trackBar_replay.Value<=1)
                //{
                //    Sample.Clear();
                //    ReadInit();
                //}

                UpdateSimulateStatus(SimulateStates.Reading);
                if (trackBar_replay.Value == trackBar_replay.Maximum)
                {
                    instanceRead.ReadLineNoThread(1, speedMulti);
                    trackBar_replay.Value = 1;
                }
                timerRead.Enabled = true;
                pictureBox_Play.Image = test.Properties.Resources.video_pause;
            }
        }
        private void pictureBox_Stop_Click(object sender, EventArgs e)
        {
            timerRead.Enabled = false;
            pictureBox_Play.Image = test.Properties.Resources.video_play;// Image.FromFile("easyicon_201810020300314909/1184885.png");
            trackBar_replay.Value = 1;

            //////stepCounts = test.Scripts.Control._instance[0].仿真步数;
            //////for (int i = 0; i < test.Scripts.Control._instance.Count; i++)
            //////{
            //////    test.Scripts.Control._instance[i]._agents.Clear();
            //////    test.Scripts.Control._instance[i].linesCount = 1;
            //////    test.Scripts.Control._instance[i].ReadInit();
            //////}
            instanceRead.readToFirst();

            labelPanelInfo.Text = GetInfoforPanel();
            doubleBufferPanel1.Refresh();
            UpdateUIpanelInfoDeligate("");
            chartOuts.Series[0].Points.Clear();
            //chartOuts.Series[1].Points.Clear();

            //for (int i = 0; i < Instance._out.Length; i++)
            //{
            //    chartOuts.Series[0].Points.AddXY((i + 1), Sample.outAgentsSetted[i]);
            //    //chartOuts.Series[1].Points.AddXY((i + 1), Sample.outAgentsSetted[i]);
            //}

            HeatMap.bmp=null;
            UpdateSimulateStatus(SimulateStates.ReadingEnd);
        }
        #endregion

        public void asyn(SimulateStates s)
        {

        }

        delegate void DeligateForStates(SimulateStates s);//委托
        

        /// <summary>
        /// 供异步线程调用，更新UI状态的
        /// </summary>
        /// <param name="s"></param>
        public void UpdateSimulateStatusDeligate(SimulateStates s)
        {
            this.Invoke(new DeligateForStates(delegate (SimulateStates state)
            {
                UpdateSimulateStatus(state);
            }), s);
        }
        public void UpdateSimulateStatus(SimulateStates s)
        {
            //根据当前状态, 设置UI界面的显示消失与失效
            //
            /*
             * 更新内容主要如下
             * 按钮仿真开始/暂停的文字与可用状态
             * 按钮仿真停止的状态
             * 按钮播放的可用状态
             * 播放栏的可视状态
             * 播放栏播放暂停的图片
             * 进度条的可视状态，寻径状态，远程仿真状态，可视
             * 
             * 仿真定时器 timerSimulate
             * 进度条定时器 timer_Progress
             * 读取定时器 timerRead
             * 热力图定时器
             */
            编辑ToolStripMenuItem.Enabled = true;

            test.Scripts.Control.state = s;
            switch (test.Scripts.Control.state)
            {
                case SimulateStates.项目为空:

                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = false;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = false;
                    panelReadPlay.Visible = false;
                    menu_Start.Image = test.Properties.Resources.simulate_play;//Image.FromFile("img_play.png");


                    //定时器
                    timerSimulate.Enabled = false;
                    timerProgressBar1.Enabled = false;
                    progressBar1.Visible = false;
                    timerRead.Enabled = false;
                    timerforHeatmap.Enabled = false;

                    //对“显示密度颜色参考”enable
                    cB_DrawColorRule.Visible = true;


                    //对显示热力图的按钮设置未点击
                    cb_drawHeatmap.Checked = false;
                    cb_drawHeatmap.Visible = false;
                    break;
                case SimulateStates.SimulatInited:

                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = false;
                    panelReadPlay.Visible = false;
                    menu_Start.Image = test.Properties.Resources.simulate_play;//Image.FromFile("img_play.png");


                    //定时器
                    timerSimulate.Enabled = false;
                    timerProgressBar1.Enabled = false;
                    progressBar1.Visible = false;
                    timerRead.Enabled = false;
                    timerforHeatmap.Enabled = false;

                    //对“显示密度颜色参考”enable
                    cB_DrawColorRule.Visible = true;


                    //对显示热力图的按钮设置未点击
                    cb_drawHeatmap.Checked = false;
                    cb_drawHeatmap.Visible = false;

                    break;

                case SimulateStates.SimulatPathfinding:
                    UpdateUIpanelInfoDeligate("正在寻径,请稍等…");
                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = false;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = false;
                    panelReadPlay.Visible = false;
                    menu_Start.Image = test.Properties.Resources.simulate_play;//Image.FromFile("img_play.png");


                    //定时器
                    timerSimulate.Enabled = false;
                    timerProgressBar1.Enabled = true;
                    progressBar1.Visible = true;
                    timerRead.Enabled = false;
                    timerforHeatmap.Enabled = false;
                    break;

                case SimulateStates.SimulatingLocalCPU:
                    UpdateUIpanelInfoDeligate("开始仿真……");
                    menu_Start.Text = "暂停";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = true;
                    播放ToolStripMenuItem.Enabled = false;
                    panelReadPlay.Visible = false;
                    panelAll.Enabled = true;
                    menu_Start.Image = test.Properties.Resources.simulate_pause;// Image.FromFile("img_pause.png");


                    progressBar1.Visible = false;

                    //定时器
                    timerSimulate.Enabled = true;
                    timerProgressBar1.Enabled = false;
                    timerRead.Enabled = false;
                    timerforHeatmap.Enabled = true;


                    //对显示热力图的按钮设置未点击
                    cb_drawHeatmap.Checked = false;
                    cb_drawHeatmap.Visible = false;

                    编辑ToolStripMenuItem.Enabled = false;

                    break;
                case SimulateStates.SimulatPaused:
                    menu_Start.Text = "继续";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = true;
                    播放ToolStripMenuItem.Enabled = false;
                    panelReadPlay.Visible = false;
                    menu_Start.Image = test.Properties.Resources.simulate_play;

                    timerforHeatmap.Enabled = false;
                    progressBar1.Visible = false;

                    //定时器
                    timerSimulate.Enabled = false;
                    timerProgressBar1.Enabled = false;
                    timerRead.Enabled = false;
                    timerforHeatmap.Enabled = false;

                    编辑ToolStripMenuItem.Enabled = false;

                    break;
                case SimulateStates.SimulateEnd:
                    //显示信息
                    //labelDebugInfo.Text = GetInfoforDebug();
                    labelPanelInfo.Text = "仿真结束";//GetInfoforPanel();
                    doubleBufferPanel1.Refresh();

                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = true;
                    panelReadPlay.Visible = false;
                    menu_Start.Image = test.Properties.Resources.simulate_play;
                    progressBar1.Visible = false;

                    //定时器
                    timerSimulate.Enabled = false;
                    timerProgressBar1.Enabled = false;
                    timerRead.Enabled = false;
                    timerforHeatmap.Enabled = false;

                    工程ToolStripMenuItem.Enabled = true;

                    //仿真停止
                    MessageBox.Show("仿真结束", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case SimulateStates.Reading:
                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = false;
                    menu_Start.Image = test.Properties.Resources.simulate_play;
                    panelReadPlay.Visible = true;
                    //trackBar_replay.Value = 1;
                    progressBar1.Visible = false;
                    pictureBox_Play.Image = test.Properties.Resources.video_pause;//用鼠标移动后，图片显示为暂停

                    //定时器
                    timerSimulate.Enabled = false;
                    timerProgressBar1.Enabled = false;
                    timerRead.Enabled = true;
                    timerforHeatmap.Enabled = false;

                    cb_drawHeatmap.Visible = true;
                    break;
                case SimulateStates.ReadingEnd:
                    menu_Start.Text = "仿真";
                    menu_Start.Enabled = true;
                    停止ToolStripMenuItem.Enabled = false;
                    播放ToolStripMenuItem.Enabled = true;
                    menu_Start.Image = test.Properties.Resources.simulate_play;
                    panelReadPlay.Visible = true;
                    progressBar1.Visible = false;
                    pictureBox_Play.Image = test.Properties.Resources.video_play;//用鼠标移动后，图片显示为暂停

                    //定时器
                    timerSimulate.Enabled = false;
                    timerProgressBar1.Enabled = false;
                    timerRead.Enabled = false;
                    timerforHeatmap.Enabled = false;

                    
                    break;
                case SimulateStates.SendingAgentsToRemote:
                    停止ToolStripMenuItem.Enabled = true;
                    break;

                case SimulateStates.SimulatingRemote:
                    停止ToolStripMenuItem.Enabled = true;
                    播放ToolStripMenuItem.Enabled = false;
                    progressBar1.Visible = true;

                    //定时器
                    timerSimulate.Enabled = false;
                    timerProgressBar1.Enabled = false;
                    timerRead.Enabled = false;
                    timerforHeatmap.Enabled = false;

                    工程ToolStripMenuItem.Enabled = false;
                    break;
                case SimulateStates.SimulatingLocalGPU:
                    停止ToolStripMenuItem.Enabled = true;
                    播放ToolStripMenuItem.Enabled = false;
                    progressBar1.Visible = true;

                    //定时器
                    timerSimulate.Enabled = false;
                    timerProgressBar1.Enabled = false;
                    timerRead.Enabled = false;
                    timerforHeatmap.Enabled = false;


                    工程ToolStripMenuItem.Enabled = false;
                    break;
            }
            this.Refresh();
        }


        #region 定时运行

        ///// <summary>
        /////新建工程，当前仿真没有结束时；
        /////点击停止按钮时；
        /////仿真自动停止时
        ///// </summary>
        //private void StopSimulate()
        //{
        //    int agentsCount = 0;
        //    for (int i = 0; i < Sample._instance.Count; i++)
        //    {
        //        try
        //        {
        //            Sample._instance[i].controlSteps = true;
        //            Sample._instance[i].sw.Stop();
        //            if (Sample._instance[i].stepsThread != null && Sample._instance[i].stepsThread.IsAlive) Sample._instance[i].stepsThread.Abort();
        //            Sample._instance[i].OutpositionFileEnd();
        //            Sample._instance[i]._agents.Clear();
                
        //            agentsCount += Sample._instance[i].agentsPositionsforExodus.Count;
        //        }
        //        catch
        //        {
        //            Console.WriteLine("stop 程序出错");
        //        }
        //    }


        //    //输出vrs
        //    if (checkBox_outputVRS.Checked)
        //    {
        //        FileHelper FHforExodus;
        //        FHforExodus = new FileHelper(Sample.mainDirectory + "_Exodus" + ".vrs");
        //        FHforExodus.Write("3 0");//先输出疏散人数
        //        FHforExodus.NewLine();//换行
        //        FHforExodus.Write(agentsCount.ToString() + " 0 " + getMaxStep());//先输出疏散人数
        //        for (int i = 0; i < Sample._instance.Count; i++)
        //        {
        //            Sample._instance[i].outForExodus(FHforExodus);
        //            Sample._instance[i].agentsPositionsforExodus.Clear();
        //        }
        //        FHforExodus.EndOut();
        //    }


        //    Sample.stepscontrolThread.Abort();
        //    doubleBufferPanel1.Refresh();

        //    UpdateSimulateStatus(SimulateStates.SimulateEnd);
        //}

        //设置图表
        private void UpdataChartInfo()
        {
            if(test.Scripts.Control.state== SimulateStates.Reading)
            {
                if(test.Scripts.Control._out != null)
                {
                    chartOuts.Series[0].Points.Clear();
                    
                    for (int i = 0; i < instanceRead.出口人数累计.Count; i++)
                    {
                        int outagents = instanceRead.出口人数累计[i];
                        chartOuts.Series[0].Points.AddXY((i + 1), outagents > 0 ? outagents : 0);
                        //s1 += " " + (i + 1).ToString() + "\r\n";
                        //s2 += "   " + (Sample.outAgentsSetted[i] - Instance._outAgentCount[i]) + "\r\n";
                        //s3 += "   " + (Instance._outAgentCount[i] * 100 / (Sample.numsPeople - agentsOutCount)).ToString() + "%\r\n";
                    }
                }
            }
            else
            {
                chartOuts.Series[0].Points.Clear();
            }


        }
        private int GetMaxOutAgentSetted()
        {
            int max = 0;
            for (int i = 0; i < test.Scripts.Control.出口人数统计.Count; i++)
            {
                max = max > test.Scripts.Control.出口人数统计[i] ? max : test.Scripts.Control.出口人数统计[i];
            }
            return max;
        }

        //参数显示
        public int agentsOutCount;
        public string GetInfoforDebug()
        {
            ////总人数，
            string s = "";
            //agentsOutCount = 0;
            //for (int i = 0; i < test.Scripts.Control._instance.Count; i++)
            //{
            //    agentsOutCount += test.Scripts.Control._instance[i]._agents.Count;
            //}
            //s += "总人数： " + agentsOutCount + " " + (agentsOutCount * 100 / test.Scripts.Control.设置.总人数) + "% \r\n";
            ////s += "疏散口1： " + Instance._outAgentCount[0] + " " + (Instance._outAgentCount[0] * 100 / Sample.numsPeople) + "% \r\n";

            //s += "0总步数： " + test.Scripts.Control._instance[0].仿真步数 + " 仿真时间：" + (test.Scripts.Control._instance[0].仿真步数 * Settings.deltaTDefault).ToString("0.0") + "秒 \r\n";
            //s += "计算耗时： " + (((float)(test.Scripts.Control._instance[0].sw.ElapsedMilliseconds)) / 1000).ToString("0.0") + "秒 \r\n";
            //s += AreaIdSelected + "  " + Instance.getAreaAcreage(test.Scripts.Control.tile, AreaIdSelected) + "  \r\n";
            ////s += Instance.getPolyAcreageTest();
            //s += "最大速度：" + test.Scripts.Control._instance[0].speedMaxTest + "  \r\n";
            ////s += "0的行走距离：" + Sample._instance[0].agentRoadLength + "  \r\n";
            //s += "鼠标距离： " + distance + "  \r\n";
            //s += "鼠标坐标：" + mouseRealX + " , " + mouseRealY + "  \r\n";
            //s += "mapcount：" + mapCount + "  \r\n";
            //s += "_zoom：" + _zoom + "  \r\n";
            //s += " _gridLeftTop：" + _gridLeftTop + "  \r\n";

            //for (int i = 0; i < test.Scripts.Control._instance.Count; i++)
            //{
            //    s += " linescount：" + test.Scripts.Control._instance[i].linesCount + "  \r\n";
            //}
            return s;
        }
        public string GetInfoforPanel()
        {
            //总人数，
            string s = "";

            s += "总人数： " + test.Scripts.Control.设置.总人数 + " \r\n\r\n";
            
            var time = getEvacuateSeconds();
            s += "疏散时间： " + time / 60 + "分 " + time % 60 + "秒 \r\n\r\n";
            //s += "计算耗时： " + (((float)(Sample._instance[0].sw.ElapsedMilliseconds)) / 1000).ToString("0.0") + "秒 \r\n";

            agentsOutCount = 0;
           
            agentsOutCount +=instanceRead._agents.Count;
            
            float agentOuted = test.Scripts.Control.设置.总人数 - agentsOutCount;
            //float outrated = (agentOuted * 100 / Sample.numsPeople);
         
            s += "已疏散人数：" + instanceRead.文件中读取的已疏散总人数 + "  百分比：" + (int)(instanceRead.文件中读取的已疏散总人数 * 100 / test.Scripts.Control.设置.总人数) + "% \r\n";
            
            return s;
        }

        /// <summary>
        /// 得到当前计算的疏散时间 单位是秒
        /// 用的int 因此最大疏散时间为1092分钟
        /// </summary>
        /// <returns></returns>
        public int getEvacuateSeconds()
        {
            return (int)(instanceRead.timeNow);
        }

        //Chart crowdChart = new Chart();
        //private void chBoxChart_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (chBoxChart.CheckState == CheckState.Checked)
        //    {
        //        crowdChart.Show(this);
        //    }
        //    else
        //    {
        //        crowdChart.Hide();
        //        //crowdChart.Dispose();
        //    }
        //}



        /// <summary>
        /// 读取仿真结果时，刷新界面，更新左侧显示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerSimulate_Tick(object sender, EventArgs e)
        {
            //if (test.Scripts.Control._instance != null)//&& stepCounts != Sample._instance[0].stepCounts
            //{
            //    //显示信息
            //    labelDebugInfo.Text = GetInfoforDebug();
            //    labelPanelInfo.Text = GetInfoforPanel();

            //    //刷新显示
            //    doubleBufferPanel1.Refresh();
            //    stepCounts = test.Scripts.Control._instance[0].仿真步数;

            //    //更新出口人数统计的图表
            //    UpdataChartInfo();//更新左上角的pannel

            //    //更新出口流率统计
            //    label5.Text = Instance.liulu;
            //}
        }

        int infectCounts = 0;//用来记录上次感染第几次
        System.Timers.Timer timerforHeatmap = new System.Timers.Timer();
        /// <summary>
        /// 用来控制是否感染
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerforHeatmap_Tick(object sender, EventArgs e)
        {
            
            if (!heatmapThreadControl)
            {
                if (test.Scripts.Control.设置.infectionIntervel_time > 0)////写的不好，重新改。。。。
                {
                    if (infectCounts != stepCounts / test.Scripts.Control.设置.infectionIntervel_time)//判断如果这次间隔数跟上次间隔数不同
                    {
                        Console.WriteLine("infectCounts " + infectCounts);
                        Console.WriteLine("stepCounts  " + stepCounts);
                       
                        HeatMap.CalculateColor计算颜色控制速度(ref test.Scripts.Control.agentsAll, stepCounts, true);//stepCounts
                        infectCounts = stepCounts / test.Scripts.Control.设置.infectionIntervel_time;
                    }
                    else//如果相同，就不用更新感染网格
                    {
                        HeatMap.CalculateColor计算颜色控制速度(ref test.Scripts.Control.agentsAll, stepCounts, false);//stepCounts
                    }
                }
                else
                {
                    //try
                    //{
                    //    foreach (var i in Sample._instance)
                    //    {
                    //        foreach (var agent in i._agents)
                    //        {
                    //            agent.state = AgentStates.Evacuating;
                    //        }
                    //    }
                    //}
                    //catch
                    //{

                    //}

                    HeatMap.CalculateColor计算颜色控制速度(ref test.Scripts.Control.agentsAll, stepCounts, false);//stepCounts
                }
            }
            {
                //生成马赛克形式的热力图，没有写
            }
        }

        /// <summary>
        /// 读取仿真结果时，刷新界面，更新左侧显示信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        static long timerreadcount = 0;
        private void timerRead_Tick(object sender, EventArgs e)
        {
            timerReadnoThread();
        }
        private void timerReadnoThread()
        {
            //while(true)
            {
                int lineNow = instanceRead.GetMaxReadLine();
                if (lineNow > trackBar_replay.Maximum)
                {
                    //lineNow = trackBar_replay.Maximum;
                    return;
                }

                instanceRead.ReadLineNoThread(0, speedMulti);//0是默认时的另外情况判断, 这里不用跳,所以0

                HeatMap.CalculateColor计算颜色控制速度(ref instanceRead._agents, 0, false);

                if (lineNow > 0) trackBar_replay.Value = lineNow;
                //if (trackBar_replay.Value < trackBar_replay.Maximum)
                //{
                //    if (lineNow > 0) trackBar_replay.Value = lineNow > trackBar_replay.Maximum ? trackBar_replay.Maximum : lineNow;
                //}

                //更新柱状图
                UpdataChartInfo();

                //判断如果出口12和出口13 已经满了的情况下，弹出提示
                checkFull();

                //显示信息
                labelDebugInfo.Text = GetInfoforDebug();
                labelPanelInfo.Text = GetInfoforPanel();

                //刷新显示
                doubleBufferPanel1.Refresh();
                //stepCounts = InstanceRVO.仿真步数;
                Console.WriteLine("读取一次 lineNow " + lineNow);
            }
        }


        int 上次是否广场满员 = 0;
        private void checkFull()
        {
            if (上次是否广场满员==0 && instanceRead.判断是否广场人满弹窗[0]==1)
            {
                上次是否广场满员 = instanceRead.判断是否广场人满弹窗[0];
                //弹窗一次
                MessageBox.Show("广场人员已满，还有"+ instanceRead.判断是否广场人满弹窗[1] +"人未疏散", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            上次是否广场满员 = instanceRead.判断是否广场人满弹窗[0];
        }

        private void timerReadnoThread原来的()
        {
            ////while(true)
            //{
            //    int lineNow = test.Scripts.Control.GetMaxReadLine();
            //    if (lineNow > trackBar_replay.Maximum)
            //    {
            //        //lineNow = trackBar_replay.Maximum;
            //        return;
            //    }

            //    List<List<AgentClass>> _agentss = new List<List<AgentClass>>();

            //    for (int i = 0; i < test.Scripts.Control._instance.Count; i++)
            //    {
            //        //Sample._instance[i].ReadformLines();
            //        test.Scripts.Control._instance[i].ReadLineNoThread(0, speedMulti);//0是默认时的另外情况判断, 这里不用跳,所以0
            //        //Sample._instance[i].ReadLineThread();
            //        _agentss.Add(test.Scripts.Control._instance[i]._agents);
            //    }

            //    Window.HeatMap.CalculateColor计算颜色控制速度(ref _agentss, 0, false);

            //    //List<Thread> _T = new List<Thread>();
            //    //foreach (Instance i in Sample._instance)
            //    //{
            //    //    Thread t = new Thread(i.ReadLineThread);
            //    //    _T.Add(t);
            //    //    t.Start();
            //    //}
            //    ////等待4个线程全部完成
            //    //for (int i = 0; i < _T.Count; i++)
            //    //{
            //    //    while (_T[i].IsAlive) ;
            //    //}


            //    if (lineNow > 0) trackBar_replay.Value = lineNow;
            //    //if (trackBar_replay.Value < trackBar_replay.Maximum)
            //    //{
            //    //    if (lineNow > 0) trackBar_replay.Value = lineNow > trackBar_replay.Maximum ? trackBar_replay.Maximum : lineNow;
            //    //}

            //    //更新柱状图
            //    UpdataChartInfo();

            //    //显示信息
            //    labelDebugInfo.Text = GetInfoforDebug();
            //    labelPanelInfo.Text = GetInfoforPanel();

            //    //刷新显示
            //    doubleBufferPanel1.Refresh();
            //    stepCounts = test.Scripts.Control._instance[0].仿真步数;
            //    Console.WriteLine("读取一次 lineNow " + lineNow);

            //}
        }
        #endregion

        private void chartOuts_Click(object sender, EventArgs e)
        {

        }


        private void SidePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void CB_ShowExits_CheckedChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void CB_DrawColorRule_CheckedChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void trackBar_speed_ValueChanged(object sender, EventArgs e)
        {
            switch (trackBar_speed.Value)
            {
                case 0:
                    speedMulti = 1;
                    break;
                case 1:
                    speedMulti = 2;
                    break;
                case 2:
                    speedMulti = 5;
                    break;
                case 3:
                    speedMulti = 10;
                    break;
                case 4:
                    speedMulti = 20;
                    break;
                case 5:
                    speedMulti = 20;
                    break;
            }
            int interval = timerReadInterval / speedMulti;
            timerRead.Interval = interval > 0 ? interval : 10;//防止出现为0导致出错
            //timerRead.Interval = 1;
        }

        private void GroupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void Cb_drawHeatmap_CheckedChanged(object sender, EventArgs e)
        {
            HeatMap.heatmapControl = cb_drawHeatmap.Checked;
            HeatMap.bmp = null;
        }

        private void TrackBar_speed_Scroll(object sender, EventArgs e)
        {
            
        }

        private void 编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void MenuStrip2_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void Panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            SocketClient.Close();
        }
    }
}
