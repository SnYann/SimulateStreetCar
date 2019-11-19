using ICSharpCode.SharpZipLib.Zip;
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
    
    /// <summary>
    /// 暂时设置成每发送一次指令+数据就连接一次
    /// </summary>
    public static class SocketClient
    {

        //static string IP = "172.18.61.129";
        //static int Port = 3477;
        //内部地址10.26.0.188 
        //外部地址118.24.7.153   
        //159.226.41.145 新地址

        //static string IP = "159.226.41.145";//"118.24.7.153";"118.24.7.153";//
        //static int Port = 11114;
        static Socket client;

        public static void Close()
        {
            try
            {
                client.Close();
            }
            catch
            {
                Console.WriteLine("连接断开！");
            }
        }

        public static bool connect(string remoteIP,int Port)
        {
            //指向远程服务端节点
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(remoteIP), Port);
            //创建套接字
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.ReceiveTimeout=60000;//60秒超时
            client.SendTimeout=60000;
          
            //连接到发送端
            try
            {
                client.Connect(ipep);
            }
            catch
            {
                Console.WriteLine("连接服务器失败！");
                return false;
            }
            
            ////获得客户端节点对象
            //IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;
            return true;
        }

        public static string ReceiveString(int size)
        {
            var temp=TransferFiles.ReceiveData(client,15);
            return Encoding.Unicode.GetString(temp);
        }

        public static string ReceiveString()
        {
            //Socket client = clientSocket as Socket;
            //获得[文件名]   
            //string SendFileName = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));
            byte[] SendFileNameBytes = TransferFiles.ReceiveData(client, 20);//文件名固定最多15个字符
            int count = 0;
            for (int i = 0; i < 15; i++)
            {
                if (SendFileNameBytes[i] == '\0') break;
                count++;
            }
            byte[] SendString = new byte[count];
            for (int i = 0; i < count; i++) SendString[i] = SendFileNameBytes[i];
            return Encoding.Default.GetString(SendString);
        }
        public static string ReceiveFile(string dic)
        {
            string SendFileName= ReceiveString();
            if (SendFileName == "end") return "end";
            //以上就是解析文件名，搞麻烦了
             Console.WriteLine("开始接收：" + SendFileName);

            //如果文件夹不存在就创建一个
            if (false == Directory.Exists(dic))
            {
                //创建pic文件夹
                Directory.CreateDirectory(dic);
            }

            string fullPath = dic + SendFileName;
            //创建一个新文件   
            FileStream MyFileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);

            //已发送包的个数   
            int SendedCount = 0;
            while (true)
            {
                byte[] data = TransferFiles.ReceiveVarData(client);
                Console.WriteLine("SendedCount:" + SendedCount + "  data.Length: " + data.Length);
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
                }
            }
            //关闭文件流   
            MyFileStream.Close();
            ////关闭套接字 
            //client.Close();

            //SocketServer.pForm.ShowMessageBox(SendFileName + "接收完毕！");
            Console.WriteLine("文件接收成功,解压文件...");
       
            return SendFileName;
        }

        public static void Send()
        {
            for (int i = 0; i < 100; i++)
            {
                if (File.Exists("output0_" + i))
                {
                    try
                    {
                        SendFile分包发送("output0_" + i);
                    }
                    catch
                    {
                        Console.WriteLine("发送 output0_" + i + "失败");
                    }
                    Console.WriteLine("发送 output0_" + i);
                }
                else
                    break;
            }

        }

        /// <summary>
        /// 从long到bytes[]
        /// </summary>
        /// <param name="s"></param>
        /// <param name="asc"></param>
        /// <returns></returns>
        public static byte[] getBytesFromLong(long s, bool asc)
        {
            byte[] buf = new byte[8];
            if (asc)
                for (int i = buf.Length - 1; i >= 0; i--)
                {
                    buf[i] = (byte)(s & 0x00000000000000ff);
                    s >>= 8;
                }
            else
                for (int i = 0; i < buf.Length; i++)
                {
                    buf[i] = (byte)(s & 0x00000000000000ff);
                    s >>= 8;
                }

            for(int i=0;i<8;i++)
            {
                Console.WriteLine(buf[i]+" ");
            }
            return buf;
        }


        public static bool SendFile原始大文件(string fullPath)
        {
            //TransferFiles.SendData(client, Encoding.Unicode.GetBytes("send positionfile"));

            //创建一个文件对象
            FileInfo EzoneFile = new FileInfo(fullPath);
            //打开文件流
            FileStream EzoneStream = EzoneFile.OpenRead();
            bool isCut = false;

            //先发送四个字节的数据大小数据
            TransferFiles.SendData(client, getBytesFromLong(EzoneStream.Length, false));
            Console.WriteLine("buffer大小 " + EzoneStream.Length);
            //数据包
            byte[] data = new byte[EzoneStream.Length];

            //从文件流读取数据并填充数据包
            EzoneStream.Read(data, 0, data.Length);
            //发送数据包
            TransferFiles.SendData(client, data);

            ////关闭套接字
            //client.Close();
            //关闭文件流
            EzoneStream.Close();
            if (!isCut)
            {
                return true;
            }
            return false;
        }
       
        public static bool SendFile分包发送(string fullPath)//分包发送
        {
            //TransferFiles.SendData(client, Encoding.Unicode.GetBytes("send positionfile"));

            //创建一个文件对象
            FileInfo EzoneFile = new FileInfo(fullPath);
            //打开文件流
            FileStream EzoneStream = EzoneFile.OpenRead();
            bool isCut = false;

            //先发送四个字节的数据大小数据
            TransferFiles.SendData(client, getBytesFromLong(EzoneStream.Length, false));
            Console.WriteLine("buffer大小 " + EzoneStream.Length);

            //数据包
            int package_size = 10240;
            byte[] data = new byte[package_size];//每个包10K
            int offset = 0;
            while (offset+ package_size < EzoneStream.Length)
            {
                Console.WriteLine("发送起点 "+offset+" 大小: "+package_size);
                //从文件流读取数据并填充数据包
                EzoneStream.Read(data, 0, data.Length);
                offset += package_size;
                //发送数据包
                TransferFiles.SendData(client, data);
            }
            //从文件流读取数据并填充数据包
            Console.WriteLine("发送起点 " + offset + " 大小: " + (int)(EzoneStream.Length - offset));
            EzoneStream.Read(data, 0, (int)(EzoneStream.Length - offset));
            //发送数据包
            TransferFiles.SendData(client, data, (int)(EzoneStream.Length - offset));

            //关闭文件流
            EzoneStream.Close();
            if (!isCut)
            {
                return true;
            }
            return false;
        }


        //static int MAX_PACK_SIZE = 10240;
        //public static bool SendFile(string fullPath)
        //{ 
        //    //发送方式是先发送大小，然后发送数据，然后发送大小为0，表示结束

        //    //TransferFiles.SendData(client, Encoding.Unicode.GetBytes("send positionfile"));

        //    //创建一个文件对象
        //    FileInfo EzoneFile = new FileInfo(fullPath);
        //    //打开文件流
        //    FileStream EzoneStream = EzoneFile.OpenRead();
        //    bool isCut = false;

        //    //得到长度
        //    long dataLength = EzoneStream.Length;

        //    while(dataLength - MAX_PACK_SIZE>0)
        //    {
        //        dataLength -= MAX_PACK_SIZE;
        //        //数据包
        //        byte[] data = new byte[MAX_PACK_SIZE];
        //        //从文件流读取数据并填充数据包
        //        EzoneStream.Read(data, 0, data.Length);
        //        //发送数据包
        //        TransferFiles.SendVarData(client, data);
        //    }
        //    if(dataLength>0)
        //    {
        //        //数据包
        //        byte[] data = new byte[dataLength];
        //        //从文件流读取数据并填充数据包
        //        EzoneStream.Read(data, 0, data.Length);
        //        //发送数据包
        //        TransferFiles.SendVarData(client, data);
        //    }
        //    //最后发个0，让那边知道接收结束
        //    byte[] dataZero = new byte[0] ;
        //    TransferFiles.SendVarData(client, dataZero);

        //    ////关闭套接字
        //    //client.Close();
        //    //关闭文件流
        //    EzoneStream.Close();
        //    if (!isCut)
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public static void SendLong(long jiange)
        {
            TransferFiles.SendData(client, getBytesFromLong(jiange, false));
        }
    }
    
}



//static string IP = "172.18.61.129";
//static int Port = 3477;
//public static void Start()
//{

//    Send();
//    SendCommond("positionfile end");
//}
//public static bool SendCommond(string commond)
//{
//    //指向远程服务端节点
//    IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IP), Port);
//    //创建套接字
//    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//    //连接到发送端
//    try
//    {
//        client.Connect(ipep);
//    }
//    catch
//    {
//        Console.WriteLine("连接服务器失败！");
//        return false;
//    }
//    TransferFiles.SendVarData(client, Encoding.Unicode.GetBytes(commond));
//    //关闭套接字
//    client.Close();
//    return true;
//}
//public static void Send()
//{
//    for (int i = 0; i < 100; i++)
//    {
//        if (File.Exists("output0_" + i))
//        {
//            try
//            {
//                SendFile("output0_" + i);
//            }
//            catch
//            {
//                Console.WriteLine("发送 output0_" + i + "失败");
//            }
//            Console.WriteLine("发送 output0_" + i);
//        }
//        else
//            break;
//    }

//}
//public static bool SendFile(string fullPath)
//{
//    //指向远程服务端节点
//    IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(IP), Port);
//    //创建套接字
//    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//    //连接到发送端
//    try
//    {
//        client.Connect(ipep);
//    }
//    catch
//    {
//        Console.WriteLine("连接服务器失败！");
//    }

//    TransferFiles.SendVarData(client, Encoding.Unicode.GetBytes("send positionfile"));

//    //创建一个文件对象
//    FileInfo EzoneFile = new FileInfo(fullPath);
//    //打开文件流
//    FileStream EzoneStream = EzoneFile.OpenRead();

//    //发送[文件名]到客户端
//    TransferFiles.SendVarData(client, System.Text.Encoding.Unicode.GetBytes(EzoneFile.Name));

//    bool isCut = false;
//    //数据包
//    byte[] data = new byte[EzoneStream.Length];

//    //从文件流读取数据并填充数据包
//    EzoneStream.Read(data, 0, data.Length);
//    //发送数据包
//    if (TransferFiles.SendVarData(client, data) == 3)
//    {
//        isCut = true;
//        return false;
//    }
//    //关闭套接字
//    client.Close();
//    //关闭文件流
//    EzoneStream.Close();
//    if (!isCut)
//    {
//        return true;
//    }
//    return false;
//}

//public static void Receive(object clientSocket)
//{
//    Socket client = clientSocket as Socket;
//    ////获得客户端节点对象
//    //IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;

//    //获得指令
//    string commands = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));

//    switch (commands)
//    {
//        case "send positionfile":
//            Console.WriteLine("接收文件");
//            ReceiveFile(client);
//            break;
//        case "test":
//            Console.WriteLine("ceshi!!!");
//            break;
//        case "positionfile end":
//            //接收完毕，合并文件
//            Console.WriteLine("接收完毕");
//            List<string> filePaths = new List<string>();
//            for (int i = 0; i < 100; i++)//最多一百个文件
//            {
//                if (File.Exists("output0_" + i))
//                {
//                    filePaths.Add("output0_" + i);
//                }
//                else
//                    break;
//            }
//            if (filePaths.Count > 0) CombineFiles(filePaths, "output0");
//            else Console.WriteLine("没有文件");
//            break;
//    }

//}
////已发送包的个数   
//static int SendedCount = 0;
//public static void ReceiveFile(object clientSocket)
//{
//    Socket client = clientSocket as Socket;
//    //获得[文件名]   
//    string SendFileName = System.Text.Encoding.Unicode.GetString(TransferFiles.ReceiveVarData(client));
//    Console.WriteLine("开始接收：" + SendFileName);
//    string fullPath = Path.Combine(Environment.CurrentDirectory, SendFileName);
//    //创建一个新文件   
//    FileStream MyFileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);




//    byte[] data = TransferFiles.ReceiveVarData(client);

//    SendedCount++;
//    //将接收到的数据包写入到文件流对象   
//    MyFileStream.Write(data, 0, data.Length);
//    //显示已发送包的个数     
//    Console.WriteLine("已经接收：" + SendedCount);



//    //关闭文件流   
//    MyFileStream.Close();
//    //关闭套接字 
//    client.Close();

//    //SocketServer.pForm.ShowMessageBox(SendFileName + "接收完毕！");
//    Console.WriteLine("文件接收成功");
//}

////这个文件结合文件会把所有文件放入内存，因此内存太小的电脑可能会有问题
//static void CombineFiles(List<string> filePaths, string combineFile)
//{
//    using (FileStream CombineStream = new FileStream(combineFile, FileMode.OpenOrCreate))
//    {
//        using (BinaryWriter CombineWriter = new BinaryWriter(CombineStream))
//        {
//            foreach (string file in filePaths)
//            {

//                using (FileStream fileStream = new FileStream(file, FileMode.Open))
//                {
//                    using (BinaryReader fileReader = new BinaryReader(fileStream))
//                    {
//                        byte[] TempBytes = fileReader.ReadBytes((int)fileStream.Length);
//                        CombineWriter.Write(TempBytes);
//                    }
//                }
//            }
//        }
//    }
//}