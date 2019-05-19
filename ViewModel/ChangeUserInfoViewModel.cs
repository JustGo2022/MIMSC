using MISMC.Model;
using MyMVVM;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MISMC.ViewModel
{
    class ChangeUserInfoViewModel : NotifyObject
    {
        //用来操作mClientViewModel中mClient的注册信息发送函数
        public MClientViewModel mClientViewModel;

        bool boRealName;
        bool boSex;
        bool boBirthDay;
        bool boAddress;
        bool boEmail;
        bool boPhoneNumber;
        bool boRemark;


        //RegisterViewModel的构造函数
        public ChangeUserInfoViewModel()
        {
            mClientViewModel = MClientViewModel.CreateInstance();
            this.RealName = "";
            this.Sex = "";
            this.Address = "";
            this.Email = "";
            this.PhoneNumber = "";
            this.Remark = "";
            this.isRegist = "false";

            boRealName = true;
            boSex = false;
            boBirthDay = false;
            boAddress = true;
            boEmail = true;
            boPhoneNumber = true;
            boRemark = true;
        }

        public void SetInfo(String RealName, String Sex, String Address, String Email, String PhoneNumber, String Remark)
        {
            this.RealName = RealName;
            this.Sex = Sex;
            this.Address = Address;
            this.Email = Email;
            this.PhoneNumber = PhoneNumber;
            this.Remark = Remark;
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

        private String isregist;
        public String isRegist
        {
            get { return isregist; }
            set
            {
                if (isregist != value)
                {
                    isregist = value;
                    RaisePropertyChanged("isRegist");
                }
            }
        }

        public void RigistButtonCheck()
        {
            if (boRealName && boBirthDay && boAddress && boEmail && boPhoneNumber && boRemark)
            {
                isRegist = "true";
            }
            else
            {
                isregist = "false";
            }

        }

        private MyCommand<Window> btRegister;
        public MyCommand<Window> BtRegister
        {
            get
            {
                if (btRegister == null)
                    btRegister = new MyCommand<Window>(
                            window =>
                            {
                                MessageBoxResult boxResult = MessageBox.Show("确认修改？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);

                                if (MessageBoxResult.OK == boxResult)
                                {
                                    JObject obj = new JObject();
                                    obj["RealName"] = this.RealName;
                                    obj["Sex"] = this.Sex;
                                    obj["BirthDay"] = this.BirthDay;
                                    obj["Address"] = this.Address;
                                    obj["Email"] = this.Email;
                                    obj["PhoneNumber"] = this.PhoneNumber;
                                    obj["Remark"] = this.Remark;
                                    String str = obj.ToString();
                                    //发送给服务端
                                    mClientViewModel.Mclient.SendModeMessage(str);
                                    //更新本地的用户信息类
                                    FriendListViewModel friendListViewModel = FriendListViewModel.CreateInstance();
                                    friendListViewModel.RealName= this.RealName;
                                    friendListViewModel.Sex = this.Sex;
                                    friendListViewModel.BirthDay = this.BirthDay;
                                    friendListViewModel.Address = this.Address;
                                    friendListViewModel.Email = this.Email;
                                    friendListViewModel.PhoneNumber = this.PhoneNumber;
                                    friendListViewModel.Remark = this.Remark;
                                    //关闭窗口
                                    window.Close();
                                }
                                    
                            });
                return btRegister;
            }
        }

        private MyCommand<DatePicker> btDateChanged;
        public MyCommand<DatePicker> BtDateChanged
        {
            get
            {
                if (btDateChanged == null)
                    btDateChanged = new MyCommand<DatePicker>(
                            datepicker =>
                            {
                                //拿到日期
                                this.BirthDay = datepicker.SelectedDate.ToString();
                                this.boBirthDay = true;
                                this.RigistButtonCheck();
                            });
                return btDateChanged;
            }
        }



        private MyCommand btMaleSexRadio;
        public MyCommand BtMaleSexRadio
        {
            get
            {
                if (btMaleSexRadio == null)
                    btMaleSexRadio = new MyCommand(
                            o =>
                            {
                                //修改性别为男性
                                this.Sex = "男";
                            });
                return btMaleSexRadio;
            }
        }

        private MyCommand btSheMaleSexRadio;
        public MyCommand BtSheMaleSexRadio
        {
            get
            {
                if (btSheMaleSexRadio == null)
                    btSheMaleSexRadio = new MyCommand(
                            o =>
                            {
                                //修改性别为女性
                                this.Sex = "女";
                            });
                return btSheMaleSexRadio;
            }
        }

        public void Resset()
        {
            //重置所有框中的文本
            RealName = "";
            Sex = "";
            BirthDay = "";
            Address = "";
            Email = "";
            PhoneNumber = "";
            Remark = "";

            boRealName = false;
            boSex = false;
            boBirthDay = false;
            boAddress = false;
            boEmail = false;
            boPhoneNumber = false;
            boRemark = false;
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
                                RealName = "";
                                Sex = "";
                                BirthDay = "";
                                Address = "";
                                Email = "";
                                PhoneNumber = "";
                                Remark = "";

                                boRealName = false;
                                boSex = false;
                                boBirthDay = false;
                                boAddress = false;
                                boEmail = false;
                                boPhoneNumber = false;
                                boRemark = false;
                            }));
                return btResetting;
            }
        }

        private MyCommand<TextBlock> tbRealName;
        public MyCommand<TextBlock> TbRealName
        {
            get
            {
                if (tbRealName == null)
                    tbRealName = new MyCommand<TextBlock>(
                            para =>
                            {
                                Regex regex = new Regex(@"[\u4E00-\u9FA5]");
                                bool isOK = regex.IsMatch(RealName);
                                String splist = "";
                                para.Inlines.Clear();

                                if (!isOK)
                                {
                                    boRealName = false;
                                    //MessageBox.Show(PassWord + "--" + sPassWord);
                                    splist = "长度为1-20个字节，且只能为中文字符";
                                }
                                else if (Encoding.Default.GetByteCount(RealName) > 20)
                                {
                                    boRealName = false;
                                    splist = "长度为1-20个字节，且只能为中文字符";
                                }
                                else
                                {
                                    boRealName = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.RigistButtonCheck();
                            });
                return tbRealName;
            }
        }

        private MyCommand<TextBlock> tbAddress;
        public MyCommand<TextBlock> TbAddress
        {
            get
            {
                if (tbAddress == null)
                    tbAddress = new MyCommand<TextBlock>(
                            para =>
                            {

                                String splist = "";
                                para.Inlines.Clear();

                                if (Address.Equals("") || Encoding.Default.GetByteCount(Address) > 50)
                                {
                                    boAddress = false;
                                    //MessageBox.Show(PassWord + "--" + sPassWord);
                                    splist = "长度为1-50个字节";
                                }
                                else
                                {
                                    boAddress = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.RigistButtonCheck();
                            });
                return tbAddress;
            }
        }

        private MyCommand<TextBlock> tbEmail;
        public MyCommand<TextBlock> TbEmail
        {
            get
            {
                if (tbEmail == null)
                    tbEmail = new MyCommand<TextBlock>(
                            para =>
                            {
                                Regex regex = new Regex(@"^\w+@\w+(\.[a-zA-Z]{2,3}){1,2}$");
                                bool isOK = regex.IsMatch(Email);

                                String splist = "";
                                para.Inlines.Clear();

                                if (!isOK)
                                {
                                    boEmail = false;
                                    //MessageBox.Show(PassWord + "--" + sPassWord);
                                    splist = "邮箱格式不正确";
                                }
                                else
                                {
                                    boEmail = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.RigistButtonCheck();
                            });
                return tbEmail;
            }
        }

        private MyCommand<TextBlock> tbPhoneNumber;
        public MyCommand<TextBlock> TbPhoneNumber
        {
            get
            {
                if (tbPhoneNumber == null)
                    tbPhoneNumber = new MyCommand<TextBlock>(
                            para =>
                            {
                                Regex regex = new Regex(@"^1\d{10}$");
                                bool isOK = regex.IsMatch(PhoneNumber);

                                String splist = "";
                                para.Inlines.Clear();

                                if (!isOK)
                                {
                                    boPhoneNumber = false;
                                    //MessageBox.Show(PassWord + "--" + sPassWord);
                                    splist = "手机号码格式不正确";
                                }
                                else
                                {
                                    boPhoneNumber = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.RigistButtonCheck();
                            });
                return tbPhoneNumber;
            }
        }

        private MyCommand<TextBlock> tbRemark;
        public MyCommand<TextBlock> TbRemark
        {
            get
            {
                if (tbRemark == null)
                    tbRemark = new MyCommand<TextBlock>(
                            para =>
                            {
                                String splist = "";
                                para.Inlines.Clear();

                                if (Encoding.Default.GetByteCount(Remark) > 255)
                                {
                                    boRemark = false;
                                    //MessageBox.Show(PassWord + "--" + sPassWord);
                                    splist = "长度为1-255个字节";
                                }
                                else
                                {
                                    boRemark = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.RigistButtonCheck();
                            });
                return tbRemark;
            }
        }
    }
}
