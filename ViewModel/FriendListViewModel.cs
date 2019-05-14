using MISMC.Model;
using MISMC.Windows;
using MyMVVM;
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
                
                Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    SqliteConnect.QueryFriendInfo(ref _friends);
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

        public ChatWindow chatWindow { get; set; }
        public FriendInfoWindow friendInfoWindow { get; set; }

        //右键菜单
        private ContextMenu contextMenu { get; set; }

        public FriendEntity()
        {
            chatWindow = null;
            friendInfoWindow = null;

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
