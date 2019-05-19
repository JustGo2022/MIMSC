using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MISMC.Model;
using MISMC.ViewModel;
using MISMC.Windows;
using Newtonsoft.Json.Linq;

namespace SocketAsyncEventArgsOfficeDemo
{
    class MessageDeal
    {

        public enum messageType
        {
            landMessage = 1,
            registMessage = 2,
            UserInfo = 3,
            FriendInfo = 4,
            friendrequestMessage = 10,
            UserMessage = 5,
            AddFriendRetMessage = 8,
            ChangeGroupMessage = 13,
            DeleteFriendMessage = 15
        }

        public static void ReceiveDeal(SocketAsyncEventArgs e)
        {
            Console.WriteLine("开始粘包处理");
            //粘包处理后，就可以分类处理
            lock (e)
            {
                if (StickyDeal(e))
                {
                    Console.WriteLine("数据长度为 : " + ((AsyncUserToken)e.UserToken).packageLen);
                    //如果粘包处理成功，那么就可以开始分类处理
                    Console.WriteLine("开始分类处理");
                    ClassifyDeal(e);
                }
            }
            
        }

        //粘包处理
        public static bool StickyDeal(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;

            //复制数据
            //e.Buffer中的数据要等到下一次调用异步接收时才会自动清除
            //所以一次异步接收后，只用复制一次数据就行，这里要想办法规避掉第二次复制
            if (!token.isCopy)
            {
                byte[] data = new byte[e.BytesTransferred];
                Array.Copy(e.Buffer, e.Offset, data, 0, e.BytesTransferred);
                Console.WriteLine("data的大小 : " + data.Count());
                //将数据放入receiveBuffer
                //receiveBuffer是list<byte>类型的
                lock (token.receiveBuffer)
                {
                    token.receiveBuffer.AddRange(data);
                }
                token.isCopy = true;
            }

            //粘包处理

            //接收到的数据长度还不足以分析包的类型和长度，跳出，让程序继续接收          TODO  break->return?
            if (token.receiveBuffer.Count < 8)
            {
                return false;
            }
            //如果packageLen长度为0，就得到包长
            else if (token.packageLen == 0)
            {
                //得到包的类型
                byte[] typeByte = token.receiveBuffer.GetRange(0, 4).ToArray();
                token.packageType = BitConverter.ToInt32(typeByte, 0);
                //得到包的长度
                byte[] lenBytes = token.receiveBuffer.GetRange(4, 8).ToArray();
                token.packageLen = BitConverter.ToInt32(lenBytes, 0);
            }
            //接收到的数据长度不够，跳出，让程序继续接收     TODO    先分析包的类型再决定如何做？
            if (token.receiveBuffer.Count() < token.packageLen)
            {
                return false;
            }
            //接收到的数据包最少有一个完整的数据，可以交给下面的函数处理
            else
            {
                return true;
            } 
        }

        //对完整的数据进行分类处理
        public static void ClassifyDeal(SocketAsyncEventArgs e)
        {
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            messageType type = (messageType)token.packageType;

            //根据数据类型处理数据
            switch (type)
            {
                case messageType.landMessage:
                    Console.WriteLine("登陆返回信息处理");
                    LandMessDeal(e);
                    break;

                case messageType.registMessage:
                    Console.WriteLine("注册返回信息处理");
                    RegisMessDeal(e);
                    break;

                case messageType.UserInfo:
                    Console.WriteLine("返回的该用户信息处理");
                    UserInfoDeal(e);
                    break;

                case messageType.FriendInfo:
                    Console.WriteLine("返回的好友信息处理");
                    FriendInfoDeal(e);
                    break;

                case messageType.UserMessage:
                    Console.WriteLine("返回的用户消息处理");
                    UserMessageDeal(e);
                    break;

                case messageType.friendrequestMessage:
                    Console.WriteLine("返回的用户请求处理");
                    FriendRequestMessageDeal(e);
                    break;

                case messageType.AddFriendRetMessage:
                    Console.WriteLine("返回的模糊搜索到的用户信息处理");
                    AddFriendRetMessage(e);
                    break;

                case messageType.ChangeGroupMessage:
                    Console.WriteLine("返回的模糊搜索到的用户信息处理");
                    ChangeGroupMessage(e);
                    break;

                case messageType.DeleteFriendMessage:
                    Console.WriteLine("返回的删除好友信息处理");
                    DeleteFriendMessage(e);
                    break;

                default:
                    //可能的处理
                    break;
            }
            //将这两个标志归零
            token.packageLen = 0;
            token.packageType = 0;
        }

        //登陆数据处理函数
        public static void LandMessDeal(SocketAsyncEventArgs e)
        {
            MClient mClient = MClient.CreateInstance();
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //得到一个完整的包的数据，放入新list,第二个参数是数据长度，所以要减去8  
            List<byte> onePackage = token.receiveBuffer.GetRange(8, token.packageLen - 8);
            //将复制出来的数据从receiveBuffer旧list中删除
            token.receiveBuffer.RemoveRange(0, token.packageLen);
            //list要先转换成数组，再转换成字符串
            String jsonStr = Encoding.Default.GetString(onePackage.ToArray());
            //得到用户名和密码
            JObject obj = JObject.Parse(jsonStr);
            if (obj["isLand"].ToString().Equals("True"))
            {
                //MessageBox.Show("登陆成功");               
            }
            else
            {
                MessageBox.Show("登陆失败");
                MClientViewModel mClientViewModel = MClientViewModel.CreateInstance();
                mClientViewModel.PassWord = "";
            }


        }

        public static void RegisMessDeal(SocketAsyncEventArgs e)
        {
            MClient mClient = MClient.CreateInstance();
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //得到一个完整的包的数据，放入新list,第二个参数是数据长度，所以要减去8  
            List<byte> onePackage = token.receiveBuffer.GetRange(8, token.packageLen - 8);
            //将复制出来的数据从receiveBuffer旧list中删除
            token.receiveBuffer.RemoveRange(0, token.packageLen);
            //list要先转换成数组，再转换成字符串
            String jsonStr = Encoding.Default.GetString(onePackage.ToArray());
            //得到用户名和密码
            JObject obj = JObject.Parse(jsonStr);
            if (obj["isRegist"].ToString().Equals("True"))
            {
                MessageBox.Show("注册成功");
                //关闭注册窗口
                MClientViewModel mClientViewModel = MClientViewModel.CreateInstance();
                RegisterViewModel registerViewModel = RegisterViewModel.CreateInstance();
                //重置输入框
                registerViewModel.Resset();
                //跨线程调用窗体组件的方法，使注册窗口关闭
                mClientViewModel.registerWindow.Dispatcher.Invoke(new Action(() => {
                    mClientViewModel.registerWindow.Close();
                }));

            }
            else
            {
                MessageBox.Show("注册失败");
                //清除掉密码
                RegisterViewModel registerViewModel = RegisterViewModel.CreateInstance();
                registerViewModel.PassWord = "";
                registerViewModel.sPassWord = "";
            }

        }

        public static void UserInfoDeal(SocketAsyncEventArgs e)
        {
            MClient mClient = MClient.CreateInstance();
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //得到一个完整的包的数据，放入新list,第二个参数是数据长度，所以要减去8  
            List<byte> onePackage = token.receiveBuffer.GetRange(8, token.packageLen - 8);
            //将复制出来的数据从receiveBuffer旧list中删除
            token.receiveBuffer.RemoveRange(0, token.packageLen);
            Console.WriteLine("清除receiveBuffer中的数据 , token.packageLen = " + token.packageLen);
            //list要先转换成数组，再转换成字符串
            String jsonStr = Encoding.Default.GetString(onePackage.ToArray());
            //得到用户名和密码
            JObject obj = JObject.Parse(jsonStr);

            //如果传回来的用户信息是正确的
            if (obj["isOk"].ToString().Equals("True"))
            {
                Console.WriteLine("保存自己的信息");
                //先初始化数据库的静态变量(这里是用id合成数据库名)
                SqliteConnect.SqliteInit(obj["id"].ToString());
                //然后创建好友表和信息表(如果有，不会重复创建)
                SqliteConnect.CreateTable();
                //保存自己的最新信息到数据库
                SqliteConnect.SaveUserInfo(obj["id"].ToString(), obj["UserName"].ToString(), obj["RealName"].ToString(), obj["Sex"].ToString(),
                                            obj["BirthDay"].ToString(), obj["Address"].ToString(), obj["Email"].ToString(), obj["PhoneNumber"].ToString(),
                                            obj["Remark"].ToString());

                //主窗口隐藏
                Application.Current.Dispatcher.Invoke(new Action(() => { Application.Current.MainWindow.Hide(); }));
                //打开好友界面
                Application.Current.Dispatcher.Invoke(new Action(() => {
                    FriendListWindow friedListWindow = new FriendListWindow();
                    friedListWindow.Show();
                }));
            }
            else
            {
                MessageBox.Show("用户信息返回失败");
            }
        }

        public static void FriendInfoDeal(SocketAsyncEventArgs e)
        {
            MClient mClient = MClient.CreateInstance();
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //得到一个完整的包的数据，放入新list,第二个参数是数据长度，所以要减去8  
            List<byte> onePackage = token.receiveBuffer.GetRange(8, token.packageLen - 8);
            //将复制出来的数据从receiveBuffer旧list中删除
            token.receiveBuffer.RemoveRange(0, token.packageLen);
            //list要先转换成数组，再转换成字符串
            String jsonStr = Encoding.Default.GetString(onePackage.ToArray());
            //得到用户名和密码
            JArray jArray = JArray.Parse(jsonStr);


            if (jArray[0]["isOk"].ToString().Equals("True"))
            {
     
                Console.WriteLine("保存好友信息");

                //保存还没有更新前的时间，如果时间比这个时间还晚，说明是已经被删除的好友
                String updatetime = DateTime.Now.ToString();

                foreach (var obj in jArray)
                {
                   
                    SqliteConnect.SaveFriendInfo(obj["id"].ToString(), obj["Group"].ToString(), obj["UserName"].ToString(), obj["RealName"].ToString(), obj["Sex"].ToString(),
                                            obj["BirthDay"].ToString(), obj["Address"].ToString(), obj["Email"].ToString(), obj["PhoneNumber"].ToString(),
                                            obj["Remarks"].ToString());
                }

                //在保存完好友信息后，就要根据更新时间对数据库表中的数据进行排查，删除掉这一次还没有更新的数据
                SqliteConnect.DeleteFriendByTime(updatetime);
            }
            else
            {
                //如果不进行这个补充，那么在只有一个好友，且服务端已经删除这个好友的情况下，客户端本地的该好友不会被删除
                String updatetime = DateTime.Now.ToString();
                SqliteConnect.DeleteFriendByTime(updatetime);
            }
        }

        public static void ChangeGroupMessage(SocketAsyncEventArgs e)
        {
            MClient mClient = MClient.CreateInstance();
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //得到一个完整的包的数据，放入新list,第二个参数是数据长度，所以要减去8  
            List<byte> onePackage = token.receiveBuffer.GetRange(8, token.packageLen - 8);
            //将复制出来的数据从receiveBuffer旧list中删除
            token.receiveBuffer.RemoveRange(0, token.packageLen);
            //list要先转换成数组，再转换成字符串
            String jsonStr = Encoding.Default.GetString(onePackage.ToArray());
            //得到用户名和密码
            JArray jArray = JArray.Parse(jsonStr);

            //先保存好友信息
            if (jArray[0]["isOk"].ToString().Equals("True"))
            {
                Console.WriteLine("保存好友信息");

                foreach (var obj in jArray)
                {
                    SqliteConnect.SaveFriendInfo(obj["id"].ToString(), obj["Group"].ToString(), obj["UserName"].ToString(), obj["RealName"].ToString(), obj["Sex"].ToString(),
                                            obj["BirthDay"].ToString(), obj["Address"].ToString(), obj["Email"].ToString(), obj["PhoneNumber"].ToString(),
                                            obj["Remarks"].ToString());
                }
            }
            //然后删除原分组List中的好友
            Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    FriendListViewModel friendListViewModel = FriendListViewModel.CreateInstance();
                    FriendEntity.InGroupListDelete(friendListViewModel.friendGroups, jArray[0]["id"].ToString());
                })
                );
        }

        public static void UserMessageDeal(SocketAsyncEventArgs e)
        {
            MClient mClient = MClient.CreateInstance();
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //得到一个完整的包的数据，放入新list,第二个参数是数据长度，所以要减去8  
            List<byte> onePackage = token.receiveBuffer.GetRange(8, token.packageLen - 8);
            //将复制出来的数据从receiveBuffer旧list中删除
            token.receiveBuffer.RemoveRange(0, token.packageLen);
            //list要先转换成数组，再转换成字符串
            String jsonStr = Encoding.Default.GetString(onePackage.ToArray());
            //得到用户名和密码
            Console.WriteLine("jsonStr = " + jsonStr);
            JArray jArray = JArray.Parse(jsonStr);

            if (jArray[0]["isOk"].ToString().Equals("True"))
            {
                Console.WriteLine("保存用户消息");

                foreach (var obj in jArray)
                {
                    SqliteConnect.SaveMessage(obj["FriendId"].ToString(), obj["Message"].ToString(), obj["MessageDate"].ToString());
                }
            }
        }

        //处理好友请求消息
        public static void FriendRequestMessageDeal(SocketAsyncEventArgs e)
        {
            MClient mClient = MClient.CreateInstance();
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //得到一个完整的包的数据，放入新list,第二个参数是数据长度，所以要减去8  
            List<byte> onePackage = token.receiveBuffer.GetRange(8, token.packageLen - 8);
            //将复制出来的数据从receiveBuffer旧list中删除
            token.receiveBuffer.RemoveRange(0, token.packageLen);
            //list要先转换成数组，再转换成字符串
            String jsonStr = Encoding.Default.GetString(onePackage.ToArray());
            //得到用户名和密码
            JArray jArray = JArray.Parse(jsonStr);

            if (jArray[0]["isOk"].ToString().Equals("True"))
            {
                Console.WriteLine("保存好友信息");

                foreach (var obj in jArray)
                {
                    SqliteConnect.SaveFriendRequestInfo(obj["id"].ToString(), obj["UserName"].ToString(), obj["RealName"].ToString(), obj["Sex"].ToString(),
                                            obj["BirthDay"].ToString(), obj["Address"].ToString(), obj["Email"].ToString(), obj["PhoneNumber"].ToString(),
                                            obj["Remarks"].ToString());
                }
            }
        }

        public static void AddFriendRetMessage(SocketAsyncEventArgs e)
        {
            MClient mClient = MClient.CreateInstance();
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //得到一个完整的包的数据，放入新list,第二个参数是数据长度，所以要减去8  
            List<byte> onePackage = token.receiveBuffer.GetRange(8, token.packageLen - 8);
            //将复制出来的数据从receiveBuffer旧list中删除
            token.receiveBuffer.RemoveRange(0, token.packageLen);
            //list要先转换成数组，再转换成字符串
            String jsonStr = Encoding.Default.GetString(onePackage.ToArray());
            //得到用户名和密码
            Console.WriteLine("jsonStr = " + jsonStr);
            JArray jArray = JArray.Parse(jsonStr);

            if (jArray[0]["isOk"].ToString().Equals("True"))
            {
                AddFriendViewModel addFriendViewModel = AddFriendViewModel.CreateInstance();
                Console.WriteLine("显示模糊搜索到的用户信息");
                FriendListViewModel friendListViewModel = FriendListViewModel.CreateInstance();
                MClientViewModel mClientViewModel = MClientViewModel.CreateInstance();
                foreach (var obj in jArray)
                {                   
                    //如果这个用户已经是好友了，那么就继续看下一个
                    if (friendListViewModel.isFriendList(obj["Id"].ToString()))
                    {
                        continue;
                    }
                    //把自己的id也排除出去
                    if (mClientViewModel.UserName.Equals(obj["UserName"].ToString()))
                    {
                        continue;
                    }
                    FriendInfo friendInfo = new FriendInfo();
                    friendInfo.Id = obj["Id"].ToString();
                    friendInfo.FriendName = obj["UserName"].ToString();
                    //把搜索到的好友数据添加进去
                    Application.Current.Dispatcher.Invoke(
                    new Action(() =>
                    {
                        //更新查询到的好友信息
                        addFriendViewModel.FrienInfoGroup.Add(friendInfo);                       
                    })
                    );
                }
            }
            else
            {
                MessageBox.Show("没有查询到接近的用户");
            }
        }

        public static void DeleteFriendMessage(SocketAsyncEventArgs e)
        {
            MClient mClient = MClient.CreateInstance();
            AsyncUserToken token = (AsyncUserToken)e.UserToken;
            //得到一个完整的包的数据，放入新list,第二个参数是数据长度，所以要减去8  
            List<byte> onePackage = token.receiveBuffer.GetRange(8, token.packageLen - 8);
            //将复制出来的数据从receiveBuffer旧list中删除
            token.receiveBuffer.RemoveRange(0, token.packageLen);
            //list要先转换成数组，再转换成字符串
            String jsonStr = Encoding.Default.GetString(onePackage.ToArray());
            //得到用户名和密码
            Console.WriteLine("jsonStr = " + jsonStr);
            JObject obj = JObject.Parse(jsonStr);

            //先从数据库中删除
            SqliteConnect.DeleteFriendById(obj["Id"].ToString());
            //然后从好友队列中删除
            Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    FriendListViewModel friendListViewModel = FriendListViewModel.CreateInstance();
                    FriendEntity.InGroupListDelete(friendListViewModel.friendGroups, obj["Id"].ToString());
                })
                );

        }
        
    }
}
