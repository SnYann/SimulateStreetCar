﻿using System;
using System.Collections;
using System.Collections.Generic;

//Navigation
//用来更新每个agent的导航点

namespace Simulate
{
    public class Decisions
    {
        //到达目标点判断
        static bool ReachGoal(AgentClass agent)
        {
            //if (MathHelper.absSq(agent.positionNow - agent.navPoints[-1]) < 1)
            //{
            return true;
            //}
            //return false;
        }

   
        /////////////////////////////////////////////////////////////////两种决策/////////////////////////////////////////////////////
        //根据agent位置和设定时间等信息对其决策,间隔调用
        public static void DecideFromAgent(IList<AgentClass> _agents)
        {
            for (int i = 0; i < _agents.Count; i++)
            {
                /*
                 * 示例：
                 *      进门 先从-20,0到0,0 状态为enter
                 *      在从0,0 到点10,10 状态为waiting
                 */
                if(ReachGoal(_agents[i]))
                {

                }

            }
        }

        //根据事件决策，由触发事件直接调用
        public static void DecideFromEvents(int e)
        {
           switch (e)
            {
                case 1:
                    Emergency();
                    break;


            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //到达目标点判断
        static void Emergency()
        {
            
        }

    }
}

