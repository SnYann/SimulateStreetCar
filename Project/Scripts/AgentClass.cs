using RVO;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Simulate
{
    //只适用于
    public class AgentClass
    {
        #region 方法1
        public float exitTime = 0;//预估疏散时间
        public float exitDistance = 0;//预估疏散距离
        public List<float> _exitTime = new List<float>();//预估疏散时间
        public List<float> _exitDistance = new List<float>();//预估疏散距离
        public float agentRadius = 0.5f;
        //public int outDoorIndex;//用下面的outIndex标识出口编号
        public int rank;//在当前出口选择情况下，比他更近的agent的人数
        #endregion

        public Agent agent;

        public int nov;//编号
        //public GameObject gObject;//绑定在场景中的物体
        public Vector2 positionNow;//当前位置
        public Vector2 positionTarget;//最终目标位置
        public int outIndex;//最终位置所在的出口编号
        public AgentStates state;//跟结构体states绑定,代表agent状态
        public TimeSpan timeShow;//出现时间
        public int timeResponse;//反应时间,秒
        public bool isMan;//是否为男性
        public int age;//年龄状态
        public int area;//所在区域编号
        public string agentType;//所在区域编号
        public List<Vector2> navPoints = new List<Vector2>();//导航点数组    一打开百度网盘就卡死
        public List<Vector2> navPointstemp = new List<Vector2>();//导航点数组    一打开百度网盘就卡死

        public Color color;
        public int colorIndex;//为了给王吉文件输出时的显示
        public List<int> polyIds = new List<int>();
        
        public Vector2 positionLast;//上一次位置
        public float speedNow = 0;

        public float ControlSpeed;//对maxspeed乘以一定数值(由密度决定)之后的真实最大速度
        public float MaxSpeed;
        public float ControlSpeedx;
        public float ControlSpeedy;
        
        public bool haveReplaned = false;
        public bool newStateChanged = false;

        public bool inQueue = false;//判断是否在队列里

        public List<Vector2> positions = new List<Vector2>();

        public AgentClass(int Nov, Vector2 PostionNow, AgentStates State, TimeSpan TimeShow, bool IsMan, int Age)
        {
            nov = Nov;
            //gObject = GObject;
            positionNow = PostionNow;
            state = State;
            timeShow = TimeShow;
            isMan = IsMan;
            age = Age;

            navPoints.Add(new Vector2(0, 0));
        }

        /// <summary>
        /// agent构造函数
        /// </summary>
        /// <param name="Nov">编号</param>
        /// <param name="GObject">绑定的场景中的物体</param>
        /// <param name="PositionNow">当前位置，初始化时当前位置也是目标</param>
        /// <param name="State">agent所处状态</param>
        public AgentClass(int Nov, Vector2 PositionNow,Vector2 target, AgentStates State)//颜色随机
        {
            nov = Nov;
            //gObject = GObject;
            positionNow = PositionNow;
            state = State;
            age = 2;
            positionTarget =target;

            int R = MathHelper.random.Next(255);
            int G = MathHelper.random.Next(255);
            int B = MathHelper.random.Next(255);
            B = (R + G > 400) ? R + G - 400 : B;//0 : 380 - R - G;
            B = (B > 255) ? 255 : B;
            color = Color.FromArgb(R, G, B);
            //color = Color.Blue;
        }

        public Color setColorfromIndex(int index)
        {
            switch (index)
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
                    color = Color.White;
                    break;
            }
            return color;
        }

        public AgentClass(int Nov, Vector2 PositionNow, Vector2 target, AgentStates State,int cIndex, int oI, float maxspeed, int response, string peopleType)//,Agent a)//根据目标设定颜色
        {
            nov = Nov;
            //gObject = GObject;
            positionNow = PositionNow;
            state = State;
            //state = AgentStates.Evacuating;//通过调整状态来调整疏散传染
            age = 2;
            positionTarget = target;
            colorIndex = cIndex;
            outIndex = oI;
            agentType = peopleType;
            setColorfromIndex(cIndex);

            //#if read
            //// 设置反应时间
            //timeResponse = 0;
            //#else
            //timeResponse = (int)((RandomNormal(-1, 1) + 1) * 10);

            //if(test.Scripts.Sample.responseDistribution==0) timeResponse = (int)(MathHelper.RandomUniform(0, 30));//均匀分布
            //else timeResponse = (int)(MathHelper.RandomUniform(0, 30));//均匀分布
            timeResponse = response;
            //Console.WriteLine("respinse:" +timeResponse );
            //#endif
            ControlSpeed = maxspeed;
            MaxSpeed = maxspeed;
            ControlSpeedx = 0;
            ControlSpeedy = 0;


            //agent = a;
            

        }


        ////产生正态分布随机数，中心在0，还未验证是否对。。一般在-2到+2之间
        //public float RandomNormal(int minz, int maxz)
        //{
        //    System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        //    double u1, u2, v1 = 0, v2 = 0, s = 0, z1 = 0;
        //    while (s > 1 || s == 0)
        //    {
        //        u1 = rand.NextDouble();
        //        u2 = rand.NextDouble();
        //        v1 = 2 * u1 - 1;
        //        v2 = 2 * u2 - 1;
        //        s = v1 * v1 + v2 * v2;
        //    }
        //    z1 = Math.Sqrt(-2 * Math.Log(s) / s) * v1;
        //    //z1 = z1;
        //    if (z1 < minz || z1 > maxz) z1 = RandomNormal(minz, maxz);
        //    //Debug.Log(z1);
        //    return (float)z1; //返回两个服从正态分布N(0,1)的随机数z0 和 z1
        //}

    }

}
