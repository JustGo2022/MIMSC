using MyMVVM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MISMC.ViewModel
{
    class ChangGroupViewModel : NotifyObject
    {
        public ChangGroupViewModel()
        {

        }

        public void ChangGroupSet(String Id)
        {
            this.FriendId = Id;
        }

        public String group;
        public String Group
        {
            get { return group; }
            set
            {
                if (group != value)
                {
                    group = value;
                    RaisePropertyChanged("Group");
                }
            }
        }

        public String FriendId { get; set; }

        //将框类的字符发给服务端，请求服务端搜索信息
        private MyCommand<Window> btChangeGroup;
        public MyCommand<Window> BtChangeGroup
        {
            get
            {
                if (btChangeGroup == null)
                    btChangeGroup = new MyCommand<Window>(
                        (window)=>
                            {
                                //把分组更改请求发给服务端
                                JObject obj = new JObject();
                                obj["FriendId"] = this.FriendId;
                                obj["Group"] = this.Group;
                                String str = obj.ToString();
                                MClientViewModel mClientViewModel = MClientViewModel.CreateInstance();
                                mClientViewModel.Mclient.SendChangeGroup(str);
                                window.Close();
                            });
                return btChangeGroup;
            }
        }

    }
}
