using MyMVVM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MISMC.ViewModel
{
    class AddFriendViewModel : NotifyObject
    {
        public AddFriendViewModel()
        {
            FrienInfoGroup = new ObservableCollection<FriendInfo>();
        }

        //AddFriendViewModel的单例函数
        private static AddFriendViewModel addFriendViewModel = null;
        public static AddFriendViewModel CreateInstance()
        {
            if (addFriendViewModel == null)
            {
                addFriendViewModel = new AddFriendViewModel();
            }
            return addFriendViewModel;
        }


        public  String searchString;
        public String SearchString
        {
            get { return searchString; }
            set
            {
                if (searchString != value)
                {
                    searchString = value;
                    RaisePropertyChanged("SearchString");
                }
            }
        }

        //将框类的字符发给服务端，请求服务端搜索信息
        private MyCommand btSearchFriend;
        public MyCommand BtSearchFriend
        {
            get
            {
                if (btSearchFriend == null)
                    btSearchFriend = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                //清空搜索结果List
                                frienInfoGroup.Clear();
                                //把查询请求发给服务端
                                JObject obj = new JObject();
                                obj["Search"] = this.SearchString;
                                String str = obj.ToString();
                                MClientViewModel mClientViewModel = MClientViewModel.CreateInstance();
                                mClientViewModel.Mclient.SendSearchFriend(str);
                            }));
                return btSearchFriend;
            }
        }

        private ObservableCollection<FriendInfo> frienInfoGroup;
        public ObservableCollection<FriendInfo> FrienInfoGroup
        {
            get
            {
                return frienInfoGroup;
            }
            set
            {
                frienInfoGroup = value;
                RaisePropertyChanged("FrienInfoGroup");
            }
        }
    }

    class FriendInfo
    {
        public String Id { set; get; }
        public String FriendName { set; get; }

        //将框类的字符发给服务端，请求服务端搜索信息
        private MyCommand btAdd;
        public MyCommand BtAdd
        {
            get
            {
                if (btAdd == null)
                    btAdd = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                //把好友添加请求发给服务端
                                JObject obj = new JObject();
                                obj["Id"] = this.Id;
                                String str = obj.ToString();
                                MClientViewModel mClientViewModel = MClientViewModel.CreateInstance();
                                mClientViewModel.Mclient.SendFriendRequest(str);
                            }));
                return btAdd;
            }
        }
    }
}
