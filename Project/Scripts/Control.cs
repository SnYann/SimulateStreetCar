using ICSharpCode.SharpZipLib.Zip;
using optimize;
using RVO;
using SharpNav;
using SharpNav.Geometry;
using SharpNav.Pathfinding;
using Simulate;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Window;
using Vector2 = Simulate.Vector2;

namespace test.Scripts
{
    

    public enum SimulateStates
    {
        项目为空,
        SimulatInited, //刚刚设置结束
        SimulatPathfinding,//寻径结束
        SendingAgentsToRemote,//发送agent文件到远程
        SimulatingLocalCPU,//开始仿真
        SimulatingLocalGPU,//开始仿真
        SimulatPaused,//仿真暂停
        SimulateEnd,//仿真结束
        Reading,//正在读取
        ReadingEnd,//读取结束
        SimulatingRemote//远程仿真状态
    }
    //一个调用示例
    static class Control
    {
        /** 
         * 初始化地图
         * 初始化设置
         *
         */

        #region 地图相关
            public static Simulate.Navigation songnav; //导航网格
            public static NavTile tile;
        #endregion
    
        public static string mainDirectory;
        public static string projectName;
        public static string snbName = "f8.snb";
        #region 读取设置的一些参数，作为静态变量传入

        #endregion

        public static List<Area> _areas;//所有区域数据
        public static List<AgentClass> agentsAll;//全部人
        public static OutDoors[] _out;

        public static List<int> 出口人数统计 = new List<int>();//每个出口预先安排应该疏散的人数

        //public List<ControlGate> controlGates;//用来控制道路的单向双向
        
        internal static bool 是否输出位置文件;
        
        
        //private static int frameCount = 0;//统计当前帧数

        public static Stopwatch sw = new Stopwatch();//用来帮助计算程序耗时

        //线程
        public static Thread stepscontrolThread;//线程，用来控制每部分地图的同步仿真

        public static SimulateStates state;

        public delegate void UpdateUIpanelInfoDelegate(string info);
        public static UpdateUIpanelInfoDelegate UpdateUIpanelInfo;

        public delegate void UpdateSimulateStatusDelegate(SimulateStates state);
        public static UpdateSimulateStatusDelegate UpdateSimulateStatus;

        public static UI.Setting 设置;

        

        public static void Clear()//读取文件时用,新的工程建立时用
        {

            是否输出位置文件 = false;
            //_areas.Clear();
            //outAgentsSetted.Clear();
            //清空图标的数据
            for (int i=0;i<出口人数统计.Count;i++)
            {
                出口人数统计[i] = 0;
            }
        }


        public static void SampleInit()
        {
            //1. 初始化地图
            sw.Start();
            InitMap();
            Console.WriteLine("读取生成耗时： " + sw.ElapsedMilliseconds / 1000);

            //2.得到疏散点给Instance
            _out = new OutDoors[songnav.map._out.Count];
            //同一个输出目标的疏散人数统计,初始化0
            for (int i = 0; i < _out.Count(); i++) 出口人数统计.Add(0);

            for (int i = 0; i < songnav.map._out.Count; i++)
            {
                _out[i] = new OutDoors(songnav.map._out[i], songnav.map._outWidth[i]);
            }
            //优化方法1 给出口宽度赋值
            //float[] width = {13.6f,15.14f,12.18f,26f,11.73f,9.1f,6.47f,7.28f,13.5f,18.43f,5.76f};
            //float[] width = { 13.6f, 15.14f, 12.18f, 26f, 11.73f, 10f, 6.47f, 7.28f, 13.5f, 18.43f, 5.76f };// ,6f ,7f};
            //float[] width = { 13.6f, 15.14f, 12.18f, 26f, 11.73f, 9.1f, 6.47f, 7.28f, 13.5f, 18.43f, 5.76f ,20f ,7f};
            //float[] width = { 15, 15, 5, 5 };
            //for (int i = 0; i < _out.Length; i++) _out[i].width = width[i];

        }

        public static void ControlInit(string 主目录,string 项目名)
        {
            //1. 设置路径
            //2. 清空原本数据
            //3. 读取设置文件
            mainDirectory = 主目录;
            projectName = 项目名;
            Clear();
            设置 = new UI.Setting(mainDirectory + projectName);
        }


        public static bool SimulateInit()
        {
            Clear();
            设置 = new UI.Setting(mainDirectory + projectName);

            是否输出位置文件 = true;


#if outDebug
        ////输出agent信息文件2
        FileHelper.SetOutPath("mydebug.txt");
#endif
            //3.得到不同区域的数据，并设定不同区域人数
            if(设置.区域数据来源=="手动设置")
            {
                从手动设置得到区域数据(out _areas);//从密度和面积得到区域人数
            }
            else
            {
                从实时数据得到区域数据(out _areas);//直接读取区域人数
            }

            //4. 根据区域人数数据, 得到地图上随机分布的人
            agentsAll = new List<AgentClass>();
            foreach (var area in _areas)
            {
                AddAreaAgents(area, ref _out, agentsAll);
            }

            ////5.初始化Instance
            //List<Line> obs = new List<Line>();
            ////得到分区域的obs
            //obs = songnav.GetObstacleByArea(_areas);
            //InstanceRVO instance = new InstanceRVO("output" + 0, _areas, obs, getMap(), 设置.outids);

            return true;
        }

        #region 初始化人相关
        /// <summary>
        /// 根据不同poly的不同面积大小进行随机人数分配的方法
        /// </summary>
        public static void AddAreaAgents(Area area, ref OutDoors[] _out, List<AgentClass> _agents)
        {
            float agentCount = area.headMaxcount;
            int areaID = area.Id;
            int recevieTime = area.receiveTime;

            if (agentCount == 0) return;
            Simulate.Vector2 posStart;
            Simulate.Vector2 posTarget;
            int positionIndex = 0;
            var tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);//得到网格

            //同一个区域同一个疏散目标点，因此只运行一次就可以了
            //根据已知区域，得到疏散目标

            int colorIndex = 0;


            if (areaID != 0xfe)//如果是已经选中设定好的区域，就找到他们的预制好的目标
            {
                //得到区域的目标在out数组中的位置

                //从areas中找到areaID相同的area，得到其目标
                int i;
                for (i = 0; i < _areas.Count; i++) if (_areas[i].Id == areaID) break;
                positionIndex = _areas[i].getGoal();//这样写，别忘了正向反向的方向无法被文件记住,positionIndex即为区域的疏散目标在out数组中的位置
                colorIndex = positionIndex;//颜色指标，跟疏散点对应，相同疏散点，相同颜色
            }
            else///如果是还未设定的区域
            {
                //从areas中找到areaID相同的area，得到其目标
                int i;
                for (i = 0; i < _areas.Count; i++) if (_areas[i].Id == areaID) break;
                positionIndex = _areas[i].getGoal();//这样写，别忘了正向反向的方向无法被文件记住,positionIndex即为区域的疏散目标在out数组中的位置
                colorIndex = positionIndex;//颜色指标，跟疏散点对应，相同疏散点，相同颜色
            }
            Console.WriteLine("目标out" + positionIndex);
            //else//如果是还未设定的区域，就重置一次最终目标 根据最近的出口是哪个(加上判断，如果当前目标的目标点是0,0,0，则重新寻找最近疏散目标点)
            //{
            //    int min = 0;
            //    float minVector2 = Simulate.MathHelper.abs(posStart - nav.level._out[0]);
            //    float delta = Simulate.MathHelper.abs(posStart - nav.level._out[0]);
            //    for (int n = 0; n < nav.level._out.Count; n++)
            //    {
            //        if (delta > Simulate.MathHelper.abs(nav.level._out[n] - posStart))
            //        {
            //            min = n;
            //            delta = Simulate.MathHelper.abs(nav.level._out[n] - posStart);
            //        }
            //    }
            //    //min = new Random().Next(nav.level._out.Count);
            //    colorIndex = min;
            //    //NavPoint navP = nav.navMeshQuery.FindRandomPointAroundCircle(nav.navMeshQuery.FindNearestPoly(new Vector3(e.x_, 0, e.y_), new Vector3(10, 10, 10)), 5);
            //    Vector3 navP = nav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(nav.navMeshQuery.FindNearestPoly(new Vector3(nav.level._out[min].x_, 0, nav.level._out[min].y_), new Vector3(20, 20, 20)).Polygon.Id));
            //    posTarget = new Simulate.Vector2(navP.X, navP.Z);
            //}


            //为每个agent配置随机起始与目标点
            //随机选择位置
            Vector3 pointStart;
            int areaIndex;

            /**在mesh中 找到相同ID的poly，求出各自面积
                * 根据总人数*单独面积/总面积得到当前poly的人数，然后找随机点
                */
            List<NavPoly> polyLists = new List<NavPoly>();
            float acreageAll = 0;
            for (int j = 0; j < tile.Polys.Length; j++)
            {
                if (tile.Polys[j].Area.Id == areaID)
                {
                    polyLists.Add(tile.Polys[j]);
                    acreageAll += getPolyAcreage(tile, tile.Polys[j]);
                }
            }

            //if (1 == 1)
            //{
            //    Vector3 navP = songnav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav.navMeshQuery.FindNearestPoly(_out[9], new Vector3(20, 20, 20)).Polygon.Id));
            //    posTarget = new Simulate.Vector2(navP.X, navP.Z);
            //    //Console.WriteLine("总面积" + acreageAll);
            //    posStart = new Simulate.Vector2(24, 13.8f);
            //    int nov = RVOInstance.addAgent(new RVO.Vector2(posStart.x_, posStart.y_), Settings.RVODefault.neighborDist, Settings.RVODefault.maxNeighbors, Settings.RVODefault.timeHorizon, Settings.RVODefault.timeHorizonObst, Settings.RVODefault.radius, 2, new RVO.Vector2(1, 1));
            //    if(nov==0)
            //    _agents.Add(new AgentClass(nov, posStart, posTarget, AgentStates.Enter, 3));//初始化agent编号、位置、目标、状态，根据目标设定颜色

            //}


            int polyAgents = 0;
            for (int j = 0; j < tile.Polys.Length; j++)
            {
                if (tile.Polys[j].Area.Id == areaID)
                {
                    float acreage = getPolyAcreage(tile, tile.Polys[j]);
                    polyAgents = (int)Math.Round((agentCount * acreage / acreageAll), MidpointRounding.AwayFromZero);
                    Console.WriteLine("poly人数：" + polyAgents);
                    //Console.WriteLine("面积" + acreage + " ;人数" + polyAgents);
                    for (int n = 0; n < polyAgents; n++)
                    {
                        pointStart = songnav.navMeshQuery.FindRandomPointOnPoly(songnav.navMeshQuery.nav.IdManager.Encode(1, 0, j));//根据polyIndex对其编码输出该poly的ID
                        posStart = new Simulate.Vector2(pointStart.X, pointStart.Z);
                        //Console.WriteLine(positionIndex);
                        int outIndex;
                        if (positionIndex == -1 || Control.设置.路径选择方式 == "就近选择")//如果出口数字是-1，代表还未设定的区域或者需要自己计算哪个出口位置最近就去哪个的区域，就重置一次最终目标 根据最近的出口是哪个(加上判断，如果当前目标的目标点是0,0,0，则重新寻找最近疏散目标点)
                        {
                            int min = 设置.outids[0];
                            //float minVector2 = Simulate.MathHelper.abs(posStart - songnav1.level._out[0]);
                            float delta = Simulate.MathHelper.abs(posStart - songnav.map._out[设置.outids[0]]);
                            for (int m = 0; m < 设置.outids.Count; m++)
                            {
                                if (delta > Simulate.MathHelper.abs(songnav.map._out[设置.outids[m]] - posStart))
                                {
                                    min = 设置.outids[m];
                                    delta = Simulate.MathHelper.abs(songnav.map._out[设置.outids[m]] - posStart);
                                }
                            }
                            //min = new Random().Next(nav.level._out.Count);
                            colorIndex = min;

                            //Console.WriteLine("min "+min);
                            //NavPoint navP = nav.navMeshQuery.FindRandomPointAroundCircle(nav.navMeshQuery.FindNearestPoly(new Vector3(e.x_, 0, e.y_), new Vector3(10, 10, 10)), 5);
                            Vector3 navP = songnav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav.navMeshQuery.FindNearestPoly(_out[min].position, new Vector3(20, 20, 20)).Polygon.Id));
                            Control.出口人数统计[min]++;
                            posTarget = new Simulate.Vector2(navP.X, navP.Z);
                            outIndex = min;

                        }
                        else
                        {
                            Vector3 navP = songnav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav.navMeshQuery.FindNearestPoly(_out[positionIndex].position, new Vector3(20, 20, 20)).Polygon.Id));
                            Control.出口人数统计[positionIndex]++;
                            posTarget = new Simulate.Vector2(navP.X, navP.Z);
                            outIndex = positionIndex;

                        }
                        //var pointEnd = nav.navMeshQuery.FindRandomPoint();//随机选择位置，后面重新赋值
                        //posTarget = new Simulate.Vector2(pointEnd.Position.X, pointEnd.Position.Z);
                        //float randomSpeed = Simulate.MathHelper.RandomNormal(10, 1.2f, 0.8f, 1.6f);//这里超过1，只是计算距离有意义，速度不会超过1，除非改其他参数

                        _agents.Add(getNewAgent(posStart, posTarget, AgentStates.Enter, colorIndex, outIndex, recevieTime));//初始化agent编号、位置、目标、状态，根据目标设定颜色
                        //_agents.Add(new AgentClass(nov, posStart, posTarget, AgentStates.Enter));//初始化agent编号、位置、目标、状态
                    }
                }
            }
        }
        public static Simulate.Vector2 求广场内随机点2()
        {
            var p1 = new Simulate.Vector2(47.75f, -51.71f);
            var p2 = new Simulate.Vector2(79.21f, -60.55f);
            var p3 = new Simulate.Vector2(85.27f, -86.11f);
            var p4 = new Simulate.Vector2(58.90f, -90.05f);
            Random random = new System.Random(Guid.NewGuid().GetHashCode());//得到不会重复的数字
            int r = random.Next(2);
            Simulate.Vector2 pos;

            if (r == 0) pos = 三角形内随机点(p1, p2, p3);
            else pos = 三角形内随机点(p1, p3, p4);

            return pos;
        }
        public static Simulate.Vector2 求广场内随机点()
        {
            var p1 = new Vector2(11.86667f, -134.5667f);
            var p2 = new Vector2(6f, -99.9f);
            var p3 = new Vector2(74.26667f, -86.56667f);
            var p4 = new Vector2(84.4f, -122.8333f);
            Random random = new System.Random(Guid.NewGuid().GetHashCode());//得到不会重复的数字
            int r = random.Next(2);
            Vector2 pos;

            if (r == 0) pos = 三角形内随机点(p1, p2, p3);
            else pos = 三角形内随机点(p1, p3, p4);

            return pos;
        }
        public static Vector2 三角形内随机点(Vector2 A, Vector2 B, Vector2 C)
        {
            Random random = new System.Random(Guid.NewGuid().GetHashCode());//得到不会重复的数字
            var r1 = random.NextDouble();
            var r2 = random.NextDouble();
            return ((float)(1 - Math.Sqrt(r1))) * A + ((float)(Math.Sqrt(r1) * (1 - r2))) * B + ((float)(Math.Sqrt(r1) * r2)) * C;
        }

        /// 从人员配置中随机选择人物类型和其对应的速度与反应时间
        public static AgentClass getNewAgent(Simulate.Vector2 posStart, Simulate.Vector2 posTarget, AgentStates enter, int colorIndex, int outIndex, int recevieTime)
        {
            //从人员配置中随机选择人物类型和其对应的速度与反应时间
            int r = Simulate.MathHelper.random.Next(100);
            for (int i = 0; i < Control.设置.peoples.Count; i++)
            {
                if (r < Control.设置.peoples[i].占比累加值)//这里已经假设人物属性里的占比最大累计数是递增
                {
                    float maxSpeed = Simulate.MathHelper.RandomUniform(Control.设置.peoples[i].speedMin, Control.设置.peoples[i].speedMax);
                    //int nov = RVOInstance.addAgent(new RVO.Vector2(posStart.x_, posStart.y_), Settings.RVODefault.neighborDist, Settings.RVODefault.maxNeighbors, Settings.RVODefault.timeHorizon, Settings.RVODefault.timeHorizonObst, Settings.RVODefault.radius, maxSpeed, new RVO.Vector2(1, 1));
                    //Agent agent = RVOInstance.addAgent(new RVO.Vector2(posStart.x_, posStart.y_), Settings.RVODefault.neighborDist, Settings.RVODefault.maxNeighbors, Settings.RVODefault.timeHorizon, Settings.RVODefault.timeHorizonObst, Settings.RVODefault.radius, maxSpeed, new RVO.Vector2(1, 1));
                    int responseTime = 0;
                    if (Control.设置.响应时间分布方式 == "平均分布") responseTime = (int)(Simulate.MathHelper.RandomUniform(Control.设置.peoples[i].responseTimeMin, Control.设置.peoples[i].responseTimeMax));//0代表平均分布
                    else responseTime = (int)(Simulate.MathHelper.RandomNormal(Control.设置.peoples[i].responseTimeMin, Control.设置.peoples[i].responseTimeMax));//均匀分布

                    //根据当前位置更改反应时间
                    //responseTime+=(int)MathHelper.abs(new Vector2(posStart.x_ + 12, posStart.y_ - 12));
                    //Console.WriteLine("responseTime " + responseTime);
                    if (Control.设置.疏散时间接收方式 == "按区域设置") enter = AgentStates.Evacuating;
                    int nov = i;
                    if (Math.Abs(posTarget.x_ + 220) < 5 && Math.Abs(posTarget.y_ - 40) < 5)
                    {
                        Console.WriteLine("asdfasd");
                    }
                    AgentClass newAgent = new AgentClass(nov, posStart, posTarget, enter, colorIndex, outIndex, maxSpeed, responseTime + recevieTime, Control.设置.peoples[i].name);//, agent);
                    return newAgent;
                }
            }
            Console.WriteLine("添加人出错，属性没超过100");
            return new AgentClass(0, posStart, posTarget, enter, colorIndex, outIndex, 0, 0, "人物出错");//, null);
        }

        #region 面积相关函数
        //song 求一个区域的面积,通过areaID
        public static float getAreaAcreage(NavTile tile, int areaID)
        {
            float acreage = 0;
            for (int j = 0; j < tile.Polys.Length; j++)
            {
                if (tile.Polys[j].Area.Id == areaID)
                {
                    acreage += getPolyAcreage(tile, tile.Polys[j]);
                }
            }
            return acreage;
        }
        //song 求一个poly的面积
        public static float getPolyAcreage(NavTile tile, NavPoly poly)
        {
            float acreage = 0;
            //开始遍历三角形，求出面积
            for (int j = 2; j < PathfindingCommon.VERTS_PER_POLYGON; j++)
            {
                if (poly.Verts[j] == 0)
                    break;

                int vertIndex0 = poly.Verts[0];
                int vertIndex1 = poly.Verts[j - 1];
                int vertIndex2 = poly.Verts[j];

                var v1 = tile.Verts[vertIndex0];
                var v2 = tile.Verts[vertIndex1];
                var v3 = tile.Verts[vertIndex2];

                float a = (v1 - v2).Length();
                float b = (v1 - v3).Length();
                float c = (v2 - v3).Length();

                float m = (a + b + c) / 2;
                acreage += (float)Math.Sqrt(m * (m - a) * (m - b) * (m - c));
            }
            return acreage;
        }

        public static float getPolyAcreageTest()
        {
            float acreage = 0;
            //开始遍历三角形，求出面积

            float a = 3;
            float b = 4;
            float c = 5;

            float m = (a + b + c) / 2;
            acreage += (float)Math.Sqrt(m * (m - a) * (m - b) * (m - c));

            return acreage;
        }
        #endregion
        #endregion

        public static Thread PathFindandSimulateThread;
        public static void PathFindandSimulate()
        {
            PathFindandSimulateThread = new Thread(PathFindandSimulateforThread);
            PathFindandSimulateThread.Start();
        }
        public static int approchNov = 0;

        public static void PathFindandSimulateforThread()
        {
            if (approchNov == 0)
            {
                List<Thread> _T = new List<Thread>();
                Thread t = new Thread(PathFind);
                _T.Add(t);
                t.Start(agentsAll);
                //等待4个线程全部完成
                for (int i = 0; i < _T.Count; i++)
                {
                    while (_T[i].IsAlive) ;
                }
            }
            else if (approchNov == 1)
            {
                ////路径优化 方法1
                //List<AgentClass> agentsAll = new List<AgentClass>();
                //for (int i = 0; i < _instance.Count; i++)
                //{
                //    agentsAll.AddRange(_instance[i]._agents);
                //}
                //Approch1 approch1 = new Approch1(agentsAll, songnav);
                ////优化方法1-根据当前目标搜索路径
                //for (int i = 0; i < agentsAll.Count; i++)
                //{
                //    var path = songnav.SmothPathfinding_paths(agentsAll[i].positionNow, Instance._out[agentsAll[i].outIndex].getPositionOnPloyV2());
                //    agentsAll[i].navPoints.Clear();
                //    agentsAll[i].navPoints = songnav.SmothPathfinding_Points(path);

                //    var pos = Instance._out[agentsAll[i].outIndex].position;
                //    agentsAll[i].navPoints.Add(new Simulate.Vector2(pos.X, pos.Z));
                //    agentsAll[i].setColorfromIndex(agentsAll[i].outIndex);
                //}
            }

            Console.WriteLine("寻径耗时： " + sw.ElapsedMilliseconds / 1000);


            //重置每个人的疏散反应时间
            reCalculateResponseTime();

            //寻径结束，开始仿真
            StartSimulate();
        }


        #region 寻径相关的函数

        //public void PathFind()
        //{
        //    sw.Start();
        //    //计算路径 第一次赋初始位置和目标位置时计算一次
        //    Console.WriteLine("开始计算路径");

        //    //Thread T1 = new Thread(PathFindandCheck);
        //    //T1.Start(_agents);
        //    //while (T1.IsAlive) ;
        //    PathFindandCheck();
        //    Console.WriteLine("计算路径结束");
        //    Console.WriteLine("初始化人物耗时： " + sw.ElapsedMilliseconds / 1000);
        //    sw.Stop();
        //}
        public static int pathFindedNums;
        //public void PathFindandCheck()//(Object obj)
        //{
        //    //List<AgentClass> agents = (List<AgentClass>)obj;
        //    List<AgentClass> agents = _agents;
        //    //别忘重新安置这段代码
        //    //nav.roadfilter.SetAreaCost(2, 1000);
        //    //var tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);

        //    pathFindedNums = 0;
        //    for (int i = 0; i < agents.Count; i++)
        //    {
        //        pathFindedNums = i;
        //        SharpNav.Pathfinding.Path path = songnav.SmothPathfinding_paths(agents[i].positionNow, agents[i].positionTarget);
        //        //SharpNav.Pathfinding.Path path1 = songnav.SmothPathfinding_paths(agents[i-1].positionNow, agents[i-1].positionTarget);
        //        #region 方向
        //        //控制方向的
        //        //for (int p = 0; p < path.polys.Count - 1; p++)
        //        //{
        //        //    for (int cg = 0; cg < controlGates.Count; cg++)
        //        //    {
        //        //        if(p>=path.polys.Count || cg>=controlGates.Count)
        //        //        {
        //        //            Console.WriteLine(" P太大 ");
        //        //        }
        //        //        else if (path.polys[p].Id == controlGates[cg].npi1.Id)
        //        //        {
        //        //            if (path.polys[p + 1].Id == controlGates[cg].npi2.Id)
        //        //            {
        //        //                tile.Polys[controlGates[cg].poly1].Area = new Area(2);
        //        //                path.Clear();
        //        //                path = nav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);

        //        //                //Console.WriteLine(i + " 逆向");
        //        //            }
        //        //        }
        //        //    }
        //        //    for (int cg = 0; cg < controlGates.Count; cg++)
        //        //    {
        //        //        tile.Polys[controlGates[cg].poly1].Area = new Area(1);
        //        //    }
        //        //}
        //        #endregion

        //        for (int pi = 0; pi < path.Count; pi++) agents[i].polyIds.Add(path[pi].Id);
        //        //Console.WriteLine(instanceID+" poly点 "+i);
        //        agents[i].navPoints = songnav.SmothPathfinding_Points(path);

        //        //for (int iii = 0; iii < _agents[i].navPoints.Count - 1; iii++)
        //        //{
        //        //    if (_agents[i].navPoints[iii].x_ == 0 && _agents[i].navPoints[iii].y_ == 0)
        //        //    {
        //        //        _agents[i].navPoints.RemoveAt(iii);
        //        //    }
        //        //    if (iii > 2 && MathHelper.abs(_agents[i].navPoints[iii]-_agents[i].navPoints[iii-1])<5)
        //        //    {
        //        //        _agents[i].navPoints.RemoveAt(iii);
        //        //        iii--;
        //        //    }
        //        //}

        //        //agents[i].navPoints.Add(agents[i].positionTarget);
        //        //把最后位置的目标也加进去
        //        var outPoint = _out[agents[i].outIndex];
        //        agents[i].navPoints.Add(new Vector2(outPoint.X, outPoint.Z));
        //        ////如果是在出口10，则把最后的目标点也加上
        //        //Vector2 out11 = new Vector2(215.21f,11.3f);
        //        //if( MathHelper.abs(agents[i].positionTarget-out11)<50)
        //        //{
        //        //    agents[i].navPoints.Add(out11);
        //        //}

        //        if (agents[i].navPoints[0].x_ == 0 && agents[i].navPoints[0].y_ == 0)
        //        {
        //            Console.WriteLine(instanceID + "编号 " + i + " 路径计算出错");
        //        }
        //        if (agents[i].navPoints.Count > 2000)
        //        {
        //            Console.WriteLine(instanceID + "编号 " + i + " 路径过多 " + agents[i].navPoints.Count);
        //        }
        //    }
        //}

        public static void PathFind(Object obj)
        {
            List<AgentClass> agents = (List<AgentClass>)obj;
            sw.Start();
            //计算路径 第一次赋初始位置和目标位置时计算一次
            Console.WriteLine("开始计算路径");

            List<AgentClass> agentssub1 = new List<AgentClass>();
            for (int i = 0; i < agents.Count; i++)
            {
                agentssub1.Add(agents[i]);
            }
            pathFindedNums = 0;
            Thread T1 = new Thread(PathFindandCheck);
            T1.Start(agentssub1);
            while (T1.IsAlive) ;
            Console.WriteLine("计算路径结束");
            Console.WriteLine("初始化人物耗时： " + sw.ElapsedMilliseconds / 1000);
            sw.Stop();
        }
        public static void PathFindandCheck(Object obj)
        {
            List<AgentClass> agents = (List<AgentClass>)obj;
            //List<AgentClass> agents = _agents;
            //别忘重新安置这段代码
            //nav.roadfilter.SetAreaCost(2, 1000);
            //var tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);

            for (int i = 0; i < agents.Count; i++)
            {
                pathFindedNums++;
                SharpNav.Pathfinding.Path path = songnav.SmothPathfinding_paths(agents[i].positionNow, agents[i].positionTarget);
                //SharpNav.Pathfinding.Path path1 = songnav.SmothPathfinding_paths(agents[i-1].positionNow, agents[i-1].positionTarget);
                #region 方向
                //控制方向的
                //for (int p = 0; p < path.polys.Count - 1; p++)
                //{
                //    for (int cg = 0; cg < controlGates.Count; cg++)
                //    {
                //        if(p>=path.polys.Count || cg>=controlGates.Count)
                //        {
                //            Console.WriteLine(" P太大 ");
                //        }
                //        else if (path.polys[p].Id == controlGates[cg].npi1.Id)
                //        {
                //            if (path.polys[p + 1].Id == controlGates[cg].npi2.Id)
                //            {
                //                tile.Polys[controlGates[cg].poly1].Area = new Area(2);
                //                path.Clear();
                //                path = nav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);

                //                //Console.WriteLine(i + " 逆向");
                //            }
                //        }
                //    }
                //    for (int cg = 0; cg < controlGates.Count; cg++)
                //    {
                //        tile.Polys[controlGates[cg].poly1].Area = new Area(1);
                //    }
                //}
                #endregion

                for (int pi = 0; pi < path.Count; pi++) agents[i].polyIds.Add(path[pi].Id);
                //Console.WriteLine(instanceID+" poly点 "+i);
                agents[i].navPoints = songnav.SmothPathfinding_Points(path);

                偏移处理导航点(agents[i]);

                //if(agents[i].navPoints[1].x_==0)
                //{
                //    Console.WriteLine(agents[i].navPoints.Count + "  count");
                //    Console.WriteLine(agents[i].positionNow);
                //}

                //方法1
                agents[i].exitDistance = (int)path.Cost;

                //for (int iii = 0; iii < _agents[i].navPoints.Count - 1; iii++)
                //{
                //    if (_agents[i].navPoints[iii].x_ == 0 && _agents[i].navPoints[iii].y_ == 0)
                //    {
                //        _agents[i].navPoints.RemoveAt(iii);
                //    }
                //    if (iii > 2 && MathHelper.abs(_agents[i].navPoints[iii]-_agents[i].navPoints[iii-1])<5)
                //    {
                //        _agents[i].navPoints.RemoveAt(iii);
                //        iii--;
                //    }
                //}

                //agents[i].navPoints.Add(agents[i].positionTarget);
                var outPoint = _out[agents[i].outIndex].position;
                agents[i].navPoints.Add(new Vector2(outPoint.X, outPoint.Z));

                //如果目标是广场，就把广场随机点加入进去
                if (agents[i].outIndex == 11)
                {
                    agents[i].navPoints.Add(求广场内随机点());
                }
                if (agents[i].outIndex == 12)
                {
                    agents[i].navPoints.Add(求广场内随机点2());
                }

                ////如果是在出口10，则把最后的目标点也加上
                //Vector2 out11 = new Vector2(215.21f,11.3f);
                //if( MathHelper.abs(agents[i].positionTarget-out11)<50)
                //{
                //    agents[i].navPoints.Add(out11);
                //}

                if (agents[i].navPoints[0].x_ == 0 && agents[i].navPoints[0].y_ == 0)
                {
                    Console.WriteLine("编号 " + i + " 路径计算出错");
                }
                if (agents[i].navPoints.Count > 2000)
                {
                    Console.WriteLine("编号 " + i + " 路径过多 " + agents[i].navPoints.Count);
                }
            }
        }

        private static void 偏移处理导航点(AgentClass agent)
        {
            ////if (agent.navPoints.Count < 2) return;
            //Random random = new Random();
            //var r = ((float)random.Next(3, 60)) / 10;
            //var fuhao = random.Next(0, 2);
            //for (int i = 1; i < agent.navPoints.Count; i++)
            //{
            //    var a = agent.navPoints[i - 1];
            //    var c = agent.navPoints[i - 1];
            //    var b = agent.navPoints[i];
            //    var k = -1 * (b.x_ - a.x_) / (b.y_ - a.y_);
            //    var deltx = r * (fuhao > 0 ? 1 : -1) / Math.Sqrt(k * k + 1);
            //    var delty = k * deltx;
            //    c.x_ += (float)deltx;
            //    c.y_ += (float)delty;

            //    //根据在左侧右侧对deltx的负号修正
            //    if (fuhao > 0)
            //    {
            //        if ((b.x_ - a.x_) * (c.y_ - a.y_) - (c.x_ - a.x_) * (b.y_ - a.y_) > 0)
            //        {
            //            a.x_ += (float)deltx * -1;
            //            a.y_ += (float)delty * -1;
            //            c = a;
            //        }
            //    }
            //    else
            //    {
            //        if ((b.x_ - a.x_) * (c.y_ - a.y_) - (c.x_ - a.x_) * (b.y_ - a.y_) < 0)
            //        {
            //            a.x_ += (float)deltx * -1;
            //            a.y_ += (float)delty * -1;
            //            c = a;
            //        }
            //    }
            //    agent.navPoints[i - 1] = c;
            //    agent.navPointstemp.Add(c);

            //    //agent.navPoints[i - 1] = a;

            //}
        }

        public static void test()
        {
            var r = 1.414f;
            var a = new Vector2(0, 0);
            var b = new Vector2(1, 1);
            var k = -1 * (b.x_ - a.x_) / (b.y_ - a.y_);
            var deltx = r / Math.Sqrt(k * k + 1);
            var delty = k * deltx;
            a.x_ += (float)deltx;
            a.y_ += (float)delty;
        }

        //重新规划
        public static void ReplanNav()
        {
            //方法1
            //long replaytimebefore = sw.ElapsedMilliseconds;
            //    for (int i = 0; i < _agents.Count; i++)
            //    {
            //        try
            //        {
            //            songnav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
            //            _agents[i].navPoints = songnav.SmothPathfinding_Points();
            //            //_agents[i].navPoints = nav.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
            //            _agents[i].navPoints.Add(_agents[i].positionTarget);
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e);
            //            Console.WriteLine("人数：" + _agents.Count + "  " + i);
            //            Console.WriteLine("多线程对_agents.count访问出错-replan");
            //            //throw e;
            //        }
            //    }
            //    //Console.WriteLine("新线程重新规划耗时： "+ (sw.ElapsedMilliseconds-replaytimebefore)/1000);


            //方法2
            //PathFindandCheck();

            //方法3
            //检查当前位置与第一个导航点，如果太大，就重新规划路径
            //for(int i = 0; i < _agents.Count; i++)
            //{
            //    if(MathHelper.abs(_agents[i].positionNow-_agents[i].navPoints[0])>8)//4米时去下一个导航点，8米时重新寻求导航点
            //    {
            //        try
            //        {
            //            songnav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
            //            _agents[i].navPoints = songnav.SmothPathfinding_Points();
            //            //_agents[i].navPoints = nav.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
            //            _agents[i].navPoints.Add(_agents[i].positionTarget);
            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e);
            //            Console.WriteLine("人数：" + _agents.Count + "  " + i);
            //            Console.WriteLine("多线程对_agents.count访问出错-replan");
            //            //throw e;
            //        }
            //    }

            //}


        }
        #endregion

























































        //判断当前是否有仿真文件
        public static bool ExitsOutput()
        {
            //这里只判断一个线程的输出文件
            if (File.Exists(mainDirectory + "output0"))
            {
                return true;
            }
            return false;
        }

       


        public class Cell
        {
            public List<int> idrow;//地图的第几块标志，地图一共4个区域
            public List<int> idcolumn;
            //public Color color;
            public bool infected = false;
            public bool newInfected = false;
            public float count = 0;
            public Cell()
            {
                idrow = new List<int>();
                idcolumn = new List<int>();
            }
        }
        public static void reCalculateResponseTime()
        {
            if (设置.infectionIntervel_time <= 0) return;
            Cell[][] mapData;
            float xMax = 200;
            float yMax = 200;
            float xMin = -250;
            float yMin = -200;
            //扩展取整边界
            xMax = xMax + HeatMap.cellWidth;
            yMax = yMax + HeatMap.cellWidth;
            xMin = xMin - HeatMap.cellWidth;
            yMin = yMin - HeatMap.cellWidth;

            //矩阵偏置
            HeatMap.xOffset = -xMin;
            HeatMap.yOffset = -yMin;

            //矩阵大小
            int xLength = (int)(xMax + HeatMap.xOffset);
            int yLength = (int)(yMax + HeatMap.yOffset);
            var xGirds = xLength * 10 / HeatMap.cellWidth;
            var yGirds = yLength * 10 / HeatMap.cellWidth;

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

            List<List<AgentClass>> _agents = new List<List<AgentClass>>();
           
            _agents.Add(agentsAll);
            

            //统计每个格子的人数
            try
            {
                for (int i = 0; i < mapData.Length; i++)
                {
                    for (int j = 0; j < mapData[i].Length; j++)
                    {
                        mapData[i][j].idrow.Clear();
                        mapData[i][j].idcolumn.Clear();
                    }
                }

                for (int i = 0; i < _agents.Count; i++)
                {
                    for (int j = 0; j < _agents[i].Count; j++)
                    {
                        int row = (int)((_agents[i][j].positionNow.x_ + HeatMap.xOffset) * 10 / HeatMap.cellWidth);
                        int column = (int)((_agents[i][j].positionNow.y_ + HeatMap.yOffset) * 10 / HeatMap.cellWidth);
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

            for(int i=0;i<250;i++)
            {
                //100步骤
                calculateColorThread(mapData, _agents,i * 设置.infectionIntervel_time);
            }
        }


        
        public static void calculateColorThread(Cell[][] mapData,List<List<AgentClass>> _agents,int stepcounts)
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
                                    //Debug.WriteLine((int)(Settings.deltaTDefault * stepcounts));
                                    }
                                }
                            }
                        }
                        

                    }
                }
            }
        }


        public static void StartSimulate()
        {
            Window.HeatMap.HeatMapInit();//先初始化后计算, 仿真的时候是否计算颜色
            if (设置.simulateMethod=="RVO")
            {
                //调用RVO开始仿真
                //foreach (var i in Sample._instance)
                //{
                //    if (设置.simulateMethod == "RVO") i.Intervals();//调用RVO中的upadatPosition，但是也要依靠下面的stepsControl来控制
                //}
                //开启对不同部分地图的控制
                stepscontrolThread = new Thread(stepsControl);
                stepscontrolThread.Start();
                UpdateSimulateStatus(SimulateStates.SimulatingLocalCPU);
            }
            else if(设置.simulateMethod == "LocalGPU")
            {
                UpdateUIpanelInfo("正在生成agents文件...");
                OutAgentNavPoints(Control.mainDirectory + "agents.txt");
                UpdateUIpanelInfo("正在调用本地程序，进行计算...");
                SimulateLocalGPU();
                UpdateUIpanelInfo("文件全部接收成功,可以点击播放进行回放。");
                UpdateSimulateStatus(SimulateStates.SimulateEnd);
            }
            else if(设置.simulateMethod == "RemoteGPU")
            {
                //Thread sr = new Thread(SimulateRemote);
                //sr.Start();
                SimulateRemote();//在里面设置状态转换
            }
        }

        public static Process process = new Process();
        //    UpdateUIpanelInfo(processStr);
        public static bool StartProcess(string runFilePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(runFilePath);//, s); // 括号里是(程序名,参数)
            
            process.StartInfo = startInfo;

            startInfo.WorkingDirectory = Control.mainDirectory;

            process.StartInfo.UseShellExecute = false;    //是否使用操作系统的shell启动
            //startInfo.RedirectStandardInput = true;      //接受来自调用程序的输入     
            //startInfo.RedirectStandardOutput = true;     //由调用程序获取输出信息
            //startInfo.CreateNoWindow = true;             //不显示调用程序的窗口 
            

            //process.StartInfo.RedirectStandardError = false;
            //process.StartInfo.CreateNoWindow = true;
            //process.StartInfo.Arguments = sb.ToString();  //参数 多个参数使用空格分开               
            process.Start();
            UpdateSimulateStatus(SimulateStates.SimulatingLocalGPU);
            //string processStr = "";
            //while (!process.StandardOutput.EndOfStream)
            //{
            //    string line= process.StandardOutput.ReadLine();
            //    Console.WriteLine(line);
            //    if (line == "end")
            //    {
            //        UpdateUIpanelInfo(processStr);
            //        processStr = "";
            //    }
            //    else processStr += line+"\n";
            //}
            process.WaitForExit();
            //process.Close();
           
            return true;
        }


        //private static void ExecuteTool(string toolFile, string args)
        //{
        //    Process p;
        //    ProcessStartInfo psi;
        //    psi = new ProcessStartInfo(toolFile);
        //    psi.Arguments += args;

        //    psi.UseShellExecute = false;
        //    psi.RedirectStandardOutput = true;  //允许重定向标准输出

        //    psi.RedirectStandardError = true;
        //    psi.CreateNoWindow = true;
        //    psi.RedirectStandardError = true;
        //    psi.WindowStyle = ProcessWindowStyle.Hidden;

        //    p = Process.Start(psi);

        //    p.OutputDataReceived += new DataReceivedEventHandler(OnDataReceived);
        //    p.BeginOutputReadLine();

        //    p.WaitForExit();

        //    if (p.ExitCode != 0)
        //    {
        //        result.Append(p.StandardError.ReadToEnd());
        //    }
        //    p.Close();
        //}

        private static void OnDataReceived(object Sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                UpdateUIpanelInfo(e.Data);
            }
        }

        public static void SimulateLocalGPU()
        {
            StartProcess(Control.mainDirectory + "powerlaw.exe");
        }

        public static List<string> fileNames = new List<string>();
        /// <summary>
        /// 远程仿真
        /// 用委托形式对UI更新
        /// </summary>
        private static void SimulateRemote()
        {
            char[] lineSplitChars = { '_' };
            char[] lineSplitChars2 = { '.' };

            //生成文件, 然后压缩, 然后连接远程, 然后发送
            UpdateUIpanelInfo("正在输出配置文件,请稍等……");
            string fullpath = Control.mainDirectory + "agents.txt";
            OutAgentNavPoints(fullpath);
            UpdateUIpanelInfo("正在压缩配置文件,请稍等……");
            //对agent文件压缩
            CreateZipFile("agents.zip", "agents.txt", Control.mainDirectory);

            UpdateUIpanelInfo("连接远程服务器……");
            if (SocketClient.connect(设置.remoteIP, int.Parse(设置.remotePort)))
            {
                try
                {
                    UpdateUIpanelInfo("连接远程服务器成功, 正在传送配置文件,请稍等……");
                    UpdateSimulateStatus(SimulateStates.SendingAgentsToRemote);
                    
                    SocketClient.SendLong(设置.记录文件时间间隔);
                    SocketClient.SendFile分包发送(Control.mainDirectory+"agents.zip");
                    var str = SocketClient.ReceiveString();//接收成功
                    if(str=="error")
                    {
                        UpdateUIpanelInfo("文件发送有误, 请重新开始……");
                        UpdateSimulateStatus(SimulateStates.SimulatInited);
                        return;
                    }
                    UpdateSimulateStatus(SimulateStates.SimulatingRemote);
                    UpdateUIpanelInfo("传送成功，准备接受仿真结果文件……");

                    fileNames.Clear();
                    int fileCount = 1;
                    string directoryPath = Control.mainDirectory + "tempFile\\";
                    if (Directory.Exists(directoryPath)) Directory.GetFiles(directoryPath).ToList().ForEach(File.Delete);
                    while (true)
                    {
                        string fileName = SocketClient.ReceiveFile(directoryPath);
                        if (fileName == "end") break;

                        //解压文件
                        UnZipFile(directoryPath+fileName,directoryPath);
                        Console.WriteLine("文件解压结束");

                        fileCount++;
                        string[] line = fileName.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
                        string[] line2= line[1].Split(lineSplitChars2, StringSplitOptions.RemoveEmptyEntries);
                        UpdateUIpanelInfo("准备接收第 " + fileCount + " 个文件\r\n剩余 " +line2[0]+"人");
                        fileNames.Add(directoryPath+ System.IO.Path.GetFileNameWithoutExtension(fileName));//去掉".zip"的文件合并
                    }
                    //合并文件：
                    CombineFiles(fileNames, Control.mainDirectory + "output0");

                    //已经结束初始化

                    UpdateUIpanelInfo("文件全部接收成功,可以点击播放进行回放。");
                    UpdateSimulateStatus(SimulateStates.SimulateEnd);
                }
                catch
                {
                    SocketClient.Close();
                    //Console.WriteLine("出错");
                    //UpdateUIpanelInfo("传输中断，请重新开始");
                    //UpdateSimulateStatus(SimulateStates.SimulatInited);
                    if (Control.CombineFiles())
                    {
                        UpdateUIpanelInfo("中断，可以点击播放观看结果");
                        UpdateSimulateStatus(SimulateStates.SimulateEnd);
                    }
                    else
                    {
                        UpdateUIpanelInfo("连接中断，可以重新仿真");
                        UpdateSimulateStatus(SimulateStates.SimulatInited);
                    }
                }
            }
            else
            {
                Console.WriteLine("连接失败");
                UpdateUIpanelInfo("连接失败,请检查网络或者远程服务器的运行状态");
                UpdateSimulateStatus(SimulateStates.SimulatInited);
            }
        }

        /// <summary>
        /// 压缩文件成zip
        /// </summary>
        /// <param name="fileZip">压缩成zip文件的绝对路径</param>
        /// <param name="fileName">被压缩指定文件的名字</param>
        /// <param name="zipFilePath"></param>
        /// <returns></returns>
        public static bool CreateZipFile(string fileZip, string fileName, string zipFilePath)
        {
            fileZip = zipFilePath + fileZip;
            bool isZip = false;
            if (!Directory.Exists(zipFilePath))
            {
                return isZip;
            }
            try
            {
                string[] filenames = Directory.GetFiles(zipFilePath);
                using (ZipOutputStream s = new ZipOutputStream(File.Create(fileZip)))
                {
                    s.SetLevel(9); // 压缩级别 0-9
                                   //s.Password = "123"; //Zip压缩文件密码
                    byte[] buffer = new byte[4096]; //缓冲区大小
                    foreach (string file in filenames.ToList())
                    {
                        if (file == zipFilePath + fileName)//指定被压缩文件的绝对路径
                        {
                            ZipEntry entry = new ZipEntry(System.IO.Path.GetFileName(file));
                            entry.DateTime = DateTime.Now;
                            s.PutNextEntry(entry);
                            using (FileStream fs = File.OpenRead(file))
                            {
                                int sourceBytes;
                                do
                                {
                                    sourceBytes = fs.Read(buffer, 0, buffer.Length);
                                    s.Write(buffer, 0, sourceBytes);
                                } while (sourceBytes > 0);
                                fs.Close();
                                fs.Dispose();
                            }
                            break;
                        }
                    }
                    s.Finish();
                    s.Close();
                    isZip = true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return isZip;
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipFilePath">压缩文件的绝对路径</param>
        public static void UnZipFile(string zipFilePath,string directoryName)
        {
            if (!File.Exists(zipFilePath))
            {
                return;
            }
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string fileName = System.IO.Path.GetFileName(theEntry.Name);
                    // create directory
                    //if (directoryName?.Length > 0)
                    //{
                    //    Directory.CreateDirectory(directoryName);
                    //}
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        using (FileStream streamWriter = File.Create(directoryName+theEntry.Name))
                        {
                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool CombineFiles()
        {
            if (fileNames.Count > 0)
            {
                CombineFiles(fileNames, Control.mainDirectory + "output0");
                return true;
            }
            else
                return false;
        }


        static void CombineFiles(List<string> filePaths, string combineFinalFile)
        {
            if (File.Exists(combineFinalFile))
            {

                File.Delete(combineFinalFile);
            }
            using (FileStream CombineStream = new FileStream(combineFinalFile, FileMode.OpenOrCreate))
            {
                using (BinaryWriter CombineWriter = new BinaryWriter(CombineStream))
                {
                    foreach (string file in filePaths)
                    {
                        if (File.Exists(file))
                        {
                            using (FileStream fileStream = new FileStream(file, FileMode.Open))
                            {
                                using (BinaryReader fileReader = new BinaryReader(fileStream))
                                {
                                    byte[] TempBytes = fileReader.ReadBytes((int)fileStream.Length);
                                    CombineWriter.Write(TempBytes);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("文件又没有了");
                        }

                    }
                }
            }
        }


        /// <summary>
        ///新建工程，当前仿真没有结束时；
        ///点击停止按钮时；
        ///仿真自动停止时
        /// </summary>
        public static void StopSimulate()
        {
            //if(state==SimulateStates.SimulateEnd || state==SimulateStates.Reading || state == SimulateStates.ReadingEnd)
            //{
            //    return;
            //}
            //int agentsCount = 0;
            //for (int i = 0; i < _instance.Count; i++)
            //{
            //    try
            //    {
            //        _instance[i].controlSteps = true;
            //        _instance[i].sw.Stop();
            //        if (_instance[i].stepsThread != null && _instance[i].stepsThread.IsAlive) _instance[i].stepsThread.Abort();
            //        _instance[i].OutpositionFileEnd();
            //        _instance[i]._agents.Clear();

            //        agentsCount += _instance[i].agentsPositionsforExodus.Count;
            //    }
            //    catch
            //    {
            //        Console.WriteLine("stop 程序出错");
            //    }
            //}

            ////输出vrs
            //if (checkBox_outputVRS.Checked)
            //{
            //    FileHelper FHforExodus;
            //    FHforExodus = new FileHelper(Sample.mainDirectory + "_Exodus" + ".vrs");
            //    FHforExodus.Write("3 0");//先输出疏散人数
            //    FHforExodus.NewLine();//换行
            //    FHforExodus.Write(agentsCount.ToString() + " 0 " + getMaxStep());//先输出疏散人数
            //    for (int i = 0; i < Sample._instance.Count; i++)
            //    {
            //        Sample._instance[i].outForExodus(FHforExodus);
            //        Sample._instance[i].agentsPositionsforExodus.Clear();
            //    }
            //    FHforExodus.EndOut();
            //}


            UpdateSimulateStatus(SimulateStates.SimulateEnd);
        }


        /// <summary>
        /// 用来控制每个Instance的仿真
        /// </summary>
        public static void stepsControl()
        {
            ////控制线程依次执行
            ////思路是每当所有instance都为false，就都令其变为true
            //while (true)
            //{
            //    //如果所有controlSteps都是false, 就控制都为true
            //    bool newStep = true;
            //    for(int i=0;i<_instance.Count;i++)
            //    {
            //        if (_instance[i].controlSteps == true)
            //        {
            //            newStep = false;//如果有任何Instance没有仿真完，就不进行下一步
            //            break;
            //        }
            //    }

            //    //////王吉-输出每个agent导航点位置
            //    //if (_instance[2].stepCounts == 10) //当区域0的仿真次数大于30的时候输出agent的导航点信息
            //    //{
            //    //    OutAgentNavPoints();
            //    //}

            //    //判断是否要停止,控制让其所有人数都少于百分之1时才停止
            //    bool stop = true;
            //    //原来的停止方式
            //    //for (int i = 0; i < _instance.Count; i++)//如果有任何人数>百分之2，就令stop为false
            //    //{
            //    //    if (_instance[i]._agents.Count > (_instance[i].agentsOrigine / 100) || _instance[i]._agents.Count > 50)//剩余百分之XX的时候
            //    //    {
            //    //        stop = false;
            //    //        break;
            //    //    }
            //    //}
            //    float 剩余agent = 0;
            //    float 总共agent = 0;
            //    for(int i = 0; i < _instance.Count; i++)
            //    {
            //        剩余agent += _instance[i]._agents.Count;
            //        总共agent += _instance[i].agentsOrigine;
            //    }
            //    if (剩余agent / 总共agent < 0.02f && 剩余agent < 300)
            //    {
            //        stop = true;
            //    }
            //    else stop = false;


            //    if (stop == true)
            //    {
            //        StopSimulate();
            //        break;
            //    }
            //    if (newStep && !stop)
            //    {
            //        foreach (var ins in _instance)
            //        {
            //            ins.controlSteps = true;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 输出每个agent导航点位置-给王吉
        /// </summary>
        public static void OutAgentNavPoints(string fullpath)
        {
            var FHagents = new FileHelper(fullpath);
            FHagents.Write(设置.总人数+" "+agentsAll.Count.ToString(),true);
            foreach(var agent in agentsAll)// (int i = 0; i < agentsAll.Count; i++)
            {
                for (int k = 0; k < agent.navPoints.Count; k++)
                {
                    if (agent.navPoints[k].x_ == 0 && agent.navPoints[k].y_ == 0)
                    {
                        agent.navPoints.RemoveAt(k--);
                    }
                }

                if (agent.navPoints.Count<200)
                {
                    FHagents.Write(agent.outIndex + " " + agent.positionNow.x_ + " " + agent.positionNow.y_ + " " + agent.MaxSpeed + " " + agent.timeResponse + " " + agent.navPoints.Count + " ");
                    for (int k = 0; k < agent.navPoints.Count; k++) FHagents.Write(agent.navPoints[k].x_.ToString("0.00") + " " + agent.navPoints[k].y_.ToString("0.00") + " ");
                    FHagents.NewLine();
                }
                else
                {
                    FHagents.Write(agent.outIndex + " " + agent.positionNow.x_ + " " + agent.positionNow.y_ + " " + agent.MaxSpeed + " " + agent.timeResponse + " " + 200 + " ");
                    for (int k = 0; k < 200; k++) FHagents.Write(agent.navPoints[k].x_.ToString("0.00") + " " + agent.navPoints[k].y_.ToString("0.00") + " ");
                    FHagents.NewLine();
                }
            }
            FHagents.EndOut();
        }

        /// <summary>
        /// 目前来看主要是得到有几个Instance
        /// </summary>
        /// <returns></returns>
        

        /// <summary>
        /// 得到不同区域的数据，并设定不同区域人数
        /// </summary>
        /// <param name="_areas"></param>
        /// <returns></returns>
        private static bool 从实时数据得到区域数据(out List<Area> _areas)
        {
            _areas = new List<Area>();
            foreach (var area in 设置.areas)
            {
                int aeraAgentNums = area.peoplenums;
                int receiveTime = area.receiveTime;
                if (设置.疏散时间接收方式=="按区域设置") receiveTime = 0;//如果当前选择的是从中心网格疏散，就让各个区域接收时间都变成0
                _areas.Add(new Area(byte.Parse(area.id.ToString()), area.direct==0? false:true, area.name, area.pos1, area.pos2, aeraAgentNums, receiveTime));
            }
            return true;
        }

        private static bool 从手动设置得到区域数据(out List<Area> _areas)
        {
            tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);//得到网格

            float acreage_density = 0;//面积和密度想乘
            foreach (var area in 设置.areas)
            {
                float aceage = getAreaAcreage(tile, area.id);
                acreage_density += aceage * (area.density);
            }

            float countfactor = 设置.总人数 / acreage_density;//系数=总人数/总(面积*密度)

            _areas = new List<Area>();
            foreach (var area in 设置.areas)
            {
                float aceage = getAreaAcreage(tile, area.id);
                acreage_density += aceage * (area.density);
                int aeraAgentNums = (int)(area.density * getAreaAcreage(tile, area.id) * countfactor);
                int receiveTime = area.receiveTime;
                if (设置.疏散时间接收方式 == "按区域设置") receiveTime = 0;//如果当前选择的是从中心网格疏散，就让各个区域接收时间都变成0
                _areas.Add(new Area(byte.Parse(area.id.ToString()), area.direct == 0 ? false : true, area.name, area.pos1, area.pos2, aeraAgentNums, receiveTime));
            }
            return true;
        }

        public static  void InitMap()
        {
            songnav = new Simulate.Navigation();
            //songnav.GenerateMeshFromFile("../../Meshes/mesh.obj");
            //songnav.LoadNavMeshFromFile("../../Meshes/f42.snb", "../../Meshes/mesh.obj");
            songnav.LoadNavMeshFromFile("Meshes/"+snbName, "Meshes/outDoors");

        }

        private static Navigation getMap(string name)
        {
            var sn = new Simulate.Navigation();
            //songnav.GenerateMeshFromFile("../../Meshes/mesh.obj");
            //sn.LoadNavMeshFromFile("../../Meshes/f42.snb", "../../Meshes/mesh.obj");
            sn.LoadNavMeshFromFile("Meshes/" + name, "Meshes/outDoors");
            //sn = songnav;
            
            return sn;
        }

        private static Navigation getMap()
        {
            var sn = new Simulate.Navigation();
            //songnav.GenerateMeshFromFile("../../Meshes/mesh.obj");
            //sn.LoadNavMeshFromFile("../../Meshes/f42.snb", "../../Meshes/mesh.obj");
            sn.LoadNavMeshFromFile("Meshes/" + snbName, "Meshes/outDoors");
            //sn = songnav;
          
            return sn;
        }

        /** 把density当成密度比例，而不是真正密度也不是人数
         *  统计面积，
         *  系数=总人数/总(面积*密度)
         *  一个区域人数=面积*密度*系数
         */
        private static void InitAgents(List<List<Area>> _areas)
        {
  
            
            

        }


        ///// <summary>
        ///// 得到当前计算的疏散时间 单位是秒
        ///// 用的int 因此最大疏散时间为1092分钟
        ///// </summary>
        ///// <returns></returns>
        //public static int getEvacuateSeconds()
        //{
        //    int time = 0;
        //    //为了方便,暂时这里控制最快读取速率
        //    if (readMode)//如果是读取模式
        //    {
        //        //int chushu = Sample.numsPeople >= 50000 ? 1 : 5;//一秒一帧和0.2秒一帧
        //        //time = (int)(GetMaxReadLine() * Settings.deltaTDefault * 10 / chushu);

        //        float maxTime = 0;
        //        for (int i = 0; i < Control._instance.Count; i++)
        //        {
        //            if (maxTime < Sample._instance[i].timeNow) maxTime = Sample._instance[i].timeNow;
        //        }
        //        time = (int)maxTime;


        //    }
        //    else
        //    {
        //        //现在线程同步,所以用最大的,以前线程不同步,用最小的
        //        int maxStepCount = 0;
        //        for (int i = 0; i < Sample._instance.Count; i++)
        //        {
        //            if (maxStepCount < Sample._instance[i].仿真步数) maxStepCount = Sample._instance[i].仿真步数;
        //        }
        //        time = (int)(maxStepCount * Settings.deltaTDefault);
        //    }
        //    return time;
        //}

        /// <summary>
        /// 从多个Instance中找最多的那个,当前读取的帧数
        /// </summary>
        /// <returns></returns>
        
    }
}
