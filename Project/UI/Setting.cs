using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace test.UI
{
    public class Setting
    {
        public int 总人数;
        public string 响应时间分布方式;
        public struct people
        {
            public string name;
            public int percentage;
            public float speedMin;
            public float speedMax;
            public int responseTimeMin;
            public int responseTimeMax;

            public int 占比累加值;//为了生成人的时候方便
        }
        public List<people> peoples;
        public string 路径选择方式;
        public bool areaManual;
        public struct area
        {
            public int id;
            public int direct;
            public string name;
            public int pos1;
            public int pos2;
            public int density;
            public int areage;
            public int receiveTime;
            public int peoplenums;//区域人数

        }
        public List<area> areas;
        public List<int> outids;
        public int colorDensity_maxDens;
        public string 疏散时间接收方式;
        public int infectionIntervel_time;
        public string simulateMethod;
        public long 记录文件时间间隔;
        public string remoteIP;
        public string remotePort;
        public string 区域数据来源;//

        public Setting(string path)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(path);
            XmlElement settings = xmlDocument.DocumentElement;

            //设置人数, 人的属性等
            XmlNodeList people = settings.GetElementsByTagName("People");
            总人数 = int.Parse(((XmlElement)people[0]).GetAttribute("num"));
            响应时间分布方式 = ((XmlElement)people[0]).GetAttribute("distribution");
            peoples = new List<Setting.people>();
            foreach (XmlElement element in people)
            {
                foreach (XmlNode node in element)
                {
                    Setting.people p;
                    p.name = node.Name;
                    p.percentage = int.Parse(((XmlElement)node).GetAttribute("percentage"));
                    p.speedMin = float.Parse(((XmlElement)node).GetAttribute("speedMin"));
                    p.speedMax = float.Parse(((XmlElement)node).GetAttribute("speedMax"));
                    p.responseTimeMin = int.Parse(((XmlElement)node).GetAttribute("responseTimeMin"));
                    p.responseTimeMax = int.Parse(((XmlElement)node).GetAttribute("responseTimeMax"));

                    p.占比累加值 = p.percentage;
                    if (peoples.Count > 0) p.占比累加值 += peoples[peoples.Count-1].占比累加值;
                    peoples.Add(p);
                }
            }
            //设置是否选择最近出口
            XmlNodeList PathFindMode = settings.GetElementsByTagName("PathFindMode");
            路径选择方式 = ((XmlElement)PathFindMode[0]).GetAttribute("mode");
            outids = new List<int>();
            //设置出口数
            XmlNodeList areaouts = settings.GetElementsByTagName("AreaOuts");
            foreach (XmlElement element in areaouts)//这里应该只有一个AgentOuts
            {
                XmlNodeList outs = element.GetElementsByTagName("Outs");
                foreach (XmlElement e in outs)
                {
                    List<Setting.area> a = new List<Setting.area>();
                    foreach (XmlNode node in e)
                    {
                        outids.Add(int.Parse(((XmlElement)node).GetAttribute("outid"))-1);
                    }
                }
            }

            //设置区域
            areas = new List<Setting.area>();
            XmlNodeList areasXML = settings.GetElementsByTagName("Areas");
            areaManual = ((XmlElement)areasXML[0]).GetAttribute("manual") == "0";
            for (int i = 0; i < areasXML.Count; i++)
            {
                XmlNodeList nodes = ((XmlElement)areasXML[i]).ChildNodes;
                for (int y = 0; y < nodes.Count; y++)
                {
                    Setting.area a = new Setting.area();
                    a.id = int.Parse(((XmlElement)nodes[y]).GetAttribute("id"));
                    a.direct = 1;
                    a.name = ((XmlElement)nodes[y]).GetAttribute("name");
                    a.density = int.Parse(((XmlElement)nodes[y]).GetAttribute("density"));
                    a.receiveTime = int.Parse(((XmlElement)nodes[y]).GetAttribute("receiveTime"));
                    a.peoplenums = int.Parse(((XmlElement)nodes[y]).GetAttribute("peoplenums"));
                    a.pos1 = int.Parse(((XmlElement)nodes[y]).GetAttribute("pos1"));
                    a.pos2 = int.Parse(((XmlElement)nodes[y]).GetAttribute("pos2"));
                    areas.Add(a);
                }
            }
            //设置最深颜色密度
            XmlNodeList colorDensity = settings.GetElementsByTagName("ColorDensity");
            colorDensity_maxDens = int.Parse(((XmlElement)colorDensity[0]).GetAttribute("maxDens"));

            //设置感染时间

            //设置是否用RVO仿真(是否本地仿真)
            XmlNodeList SimulateMethod = settings.GetElementsByTagName("SimulateMethod");
            simulateMethod = ((XmlElement)SimulateMethod[0]).GetAttribute("method");

            //设置疏散时间接收方式
            XmlNodeList ReciveTimeMode = settings.GetElementsByTagName("ReciveTimeMode");
            疏散时间接收方式 = ((XmlElement)ReciveTimeMode[0]).GetAttribute("mode");

            //设置远程仿真的时候的返回时间间隔
            XmlNodeList RecordInterval = settings.GetElementsByTagName("RecordInterval");
            记录文件时间间隔 = int.Parse(((XmlElement)RecordInterval[0]).GetAttribute("interval"));

            XmlNodeList RemoteIP = settings.GetElementsByTagName("RemoteIP");
            remoteIP = ((XmlElement)RemoteIP[0]).GetAttribute("ip");
            remotePort = ((XmlElement)RemoteIP[0]).GetAttribute("port");

            XmlNodeList DataFrom = settings.GetElementsByTagName("DataFrom");
            区域数据来源 = ((XmlElement)DataFrom[0]).GetAttribute("method");
        }
    }
}
