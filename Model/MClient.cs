using MISMC.Model;
using MyMVVM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace SocketAsyncEventArgsOfficeDemo
{
    class MClient: NotifyObject
    {
        //客户端需要的成员变量
        Socket connSocket;                //监听Socket
        private String ip;
        private String port;
        public SocketAsyncEventArgs readSAEA;
        public SocketAsyncEventArgs sendSAEA;
        byte[] receiveBuffer;
        byte[] sendBuffer;

        private static MClient mClient = null;
        public static MClient CreateInstance(String ip, String port)
        {
            if (mClient == null)
            {
                Console.WriteLine("mClient创建开始");
                mClient = new MClient(ip, port);
                Console.WriteLine("mClient创建完毕");
            }
            return mClient;
        }

        public static MClient CreateInstance()
        {
            return mClient;
        }


        //创建一个未初始化的服务器实例
        //开始监听
        //先调用Init方法，然后调用Start方法
        //
        //<param name = "numConnections">同时处理的最大连接数</param>
        //<param name = "receiveBufferSize">用于每个套接字操作的缓存区大小</param>
        private MClient(String ip, String port)
        {
            //初始化ip和port
            this.ip = ip;
            this.port = port;
            //实列化两个SAEA，分别用于接收和发送
            this.readSAEA = new SocketAsyncEventArgs();
            this.sendSAEA = new SocketAsyncEventArgs();

            //TODO 分配缓存区
            

            //绑定完成事件
            this.readSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            this.sendSAEA.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
            //分配信息保存空间
            AsyncUserToken userToken = new AsyncUserToken();
            this.readSAEA.UserToken = userToken;
            this.sendSAEA.UserToken = userToken;

            Console.WriteLine("MClient构造函数执行完毕");
        }

        //连接目标ip地址
        public void ConnectServer()
        {
            //Ip4地址，流方式，Tcp协议
            connSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
            //连接服务端
            connSocket.Connect(endPoint);

            receiveBuffer = new byte[1024];
            //receiveBuffer = new byte[2048];
            sendBuffer = new byte[1024];
            
            //因为userToken是同一个,所以设置一个就行
            ((AsyncUserToken)readSAEA.UserToken).Socket = connSocket;
            ((AsyncUserToken)readSAEA.UserToken).Remote = endPoint;

            readSAEA.SetBuffer(receiveBuffer, 0, 1014);
 
            readSAEA.RemoteEndPoint = endPoint;
            sendSAEA.RemoteEndPoint = endPoint;

            Console.WriteLine("已经连接");

            //然后直接开启接收的循环
            bool willRaiseEvent = connSocket.ReceiveAsync(readSAEA);
            if (!willRaiseEvent)
            {
                ProcessReceive(readSAEA);
            }
        }

        //什么都还没有写
        public void Stop()
        {
  
        }

        //当一个接收或者发送操作完成时调用此函数
        //
        //<param name = "e"> SocketAsyncEventArg </param>

        void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            //确定刚刚完成的操作类型并调用关联的处理程序
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    Console.WriteLine("调用了一次回调函数 RECV");
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    Console.WriteLine("调用了一次回调函数 SEND");
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("Last Operation error");
            }
        }

        //异步接收操作完成时调用此方法
        //如果远程主机关闭了连接，那么就关闭套接字
        //接收到了数据，将数据返回给客户端
        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            try
            {
                //检查这个远程主机是否关闭连接
               
                if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
                {
                    //使用MessageDeal类处理数据
                    //TODO  在有大量数据包发送过来时，因为忙着处理数据库，所以有数据没有接收到
                    //这里当数据大于8且不知道长度时，说明还有数据可以分析，那么就继续分析
                    //如果剩下的包足够长，那么数据就会被处理，不够长，那么下一次剩余数据到达，必然会调用一次这个函数，也能处理    
                    do
                    {
                        MessageDeal.ReceiveDeal(e);
                    }
                    while (token.receiveBuffer.Count > 8 && token.packageLen == 0);

                    //Thread thread = new Thread(()=>MessageDeal.ReceiveDeal(e));
                    //thread.Start();

                    //这里每一次接收到数据后，就会调用发送函数的回调函数
                    //那么后面服务端自己主动发送的时候，就需要自己主动调用了
                    if (token.sendPacketNum.Count() > 0)
                    {
                        //调用发送函数的回调函数
                        ProcessSend(sendSAEA);
                    }

                    //接收完继续接收
                    Console.WriteLine("开始异步接收");
                    token.isCopy = false;
                    bool willRaiseEvent = token.Socket.ReceiveAsync(e);
                    if (!willRaiseEvent)
                    {
                        ProcessReceive(e);
                    }
                }
                else
                {
                    CloseClientSocket();
                }
            }
            catch (Exception xe)
            {
                Console.WriteLine(xe.Message + "\r\n" + xe.StackTrace);
            }
        }

        //用来打包要发送的数据的函数，只需要传入数据类型，保存数据的字符串和发送数据所用的SAEA就行
        public void SendMessage(int packageType, String str, SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //这里是要获得字节数而不是元素数
            int packageLen = System.Text.Encoding.Default.GetByteCount(str) + 8;
            Console.WriteLine("包的大小为" + packageLen);
            byte[] bType = System.BitConverter.GetBytes(packageType);
            byte[] bLen = System.BitConverter.GetBytes(packageLen);
            //将数据放入发送buffer
            token.sendBuffer.AddRange(bType);
            token.sendBuffer.AddRange(bLen);
            token.sendBuffer.AddRange(System.Text.Encoding.Default.GetBytes(str));
            //接下来可以调用发送函数的回调函数了
            //下一次要发送多少数据
            token.sendPacketNum.Add(packageLen);
            Console.WriteLine("数据装载完毕");
            ProcessSend(sendSAEA);
        }

        //异步发送操作完成时调用此方法
        //该方法在套接字上发出另一个接收以读取任何其他接收  ??
        //从客户端发送的数据
        //
        //<param name = "e"></param>
        private void ProcessSend(SocketAsyncEventArgs e)
        {
            //服务端的sendSAEA的Iocomplete没有绑定完成事件，所以服务端SendAsync后不会重复调用ProcessSend
            //而客户端的绑定了，如果不加下面的跳出函数，会一直循环。
            //两种方法也说不上谁好，所以客户端先采用与服务端不同的方式，也许后面可以用于发送很大的数据
            if (((AsyncUserToken)e.UserToken).sendBuffer.Count == 0)
            {
                return;
            }

            Console.WriteLine(e.SocketError);

            if (e.SocketError == SocketError.Success)
            {
                //用来将sendSAEA.UserToken中已经准备好的数据发送出去
                AsyncUserToken token = (AsyncUserToken)e.UserToken;
                byte[] data;
                int count = token.sendBuffer.Count;

                //TODO 不能就这么莽撞地取出和发送所有数据，或许应该添加一个int类型的list，将下一次发送多少数据存入里面

                //从sendBuffer中取出数据
                data = token.sendBuffer.ToArray();
                token.sendBuffer.Clear();

                e.SetBuffer(data, 0, data.Length);

                Console.WriteLine("开始异步发送 datasize:" + data.Count() + "data:" + System.Text.Encoding.UTF8.GetString(data));
                bool willRaiseEvent = token.Socket.SendAsync(e);
                if (!willRaiseEvent)
                {
                    Console.WriteLine("调用了这个吗?");
                    ProcessSend(e);
                }
                Console.WriteLine("异步发送完毕");
            }
            else
            {
                CloseClientSocket();
            }
        }

        private void CloseClientSocket()
        {
            //关闭套接字
            try
            {
                connSocket.Shutdown(SocketShutdown.Send);
            }
            //throws if client process has already closed
            catch (Exception) { }
            connSocket.Close();
        }

        public void SendLogin(String userName, String passWord)
        {
            //构造json字符串
            JObject json = new JObject();
            json["UserName"] = userName;
            json["PassWord"] = passWord;
            String str = json.ToString();


            //设置消息类型和消息数据,使用发送函数发送
            this.SendMessage(1, str, sendSAEA);
        }

        public void SendRegister(String userName, String passWord, String realName, String Sex, String birthDay, String Address, String Email, String phoneNumber, String remark)
        {
            //构造json字符串
            JObject json = new JObject();
            json["UserName"] = userName;
            json["PassWord"] = passWord;
            json["RealName"] = realName;
            json["Sex"] = Sex;
            json["BirthDay"] = birthDay;
            json["Address"] = Address;
            json["Email"] = Email;
            json["PhoneNumber"] = phoneNumber;
            json["Remark"] = remark;
            String str = json.ToString();

            //设置消息类型和消息数据,使用发送函数发送
            this.SendMessage(2, str, sendSAEA);
        }
    }
}
