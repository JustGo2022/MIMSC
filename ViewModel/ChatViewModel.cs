using MISMC.Model;
using MyMVVM;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MISMC.ViewModel
{
    class ChatViewModel : NotifyObject
    {
        public ChatViewModel()
        {
            MessageMixGroup = new ObservableCollection<MessageMix>();
            //启动一个线程，这个线程负责更新聊天消息
            Thread thread = new Thread(MessageUpdata);
            thread.Start();
        }

        public void MessageUpdata()
        {
            while (true)
            {

                Application.Current.Dispatcher.Invoke(
                new Action(() =>
                {
                    //数据库消息更新函数
                    SqliteConnect.QueryMessage(this.Id, ref _messageMixGroup);
                })
                );

                Thread.Sleep(5000);
            }
        }

        public void  ChatSet(String Id, String UserName, String RealName, String Sex, String BirthDay, String Address, String Email, String PhoneNumber, String Remarks)
        {
            this.Id = Id;
            this.UserName = UserName;
            this.RealName = RealName;
            this.Sex = Sex;
            this.BirthDay = BirthDay;
            this.Address = Address;
            this.Email = Email;
            this.PhoneNumber = PhoneNumber;
            this.Remark = Remarks;
        }

        private String id;
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

        private String username;
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


        private String realname;
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

        private String sex;
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

        private String birthday;
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

        private String address;
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

        private String email;
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

        private String phonenumber;
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

        private String remark;
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

        //绑定聊天框的消息
        private String mess;
        public String Mess
        {
            get { return mess; }
            set
            {
                if (mess != value)
                {
                    mess = value;
                    RaisePropertyChanged("Mess");
                }
            }
        }

        private ObservableCollection<MessageMix> _messageMixGroup;
        public ObservableCollection<MessageMix> MessageMixGroup
        {
            get
            {
                return _messageMixGroup;
            }
            set
            {
                _messageMixGroup = value;
                RaisePropertyChanged("MessageMixGroup");
            }
        }

        //消息发送函数
        private MyCommand btSendMessage;
        public MyCommand BtSendMessage
        {
            get
            {
                if (btSendMessage == null)
                    btSendMessage = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                //将消息发送给服务端S

                                //将消息保存到本地数据库

                                //调用读取，或者直接修改消息List
                               
                            }));
                return btSendMessage;
            }
        }
    }

    class MessageMix
    {
        public String FriendId { get; set; }
        public String FriendName{ get; set; }
        public String Message { get; set; }
        public String MessageDate { get; set; }
        public String Type { get; set; }
    }
}
