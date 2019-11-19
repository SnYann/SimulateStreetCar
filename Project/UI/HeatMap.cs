using Simulate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Window
{

    static partial class HeatMap
    {
        public class Cell
        {
            public List<int> idrow;//地图的第几块标志，地图一共4个区域
            public List<int> idcolumn;
            public Color color;
            public bool infected = false;
            public bool newInfected = false;
            public float count = 0;
            public Cell()
            {
                idrow = new List<int>();
                idcolumn = new List<int>();
            }
        }

        //static int pixWidth;//四分之一像素宽度
        public static int cellWidth = 10;//单位为分米，每个格子宽度为2米
        //static int girdWidth;//100个格子

        //public static List<float> DensityFloat = new List<float>();

        //static Cell[][] mapData;
        public static bool heatmapControl = true;


        //public HeatMap()
        //{

        //    mapData = new Cell[girdWidth][];
        //    girdWidth = pixWidth / cellWidth;
        //    for (int i = 0; i < mapData.Length; i++)
        //    {
        //        mapData[i] = new Cell[girdWidth];
        //        for (int j = 0; j < girdWidth; j++)
        //        {
        //            mapData[i][j] = new Cell();
        //        }
        //    }
        //}

        ////统计表格的数据,搜寻每个格子的agent有多少个，并放到mapData里面
        //public static Bitmap CalculateMap(ref List<AgentClass> _agents)
        //{


        //    //找到最大的agent坐标，4个值，x最大最小，y最大最小，得到边界范围
        //    //根据边界范围和格子宽度申请内存
        //    //
        //    float xMax = 0;
        //    float yMax = 0;
        //    float xMin = 0;
        //    float yMin = 0;

        //    for (int j = 0; j < _agents.Count; j++)
        //    {
        //        if (_agents[j].positionNow.x_ > xMax) xMax = _agents[j].positionNow.x_;
        //        if (_agents[j].positionNow.y_ > yMax) yMax = _agents[j].positionNow.y_;
        //        if (_agents[j].positionNow.x_ < xMin) xMin = _agents[j].positionNow.x_;
        //        if (_agents[j].positionNow.y_ < yMin) yMin = _agents[j].positionNow.y_;
        //    }

        //    //扩展取整边界
        //    xMax = (int)xMax + cellWidth;
        //    yMax = (int)yMax + cellWidth;
        //    xMin = (int)xMin - cellWidth;
        //    xMin = (int)xMin - cellWidth;

        //    //矩阵偏置
        //    float xOffset = -xMin;
        //    float yOffset = -yMin;

        //    //矩阵大小
        //    int xLength = (int)(xMax + xOffset+0.5f);
        //    int yLength = (int)(yMax + yOffset+0.5f);
        //    int xGirds = xLength * 10 / cellWidth;
        //    int yGirds = yLength * 10 / cellWidth;

        //    //申请矩阵
        //    Cell[][] mapData = new Cell[xGirds][];
        //    for (int i = 0; i < mapData.Length; i++)
        //    {
        //        mapData[i] = new Cell[yGirds];
        //        for (int j = 0; j < yGirds; j++)
        //        {
        //            mapData[i][j] = new Cell();
        //        }
        //    }

        //    //统计每个格子的人数
        //    for (int i = 0; i < _agents.Count; i++)
        //    {
        //        int row = (int)((_agents[i].positionNow.x_ + xOffset) * 10 / cellWidth);
        //        int column = (int)((_agents[i].positionNow.y_ + yOffset) * 10 / cellWidth);
        //        mapData[row][column].id.Add(i);
        //    }

        //    //根据格子人数设置颜色
        //    for (int r = 2; r < mapData.Length-2; r++)
        //    {
        //        for (int c = 2; c < mapData[r].Length-2; c++)
        //        {
        //            //int count = mapData[r][c].id == null ? 0 : mapData[r][c].id.Count;
        //            float count = ((mapData[r - 1][c].id.Count + mapData[r + 1][c].id.Count + mapData[r][c - 1].id.Count + mapData[r][c + 1].id.Count) + (mapData[r - 1][c - 1].id.Count + mapData[r + 1][c - 1].id.Count + mapData[r - 1][c + 1].id.Count + mapData[r + 1][c + 1].id.Count) + mapData[r][c].id.Count*4);
        //            //int count2 = ((mapData[r - 2][c].id.Count + mapData[r + 1][c].id.Count + mapData[r][c - 1].id.Count + mapData[r][c + 1].id.Count) + (mapData[r - 1][c - 1].id.Count + mapData[r + 1][c - 1].id.Count + mapData[r - 1][c + 1].id.Count + mapData[r + 1][c + 1].id.Count)) /8;



        //            mapData[r][c].color = GetColor(count, 0, maxNum);
        //            for (int i = 0; i < mapData[r][c].id.Count; i++)
        //            {
        //                try
        //                {
        //                    _agents[mapData[r][c].id[i]].color = mapData[r][c].color;
        //                }
        //                catch
        //                {
        //                }
        //            }
        //        }
        //    }



        //    Bitmap bmp = new Bitmap(xGirds * cellWidth, yGirds * cellWidth);
        //    var ig = Graphics.FromImage(bmp);
        //    Brush brush = new SolidBrush(Color.Red);
        //    for (int i = 0; i < mapData.Length; i++)
        //    {
        //        for (int j = 0; j < mapData[i].Length; j++)
        //        {
        //            Color color = Color.FromArgb(100, mapData[i][j].color.R, mapData[i][j].color.G, mapData[i][j].color.B);
        //            brush = new SolidBrush(color);
        //            ig.FillRectangle(brush, new Rectangle(i * cellWidth, j * cellWidth, cellWidth, cellWidth));
        //        }
        //    }
        //    brush.Dispose();
        //    ig.Dispose();

        //    //Color[][] colors = new Color[mapData.Length][];
        //    //for (int i = 0; i < mapData.Length; i++)
        //    //{
        //    //    colors[i] = new Color[mapData[i].Length];
        //    //    for (int j = 0; j < mapData[i].Length; j++)
        //    //    {
        //    //        colors[i][j] = Color.FromArgb(100, mapData[i][j].color.R, mapData[i][j].color.G, mapData[i][j].color.B);

        //    //    }
        //    //}
        //    return bmp;

        //}


        public static Cell[][] mapData;
        public static float xOffset;
        public static float yOffset;
        static int xGirds;
        static int yGirds;
        public static void HeatMapInit()
        {
            //找到最大的agent坐标，4个值，x最大最小，y最大最小，得到边界范围
            //根据边界范围和格子宽度申请内存
            //
            float xMax = 200;
            float yMax = 200;
            float xMin = -250;
            float yMin = -200;

            //动态分析大小的时候用的
            //for (int i = 0; i < _agents.Count; i++)
            //{
            //    for (int j = 0; j < _agents.Count; j++)
            //    {
            //        if (_agents[i][j].positionNow.x_ > xMax) xMax = _agents[i][j].positionNow.x_;
            //        if (_agents[i][j].positionNow.y_ > yMax) yMax = _agents[i][j].positionNow.y_;
            //        if (_agents[i][j].positionNow.x_ < xMin) xMin = _agents[i][j].positionNow.x_;
            //        if (_agents[i][j].positionNow.y_ < yMin) yMin = _agents[i][j].positionNow.y_;
            //    }
            //}


            //扩展取整边界
            xMax = xMax + cellWidth;
            yMax = yMax + cellWidth;
            xMin = xMin - cellWidth;
            yMin = yMin - cellWidth;

            //矩阵偏置
            xOffset = -xMin;
            yOffset = -yMin;

            //矩阵大小
            int xLength = (int)(xMax + xOffset);
            int yLength = (int)(yMax + yOffset);
            xGirds = xLength * 10 / cellWidth;
            yGirds = yLength * 10 / cellWidth;

            //申请矩阵
            mapData = new Cell[xGirds][];
            for (int i = 0; i < mapData.Length; i++)
            {
                mapData[i] = new Cell[yGirds];
                for (int j = 0; j < yGirds; j++)
                {
                    mapData[i][j] = new Cell();
                }
            }

            //第一种情况，中心扩散
            mapData[255][225].infected = true;
            ////第二种情况，四周开始扩散
            //for (int i = 0; i < mapData.Length; i++)
            //{
            //    mapData[i][20].infected = true;
            //    mapData[i][yGirds - 3].infected = true;
            //}
            //for (int i = 0; i < mapData[0].Length; i++)
            //{
            //    mapData[20][i].infected = true;
            //    mapData[xGirds - 3][i].infected = true;
            //}


            Init();

        }


        static List<List<AgentClass>> _agents=new List<List<AgentClass>>();//不同区域不同组的agents
        static int stepcounts;
        static bool updataInfection;
        public static void CalculateColor计算颜色控制速度(ref List<AgentClass> _agents_, int stepcounts_, bool updataInfection_)
        {
            if (_agents_ == null) return;
            _agents.Clear();//先把_agents清空， 主要原因是原来计算颜色时不同区域，现在只有一个，所以要先清空
            _agents.Add(_agents_);
            stepcounts = stepcounts_;
            updataInfection = updataInfection_;
            //Thread t = new Thread(calculateColorThread);
            //t.Start();
            calculateColorThread();
        }
        public static Bitmap calculateColorFromQueuesForImg(ref List<List<AgentClass>> _agents)
        {
            if (mapData == null || _agents == null) return null;
            for (int i = 0; i < mapData.Length; i++)
            {
                for (int j = 0; j < mapData[i].Length; j++)
                {
                    mapData[i][j].idrow.Clear();
                    mapData[i][j].idcolumn.Clear();
                }
            }
            //统计每个格子的人数
            try
            {
                for (int i = 0; i < _agents.Count; i++)
                {
                    for (int j = 0; j < _agents[i].Count; j++)
                    {
                        int row = (int)((_agents[i][j].positionNow.x_ + xOffset) * 10 / cellWidth);
                        int column = (int)((_agents[i][j].positionNow.y_ + yOffset) * 10 / cellWidth);
                        if (row >= 0 && row < xGirds && column >= 0 && column < yGirds)
                        {
                            mapData[row][column].idrow.Add(i);
                            mapData[row][column].idcolumn.Add(j);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("统计每个格子人数出错");
            }
            //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒1");
            //根据格子人数设置颜色
            for (int r = 2; r < mapData.Length - 2; r++)
            {
                for (int c = 2; c < mapData[r].Length - 2; c++)
                {
                    //int count = mapData[r][c].id == null ? 0 : mapData[r][c].id.Count;
                    float count = ((mapData[r - 1][c].idrow.Count + mapData[r + 1][c].idrow.Count + mapData[r][c - 1].idrow.Count + mapData[r][c + 1].idrow.Count) + (mapData[r - 1][c - 1].idrow.Count + mapData[r + 1][c - 1].idrow.Count + mapData[r - 1][c + 1].idrow.Count + mapData[r + 1][c + 1].idrow.Count) + mapData[r][c].idrow.Count * 4);
                    //int count2 = ((mapData[r - 2][c].id.Count + mapData[r + 1][c].id.Count + mapData[r][c - 1].id.Count + mapData[r][c + 1].id.Count) + (mapData[r - 1][c - 1].id.Count + mapData[r + 1][c - 1].id.Count + mapData[r - 1][c + 1].id.Count + mapData[r + 1][c + 1].id.Count)) /8;

                    mapData[r][c].count = count;
                    count /= 27;//得到每平米多少人
                    mapData[r][c].color = GetColor(count, 1);

                    //先求出xy的偏移多少
                    int offsetX = (mapData[r][c - 1].idrow.Count - mapData[r][c + 1].idrow.Count);
                    int offsetY = (mapData[r - 1][c].idrow.Count - mapData[r + 1][c].idrow.Count);

                    lock (_agents)
                    {
                        try
                        {
                            for (int i = 0; i < mapData[r][c].idrow.Count; i++)
                            {
                                if (mapData[r][c].idcolumn[i] < _agents[mapData[r][c].idrow[i]].Count)
                                {
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].color = mapData[r][c].color;
                                    // _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeed = _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].MaxSpeed * ChangeVelocity(count);
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeed = ChangeVelocity2(count, _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].MaxSpeed);
                                    //_agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeed =  ChangeVelocity(count);
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeedx = offsetX / 6;
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeedy = offsetY / 6;
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("又超出范围，统计颜色后，被删除了，会出现这个错误");
                        }
                    }
                }
            }
            return outHeatmapFromQueue();
        }
        public static void calculateColorThread()
        {
            Stopwatch sw = new Stopwatch();//用来帮助计算程序耗时
            sw.Start();
            //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒0");

            //for (int i = 0; i < mapData.Length; i++)
            //{
            //    mapData[i] = new Cell[yGirds];
            //    for (int j = 0; j < yGirds; j++)
            //    {
            //        mapData[i][j]=new Cell();
            //    }
            //}
            //for (int i = 0; i < mapData.Length; i++)
            //{
            //    for (int j = 0; j < mapData[i].Length; j++)
            //    {
            //        mapData[i][j].idrow.Clear();
            //        mapData[i][j].idcolumn.Clear();
            //    }
            //}

            //统计每个格子的人数
            try
            {
                for (int i = 0; i < mapData.Length; i++)
                {
                    for (int j = 0; j < mapData[i].Length; j++)
                    {
                        mapData[i][j].idrow.Clear();
                        mapData[i][j].idcolumn.Clear();
                        mapData[i][j].count = 0;
                    }
                }

                for (int i = 0; i < _agents.Count; i++)
                {
                    for (int j = 0; j < _agents[i].Count; j++)
                    {
                        int row = (int)((_agents[i][j].positionNow.x_ + xOffset) * 10 / cellWidth);
                        int column = (int)((_agents[i][j].positionNow.y_ + yOffset) * 10 / cellWidth);
                        if (row >= 0 && row < xGirds && column >= 0 && column < yGirds)
                        {
                            mapData[row][column].idrow.Add(i);
                            mapData[row][column].idcolumn.Add(j);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("统计每个格子人数出错");
            }


            //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒1");
            //根据格子人数设置颜色
            for (int r = 2; r < mapData.Length - 2; r++)
            {
                for (int c = 2; c < mapData[r].Length - 2; c++)
                {
                    //int count = mapData[r][c].id == null ? 0 : mapData[r][c].id.Count;
                    float count = ((mapData[r - 1][c].idrow.Count + mapData[r + 1][c].idrow.Count + mapData[r][c - 1].idrow.Count + mapData[r][c + 1].idrow.Count) + (mapData[r - 1][c - 1].idrow.Count + mapData[r + 1][c - 1].idrow.Count + mapData[r - 1][c + 1].idrow.Count + mapData[r + 1][c + 1].idrow.Count) + mapData[r][c].idrow.Count * 4);
                    //int count2 = ((mapData[r - 2][c].id.Count + mapData[r + 1][c].id.Count + mapData[r][c - 1].id.Count + mapData[r][c + 1].id.Count) + (mapData[r - 1][c - 1].id.Count + mapData[r + 1][c - 1].id.Count + mapData[r - 1][c + 1].id.Count + mapData[r + 1][c + 1].id.Count)) /8;

                    mapData[r][c].count = count;
                    count /= 12;//得到每平米多少人 原来格子是1.5*1.5 2.25*12=27 现在是1*1*12=12；
                    //if (count > 3) Console.WriteLine("count: "+count);
                    if (heatmapControl == true) mapData[r][c].color = GetColor(count, 1);

                    //先求出xy的偏移多少
                    int offsetX = (mapData[r][c - 1].idrow.Count - mapData[r][c + 1].idrow.Count);
                    int offsetY = (mapData[r - 1][c].idrow.Count - mapData[r + 1][c].idrow.Count);

                    lock (_agents)
                    {
                        try
                        {
                            for (int i = 0; i < mapData[r][c].idrow.Count; i++)
                            {
                                if (mapData[r][c].idcolumn[i] < _agents[mapData[r][c].idrow[i]].Count)
                                {
                                    if (heatmapControl == true) _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].color = mapData[r][c].color;
                                    // _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeed = _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].MaxSpeed * ChangeVelocity(count);
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeed = ChangeVelocity2(count, _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].MaxSpeed);
                                    //_agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeed =  ChangeVelocity(count);
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeedx = offsetX / 6;
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeedy = offsetY / 6;
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("又超出范围，统计颜色后，被删除了，会出现这个错误");
                        }
                    }
                }
            }

            if (heatmapControl == true) outHeapmapForPool();

            if (updataInfection)
            {
                //2. 把所有已经感染的方块设置成不是新感染的
                for (int r = 1; r < mapData.Length; r++)
                {
                    for (int c = 1; c < mapData[r].Length; c++)
                    {
                        mapData[r][c].newInfected = false;
                    }
                }

                for (int r = 2; r < mapData.Length - 1; r++)
                {
                    for (int c = 2; c < mapData[r].Length - 1; c++)
                    {

                        //如果当前方块是不是当次扫描新感染的方块，就把四周未感染的方块变为新感染状态
                        if (mapData[r][c].newInfected == false && mapData[r][c].infected == true)
                        {
                            //只有有agent的格子才会比感染，缺点是人少会出问题，所以暂时不用了
                            //bool noAgents = true;
                            //for (int i = 0; i < mapData[r][c].idrow.Count; i++)
                            //{
                            //    if (mapData[r][c].idcolumn.Count > 0) noAgents = false;
                            //}
                            //if (!noAgents)//只有有agent的格子才会比感染，缺点是人少会出问题

                            {
                                //以该格子为中心的九个格子
                                for (int rr = -1; rr < 2; rr++)
                                {
                                    for (int cc = -1; cc < 2; cc++)
                                    {
                                        mapData[r + rr][c + cc].newInfected = true;
                                        mapData[r + rr][c + cc].infected = true;

                                        //将新感染的方块内agent变为感染态
                                        for (int i = 0; i < mapData[r + rr][c + cc].idrow.Count; i++)
                                        {
                                            int x = mapData[r + rr][c + cc].idrow[i];
                                            int y = mapData[r + rr][c + cc].idcolumn[i];
                                            if (y < _agents[x].Count && _agents[x][y].state != AgentStates.Evacuating)
                                            {
                                                _agents[x][y].state = AgentStates.Evacuating;
                                                _agents[x][y].timeResponse += (int)(Settings.deltaTDefault * stepcounts);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }

            }
        }

        public static void CalculateColorOld(ref List<List<AgentClass>> _agents, int stepcounts, bool updataInfection)
        {
            if (heatmapControl == false) return;
            Stopwatch sw = new Stopwatch();//用来帮助计算程序耗时
            sw.Start();
            //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒0");

            //for (int i = 0; i < mapData.Length; i++)
            //{
            //    mapData[i] = new Cell[yGirds];
            //    for (int j = 0; j < yGirds; j++)
            //    {
            //        mapData[i][j]=new Cell();
            //    }
            //}
            for (int i = 0; i < mapData.Length; i++)
            {
                for (int j = 0; j < mapData[i].Length; j++)
                {
                    mapData[i][j].idrow.Clear();
                    mapData[i][j].idcolumn.Clear();
                }

            }

            //统计每个格子的人数
            try
            {
                for (int i = 0; i < _agents.Count; i++)
                {
                    for (int j = 0; j < _agents[i].Count; j++)
                    {
                        int row = (int)((_agents[i][j].positionNow.x_ + xOffset) * 10 / cellWidth);
                        int column = (int)((_agents[i][j].positionNow.y_ + yOffset) * 10 / cellWidth);
                        if (row >= 0 && row < xGirds && column >= 0 && column < yGirds)
                        {
                            mapData[row][column].idrow.Add(i);
                            mapData[row][column].idcolumn.Add(j);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("统计每个格子人数出错");
            }


            //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒1");
            //根据格子人数设置颜色
            for (int r = 2; r < mapData.Length - 2; r++)
            {
                for (int c = 2; c < mapData[r].Length - 2; c++)
                {
                    //int count = mapData[r][c].id == null ? 0 : mapData[r][c].id.Count;
                    float count = ((mapData[r - 1][c].idrow.Count + mapData[r + 1][c].idrow.Count + mapData[r][c - 1].idrow.Count + mapData[r][c + 1].idrow.Count) + (mapData[r - 1][c - 1].idrow.Count + mapData[r + 1][c - 1].idrow.Count + mapData[r - 1][c + 1].idrow.Count + mapData[r + 1][c + 1].idrow.Count) + mapData[r][c].idrow.Count * 4);
                    //int count2 = ((mapData[r - 2][c].id.Count + mapData[r + 1][c].id.Count + mapData[r][c - 1].id.Count + mapData[r][c + 1].id.Count) + (mapData[r - 1][c - 1].id.Count + mapData[r + 1][c - 1].id.Count + mapData[r - 1][c + 1].id.Count + mapData[r + 1][c + 1].id.Count)) /8;

                    mapData[r][c].count = count;
                    count /= 27;//得到每平米多少人
                    mapData[r][c].color = GetColor(count, 1);

                    //先求出xy的偏移多少
                    int offsetX = (mapData[r][c - 1].idrow.Count - mapData[r][c + 1].idrow.Count);
                    int offsetY = (mapData[r - 1][c].idrow.Count - mapData[r + 1][c].idrow.Count);

                    lock (_agents)
                    {
                        try
                        {
                            for (int i = 0; i < mapData[r][c].idrow.Count; i++)
                            {
                                if (mapData[r][c].idcolumn[i] < _agents[mapData[r][c].idrow[i]].Count)
                                {
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].color = mapData[r][c].color;
                                    // _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeed = _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].MaxSpeed * ChangeVelocity(count);
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeed = ChangeVelocity2(count, _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].MaxSpeed);
                                    //_agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeed =  ChangeVelocity(count);
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeedx = offsetX / 6;
                                    _agents[mapData[r][c].idrow[i]][mapData[r][c].idcolumn[i]].ControlSpeedy = offsetY / 6;
                                }
                            }
                        }
                        catch
                        {
                            Console.WriteLine("又超出范围，统计颜色后，被删除了，会出现这个错误");
                        }
                    }
                }
            }

            outHeapmapForPool();

            if (updataInfection)
            {
                //2. 把所有已经感染的方块设置成不是新感染的
                for (int r = 1; r < mapData.Length; r++)
                {
                    for (int c = 1; c < mapData[r].Length; c++)
                    {
                        mapData[r][c].newInfected = false;
                    }
                }

                for (int r = 2; r < mapData.Length - 1; r++)
                {
                    for (int c = 2; c < mapData[r].Length - 1; c++)
                    {

                        //如果当前方块是不是当次扫描新感染的方块，就把四周未感染的方块变为新感染状态
                        if (mapData[r][c].newInfected == false && mapData[r][c].infected == true)
                        {
                            //只有有agent的格子才会比感染，缺点是人少会出问题，所以暂时不用了
                            //bool noAgents = true;
                            //for (int i = 0; i < mapData[r][c].idrow.Count; i++)
                            //{
                            //    if (mapData[r][c].idcolumn.Count > 0) noAgents = false;
                            //}
                            //if (!noAgents)//只有有agent的格子才会比感染，缺点是人少会出问题

                            {
                                //以该格子为中心的九个格子
                                for (int rr = -1; rr < 2; rr++)
                                {
                                    for (int cc = -1; cc < 2; cc++)
                                    {
                                        mapData[r + rr][c + cc].newInfected = true;
                                        mapData[r + rr][c + cc].infected = true;

                                        //将新感染的方块内agent变为感染态
                                        for (int i = 0; i < mapData[r + rr][c + cc].idrow.Count; i++)
                                        {
                                            int x = mapData[r + rr][c + cc].idrow[i];
                                            int y = mapData[r + rr][c + cc].idcolumn[i];
                                            if (y < _agents[x].Count && _agents[x][y].state != AgentStates.Evacuating)
                                            {
                                                _agents[x][y].state = AgentStates.Evacuating;
                                                _agents[x][y].timeResponse += (int)(Settings.deltaTDefault * stepcounts);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }

            }

            //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒2");

        }



        //public static float ChangeVelocity(float count)
        //{
        //    float n=1f;
        //    if (count > 3.5f)
        //        n = 0.25f;
        //    else if(count > 3f)
        //        n = 0.27f;
        //    else if (count > 2.6f)
        //        n = 0.32f;
        //    else if (count > 2.2f) n = 0.4f;
        //    else if (count > 1.8f) n = 0.5f;
        //    else if (count > 1.4) n = 0.6f;
        //    else if (count > 1) n = 0.7f;
        //    else if (count > 0.6) n = 0.8f;
        //    else if (count > 0.3) n = 0.9f;
        //    return n;
        //}

        static float DS_ALPHA = 0.42f; //用于控制速度密度关系的参数a
        static float DS_BETA = 0.032f; //用于控制速度密度关系的参数b
        static float DS_GAMMA = 0.15f; //用于控制速度密度关系的参数y
        static float RHO1M = 3; //用于控制速度密度关系的参数rho1m
        static float RHO2M = 2; //用于控制速度密度关系的参数rho2m
        static float RHO1C = 0.89f; //用于控制速度密度关系的参数rho1c
        static float RHO2C = 1.33f; //用于控制速度密度关系的参数rho2c

        public static float ChangeVelocity2(float rho, float maxspeed)
        {
            float rho1star = (RHO1C + RHO1M) / 2;
            float rho2star = (RHO2C + RHO2M) / 2;

            double A = Math.Log(RHO1M * rho2star / rho) / Math.Log(RHO1M / RHO1C);
            float B = (rho1star * RHO2M - rho) / (rho1star * (RHO2M - RHO2C));

            float um = (float)(DS_ALPHA * A + DS_BETA * B + DS_GAMMA);
            return Math.Min(um, maxspeed);
        }
        public static float ChangeVelocity(float count)
        {
            float n = 1f;
            if (count > 3.6f)
                n = 0.2f;
            else if (count > 3f)
                n = 0.2f;
            else if (count > 2.6f)
                n = 0.4f;
            else if (count > 2.2f) n = 0.5f;
            else if (count > 1.7f) n = 0.6f;
            else if (count > 1.2) n = 0.7f;
            else if (count > 0.8) n = 0.8f;
            else if (count > 0.6) n = 0.9f;
            //else if (count > 0.2) n = 0.8f;
            return n;
            //float n = 1f;
            //if (count > 3.5f)
            //    n = 0.29f;
            //else if (count > 3f)
            //    n = 0.33f;
            //else if (count > 2.6f)
            //    n = 0.38f;
            //else if (count > 2.2f) n = 0.4f;
            //else if (count > 1.8f) n = 0.5f;
            //else if (count > 1.4) n = 0.6f;
            //else if (count > 1) n = 0.7f;
            //else if (count > 0.6) n = 0.8f;
            //else if (count > 0.3) n = 0.9f;
            //return n;
        }
        /*
                float n = 1f;
            if (count > 3.5f)
                n = 0.25f;
            else if (count > 3f)
                n = 0.27f;
            else if (count > 2.6f)
                n = 0.32f;
            else if (count > 2.2f) n = 0.4f;
            else if (count > 1.8f) n = 0.5f;
            else if (count > 1.4) n = 0.6f;
            else if (count > 1) n = 0.7f;
            else if (count > 0.6) n = 0.8f;
            else if (count > 0.3) n = 0.9f;
            return n;
             */


        //public static Bitmap CalculateMap(ref List<List<AgentClass>> _agents)
        //{
        //    CalculateColor(ref _agents);
        //    Bitmap bmp = new Bitmap(xGirds * cellWidth - 2, yGirds * cellWidth - 2);
        //    var ig = Graphics.FromImage(bmp);
        //    Brush brush = new SolidBrush(Color.Red);
        //    for (int i = 2; i < mapData.Length - 2; i++)
        //    {
        //        for (int j = 2; j < mapData[i].Length - 2; j++)
        //        {
        //            Color color = Color.FromArgb(250, mapData[i][j].color.R, mapData[i][j].color.G, mapData[i][j].color.B);
        //            brush = new SolidBrush(color);
        //            ig.FillRectangle(brush, new Rectangle(i * cellWidth, j * cellWidth, cellWidth, cellWidth));
        //        }
        //    }
        //    brush.Dispose();
        //    ig.Dispose();
        //    //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒3");
        //    return bmp;

        //}



        //public static Color GetColor(decimal value, decimal min, decimal max)
        //{
        //    if (value > max) value = max;
        //    decimal val = 255 * (value - min) / (max - min);
        //    return Color.FromArgb(255, Convert.ToByte(val) , Convert.ToByte(255  - val),0);
        //    //return new Color
        //    //{
        //    //    A = 255,
        //    //    R = Convert.ToByte(255 * val),
        //    //    G = Convert.ToByte(255 * (1 - val)),
        //    //    B = 0
        //    //};
        //}

        public static float H;
        public static float S;
        public static float V;
        //public static Color GetColor(float value, float min, float max)
        //{
        //    if (value==0)return Color.FromArgb(0,255,255,255) ;
        //    if (value > max) value = max;
        //    //decimal val = 255 * (value - min) / (max - min);
        //    //return Color.FromArgb(255, Convert.ToByte(val) , Convert.ToByte(255  - val),0);
        //    //double H = 240 * Math.Pow(Convert.ToDouble((value - min) / (max - min)), 0.618);
        //    double H = 240 * Convert.ToDouble((value - min) / (max - min));
        //    double S = 0.25 * Convert.ToDouble((value - min) / (max - min));
        //    double V = 0.4 * Convert.ToDouble((value - min) / (max - min));
        //    return HSV2RGB(240 - H, 1, 1);

        //    //return HSV2RGB(H,S/100,V/100);

        //    //return HSV2RGB(240 - H, 0.75+S, 0.7);
        //    //return new Color
        //    //{
        //    //    A = 255,
        //    //    R = Convert.ToByte(255 * val),
        //    //    G = Convert.ToByte(255 * (1 - val)),
        //    //    B = 0
        //    //};
        //}

        //先求出密度
        public static Color GetColor(float value, float min)
        {
            //原来Exodus的参数
            //if(value<0.88)
            //{
            //    return Color.FromArgb(255, 0, 0, 255);
            //}
            //else if (value < 1.32)
            //{
            //    return Color.FromArgb(255, 0, 255, 255);
            //}
            //else if (value < 2.2)
            //{
            //    return Color.FromArgb(255, 0, 255,0);
            //}
            //else if (value < 2.64)
            //{
            //    return Color.FromArgb(255, 255, 255, 0);
            //}
            //else if (value < 3.52)
            //{
            //    return Color.FromArgb(255, 255, 0, 255);
            //}
            //else 
            //{
            //    return Color.FromArgb(255, 255, 0,0);
            //}

            ////////新版可调节颜色
            //////if (value <= DensityFloat[0])
            //////{
            //////    return Color.FromArgb(255, 0, 0, 255);
            //////}
            //////else if (value <= DensityFloat[1])
            //////{
            //////    return Color.FromArgb(255, 0, 255, 255);
            //////}
            //////else if (value <= DensityFloat[2])
            //////{
            //////    return Color.FromArgb(255, 0, 255, 0);
            //////}
            //////else if (value <= DensityFloat[3])
            //////{
            //////    return Color.FromArgb(255, 255, 255, 0);
            //////}
            //////else if (value <= DensityFloat[4])
            //////{
            //////    return Color.FromArgb(255, 255, 154, 0);
            //////}
            //////else
            //////{
            //////    return Color.FromArgb(255, 255, 0,0);
            //////}
            ///
            return Color.Blue;

        }

        public static Color GetColor(float density)
        {
            var _12个网格面积 = 12 * cellWidth * cellWidth / 100;
            int value = (int)(density * _12个网格面积 * densityColorFactor);
            return HSVtoRGB(240 - value, 255, 255, 255);
        }

        public static Color HSV2RGB(double H, double S, double V)
        {
            double R, G, B;
            R = G = B = 0;
            H /= 60;
            int i = Convert.ToInt32(Math.Floor(H));
            double f = H - i;
            double a = V * (1 - S);
            double b = V * (1 - S * f);
            double c = V * (1 - S * (1 - f));
            switch (i)
            {
                case 0: R = V; G = c; B = a; break;
                case 1: R = b; G = V; B = a; break;
                case 2: R = a; G = V; B = c; break;
                case 3: R = a; G = b; B = V; break;
                case 4: R = c; G = a; B = V; break;
                case 5: R = V; G = a; B = b; break;
            }
            R = R > 1.0 ? 255 : R * 255;
            G = G > 1.0 ? 255 : G * 255;
            B = B > 1.0 ? 255 : B * 255;
            return Color.FromArgb(255, Convert.ToInt32(R), Convert.ToInt32(G), Convert.ToInt32(B));
        }

        public static Color HSVRGB(float hsvH, float hsvS, float hsvV)
        {
            if (hsvH == 360) hsvH = 359; // 360为全黑，原因不明
            float R = 0f, G = 0f, B = 0f;
            if (hsvS == 0)
            {
                return Color.FromArgb((int)hsvV, (int)hsvV, (int)hsvV);
            }
            float S = hsvS * 1.0f / 255, V = hsvV * 1.0f / 255;
            int H1 = (int)(hsvH * 1.0f / 60), H = (int)hsvH;
            float F = H * 1.0f / 60 - H1;
            float P = V * (1.0f - S);
            float Q = V * (1.0f - F * S);
            float T = V * (1.0f - (1.0f - F) * S);
            switch (H1)
            {
                case 0: R = V; G = T; B = P; break;
                case 1: R = Q; G = V; B = P; break;
                case 2: R = P; G = V; B = T; break;
                case 3: R = P; G = Q; B = V; break;
                case 4: R = T; G = P; B = V; break;
                case 5: R = V; G = P; B = Q; break;
            }
            R = R * 255;
            G = G * 255;
            B = B * 255;
            while (R > 255) R -= 255;
            while (R < 0) R += 255;
            while (G > 255) G -= 255;
            while (G < 0) G += 255;
            while (B > 255) B -= 255;
            while (B < 0) B += 255;
            return Color.FromArgb((int)R, (int)G, (int)B);
        }

    }
}
