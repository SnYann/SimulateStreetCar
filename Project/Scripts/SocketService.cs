using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test.Scripts
{
    class SocketService
    {
        private static Socket serverSocket;
        public void Start()
        {
            //服务器IP地址
            //IPAddress ip = IPAddress.Parse(ConfigurationManager.AppSettings["ListenIP"]);
            IPAddress ip = IPAddress.Parse("172.18.61.129");
            //int myProt = Convert.ToInt32(ConfigurationManager.AppSettings["ListenFilePort"]);
            int myPort = 3477;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(ip, myPort));  //绑定IP地址：端口
            serverSocket.Listen(10);    //设定最多10个排队连接请求
            Console.WriteLine("启动监听{0}成功", serverSocket.LocalEndPoint.ToString());
            //通过Clientsoket发送数据
            Thread myThread = new Thread(ListenClientConnect);
            myThread.Start();
        }
        public static void Exit()
        {
            serverSocket.Close();
            serverSocket = null;
        }
        private static void ListenClientConnect()
        {
            while (true)
            {
                if (serverSocket != null)
                {
                    try
                    {
                        Socket clientSocket = serverSocket.Accept();
                        Thread receiveThread = new Thread(Receive);
                        receiveThread.Start(clientSocket);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }
        public static void Receive(object clientSocket)
        {
            Socket client = clientSocket as Socket;
            //获得客户端节点对象
            IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;


            
            //获得[文件名]   
            string commands = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));

            switch(commands)
            {
                case "positionfile start":
                    ReceiveFile(client);
                    break;
                case "test":
                    Console.WriteLine("ceshi!!!");
                    break;
                case "positionfile end":
                    //接收完毕，合并文件
                    List<string> filePaths=new List<string>();
                    for (int i=0;i<100;i++)//最多一百个文件
                    {
                        if (File.Exists(Control.mainDirectory + "output0_" + i))
                        {
                            filePaths.Add(Control.mainDirectory + "output0_" + i);
                        }
                        else
                            break;
                    }
                    if (filePaths.Count > 0) CombineFiles(filePaths, "output0");
                    break;
            }

            client.Close();
        }

        public static void ReceiveFile(object clientSocket)
        {
            Socket client = clientSocket as Socket;
            //获得[文件名]   
            string SendFileName = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));

            ////获得[包的大小]
            //string bagSize = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));
            //Console.WriteLine("包大小：" + bagSize);
            ////获得[包的总数量]   
            //int bagCount = int.Parse(System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client)));
            //Console.WriteLine("包总数：" + bagCount);
            ////获得[最后一个包的大小]   
            //string bagLast = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));
            //Console.WriteLine("最后一个包大小：" + bagCount);

            string fullPath = Path.Combine(Environment.CurrentDirectory, SendFileName);
            //创建一个新文件   
            FileStream MyFileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);

            //已发送包的个数   
            int SendedCount = 0;
            while (true)
            {

                byte[] data = TransferFiles.ReceiveVarData(client);
                if (data.Length == 0)
                {
                    break;
                }
                else
                {
                    SendedCount++;
                    //将接收到的数据包写入到文件流对象   
                    MyFileStream.Write(data, 0, data.Length);
                    //显示已发送包的个数     
                    Console.WriteLine("已经接收：" + SendedCount);
                }
            }
            //关闭文件流   
            MyFileStream.Close();
            //关闭套接字   
            
            //SocketServer.pForm.ShowMessageBox(SendFileName + "接收完毕！");
            Console.WriteLine("文件接收成功");
        }

        static void CombineFiles(List<string> filePaths, string combineFile)
        {
            using (FileStream CombineStream = new FileStream(combineFile, FileMode.OpenOrCreate))
            {
                using (BinaryWriter CombineWriter = new BinaryWriter(CombineStream))
                {
                    foreach (string file in filePaths)
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
                }
            }
        }
    }
}
