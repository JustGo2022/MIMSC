using MISMC.Model;
using MISMC.Windows;
using MyMVVM;
using Newtonsoft.Json.Linq;
using SocketAsyncEventArgsOfficeDemo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MISMC.ViewModel
{
    //结构，ViewModel持有一个分组的ObservableCollection<>(类似list<>)，然后每个分组持有一个好友的ObservableCollection<>(类似list<>)

    class FriendListViewModel : NotifyObject
    {
        public FriendListViewModel()
        {
            //查询数据库，拿到当前用户的所有信息，并保存在这个类里面
            SqliteConnect.QueryUserInfo(out FriendListViewModel.id, out FriendListViewModel.username, out FriendListViewModel.realname, out FriendListViewModel.sex, out FriendListViewModel.birthday, out FriendListViewModel.address, out FriendListViewModel.email, out FriendListViewModel.phonenumber, out FriendListViewModel.remark);
            //分组列表
            friendGroups = new ObservableCollection<FriendGroup>();
            friendRequestList = new ObservableCollection<FriendEntity>();
            //启动一个线程，这个函数负责对好友列表界面进行维护
            scanThread = new Thread(FriendListUpdata);
            scanThread.Start();       
        }

        //RegisterViewModel的单例函数
        private static FriendListViewModel friendListViewModel = null;
        public static FriendListViewModel CreateInstance()
        {
            if (friendListViewModel == null)
            {
                friendListViewModel = new FriendListViewModel();
            }
            return friendListViewModel;
        }

        //扫描线程
        Thread scanThread;

        public void FriendListUpdata()
        {
            //进行数据库查询,从好友信息表中获得好友的信息
            //这个查询函数最好可以直接对好友分组列表进行操作
            //即可以传入好友分组列表对象
            //将while写进invoke里面会造成界面卡死，推测Application.Current.Dispatcher.Invoke这个方法会占用界面线程
            //所以要把while写在外面，并设置阻塞，每5秒访问一次Application.Current.Dispatcher.Invoke方法更新好友列表
            //TODO  其实还是可以先用数据库读取完，得到结果再使用Application.Current.Dispatcher.Invoke方法，因为数据库操作还是很占时间的
            while (true)
            {
               // MessageBox.Show("这里就查询了一次");
                Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    //更新好友列表信息
                    SqliteConnect.QueryFriendInfo(ref _friends);
                    //更新好友请求信息
                    SqliteConnect.QueryFriendRequestInfo(ref friendRequestList);
                })
                );

                Thread.Sleep(5000);
            }         
        }

        //ViewModel类的一个成员对象，是一个分组列表
        private ObservableCollection<FriendGroup> _friends;
        public ObservableCollection<FriendGroup> friendGroups
        {
            get
            {
                return _friends;
            }
            set
            {
                _friends = value;
                RaisePropertyChanged("FriendGroup");
            }
        }

        //这个是好友请求列表
        private ObservableCollection<FriendEntity> friendRequestList;
        public ObservableCollection<FriendEntity> FriendRequestList
        {
            get
            {
                return friendRequestList;
            }
            set
            {
                friendRequestList = value;
                RaisePropertyChanged("FriendRequestList");
            }
        }


        public static String id;
        public String Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        public static String username;
        public String UserName
        {
            get { return username; }
            set
            {
                if (username != value)
                {
                    username = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }


        public static String realname;
        public String RealName
        {
            get { return realname; }
            set
            {
                if (realname != value)
                {
                    realname = value;
                    RaisePropertyChanged("Realname");
                }
            }
        }

        public static String sex;
        public String Sex
        {
            get { return sex; }
            set
            {
                if (sex != value)
                {
                    sex = value;
                    RaisePropertyChanged("Sex");
                }
            }
        }

        public static String birthday;
        public String BirthDay
        {
            get { return birthday; }
            set
            {
                if (birthday != value)
                {
                    birthday = value;
                    RaisePropertyChanged("BirthDay");
                }
            }
        }

        public static String address;
        public String Address
        {
            get { return address; }
            set
            {
                if (address != value)
                {
                    address = value;
                    RaisePropertyChanged("Address");
                }
            }
        }

        public static String email;
        public String Email
        {
            get { return email; }
            set
            {
                if (email != value)
                {
                    email = value;
                    RaisePropertyChanged("Email");
                }
            }
        }

        public static String phonenumber;
        public String PhoneNumber
        {
            get { return phonenumber; }
            set
            {
                if (phonenumber != value)
                {
                    phonenumber = value;
                    RaisePropertyChanged("PhoneNumber");
                }
            }
        }

        public static String remark;
        public String Remark
        {
            get { return remark; }
            set
            {
                if (remark != value)
                {
                    remark = value;
                    RaisePropertyChanged("Remark");
                }
            }
        }

        //看目标用户是否在此用户的好友列表中
        public bool isFriendList(String Id)
        {
            foreach (var group in friendGroups)
            {
                foreach (var friend in group.Friends)
                {
                    if (friend.Id.Equals(Id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //窗口Close事件
        private MyCommand friendListWindowClose;
        public MyCommand FriendListWindowClose
        {
            get
            {
                if (friendListWindowClose == null)
                    friendListWindowClose = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                //退出扫描线程
                                this.scanThread.Abort();
                                //直接干掉整个程序
                                System.Environment.Exit(0);
                            }));
                return friendListWindowClose;
            }
        }

        private MyCommand btAddFriend;
        public MyCommand BtAddFriend
        {
            get
            {
                if (btAddFriend == null)
                    btAddFriend = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                AddFriendWindow addFriendWindow = new AddFriendWindow();
                                addFriendWindow.Show();
                            }));
                return btAddFriend;
            }
        }
    }


    //朋友的信息类
    //TODO添加更多信息类型
    public class FriendEntity : NotifyObject
    {
        public String Id { get; set; }
        public String Name { get; set; }
        public String RealName { get; set; }
        public String Sex { get; set; }
        public String BirthDay { get; set; }
        public String Address { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public String Remarks { get; set; }


        //这里要加这个，要不然在ViewModel中更新的时候，不会返回给客户端
        public String status;
        public String Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    RaisePropertyChanged("Status");
                }
            }
        }

        public ChatWindow chatWindow { get; set; }
        public FriendInfoWindow friendInfoWindow { get; set; }

        //右键菜单
        private ContextMenu contextMenu { get; set; }

        public FriendEntity()
        {
            chatWindow = null;
            friendInfoWindow = null;
            Status = "0";

            //这里注册右键菜单
            contextMenu = new ContextMenu();

            MenuItem chatBt = new MenuItem();
            chatBt.Header = "聊天";
            chatBt.Click += OpenChat;
            contextMenu.Items.Add(chatBt);

            MenuItem InfoBt = new MenuItem();
            InfoBt.Header = "详细信息";
            InfoBt.Click += OpenInfo;
            contextMenu.Items.Add(InfoBt);

            MenuItem groupBt = new MenuItem();
            groupBt.Header = "更换分组";
            groupBt.Click += ChangeGroup;
            contextMenu.Items.Add(groupBt);

            MenuItem deleteBt = new MenuItem();
            deleteBt.Header = "删除好友";
            deleteBt.Click += DeleteFriend;
            contextMenu.Items.Add(deleteBt);
        }

        public static FriendEntity InGroup(ref ObservableCollection<FriendEntity> friendGroup, String name)
        {
            foreach (var entity in friendGroup)
            {
                if (entity.Name == name )
                {
                    return entity;
                }
            }
            return null;
        }

        public static FriendEntity InRequestAdd(ref ObservableCollection<FriendEntity> friendRequestGroup, String name)
        {
            foreach (var entity in friendRequestGroup)
            {
                if (entity.Name == name)
                {
                    return entity;
                }
            }
            FriendEntity friendentity = new FriendEntity();
            friendRequestGroup.Add(friendentity);
            return friendentity;
        }

        //好友在不在分组中，在，返回该好友，不在，添加新的好友，返回该好友
        public static FriendEntity InGroupAdd(ref FriendGroup friendGroup, String name)
        {
            foreach (var entity in friendGroup.Friends)
            {
                if (entity.Name == name)
                {
                    return entity;
                }
            }
            FriendEntity friendentity = new FriendEntity();
            friendGroup.FriendGroupAdd(ref friendentity);
            return friendentity;
        }

        //从整个好友列表中删除对应的对象
        public static void InGroupListDelete(ObservableCollection<FriendGroup> friendGroups, String id)
        {
            FriendGroup friendGroup = null;
            FriendEntity friendEntity =null;
            foreach (var group in friendGroups)
            {
                foreach(var entity in group.Friends)
                {
                    if (entity.Id == id)
                    {
                        friendGroup = group;
                        friendEntity = entity;
                        break;
                    }
                }
            }
            //删除对应的对象
            friendGroup.Friends.Remove(friendEntity);
            if (friendGroup.Friends.Count == 0)
            {
                friendGroups.Remove(friendGroup);
            }
        }

        //更新好友状态
        //TODO这样子应该很影响性能，最好是放到数据库中，或者将这个改为字典
        public static void UpdateFriendStatus(ObservableCollection<FriendGroup> friendGroups, String id, String Group, String Status)
        {
            foreach(var friendgroup in friendGroups)
            {
                if (friendgroup.Name.Equals(Group))
                {
                    foreach(var friend in friendgroup.Friends)
                    {
                        if (friend.Id.Equals(id))
                        {
                            friend.Status = Status;
                            //MessageBox.Show(friend.Status);
                        }
                    }
                }
            }
        }

        //用于ContextMenu选项单机注册
        public void OpenInfo(object sender, RoutedEventArgs e)
        {
            //这样子写好别扭。。。，还要拿到窗口的ViewModel
            if (friendInfoWindow == null || friendInfoWindow.IsVisible == false)
            {
                friendInfoWindow = new FriendInfoWindow();
                FriendInfoViewModel friendInfoViewModel = (FriendInfoViewModel)friendInfoWindow.DataContext;
                friendInfoViewModel.FriendInfoSet(this.Name, this.RealName, this.Sex, this.BirthDay, this.Address, this.Email, this.PhoneNumber, this.Remarks);
                friendInfoWindow.Show();
            }
        }

        public void OpenInfo()
        {
            //这样子写好别扭。。。，还要拿到窗口的ViewModel
            if (friendInfoWindow == null || friendInfoWindow.IsVisible == false)
            {
                friendInfoWindow = new FriendInfoWindow();
                FriendInfoViewModel friendInfoViewModel = (FriendInfoViewModel)friendInfoWindow.DataContext;
                friendInfoViewModel.FriendInfoSet(this.Name, this.RealName, this.Sex, this.BirthDay, this.Address, this.Email, this.PhoneNumber, this.Remarks);
                friendInfoWindow.Show();
            }
        }

        public void OpenChat(object sender, RoutedEventArgs e)
        {
            if (chatWindow == null || chatWindow.IsVisible == false)
            {
                chatWindow = new ChatWindow();
                ChatViewModel chatViewModel = (ChatViewModel)chatWindow.DataContext;
                chatViewModel.ChatSet(FriendListViewModel.username, this.Id, this.Name, this.RealName, this.Sex, this.BirthDay, this.Address, this.Email, this.PhoneNumber, this.Remarks);
                chatWindow.Show();
            }     
        }

        public void OpenChat()
        {
            if (chatWindow == null || chatWindow.IsVisible == false)
            {
                chatWindow = new ChatWindow();
                ChatViewModel chatViewModel = (ChatViewModel)chatWindow.DataContext;
                chatViewModel.ChatSet(FriendListViewModel.username, this.Id, this.Name, this.RealName, this.Sex, this.BirthDay, this.Address, this.Email, this.PhoneNumber, this.Remarks);
                chatWindow.Show();
            }
        }

        //更换分组的函数
        public void ChangeGroup(object sender, RoutedEventArgs e)
        {
            //弹出一个分组改变窗口
            ChangeGroupWindow changeGroupWindow = new ChangeGroupWindow();
            //拿到viewmodel
            ChangGroupViewModel viewModel = (ChangGroupViewModel)changeGroupWindow.DataContext;
            //拿到好友的id
            viewModel.FriendId = this.Id;
            changeGroupWindow.ShowDialog();
        }

        //删除好友
        public void DeleteFriend(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("删除该好友？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (MessageBoxResult.OK == result)
            {
                //进行删除好友的操作
                //将删除好友的信息发送给服务端，由服务端同时删除用户和好友的好友表中的对应ID
                JObject obj = new JObject();
                obj["Id"] = this.Id;
                String str = obj.ToString();
                MClient mClient = MClient.CreateInstance();
                mClient.SendDeleteFriend(str);
                //同时客户端本地将该好友从队列中删除
                SqliteConnect.DeleteFriendById(this.Id);
                //然后从好友队列中删除
                Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    FriendListViewModel friendListViewModel = FriendListViewModel.CreateInstance();
                    FriendEntity.InGroupListDelete(friendListViewModel.friendGroups, this.Id);
                })
                );
            }

        }

        //右键事件，打开选项菜单
        private MyCommand btOpenChoose;
        public MyCommand BtOpenChoose
        {
            get
            {
                if (btOpenChoose == null)
                    btOpenChoose = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                contextMenu.IsOpen = true;
                            }));
                return btOpenChoose;
            }
        }

        //双击事件
        private MyCommand btOpenChat;
        public MyCommand BtOpenChat
        {
            get
            {
                if (btOpenChat == null)
                    btOpenChat = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                this.OpenChat();
                            }));
                return btOpenChat;
            }
        }

        //同意好友请求
        private MyCommand bAgreeFriendRequest;
        public MyCommand BtAgreeFriendRequest
        {
            get
            {
                if (bAgreeFriendRequest == null)
                    bAgreeFriendRequest = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                //发送同意消息
                                JObject obj = new JObject();
                                obj["Id"] = this.Id;
                                obj["isAgree"] = "True";
                                String str = obj.ToString();
                                MClient mClient = MClient.CreateInstance();
                                mClient.SendFriendRequestAgree(str);
                                //删除好友请求表中的内容
                                SqliteConnect.RemoveFriendQuest(this.Id);
                                //删除好友请求列表中的内容
                                FriendListViewModel friendListViewModel = FriendListViewModel.CreateInstance();
                                foreach (var friend in friendListViewModel.FriendRequestList)
                                {
                                    if (friend.Id == this.Id)
                                    {
                                        friendListViewModel.FriendRequestList.Remove(friend);
                                        break;
                                    }
                                }
                                //服务端删除好友请求表中的内容
                                //然后在双方好友表中添加对方Id，分组为新好友
                            }));
                return bAgreeFriendRequest;
            }
        }

        //拒绝好友请求
        private MyCommand bDeagreeFriendRequest;
        public MyCommand BtDeagreeFriendRequest
        {
            get
            {
                if (bDeagreeFriendRequest == null)
                    bDeagreeFriendRequest = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                //发送拒绝消息
                                JObject obj = new JObject();
                                obj["Id"] = this.Id;
                                obj["isAgree"] = "False";
                                String str = obj.ToString();
                                MClient mClient = MClient.CreateInstance();
                                mClient.SendFriendRequestAgree(str);
                                //删除好友请求表中的内容
                                SqliteConnect.RemoveFriendQuest(this.Id);
                                //删除好友请求列表中的内容
                                FriendListViewModel friendListViewModel = FriendListViewModel.CreateInstance();
                                FriendEntity friendTemp = null;
                                foreach (var friend in friendListViewModel.FriendRequestList)
                                {
                                    if (friend.Id == this.Id)
                                    {
                                        friendTemp = friend;
                                    }
                                }
                                friendListViewModel.FriendRequestList.Remove(friendTemp);
                                //服务端删除好友列表中的内容
                            }));
                return bDeagreeFriendRequest;
            }
        }

    }



    //分组类
    //添加更多的方法(添加，删除，修改 朋友列表中的元素)
    public class FriendGroup : NotifyObject
    {


        //分组的名称
        private String _name;
        public String Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                RaisePropertyChanged("Name");
            }
        }

        

        //分组类的初始化函数，可以自建一个添加函数，向该对象中添加好友对象
        public FriendGroup()
        {
            Friends = new ObservableCollection<FriendEntity>();
        }

        //分组对象添加函数
        public void  FriendGroupAdd(ref FriendEntity friendEntities)
        {
            Friends.Add(friendEntities);
        }

        //检查指定名字的分组是否在分组列表中,如果在分组List中，返回该分组，如果不在，返回Null
        public static  FriendGroup InGroupList(ref ObservableCollection<FriendGroup> grouplist,String name)
        {
            foreach(var group in grouplist)
            {
                if (group.Name == name)
                {
                    return  group;
                }
            }
            return null;
        }

        //检查指定名字的分组是否在分组列表中,如果在分组List中，返回该分组，如果不在，添加一个分组并返回该分组
        public static FriendGroup InGroupListAdd(ref ObservableCollection<FriendGroup> grouplist, String name)
        {
            foreach (var group in grouplist)
            {
                if (group.Name == name)
                {
                    return group;
                }
            }
            FriendGroup friendGroup = new FriendGroup() { Name = name };
            grouplist.Add(friendGroup);
            return friendGroup;
        }

        //分组类中的一个成员变量，为好友列表
        private ObservableCollection<FriendEntity> _friends;
        public ObservableCollection<FriendEntity> Friends
        {
            get
            {
                return _friends;
            }
            set
            {
                _friends = value;
                RaisePropertyChanged("Friends");
            }
        }
    }
}
