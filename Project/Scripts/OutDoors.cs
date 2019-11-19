using RVO;
using SharpNav.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;


namespace Simulate
{
    //只适用于
    public class OutDoors
    {
        public Vector3 position;//当前位置
        public int outAgentCount;//出去的人数统计
        public int outAgentCountLast;//上次出去的人数统计
        public float flowrate;
        private float width;   // the name field
        public float Width   // the Name property
        {
            get
            {
                return width;
            }
        }
        public static float c = 1.33f;//单位流量
        public Queue<AgentClass> queueToDelete = new Queue<AgentClass>();

       
        public OutDoors(Vector2 pos,float w)
        {
            width = w;
            position = new Vector3(pos.x_, 0, pos.y_);
            outAgentCount = 0;
        }
        public Vector2 getPositionOnPloyV2()
        {
            return new Vector2(position.X, position.Z);
            //return new Vector2(positionOnPloy.X, positionOnPloy.Z);
        }

        public void calculateFlow(int time)
        {

        }
    }

}
