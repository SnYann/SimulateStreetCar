using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using test.Scripts;
using Window;

namespace test.UI
{
    public partial class SettingPanel : Form
    {
        //是否是浏览模式
        public bool isScan = false;
        public string filePath;

        public SettingPanel(int command, string filep)
        {
            InitializeComponent();
            filePath = filep;

            this.cb_outMode.Text = cb_outMode.Items[0].ToString();
            switch (command)
            {
                case 0://新建, 只有确定
                    btn_fromFile.Visible = true;
                    btn_OK.Visible = true;
                    btn_Save.Visible = false;
                    break;
                case 1:// 打开当前设置, 没有确定也没有保存
                    btn_fromFile.Visible = false;
                    btn_OK.Visible = false;
                    btn_Save.Visible = false;
                    break;
                case 2://编辑 保存XML配置文件, 只有保存按键
                    btn_fromFile.Visible = true;
                    btn_OK.Visible = false;
                    btn_Save.Visible = true;
                    break;
            }

            Setting 设置=new Setting(filePath);
            初始化设置界面(filePath, 设置);
        }

        //切换为浏览模式
        public void EditModeToScanMode()
        {
            this.button4.Visible = true;
            this.取消.Visible = false;
            this.btn_OK.Visible = false;
            this.btn_fromFile.Visible = false;
            this.tB_总人数.Enabled = false;
            ((DataGridViewComboBoxColumn)dataGridView_Areas.Columns[3]).DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            isScan = true;
            ChangeEditState();
        }

        //改变datagridview可编辑状态
        public void ChangeEditState()
        {
            if (isScan)
            {
                dataGridView_Areas.Columns[1].ReadOnly = true;
                dataGridView_Areas.Columns[2].ReadOnly = true;
                dataGridView_Areas.Columns[3].ReadOnly = true;
                dataGridView2.Columns[1].ReadOnly = true;
                dataGridView2.Columns[2].ReadOnly = true;
            }
            else
            {
                dataGridView_Areas.Columns[1].ReadOnly = false;
                dataGridView_Areas.Columns[2].ReadOnly = false;
                dataGridView_Areas.Columns[3].ReadOnly = false;
                dataGridView2.Columns[1].ReadOnly = false;
                dataGridView2.Columns[2].ReadOnly = false;
            }
        }

       

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_Areas.BeginEdit(true); ;
            /* dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
             if(e.ColumnIndex>1&&e.ColumnIndex>=0)
             {
                 DataGridViewComboBoxColumn combo = dataGridView1.Columns[e.ColumnIndex] as DataGridViewComboBoxColumn;
                 if(combo!=null)
                 {
                     DataGridViewComboBoxEditingControl comboEdit = dataGridView1.EditingControl as DataGridViewComboBoxEditingControl;
                     if(comboEdit!=null)
                     {
                         comboEdit.DroppedDown = true;
                     }
                 }
                 DataGridViewTextBoxColumn textBoxColumn = dataGridView1.Columns[e.ColumnIndex] as DataGridViewTextBoxColumn;
                 if(textBoxColumn!=null)
                 {
                     dataGridView1.BeginEdit(true);
                 }
             }*/
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == 3 || isScan)
            {
                dataGridView_Areas.Cursor = Cursors.Default;
                return;
            }
            if (e.ColumnIndex == 2)
            {
                dataGridView_Areas.Cursor = Cursors.IBeam;
            }
            else if (e.ColumnIndex < 2)
            {
                dataGridView_Areas.Cursor = Cursors.No;
            }
        }

        private void dataGridView2_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || isScan)
            {
                dataGridView2.Cursor = Cursors.Default;
                return;
            }
            if (e.ColumnIndex > 0)
            {
                dataGridView2.Cursor = Cursors.IBeam;
            }
            else
            {
                dataGridView2.Cursor = Cursors.No;
            }
        }

        private void 启用编辑_Click(object sender, EventArgs e)
        {
            isScan = false;
            btn_fromFile.Visible = true;
            btn_OK.Visible = true;
            取消.Visible = true;
            button4.Visible = false;
            tB_总人数.Enabled = true;
            ((DataGridViewComboBoxColumn)dataGridView_Areas.Columns[3]).DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
        }
        public void SaveXmlFile(string path)
        {
            if (File.Exists(path))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(path);
                XmlElement settings = xmlDocument.DocumentElement;
                XmlNodeList people = settings.GetElementsByTagName("People");
                XmlNodeList PathFindMode = settings.GetElementsByTagName("PathFindMode");
                XmlNodeList areas = settings.GetElementsByTagName("Areas");
                ((XmlElement)areas[0]).SetAttribute("manual", rb_手动设置.Checked == true ? "0" : "1");
                XmlNodeList colorDensity = settings.GetElementsByTagName("ColorDensity");

                //新增
                XmlNodeList SimulateMethod = settings.GetElementsByTagName("SimulateMethod");
                ((XmlElement)SimulateMethod[0]).SetAttribute("method", rB_localGPU.Checked == true ? "LocalGPU" : "RemoteGPU");
                XmlNodeList RecordInterval = settings.GetElementsByTagName("RecordInterval");
                ((XmlElement)RecordInterval[0]).SetAttribute("interval", cb_jiange.Text.ToString());

                //感染间隔
                XmlNodeList ReciveTimeMode = settings.GetElementsByTagName("ReciveTimeMode");
                if(timeRev_area.Checked)
                {
                    ((XmlElement)ReciveTimeMode[0]).SetAttribute("mode", "按区域设置");
                }
                else
                {
                    ((XmlElement)ReciveTimeMode[0]).SetAttribute("mode", "从中心开始蔓延");
                }
                
                XmlNodeList InfectionInterval = settings.GetElementsByTagName("InfectionIntervel");
                ((XmlElement)InfectionInterval[0]).SetAttribute("time", textBox_girdTimeRevIntervel.Text);

                //1.总人数
                int rowCount = 0;
                foreach (XmlElement element in people)
                {
                    element.SetAttribute("num", tB_总人数.Text.ToString());
                    element.SetAttribute("distribution", cb_ditribution.Text.ToString());
                    foreach (XmlNode node in element)
                    {
                        ((XmlElement)node).SetAttribute("percentage", dataGridView2.Rows[rowCount].Cells[1].Value.ToString());
                        ((XmlElement)node).SetAttribute("speedMin", dataGridView2.Rows[rowCount].Cells[2].Value.ToString());
                        ((XmlElement)node).SetAttribute("speedMax", dataGridView2.Rows[rowCount].Cells[3].Value.ToString());
                        ((XmlElement)node).SetAttribute("responseTimeMin", dataGridView2.Rows[rowCount].Cells[4].Value.ToString());
                        ((XmlElement)node).SetAttribute("responseTimeMax", dataGridView2.Rows[rowCount].Cells[5].Value.ToString());
                        rowCount++;
                    }
                }
                //2. 是否选择最近出口
                ((XmlElement)(PathFindMode[0])).SetAttribute("mode", cb_outMode.Text.ToString());

                //3.密度和目标
                rowCount = 0;
                foreach (XmlElement element in areas)
                {
                    foreach (XmlNode node in element)
                    {
                        ((XmlElement)node).SetAttribute("density", dataGridView_Areas.Rows[rowCount].Cells[2].Value.ToString());
                        if (dataGridView_Areas.Rows[rowCount].Cells[3].Value.ToString() == "最近出口")
                        {
                            ((XmlElement)node).SetAttribute("pos1", "-1");
                        }
                        else
                        {
                            ((XmlElement)node).SetAttribute("pos1", (int.Parse(dataGridView_Areas.Rows[rowCount].Cells[3].Value.ToString()) - 1).ToString());
                        }
                        rowCount++;
                    }
                }

                //4密度颜色设置
                ((XmlElement)colorDensity[0]).SetAttribute("maxDens", tb_maxDensity.Text);

                //设置数据来源
                XmlNodeList DataFrom = settings.GetElementsByTagName("DataFrom");
                if(rb_实时设置.Checked)
                {
                    ((XmlElement)DataFrom[0]).SetAttribute("method", "实时设置");
                }
                else if(rb_手动设置.Checked)
                {
                    ((XmlElement)DataFrom[0]).SetAttribute("method", "手动设置");
                }

                xmlDocument.Save(path);
            }
            //没有XML则拷贝
            else
            {
                File.Copy("set.xml", path);
                SaveXmlFile(path);
            }
        }

        private void 取消_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            saveAndOK(true);
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            saveAndOK(false);
        }

        void saveAndOK(bool isOK)
        {
            
            //检查感染时间间隔设置
            if (!CheckInfectInterval()) return;
            //检查人员设置
            if (!CheckPeopleSetting()) return;
            //检查区域设置
            if (!CheckAreaSetting()) return;
            //检查颜色设置
            if (!CheckDensity()) return;
            

           
            //HeatMap.calculateColorFactor(float.Parse(tb_maxDensity.Text));

            SaveXmlFile(filePath);
            if (isOK) (this.Owner as MainUI).UISimulateInit();
            this.Close();
            
        }


        bool CheckAreaSetting()
        {
            //检查密度等级
            try
            {
                for (int i = 0; i < dataGridView_Areas.RowCount; i++)
                {
                    int temp = int.Parse(dataGridView_Areas.Rows[i].Cells[2].Value.ToString());
                    if (temp > 20)
                    {
                        MessageBox.Show("区域密度等级不能超过20，请重新填写", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            catch
            {
                MessageBox.Show("区域密度设置非数字，请重新填写", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //检查接收时间
            try
            {
                for (int i = 0; i < dataGridView_Areas.RowCount; i++)
                {
                    int temp = int.Parse(dataGridView_Areas.Rows[i].Cells[4].Value.ToString());
                }
            }
            catch
            {
                MessageBox.Show("区域接收指令时间设置非数字，请重新填写", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }


            return true;
        }

        bool CheckPeopleSetting()
        {
            //保存到set中的
            int tempNum = int.Parse(tB_总人数.Text.ToString());
            if (tempNum > 110000)
            {
                MessageBox.Show("总人数不能超过11万", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tB_总人数.Text = "100000";
                return false;
            }
            if (tempNum < 200 && rB_localGPU.Checked==false)
            {
                MessageBox.Show("远程仿真时人数不能低于200", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            int percentage = 0;
            for (int i = 0; i< 6; i++)
            {
                //首先检查是否为数字
                for (int j = 1; j< 6;j++)
                {
                    try
                    {
                        float.Parse(dataGridView2.Rows[i].Cells[j].Value.ToString());
                    }
                    catch
                    {
                        MessageBox.Show("人员设置输入数据有问题,请重新修改", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }

                percentage += int.Parse(dataGridView2.Rows[i].Cells[1].Value.ToString());
                if (float.Parse(dataGridView2.Rows[i].Cells[2].Value.ToString()) > float.Parse(dataGridView2.Rows[i].Cells[3].Value.ToString()))
                {
                    MessageBox.Show("人员设置有问题,最小速度不能大于最大速度,请重新修改", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (int.Parse(dataGridView2.Rows[i].Cells[4].Value.ToString()) > int.Parse(dataGridView2.Rows[i].Cells[5].Value.ToString()))
                {
                    MessageBox.Show("人员设置有问题,最短响应时间不能大于最长响应时间,请重新修改", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            //检查是否
            if (percentage != 100)
            {
                MessageBox.Show("人员设置比例错误", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        bool CheckInfectInterval()
        {
            try
            {
                if (timeRev_girdCenter.Checked)
                {
                    int temp = int.Parse(textBox_girdTimeRevIntervel.Text);
                    if (temp == 0)
                    {
                        MessageBox.Show("时间间隔设置有问题，至少为1秒，请重新设置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        Console.WriteLine("时间间隔设置有问题，至少为1秒，请重新设置");
                        return false;
                    }
                }

            }
            catch
            {
                MessageBox.Show("时间间隔设置有问题，请重新设置", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Console.WriteLine("时间间隔设置有问题，请重新设置");
                return false;
            }
            return true;
        }

        bool CheckDensity()
        {
            float maxDensity;
            try
            {
                maxDensity = float.Parse(tb_maxDensity.Text);
                if (maxDensity < 1)
                {
                    MessageBox.Show("颜色密度设置过小", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                if (maxDensity > 6)
                {
                    MessageBox.Show("颜色密度设置过大", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            catch
            {
                MessageBox.Show("最大颜色密度设置不是合法数字", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        private void cb_outMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //就近选择方式，应更新panel显示
            if (cb_outMode.SelectedIndex == 1)
            {
                dataGridView_Areas.Columns[3].Visible = false;
            }
            else
            {
                dataGridView_Areas.Columns[3].Visible = true;
            }
        }

        private void btn_fromFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog f = new OpenFileDialog();
            //默认所有工程都在project文件夹中
            f.InitialDirectory = Directory.GetCurrentDirectory();
            f.Multiselect = false;
            f.Title = "请选择需要导入的配置文件";
            f.Filter = "配置文件(*.sim)|*.sim";
            if (f.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(f.FileName);
                if(File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                File.Copy(f.FileName, filePath);
                //InitDataView(filePath);
                Setting 设置 = new Setting(filePath);
                初始化设置界面(filePath, 设置);
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 如果输入的不是数字键，也不是回车键、Backspace键，则取消该输入
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (MessageBox.Show("XXX不知道什么错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.None)
            {
               
            }
        }

        //控制只能输入数字
        public DataGridViewTextBoxEditingControl CellEdit = null;
        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            //if ((this.dataGridView1.CurrentCellAddress.X == 1) || (this.dataGridView1.CurrentCellAddress.X == 2))
            {
                CellEdit = (DataGridViewTextBoxEditingControl)e.Control;
                CellEdit.SelectAll();
                CellEdit.KeyPress += Cells_KeyPress; //绑定事件
            }
        }

        private void Cells_KeyPress(object sender, KeyPressEventArgs e) //自定义事件
        {
                if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar=='.')) e.Handled = true;
                if (e.KeyChar == '\b') e.Handled = false;
        }

        private void timeRev_girdCenter_CheckedChanged(object sender, EventArgs e)
        {
            疏散指令时间接收方式.Height = 145;
            if (timeRev_girdCenter.Checked)
            {
                textBox_girdTimeRevIntervel.Text = "10";
                dataGridView_Areas.Columns[4].Visible = false;
            }
            else
            {
                //疏散指令时间接收方式.Height = 93;
                dataGridView_Areas.Columns[4].Visible = true;
            }
        }

        private void RB_remote_CheckedChanged(object sender, EventArgs e)
        {
            仿真方式.Height = 137;
        }

        private void TimeRev_area_CheckedChanged(object sender, EventArgs e)
        {
            疏散指令时间接收方式.Height = 88;
            textBox_girdTimeRevIntervel.Text = "0";
        }

        private void RB_rvo_CheckedChanged(object sender, EventArgs e)
        {
            仿真方式.Height = 88;
            //timeRev_girdCenter.Enabled = true;
        }

        private void Tb_maxDensity_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)8 && e.KeyChar != '.')//&& density1.Text.Length>4
            {
                e.Handled = true;
            }
        }

        private void Tb_maxDensity_TextChanged(object sender, EventArgs e)
        {
            float maxColor=1;
            try
            {
                maxColor = float.Parse(tb_maxDensity.Text);
            }
            catch
            {
                return;
            }
            label_density11.Text = (1 * maxColor / 5).ToString();
            label_density2.Text = (1 * maxColor / 5).ToString();

            label_density22.Text = (2 * maxColor / 5).ToString();
            label_density3.Text = (2 * maxColor / 5).ToString();

            label_density33.Text = (3 * maxColor / 5).ToString();
            label_density4.Text = (3 * maxColor / 5).ToString();

            label_density44.Text = (4 * maxColor / 5).ToString();
            label_density5.Text = (4 * maxColor / 5).ToString();

            label_density55.Text = maxColor.ToString();


        }
        private void 得到实时数据_Click(object sender, EventArgs e)
        {
            实时数据进度条.Value = 50;
            实时数据进度条.Visible = true;
            string str=getHtml("http://47.107.52.7:90/EarlyWarning/dm_admin/index.php?/api/anchorData?DateTime=2019-07-26 17:03:00&DataNumber=4&DataStep=1");

            str = "";
            str = File.ReadAllText("temp.txt",Encoding.GetEncoding("utf-8"));

            List<areaData> areaDatas = new List<areaData>();
            if (解码(str, ref areaDatas) == false)
            {
                MessageBox.Show("获取数据失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                //分布人群
            }
            foreach(var ad in areaDatas)
            {
                dataGridView_Areas.Rows[ad.section_code].Cells[2].Value = ad.counts;
            }

           
            实时数据进度条.Value = 100;
            实时数据进度条.Visible = false;
        }

        private string getHtml(string html)//传入网址
        {
            string pageHtml = "";
            WebClient MyWebClient = new WebClient();
            MyWebClient.Credentials = CredentialCache.DefaultCredentials;//获取或设置用于向Internet资源的请求进行身份验证的网络凭据
            Byte[] pageData = MyWebClient.DownloadData(html); //从指定网站下载数据
            MemoryStream ms = new MemoryStream(pageData);
            using (StreamReader sr = new StreamReader(ms, Encoding.GetEncoding("utf-8")))
            {
                pageHtml = sr.ReadLine();
            }
            return pageHtml;
        }

        public struct areaData
        {
            public int section_code;//"section_code": 1,//区域ID：1~24
            public string section_name;//"section_name": "金世界广场",//区域名称
            public int counts;
            //public areaData(int id,string name,int density)
            //{
            //    section_code = id;
            //    section_name = name;
            //    density_value = density;
            //}
        }
        public bool 解码(string str,ref List<areaData> areaDatas)
        {
            try
            {
                JsonSerializer serializer;
                serializer = JsonSerializer.Create(new JsonSerializerSettings());
                JObject root = JObject.Parse(str);

                bool success = root["success"].ToObject<bool>(serializer);
                if (!success) return false;

                JArray tilesToken = (JArray)root["data"];

                foreach (JToken tileToken in tilesToken)
                {
                    areaData ad;
                    ad.section_code = tileToken["section_code"].ToObject<int>(serializer);
                    ad.section_name = tileToken["section_name"].ToObject<string>(serializer);
                    ad.counts = tileToken["counts"].ToObject<int>(serializer);
                    areaDatas.Add(ad);
                }
            }
            catch
            {
                return false;
            }
           
            return true;
        }

        private void Rb_手动设置_CheckedChanged(object sender, EventArgs e)
        {
            tB_总人数.Enabled = true;
            得到实时数据.Visible = false;
            dataGridView_Areas.Columns[2].Visible = true;
            dataGridView_Areas.Columns[5].Visible = false;
        }

        private void Rb_实时设置_CheckedChanged(object sender, EventArgs e)
        {
            tB_总人数.Enabled = false;
            得到实时数据.Visible = true;
            dataGridView_Areas.Columns[2].Visible = false;
            dataGridView_Areas.Columns[5].Visible = true;
        }

      
        public void 初始化设置界面(string path, Setting 设置)
        {
            dataGridView_Areas.Rows.Clear();
            dataGridView2.Rows.Clear();
            tB_总人数.Text = 设置.总人数.ToString();
            cb_outMode.Text = 设置.路径选择方式;

            foreach (var p in 设置.peoples)
            {
                int index = dataGridView2.Rows.Add();
                dataGridView2.Rows[index].Cells[0].Value = p.name;
                dataGridView2.Rows[index].Cells[1].Value = p.percentage;
                dataGridView2.Rows[index].Cells[2].Value = p.speedMin;
                dataGridView2.Rows[index].Cells[3].Value = p.speedMax;
                dataGridView2.Rows[index].Cells[4].Value = p.responseTimeMin;
                dataGridView2.Rows[index].Cells[5].Value = p.responseTimeMax;
            }
            cb_ditribution.Text = 设置.响应时间分布方式;

            foreach(var area in 设置.areas)
            {
                int index = dataGridView_Areas.Rows.Add();
                dataGridView_Areas.Rows[index].Cells[0].Value = area.id.ToString();
                dataGridView_Areas.Rows[index].Cells[1].Value = area.name;
                dataGridView_Areas.Rows[index].Cells[2].Value = area.density.ToString();
                dataGridView_Areas.Rows[index].Cells[4].Value = area.receiveTime.ToString();
                dataGridView_Areas.Rows[index].Cells[5].Value = area.peoplenums.ToString();
                var cb = (DataGridViewComboBoxCell)dataGridView_Areas.Rows[index].Cells[3];
                foreach (var door in 设置.outids)
                {
                    cb.Items.Add(door.ToString());
                }
                cb.Items.Add("最近出口");
                //判断当前出口在出口选项中排第几,然后让其选择

                if (area.pos1==-1)
                {
                    dataGridView_Areas.Rows[index].Cells[3].Value = "最近出口";
                }
                else
                {
                    int pos1 = area.pos1 + 1;
                    dataGridView_Areas.Rows[index].Cells[3].Value =pos1.ToString();
                }
            }

            tb_maxDensity.Text = 设置.colorDensity_maxDens.ToString();
            if(设置.疏散时间接收方式 == "按区域设置")
            {
                timeRev_area.Checked=true;
            }
            else if(设置.疏散时间接收方式 == "从中心开始蔓延")
            {
                timeRev_girdCenter.Checked = true;
            }

            textBox_girdTimeRevIntervel.Text = 设置.infectionIntervel_time.ToString();
            if(设置.simulateMethod=="LocalGPU")
            {
                rB_localGPU.Checked=true;
            }
            else if(设置.simulateMethod == "RemoteGPU")
            {
                rB_remote.Checked = true;
            }
            cb_jiange.Text = 设置.记录文件时间间隔.ToString() ;

            if(设置.区域数据来源=="手动设置")
            {
                rb_手动设置.Checked = true;
            }
            else if(设置.区域数据来源 == "实时设置")
            {
                rb_实时设置.Checked = true;
            }
        }

        private void SettingPanel_Load(object sender, EventArgs e)
        {

        }
    }
}
