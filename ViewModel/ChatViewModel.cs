using MyMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISMC.ViewModel
{
    class ChatViewModel : NotifyObject
    {
        public ChatViewModel()
        {

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
    }
}
