
using SharpNav.Geometry;
using SharpNav.Pathfinding;
using Simulate;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using test.Scripts;

namespace optimize
{
    /// <summary>
    /// 对于所有的方法
    /// 传入：
    ///     agent的位置
    ///     地图
    ///     
    ///传出：
    ///     agent的导航路线
    /// </summary>
    class Approch1
    {
        //得到距离，与出口安排->对其排序->得到exitTime
        public Approch1(List<AgentClass> _agentsAll, Navigation songnav1)//这里将所有Agent添加进来
        {
            ////////案例测试
            ////////SharpNav.Pathfinding.Path path1 = songnav1.SmothPathfinding_paths(new Simulate.Vector2(-98f,-1.5f), new Simulate.Vector2(97f,-0.5f));
            ////////Console.WriteLine("cost:" + path1.Cost);
            ////////Console.WriteLine("距离："+MathHelper.abs(new Simulate.Vector2(-98f, -1.5f) - new Simulate.Vector2(97f, -0.5f)));

            ////////统计到各个出口的距离，该距离只寻一次，然后不变
            ////////第一次按照最短距离代表时间，先求出第一次排序！
            //////for (int i = 0; i < _agentsAll.Count; i++)
            //////{
            //////    //分别计算到各个出口点的距离
            //////    for (int d = 0; d < Instance._out.Length; d++)
            //////    {
            //////        SharpNav.Pathfinding.Path path = songnav1.SmothPathfinding_paths(_agentsAll[i].positionNow, Instance._out[d].getPositionOnPloyV2());
            //////        _agentsAll[i].navPoints = songnav1.SmothPathfinding_Points(path);//可能可以去掉
            //////        _agentsAll[i]._exitDistance.Add(path.Cost);
            //////        _agentsAll[i]._exitTime.Add(path.Cost);
            //////    }

            //////    float timeMin = _agentsAll[i]._exitTime[0];
            //////    _agentsAll[i].outIndex = 0;//检验是否从0开始
            //////    for (int d = 0; d < Instance._out.Length; d++)
            //////    {
            //////        if (_agentsAll[i]._exitTime[d] < timeMin)
            //////        {
            //////            timeMin = _agentsAll[i]._exitTime[d];
            //////            _agentsAll[i].outIndex = d;
            //////        }
            //////    }
            //////    _agentsAll[i].exitTime = timeMin;
            //////    _agentsAll[i].exitDistance = _agentsAll[i]._exitDistance[_agentsAll[i].outIndex];
            //////}

            ////////估计时间
            //////int circulationCount = 0;//循环次数
            //////int agentOutdoorChangedCount = -1;
            //////while (true)
            //////{
            //////    if (circulationCount++ > 20) break;
            //////    //if (agentOutdoorChangedCount != -1 && agentOutdoorChangedCount < 3) break;
            //////    //对选择同一个出口的人进行排序
            //////    UpdateRank(ref _agentsAll);
            //////    //估计疏散时间
            //////    CalculateExitTime(ref _agentsAll);
            //////    //更改出口
            //////    agentOutdoorChangedCount = ChangeOutDoor(ref _agentsAll, songnav1);
            //////    Console.WriteLine("agentOutdoorChangedCount: " + agentOutdoorChangedCount);
            //////    Console.WriteLine("circulationCount: " + circulationCount);
            //////}
        }

        /// <summary>
        /// 更改出口，返回有多少人更改了出口
        /// </summary>
        /// <returns></returns>
        public int ChangeOutDoor(ref List<AgentClass> agents, Navigation songnav1)
        {
            ////int count = 0;
            ////List<AgentClass> ls = new List<AgentClass>();
            ////for (int i = 0; i < agents.Count; i++)
            ////{
            ////    float oldTimeMin = agents[i].exitTime;
            ////    int oldoutIndex = agents[i].outIndex;
            ////    for (int d = 0; d < Instance._out.Length; d++)
            ////    {
            ////        if (agents[i]._exitTime[d] < agents[i].exitTime && agents[i]._exitTime[d] != 0)
            ////        {
            ////            agents[i].exitTime = agents[i]._exitTime[d];
            ////            agents[i].outIndex = d;
            ////        }
            ////    }
            ////    if (oldoutIndex != agents[i].outIndex)
            ////    {
            ////        count++;
            ////        ls.Add(agents[i]);
            ////        agents[i].color = Color.DeepPink;
            ////        agents[i].agentRadius = 1;

            ////        Path path = songnav1.SmothPathfinding_paths(agents[i].positionNow, Instance._out[agents[i].outIndex].getPositionOnPloyV2());
            ////        agents[i].navPoints.Clear();
            ////        agents[i].navPoints = songnav1.SmothPathfinding_Points(path);//可能可以去掉

            ////    }
            ////}
            ////return count;
            ///
            return 0;
        }


        public void CalculateExitTime(ref List<AgentClass> agents)
        {
            //////foreach (var agent in agents)
            //////{
            //////    for (int d = 0; d < Instance._out.Length; d++)
            //////    {
            //////        agent._exitTime[d] = (int)(agent._exitDistance[d] + agent.rank / (OutDoors.c * Instance._out[d].Width));//
            //////    }
            //////    agent.exitTime = (int)(agent.exitDistance + agent.rank / (OutDoors.c * Instance._out[agent.outIndex].Width));// / (OutDoors.c * Instance._out[agent.outIndex].width));
            //////}
        }

        public void UpdateRank(ref List<AgentClass> agents)
        {
            ////////List<AgentClass>[] _doorAgents = new List<AgentClass>[Instance._out.Length];
            ////////for (int i = 0; i < agents.Count; i++)
            ////////{
            ////////    _doorAgents[agents[i].outIndex].Add(agents[i]);
            ////////}
            ////////for (int i = 0; i < _doorAgents.Length; i++)
            ////////{
            ////////    _doorAgents[i].Sort(new AgentComparer());
            ////////    for (int j = 0; j < _doorAgents[i].Count; j++)
            ////////    {
            ////////        _doorAgents[i][j].rank = j;
            ////////    }
            ////////}

            //////List<List<AgentClass>> _doorAgents = new List<List<AgentClass>>();
            //////foreach (var i in Instance._out) _doorAgents.Add(new List<AgentClass>());
            //////for (int i = 0; i < agents.Count; i++)
            //////{
            //////    _doorAgents[agents[i].outIndex].Add(agents[i]);
            //////}
            //////for (int i = 0; i < _doorAgents.Count; i++)
            //////{
            //////    var doorAgents = _doorAgents[i];
            //////    doorAgents.Sort(new AgentComparer());
            //////    for (int j = 0; j < doorAgents.Count; j++)
            //////    {
            //////        doorAgents[j].rank = j;
            //////    }
            //////}

            ////////var ls = _doorAgents[0];
            ////////ls.Sort(new AgentComparer());
            ////////for (int i = 0; i < _doorAgents.Count; i++)
            ////////{
            ////////    _doorAgents[i].Sort(new AgentComparer());
            ////////    for (int j = 0; j < _doorAgents[i].Count; j++)
            ////////    {
            ////////        _doorAgents[i][j].rank = j;
            ////////    }
            ////////}
        }
        /// <summary>
        /// 比较类实例大小，实现接口IComparer
        /// </summary>
        public class AgentComparer : IComparer<AgentClass>
        {
            public int Compare(AgentClass x, AgentClass y)
            {
                if (x.exitTime > y.exitTime) return 1;
                if (x.exitTime == y.exitTime) return 0;
                else return -1;
            }
        }
    }




}
