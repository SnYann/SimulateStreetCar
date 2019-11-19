using System;
using System.Collections.Generic;
using System.Threading;


using Simulate;
using RVO;
using SharpNav;
using SharpNav.Geometry;
using SharpNav.Pathfinding;
using System.Diagnostics;
using System.IO;
using System.Xml;
using test.Scripts;
using System.Drawing;
using System.Text;
using System.Drawing.Imaging;

namespace Simulate
{

    public class InstanceRVO
    {
        public List<AgentClass> _agents = new List<AgentClass>();
        public Navigation songnav;
        public string instanceID;
        //public List<ControlGate> controlGates;//用来控制道路的单向双向
        public Stopwatch sw = new Stopwatch();//用来帮助计算程序耗时
        public int 仿真步数 = 0;//统计步数
        
        //输出位置文件相关
        public FileHelper FH;
        public List<Area> areas;
        public Simulator RVOInstance;
        public int agentsOrigine;
        public static OutDoors[] _out;
        //线程
        public Thread stepsThread;//用于控制每步仿真
        //限制的出口
        public List<int> OutIDs = new List<int>();
        public InstanceRVO(string id, List<Area> a, List<Simulate.Line> obs, Navigation sn, List<int> outs)
        {
            instanceID = id;
            //_agents = _a;//初始化人
            songnav = sn;//总地图

            areas = a;
            重置网格权重();

            OutIDs = outs;

            //RVO的设置
            RVOInstance = new Simulator();
            InitRVO(obs);//初始化RVO

            //for (int i = 0; i < areas.Count; i++)
            //{
            //    AddAreaAgents(areas[i],_out);
            //}


            agentsOrigine = _agents.Count;//方便界面调用统计人数

            //_agents[10].state = AgentStates.Evacuating;//感染新agent方法初始化时


            //感觉位置不对,暂时这样 
            if (Control.是否输出位置文件 && Control.设置.simulateMethod=="RVO")
            {
                //FH = new FileHelper(Sample.mainDirectory + instanceID);//王吉 没有RVO所以隐藏这个

                //int chushu = Sample.numsPeople >= 50000 ? 10 : 2;//一秒一帧和0.2秒一帧
                //FH.Write((Settings.RVODefault.deltaT * chushu).ToString(), true);
            }
        }

        //重新设置area
        private void 重置网格权重()
        {
            for (int i = 0; i < areas.Count; i++)
            {
                songnav.roadfilter.SetAreaCost(areas[i], 2);
            }
            var a=GetAreabyName("35_宝华楼小路");
            songnav.roadfilter.SetAreaCost(a, 3);
            a = GetAreabyName("27_一横街");
            songnav.roadfilter.SetAreaCost(a, 4);
           
            a = GetAreabyName("30_中威巷子");
            songnav.roadfilter.SetAreaCost(a, 3);

            a = GetAreabyName("28_陶兴巷");
            songnav.roadfilter.SetAreaCost(a, 3);

        }
       
        private Area GetAreabyName(string text)
        {
            for (int j = 0; j < Control._areas.Count; j++)
            {
                if (Control._areas[j].AreaName == text)
                {
                    return Control._areas[j];
                }
            }
            return Area.None;
        }

        

        #region 初始化RVO相关
        /// <summary>
        /// RVO初始化函数
        /// 设置步长等等参数，设置障碍物
        /// </summary>
        
        public List<Vector3> obsTest;
        private void InitRVO(List<Simulate.Line> obs)
        {
            ///* Specify the global time step of the simulation. */
            RVOInstance.setTimeStep(Settings.RVODefault.deltaT);
            RVOInstance.setAgentDefaults(Settings.RVODefault.neighborDist, Settings.RVODefault.maxNeighbors, Settings.RVODefault.timeHorizon, Settings.RVODefault.timeHorizonObst, Settings.RVODefault.radius, Settings.RVODefault.maxSpeed, new RVO.Vector2(1f, 1f));
            obsTest = new List<Vector3>();
#if outObsfile //王吉-输出障碍物边界，4个
            var FHObs=new FileHelper(Sample.mainDirectory + " " + instanceID +"obs.txt");
#endif
            //obs.Clear();
            //obs = songnav.GetObstacle();

            for (int i = 0; i < obs.Count; i++)
            {
                if (obs[i].valid == true)
                {
                    IList<RVO.Vector2> ObsVector = new List<RVO.Vector2>();
                    ObsVector.Add(new RVO.Vector2(obs[i].point1.X, obs[i].point1.Z));
                    ObsVector.Add(new RVO.Vector2(obs[i].point2.X, obs[i].point2.Z));
                    RVOInstance.addObstacle(ObsVector);

                    obsTest.Add(obs[i].point1);
                    obsTest.Add(obs[i].point2);

#if outObsfile
                    FHObs.Write(obs[i].point1.X + " " + obs[i].point1.Z + " " + obs[i].point2.X + " " + obs[i].point2.Z, true);
#endif
                }
            }
#if outObsfile
            //在out时可以先在底下设置断点
            FHObs.EndOut();
#endif
            RVOInstance.processObstacles();
        }

        /// <summary>
        /// 初始化障碍物
        /// </summary>
        private void InitObstacle()
        {
            IList<RVO.Vector2> ObsVector = new List<RVO.Vector2>();
            float x = 10;
            float z = 0;
            float sx = 10;
            float sz = 10;

            //Debug.Log(Obs.name);
            //Debug.Log((x + (sx * 0.5f))+ "  " +  (z + (sz * 0.5f)));
            //Debug.Log((x - (sx * 0.5f)) + "  " + (z + (sz * 0.5f)));
            //Debug.Log((x + (sx * 0.5f)) + "  " + (z - (sz * 0.5f)));
            //Debug.Log((x - (sx * 0.5f)) + "  " + (z - (sz * 0.5f)));

            ObsVector.Add(new RVO.Vector2(x + (sx * 0.5f), z + (sz * 0.5f)));
            ObsVector.Add(new RVO.Vector2(x - (sx * 0.5f), z + (sz * 0.5f)));
            ObsVector.Add(new RVO.Vector2(x - (sx * 0.5f), z - (sz * 0.5f)));
            ObsVector.Add(new RVO.Vector2(x + (sx * 0.5f), z - (sz * 0.5f)));

            RVOInstance.addObstacle(ObsVector);

            RVOInstance.processObstacles();
        }

        #endregion

        //方向控制
        //public class ControlGate
        //{
        //    public int poly1;
        //    public int poly2;
        //    public NavPolyId npi1;
        //    public NavPolyId npi2;
        //    public Vector3 pos1;
        //    public Vector3 pos2;
        //    public ControlGate(int p1, int p2)
        //    {
        //        poly1 = p1;
        //        poly2 = p2;
        //        npi1 = songnav.navMeshQuery.nav.IdManager.Encode(1, 0, poly1);
        //        npi2 = songnav.navMeshQuery.nav.IdManager.Encode(1, 0, poly2);
        //        pos1 = new Vector3();
        //        pos2 = new Vector3();
        //        songnav.navMeshQuery.CenterPointOnPoly(npi1, ref pos1);
        //        songnav.navMeshQuery.CenterPointOnPoly(npi2, ref pos2);
        //    }
        //    public void SwitchGate()
        //    {
        //        int poly = poly1;
        //        NavPolyId npi = npi1;
        //        Vector3 pos = pos1;

        //        poly1 = poly2;
        //        npi1 = npi2;
        //        pos1 = pos2;

        //        poly2 = poly;
        //        npi2 = npi;
        //        pos2 = pos;
        //    }
        //}



        //public void PathFindandCheck()
        //{
        //    //别忘重新安置这段代码
        //    //nav.roadfilter.SetAreaCost(2, 1000);
        //    //var tile = songnav.tiledNavMesh.GetTileAt(0, 0, 0);

        //    for (int i = 0; i < _agents.Count; i++)
        //    {
        //        SharpNav.Pathfinding.Path path = songnav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
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
        //        for (int pi = 0; pi < path.Count; pi++) _agents[i].polyIds.Add(path[pi].Id);
        //        //Console.WriteLine(instanceID+" poly点 "+i);
        //        _agents[i].navPoints = songnav.SmothPathfinding_Points();
        //        _agents[i].navPoints.Add(_agents[i].positionTarget);

        //        if (_agents[i].navPoints[0].x_ == 0 && _agents[i].navPoints[0].y_ == 0)
        //        {
        //            Console.WriteLine(instanceID + "编号 " + i + " 路径计算出错");
        //        }
        //        if (_agents[i].navPoints.Count > 2000)
        //        {
        //            Console.WriteLine(instanceID + "编号 " + i + " 路径过多 " + _agents[i].navPoints.Count);
        //        }
        //    }

        //}

        //public void RePathFindandCheck()
        //{
        //    //别忘重新安置这段代码
        //    //nav.roadfilter.SetAreaCost(2, 1000);
        //    //var tile = nav.tiledNavMesh.GetTileAt(0, 0, 0);
        //    //SharpNav.Pathfinding.Path path;
        //    try
        //    {
        //        for (int i = 0; i < _agents.Count; i++)
        //        {
        //            for (int p = 0; p < _agents[i].polyIds.Count - 1; p++)
        //            {
        //                for (int cg = 0; cg < controlGates.Count; cg++)
        //                {
        //                    if (_agents[i].polyIds[p] == controlGates[cg].npi1.Id)
        //                    {
        //                        if (_agents[i].polyIds[p + 1] == controlGates[cg].npi2.Id)
        //                        {
        //                            //tile.Polys[controlGates[cg].poly1].Area = new Area(2);
        //                            //path.Clear();
        //                            songnav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
        //                            _agents[i].navPoints = songnav.SmothPathfinding_Points();
        //                            _agents[i].navPoints.Add(_agents[i].positionTarget);
        //                        }
        //                    }
        //                }
        //                //for (int cg = 0; cg < controlGates.Count; cg++)
        //                //{
        //                //    tile.Polys[controlGates[cg].poly1].Area = new Area(1);
        //                //}
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}

        #region 速度计算与位置更新相关
        /// <summary>
        /// 用来计算agent下一步位置并在场景中更新显示
        /// </summary>
        static int maxNavSteps = 0;//仿真多少步后重新计算导航点
        long steptimebefore = 0;
        public float speedMaxTest = 0f;//速度
        //public float agentRoadLength = 0;
        public bool controlSteps = true;
        public bool continueSteps = true;

        public void Intervals()
        {
            sw.Restart();
            stepsThread = new Thread(UpdatePosition);
            stepsThread.Start();
        }

        public void outputResult()
        {
            
            if (Control.是否输出位置文件)
            {
                int chushu = Control.设置.总人数 >= 10000 ? 10 : 5;//一秒一帧和0.5秒一帧
                if (仿真步数 % chushu == 0 && _agents.Count > 0)
                {
                    //写入一帧帧头  song
                    FH.NewFrame();//现在只是用来记录帧数,写入帧头
                    long timebefore = sw.ElapsedMilliseconds;
                    float timeNow = 仿真步数 * Settings.deltaTDefault;
                    FH.Write(timeNow.ToString("f2")+" ");//写当前时间

                    //写入每个口的当前疏散个数
                    for (int i = 0; i < _out.Length; i++)
                    {
                        FH.Write(Control.出口人数统计[i] + " ");
                    }
                    for (int i = 0; i < _out.Length; i++)
                    {
                        FH.Write(_out[i].outAgentCount + " ");
                    }
                    
                    //写入一帧所有数据
                    for (int i = 0; i < _agents.Count; i++)
                    {
                        if (_agents[i].state != AgentStates.Hide)
                        {
                            RVO.Vector2 pos = RVOInstance.getAgentPosition(i);//
                            _agents[i].positionNow = new Simulate.Vector2(pos.x(), pos.y());
                            if (Double.IsNaN(pos.x())) Console.WriteLine("不是数！" + "nov:" + _agents[i].nov + "; agent[i]:" + i);
                            else
                            {
                                //位置输出到文件
                                //FileHelper.Write("/" + _agents[i].nov + " " + _agents[i].positionNow.x_ + " " + _agents[i].positionNow.y_ + " 0");//最后的0是预留的
                                //FileHelper.Write("/" + i + " " + (RVOInstance.getAgentPosition(i).x_ * 100).ToString("f1") + " " + (RVOInstance.getAgentPosition(i).y_ * 100).ToString("f1") + " 0");//最后的0是预留的

                                //FileHelper.Write("/" + i + " " + (RVOInstance.getAgentPosition(i).x_).ToString("f1") + "," + (RVOInstance.getAgentPosition(i).y_).ToString("f1") + " 0");//最后的0是预留的
                                FH.Write(_agents[i].colorIndex + " " + (RVOInstance.getAgentPosition(i).x() * 100).ToString("f0") + " " + (RVOInstance.getAgentPosition(i).y() * 100).ToString("f0") + " ");//最后的0是预留的
                                _agents[i].positions.Add(new Simulate.Vector2((int)(pos.x()*100), (int)(pos.y()*100)));                                                                                                                                                                            //Console.WriteLine("/" + _agents[i].nov + " " + RVOInstance.getAgentPosition(i).x_.ToString("f1") + " " + RVOInstance.getAgentPosition(i).y_.ToString("f1") + " 0");
                            }
                        }
                    }
                    //Console.WriteLine("储存一帧总时间： " + (sw.ElapsedMilliseconds - timebefore).ToString());
                    FH.NewLine();//换行
                }
                if (FH.writeflame > 50000) OutpositionFileEnd();//3万步，就是30000/10/60=50分钟
            }

        }

        public void nextTarget()
        {
            //如果到达导航点，就到下一个目标
            for (int i = 0; i < _agents.Count; i++)
            {

                //if (_agents[i].navPoints.Count > 0 && _agents[i].navPoints[0].x_ == 0 && _agents[i].navPoints[0].y_ == 0)
                //{
                //    ////方法1, 删除
                //    ////删掉目标为0,0的agent
                //    ////for (int n = 0; n < songnav1.level._out.Count; n++)
                //    //{
                //    //    //if (i < _agents.Count && MathHelper.abs(songnav1.level._out[n] - _agents[i].positionNow) < 15)
                //    //    {
                //    //        RVOInstance.delAgentAt(i);
                //    //        _agents.RemoveAt(i);
                //    //        //_outAgentCount[n]++;
                //    //        Console.WriteLine(i+"目标为0, 移除");
                //    //    }
                //    //}
                //    //continue;

                //    //方法2, 重新规划
                //    //_agents[i].navPoints.RemoveAt(0);
                //    ////判断是否后面都为0 如果是就都移除
                //    //for (int iii = 0; iii < _agents[i].navPoints.Count; iii++)
                //    //{
                //    //    if (_agents[i].navPoints[0].x_ == 0 && _agents[i].navPoints[0].y_ == 0)
                //    //    {
                //    //        _agents[i].navPoints.RemoveAt(0);
                //    //    }
                //    //}

                //    Console.WriteLine(instanceID + "编号 " + i + " 出错，重新规划1");
                //    ChangeTarget(_agents[i]);//根据当前在的poly位置重新寻径
                //    var path = songnav1.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
                //    _agents[i].navPoints.Clear();
                //    _agents[i].navPoints = songnav1.SmothPathfinding_Points(path);

                //    //_agents[i].navPoints = nav.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
                //    //把最后位置的目标也加进去
                //    var outPoint = _out[_agents[i].outIndex].position;
                //    _agents[i].navPoints.Add(new Vector2(outPoint.X, outPoint.Z));

                //}

                if (_agents[i].navPoints.Count > 1)
                {
                    #region 原来那种判断距离移除目标点的方式
                    if (Simulate.MathHelper.abs(_agents[i].positionNow - _agents[i].navPoints[0]) < Settings.removeDistance)//不知道为什么会有很多点导向原点，因此添加这个删除他
                    {

                        if (_agents[i].navPoints.Count > 1) _agents[i].navPoints.RemoveAt(0);
                        //for (int iii = 0; iii < _agents[i].navPoints.Count - 1; iii++)
                        //{
                        //    if (_agents[i].navPoints[iii].x_ == 0 && _agents[i].navPoints[iii].y_ == 0)
                        //    {
                        //        _agents[i].navPoints.RemoveAt(iii);
                        //    }
                        //    if (iii > 2 && iii < 20 && MathHelper.abs(_agents[i].navPoints[iii] - _agents[i].navPoints[iii - 1]) < 5)
                        //    {
                        //        _agents[i].navPoints.RemoveAt(iii);
                        //        iii--;
                        //    }
                        //}

                    }
                    else if (Simulate.MathHelper.abs(_agents[i].positionNow - _agents[i].navPoints[0]) > Settings.removeDistance+ Settings.removeDistance)//超过10+5=15米时重新规划
                    {

                        //多个线程时重新寻径会出错，不知道为什么！！
                        ChangeTarget(_agents[i]);//根据当前在的poly位置重新寻径
                                                 //ChangeTarget();
                        var path = songnav.SmothPathfinding_paths(_agents[i].positionNow, _agents[i].positionTarget);
                        _agents[i].navPoints.Clear();
                        _agents[i].navPoints = songnav.SmothPathfinding_Points(path);

                        //for (int iii = 0; iii < _agents[i].navPoints.Count - 1; iii++)
                        //{
                        //    if (_agents[i].navPoints[iii].x_ == 0 && _agents[i].navPoints[iii].y_ == 0)
                        //    {
                        //        _agents[i].navPoints.RemoveAt(iii);
                        //    }
                        //    if (iii > 2 && MathHelper.abs(_agents[i].navPoints[iii] - _agents[i].navPoints[iii - 1]) < 2)
                        //    {
                        //        _agents[i].navPoints.RemoveAt(iii);
                        //        iii--;
                        //    }
                        //}

                        //_agents[i].navPoints = nav1.SmothPathfinding(_agents[i].positionNow, _agents[i].positionTarget);
                        //_agents[i].navPoints.Add(_agents[i].positionTarget);

                        ////把最后位置的目标也加进去
                        var outPoint = _out[_agents[i].outIndex].position;
                        _agents[i].navPoints.Add(new Vector2(outPoint.X, outPoint.Z));



                        Console.WriteLine(i + " 重新规划2 " + _agents[i].haveReplaned);
                        _agents[i].color = Color.Black;

                        _agents[i].haveReplaned = true;

                    }
                    #endregion

                    ////新的方法移除当前目标点
                    //if(MathHelper.absSq(_agents[i].positionNow - _agents[i].navPoints[0])+ MathHelper.absSq(_agents[i].navPoints[0] - _agents[i].navPoints[1])> MathHelper.absSq(_agents[i].positionNow - _agents[i].navPoints[1]))
                    //{
                    //    _agents[i].navPoints.RemoveAt(0);
                    //}
                }

            }
        }

        public void UpdatePosition()
        {
            while (continueSteps)
            {
                //Thread.Sleep(20);//防止过快,价格延时
                while (controlSteps)
                {

                    Thread.Sleep(30);
                    //Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXinstanceID: " + this.instanceID);
                    SetPreferredVelocities();
                    RVOInstance.doStep();
                    //Thread.Sleep(10);

                    outputResult();
                    //Console.WriteLine("stepCounts " + stepCounts);

                    nextTarget();

                    //////暂时不进行重新规划
                    ////if (maxNavSteps++ > 30)
                    ////{
                    ////    maxNavSteps = 0;
                    ////    if (_agents.Count > 0)
                    ////    {
                    ////        Thread thread = new Thread(ReplanNav);
                    ////        thread.Start();
                    ////    }
                    ////}
                    
                    //统计
                    if (_agents.Count > 0) 仿真步数++;

                    
                    bool 用原来的方法 = true;
                    if(用原来的方法)
                    {
                        CheckDelete原来的删除与统计();
                    }
                    else
                    {
                        if (仿真步数 % 10 == 0)//deltaT是0.1,所以运行10次是一秒；
                        {
                            统计出口人数();
                            每秒删除出口队列中的人();

                            //Console.WriteLine("总人数： " + _agents.Count);
                            //Console.WriteLine("10步耗时： " + ((float)(sw.ElapsedMilliseconds-steptimebefore)) / 1000);
                            //steptimebefore = sw.ElapsedMilliseconds;
                            //infectAgents();
                        }
                    }
                    
                    
                    
                    
                    //if (_agents.Count < (agentsOrigine / 50))//剩余百分之XX的时候
                    //{
                    //    ////Console.WriteLine("总步数： " + stepCounts);
                    //    ////Console.WriteLine("总耗时： " + sw.ElapsedMilliseconds / 1000);
                    //    //sw.Stop();
                    //    //steps.Abort();

                    //}
                    if (仿真步数 % 10 == 0 && instanceID == "output2")  //if (stepCounts % 60 == 0 && _agents.Count > 1200)
                    {
                        //暂时只输出instance2的
                        
                        Console.Write("\t" + instanceID + "总步数： " + 仿真步数 / 10);
                        Console.Write("\t" + instanceID + "总人数： " + _agents.Count);
                        Console.Write("\t" + instanceID + "总耗时： " + ((float)(sw.ElapsedMilliseconds)) / 1000);
                        Console.WriteLine("\t" + instanceID + "10步耗时： " + ((float)(sw.ElapsedMilliseconds - steptimebefore)) / 1000);


#if outDebug
                    FileHelper.Write("\t" + instanceID + "总步数： " + stepCounts / 60);
                    FileHelper.Write("\t" + instanceID + "总人数： " + _agents.Count);
                    FileHelper.Write("\t" + instanceID + "总耗时： " + ((float)(sw.ElapsedMilliseconds)) / 1000);
                    FileHelper.Write("\t" + instanceID + "100步耗时： " + ((float)(sw.ElapsedMilliseconds - steptimebefore)) / 1000,true);
#endif

                        steptimebefore = sw.ElapsedMilliseconds;
                    }
                    controlSteps = false;
                }
            }
                

        }

        public void infectAgents()
        {
            //for(int i=0;i<_agents.Count;i++)
            //{

            //}

            //更改状态  舍弃掉通过自带KD树得到邻居的方法
            //弄一个表，如果当前agent是新改变的状态，就让其不搜索
            //对所有
            for (int agenti = 0; agenti < _agents.Count; agenti++)
            {
                _agents[agenti].newStateChanged = false;
            }
            int count = 0;
            for (int i = 0; i < _agents.Count; i++)
            {
                if (_agents[i].newStateChanged == false && _agents[i].state == AgentStates.Evacuating)//如果没有发生新的颜状态变化
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int id = RVOInstance.getAgentAgentNeighbor(_agents[i].nov, j);
                        if (id < _agents.Count)
                        {
                            _agents[id].state = AgentStates.Evacuating;
                            _agents[id].newStateChanged = true;
                            count++;
                        }
                    }
                }
            }
            Console.WriteLine("count: "+count);
        }

        private void ChangeTarget(AgentClass agent)
        {
            //找到当前位置的poly
            //找到当前poly的area
            //根据area得到POS ，根据pos1得到目标

            var navpoint = songnav.navMeshQuery.FindNearestPoly(new Vector3(agent.positionNow.x_, 0, agent.positionNow.y_), new Vector3(10, 10, 10));
            var navpolyid = new NavPolyId(navpoint.Polygon.Id);

            NavTile tile;
            NavPoly poly;
            songnav.navMeshQuery.nav.TryGetTileAndPolyByRef(navpolyid, out tile, out poly);
            foreach (var area in Control._areas)
            {
                if (poly.Area.Id == area.Id)
                {
                    if (area.pos1 == -1 || Control.设置.路径选择方式=="就近选择")//如果出口数字是-1，代表还未设定的区域或者需要自己计算哪个出口位置最近就去哪个的区域，就重置一次最终目标 根据最近的出口是哪个(加上判断，如果当前目标的目标点是0,0,0，则重新寻找最近疏散目标点)
                    {
                        var posStart = agent.positionNow;
                        int min = OutIDs[0];
                        //float minVector2 = Simulate.MathHelper.abs(posStart - songnav1.level._out[0]);

                        //float delta = Simulate.MathHelper.abs(posStart - songnav1.level._out[0]);
                        //for (int m = 0; m < songnav1.level._out.Count; m++)
                        //{
                        //    if (delta > Simulate.MathHelper.abs(songnav1.level._out[m] - posStart))
                        //    {
                        //        min = m;
                        //        delta = Simulate.MathHelper.abs(songnav1.level._out[m] - posStart);
                        //    }
                        //}

                        float delta = Simulate.MathHelper.abs(posStart - songnav.map._out[OutIDs[0]]);
                        for (int m = 0; m < OutIDs.Count; m++)
                        {
                            if (delta > Simulate.MathHelper.abs(songnav.map._out[OutIDs[m]] - posStart))
                            {
                                min = OutIDs[m] ;
                                delta = Simulate.MathHelper.abs(songnav.map._out[OutIDs[m]] - posStart);
                            }
                        }

                        var p = _out[min].position;
                        Vector3 navP = songnav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav.navMeshQuery.FindNearestPoly(p, new Vector3(20, 20, 20)).Polygon.Id));
                        agent.outIndex = min;
                        agent.positionTarget = new Simulate.Vector2(navP.X, navP.Z);
                        //agent.positionTarget = new Simulate.Vector2(p.X, p.Z);
                        return;
                    }
                    else
                    {
                        var p = _out[area.pos1].position;
                        agent.outIndex = area.pos1;
                        Vector3 navP = songnav.navMeshQuery.FindRandomPointOnPoly(new NavPolyId(songnav.navMeshQuery.FindNearestPoly(p, new Vector3(20, 20, 20)).Polygon.Id));
                        agent.positionTarget = new Simulate.Vector2(navP.X, navP.Z);
                        //agent.positionTarget = new Simulate.Vector2(p.X, p.Z);
                        return;
                    }

                }
            }
        }

        /// <summary>
        /// 根据agent当前位置与目的地计算设置最佳速度矢量，每次更新位置需要调用
        /// </summary>
        void SetPreferredVelocities()
        {
            /*
                * Set the preferred velocity to be a vector of unit magnitude
                * (speed) in the direction of the goal.
                */
            //for (int i = 0; i < RVOInstance.getNumAgents(); ++i)
            for (int i = 0; i < _agents.Count; ++i)
            {
                if (_agents[i].state == AgentStates.Evacuating && _agents[i].timeResponse - Settings.deltaTDefault * 仿真步数 < 0)
                {
                    RVO.Vector2 goalVector;



                    //给agent的属性赋值，方便计算path
                    RVO.Vector2 pos = RVOInstance.getAgentPosition(i);
                    _agents[i].positionNow = new Simulate.Vector2(pos.x(), pos.y());

                    _agents[i].speedNow = MathHelper.abs(_agents[i].positionLast - _agents[i].positionNow)/Settings.deltaTDefault;
                    _agents[i].positionLast = _agents[i].positionNow;

                    if (_agents[i].navPoints.Count > 0)
                    {
                         goalVector = new RVO.Vector2(_agents[i].navPoints[0].x_, _agents[i].navPoints[0].y_) - RVOInstance.getAgentPosition(i);
                        
                    }
                    else goalVector = new RVO.Vector2(0, 0);
                    //if (RVOMath.absSq(goalVector) > 1.0f)
                    //{
                    //    goalVector = RVOMath.normalize(goalVector);
                    //}

                    ////乘以一个系数让其接近人行走速度
                    //goalVector *= 1f;//人快走的速度

                    //goalVector.x_ += _agents[i].ControlSpeedx;
                    //goalVector.y_ += _agents[i].ControlSpeedy;
                    //goalVector *= _agents[i].ControlSpeed;

                    //float offsetx=ClaculateX(goalVector.x_,goalVector.y_,_agents[i].ControlSpeedx,_agents[i].ControlSpeedy);
                    //float offsety = ClaculateY(goalVector.x_, goalVector.y_, _agents[i].ControlSpeedx, _agents[i].ControlSpeedy,offsetx);
                    //goalVector.x_ += offsetx;
                    //goalVector.y_ += offsety;

                    RVOInstance.setAgentMaxSpeed(i, _agents[i].ControlSpeed);
                    RVOInstance.setAgentPrefVelocity(i, goalVector);
                    
                    

                    /* Perturb a little to avoid deadlocks due to perfect symmetry. */
                    float angle = (float)Simulate.MathHelper.random.NextDouble() * 2.0f * (float)Math.PI;
                    float dist = (float)Simulate.MathHelper.random.NextDouble() * 0.001f;

                    RVOInstance.setAgentPrefVelocity(i, RVOInstance.getAgentPrefVelocity(i) +
                        dist * new RVO.Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
                }
            }

        }

        public float ClaculateX(float x1, float y1, float x2, float y2)
        {
            return (x2 * y1 * y1 - x1 * y1 * y2) / (y1 * y1 + x1 * x1);
        }
        public float ClaculateY(float x1, float y1, float x2, float y2, float x3)
        {
            return y2-y1*(x2-x3)/x1;
        }

        public List<List<Vector2>> agentsPositionsforExodus = new List<List<Vector2>>();
        public void 统计出口人数()
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                //for (int n = 0; n < songnav1.map._out.Count; n++)
                {
                    if(i < _agents.Count)
                    {
                        var 距离终点距离 = MathHelper.abs(songnav.map._out[_agents[i].outIndex] - _agents[i].positionNow);
                        if (距离终点距离<10)
                        {
                            if(_agents[i].inQueue == false)
                            {
                                agentsPositionsforExodus.Add(_agents[i].positions);
                                _out[_agents[i].outIndex].outAgentCount++;
                                while (_agents[i].navPoints.Count > 1) _agents[i].navPoints.RemoveAt(0);
                                _out[_agents[i].outIndex].queueToDelete.Enqueue(_agents[i]);
                                _agents[i].MaxSpeed = 0.4f;
                                _agents[i].inQueue = true;
                                //agentsPositionsforExodus.Add(_agents[i].positions);
                                //RVOInstance.delAgentAt(i);
                                //_agents.RemoveAt(i);
                                ////_outAgentCount[n]++;
                                //_out[n].outAgentCount++;
                            }
                        }
                        else if(距离终点距离 < 12)
                        {
                            _agents[i].MaxSpeed = 0.5f;
                        }
                    }
                    

                }
            }
        }

        public static string liulu="";
        public void 每秒删除出口队列中的人()
        {
            liulu = "";
            for (int i = 0; i < _out.Length; i++)
            {
                int a =(int)(_out[i].Width* OutDoors.c);//宽度乘以流率
                //a = _out[i].queueToDelete.Count;
                int temp = _out[i].queueToDelete.Count > a ? a : _out[i].queueToDelete.Count;
                liulu += (i+1) + ". " + "宽度：" + _out[i].Width +" 人数："+ temp+ " 流率："+temp/ _out[i].Width+"\n";
                while (a-->0)
                {
                    if (_out[i].queueToDelete.Count == 0)
                    {
                        a = 0;
                        continue;
                    }
                    var agent = _out[i].queueToDelete.Dequeue();

                    RVOInstance.delAgent(agent.agent);
                    _agents.Remove(agent);
                }
            }
        }

        public void CheckDelete原来的删除与统计()
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                for (int n = 0; n < songnav.map._out.Count; n++)
                {
                    //try
                    {
                        if (i < _agents.Count && MathHelper.abs(songnav.map._out[n] - _agents[i].positionNow) < 12)
                        {
                            agentsPositionsforExodus.Add(_agents[i].positions);
                            RVOInstance.delAgentAt(i);
                            _agents.RemoveAt(i);
                            //_outAgentCount[n]++;
                            _out[n].outAgentCount++;
                        }
                    }
                    //catch
                    {

                    }
                }
                //if (_agents[i].navPoints.Count < 7)
                //{
                //    RVOInstance.delAgentAt(i);
                //    _agents.RemoveAt(i);
                //    //Console.WriteLine("del " + i);
                //}
            }
        }

        #endregion

        //出错!!
        public void OutpositionFileEnd()
        {
            continueSteps = false;
            FH.EndOutput(Settings.deltaTDefault, _agents.Count);

            ////开放一个新线程，把agentsPositionsforExodus所有位置输出文件
            //outForExodus();
        }
    }
}
