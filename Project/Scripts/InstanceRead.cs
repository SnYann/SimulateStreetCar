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

    public class InstanceRead
    {
        public List<AgentClass> _agents = new List<AgentClass>();
        public List<int> 出口人数累计;
        public int 出口数;
        //读取相关
        public string pathReading;
        public StreamReader reader;
        public float timeNow;//当前时间，读取的时候用d
        public int linesCount;
        private static readonly char[] lineSplitChars = { ' ' };

        Stopwatch readsw = new Stopwatch();
        public string readswCishu = "";
        public int[] 判断是否广场人满弹窗 = { 0, 0 };
        public int 文件中读取的已疏散总人数 = 0;//当时是因为传过去的人数跟设定人数有误差，返回总和达不到总人数

        public float readdeltT = 1;//读取时的时间间隔标识

        public void InitRead(string path,int outnums)
        {
            //初始化读取路径
            pathReading = path;
            出口数 = outnums;
            出口人数累计 = new List<int>();
            for (int i = 0; i < 出口数; i++) 出口人数累计.Add(0);
            timeNow = 0;

            if (File.Exists(pathReading))
            {
                float delt = 1;
                try
                {
                    reader = new StreamReader(pathReading);
                    var l = reader.ReadLine();
                    string[] line = l.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
                    float deltT1 = float.Parse(line[0]);
                    l = reader.ReadLine();
                    line = l.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
                    float deltT2 = float.Parse(line[0]);
                    delt = deltT2 - deltT1;
                    reader.ReadToEnd();
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                }
                catch
                {
                    Console.WriteLine("读取deltT出错！");
                }

                //Thread t = new Thread(pushToLines);
                //t.Start();

                //ThreadPool.QueueUserWorkItem(pushToLines);

                readdeltT = delt;
            }
            _agents.Clear();
        }


        public int GetMaxReadLine()//
        {
            float maxTime = timeNow / readdeltT;
            return (int)maxTime;
        }


        public int GetFrameCount()
        {
            int lines = 0;
            if(File.Exists(pathReading))
            {
                using (var readers = new StreamReader(pathReading))
                {
                    while (readers.ReadLine() != null)
                    {
                        lines++;
                    }
                    readers.Close();
                }
            }
            return lines;
        }

        public int GetOutMaxCount()
        {
            int maxCount = 0;
            string str;
            if (File.Exists(pathReading))
            {
                if(reader == null) return 0;
                {
                    while (true)
                    {
                        str = reader.ReadLine();
                        if (str != null && reader.EndOfStream)
                        {
                            string[] line = str.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
                            if (line == null || line.Length == 0)
                                break;
                            if (line.Length <= 10)//只有一个代表是时间间隔！！
                            {
                                break;
                            }

                            //先加上当前每个出口的输出人数
                            for (int j = 出口数 + 1; j < 出口数 * 2 + 1; j++)
                            {
                                var agentCount = int.Parse(line[j]);
                                maxCount = maxCount > agentCount ? maxCount : agentCount;
                            }
                            break;
                        }
                    }
                    reader.ReadToEnd();
                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                }
            }
            return maxCount;
        }
        
        public void readToFirst()
        {
            _agents.Clear();
            reader.ReadToEnd();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            linesCount = 1;
            timeNow = 0;
        }

        public void ReadLineNoThread(int linesIndex = 0, int multiple = 1)//,bool 跳帧=false)
        {
            if (readsw.ElapsedMilliseconds > 0) readswCishu = (1000 / (float)readsw.ElapsedMilliseconds).ToString("f3");
            readsw.Reset();
            readsw.Start();

            //if (跳帧==false)multiple = 1;//之前通过跳帧方式调整速度，现在通过调整定时器的间隔来调整速度。因此让其为1
            if ((int)(readdeltT + 0.1f) > 0) multiple /= (int)(readdeltT + 0.1f);

            lock (_agents)
            {
                //Console.WriteLine((float)sw.ElapsedMilliseconds / 1000 + " 秒2");

                string l;
                //Console.WriteLine(instanceID + "new");

                bool flag = true;

                //偏移文件指针位置
                if (linesIndex != 0)
                {
                    /*Console.WriteLine("-----------------" + linesIndex+"count:"+linesCount);
                    int offset = linesIndex > linesCount ? 1 : -1;
                    Console.WriteLine(offset);
                    while(linesCount!=linesIndex)
                    {
                        reader.BaseStream.Seek(offset, SeekOrigin.Current);
                        if((char)reader.Peek()=='\n')
                        {
                            linesIndex += offset;
                        }
                    }
                    reader.ReadLine();*/
                    if (linesIndex > linesCount)
                    {
                        timeNow = linesIndex * readdeltT;
                        while (linesIndex != linesCount)
                        {
                            if (reader == null || reader.ReadLine() == null) return;
                            else linesCount++;
                        }
                        flag = false;
                    }
                    else
                    {
                        readToFirst();
                        if (linesIndex != 1) ReadLineNoThread(linesIndex, multiple);
                        else if (linesIndex == 1) ReadLineNoThread(0, multiple);
                    }
                }


                if (reader != null)
                {
                    l = reader.ReadLine();
                    if (l != null)
                    {
                        linesCount++;
                        string l_last="";
                        while (--multiple > 0)
                        {
                            l = reader.ReadLine();
                            //确保最后一行读到，这样才能显示广场已经满了
                            if (l != null)
                            {
                                linesCount++;
                                l_last = l;
                            }
                            else
                            {
                                l = l_last;
                                //return;
                            }
                                
                        }
                        //if (flag) linesCount++; 不明白


                        //trim any extras
                        //string tl = list[frame++];
                        string[] line = l.Split(lineSplitChars, StringSplitOptions.RemoveEmptyEntries);
                        if (line == null || line.Length == 0)
                            return;
                        if (line.Length < 30)//只有一个代表是时间间隔！！
                        {
                            return;
                        }
                        //lock (_agents)
                        {
                            //_agents.Clear();

                            _agents.Clear();//先清空

                            int 序号 = 0;

                            timeNow = float.Parse(line[序号++]);
                            Console.WriteLine("timeNow: "+timeNow);
                            文件中读取的已疏散总人数 = int.Parse(line[序号++]);

                            //先加上当前每个出口的输出人数
                            int outlength = 出口数;

                            for (int i = 序号; i < outlength + 序号; i++)
                            {
                                Control.出口人数统计[i - 序号] = int.Parse(line[i]);
                            }
                            for (int j = outlength + 序号; j < outlength * 2 + 序号; j++)
                            {
                                出口人数累计[j - outlength - 序号] = int.Parse(line[j]);
                            }

                            序号 = outlength * 2 + 序号;
                            判断是否广场人满弹窗[0] = int.Parse(line[序号++]);
                            判断是否广场人满弹窗[1] = int.Parse(line[序号++]);

                            for (int i = 序号; i < line.Length - 2 + 1; i += 3)
                            {
                                try
                                {
                                    _agents.Add(new AgentClass(_agents.Count, new Simulate.Vector2(float.Parse(line[i + 1]) / 100, float.Parse(line[i + 2]) / 100), new Simulate.Vector2(0, 0), AgentStates.Evacuating, int.Parse(line[i]), 0, 0, 0, "青年男性"));//, null));//出口位置无所谓，写成0
                                }
                                catch
                                {
                                    Console.WriteLine("超过：" + i);
                                }
                            }
                        }
                    }
                    else
                    {
                        timeNow += readdeltT;
                    }
                }

            }

        }


        #region 暂存
        public void ReadLineThread()
        {
            ThreadPool.QueueUserWorkItem(temp);

        }
        public void temp(Object obj)
        {
            ReadLineNoThread();
        }

        struct frameRecord
        {
            public List<AgentClass> agents;
            public List<int> outAgentsSetted;
            public OutDoors[] _out;
            public Bitmap image;
            public Bitmap Heatimage;
        }

        Bitmap 绘图(List<AgentClass> _agents)
        {
            Bitmap bmpMask = new Bitmap(500, 500, PixelFormat.Format32bppArgb);
            Graphics gc = Graphics.FromImage(bmpMask);
            DrawCrowd(_agents,gc);
            return bmpMask;
        }

        public void DrawCrowd(List<AgentClass> _agents, Graphics gc)
        {
            int pianzhi = 250;
            Brush brush = new SolidBrush(Color.Red);//填充的颜色
            lock (_agents)
            {
                for (int i = 0; i < _agents.Count; i++)
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
                        if (_agents[i].navPoints.Count > 0 && _agents[i].navPoints[0].x_ == 0)
                            brush = new SolidBrush(Color.Black);
                        else
                            brush = new SolidBrush(_agents[i].color);

                        //if (instance._agents[i].haveReplaned) brush = new SolidBrush(Color.Black);

                        SharpNav.Geometry.Vector3 p = new SharpNav.Geometry.Vector3(_agents[i].positionNow.X(), 0, _agents[i].positionNow.Y());
                        //agentCylinder.Draw(new OpenTK.Vector3(p.X, p.Y, p.Z), instance._agents[i].color);
                        var r = _agents[i].agentRadius;
                        gc.FillEllipse(brush, p.X - r + pianzhi, p.Z - r + pianzhi, r, r);
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

        #endregion
    }
}
