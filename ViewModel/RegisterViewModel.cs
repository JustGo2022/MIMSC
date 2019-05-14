using MISMC.Model;
using MyMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MISMC.ViewModel
{
    class RegisterViewModel : NotifyObject
    {
        //用来操作mClientViewModel中mClient的注册信息发送函数
        public MClientViewModel mClientViewModel;

        //RegisterViewModel的构造函数
        public RegisterViewModel()
        {
            mClientViewModel = MClientViewModel.CreateInstance();
        }

        //RegisterViewModel的单例函数
        private static RegisterViewModel registerViewModel = null;
        public static RegisterViewModel CreateInstance()
        {
            if (registerViewModel == null)
            {
                registerViewModel = new RegisterViewModel();
            }
            return registerViewModel;
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

        private String password;
        public String PassWord
        {
            get { return password; }
            set
            {
                if (password != value)
                {
                    password = value;
                    RaisePropertyChanged("PassWord");
                }
            }
        }

        private String spassword;
        public String sPassWord
        {
            get { return spassword; }
            set
            {
                if (spassword != value)
                {
                    spassword = value;
                    RaisePropertyChanged("sPassWord");
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

        private MyCommand btRegister;
        public MyCommand BtRegister
        {
            get
            {
                if (btRegister == null)
                    btRegister = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                MessageBox.Show("注册按钮");
                                mClientViewModel.Mclient.SendRegister(UserName, PassWord, RealName, Sex, BirthDay, Address, Email, PhoneNumber, Remark);
                            }));
                return btRegister;
            }
        }

        private MyCommand btResetting;
        public MyCommand BtResetting
        {
            get
            {
                if (btResetting == null)
                    btResetting = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                //重置所有框中的文本
                                UserName = "";
                                PassWord = "";
                                sPassWord = "";
                                RealName = "";
                                Sex = "";
                                BirthDay = "";
                                Address = "";
                                Email = "";
                                PhoneNumber = "";
                                Remark = "";
                            }));
                return btResetting;
            }
        }
    }
}
