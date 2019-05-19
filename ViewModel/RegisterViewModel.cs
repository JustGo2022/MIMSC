using MISMC.Model;
using MyMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Winform = System.Windows.Forms;
using System.Windows.Media;

namespace MISMC.ViewModel
{
    class RegisterViewModel : NotifyObject
    {
        //用来操作mClientViewModel中mClient的注册信息发送函数
        public MClientViewModel mClientViewModel;

        bool boUserName;
        bool boPassWord;
        bool boSPassWord;
        bool boRealName;
        bool boSex;
        bool boBirthDay;
        bool boAddress;
        bool boEmail;
        bool boPhoneNumber;
        bool boRemark;
        

        //RegisterViewModel的构造函数
        public RegisterViewModel()
        {
            mClientViewModel = MClientViewModel.CreateInstance();
            this.UserName = "";
            this.PassWord = "";
            this.sPassWord = "";
            this.RealName = "";
            this.Sex = "";
            this.Address = "";
            this.Email = "";
            this.PhoneNumber = "";
            this.Remark = "";
            this.isRegist = "false";

            boUserName = false;
            boPassWord = false;
            boSPassWord = false;
            boRealName = false;
            boSex = false;
            boBirthDay = false;
            boAddress = false;
            boEmail = false;
            boPhoneNumber = false;
            boRemark = false;
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
            if (boUserName&&boPassWord&&boSPassWord&&boRealName&&boBirthDay&&boAddress&&boEmail&&boPhoneNumber&&boRemark)
            {
                isRegist = "true";
            }
            else
            {
                isregist = "false";
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
                                //MessageBox.Show("注册按钮");
                                mClientViewModel.Mclient.SendRegister(UserName, PassWord, RealName, Sex, BirthDay, Address, Email, PhoneNumber, Remark);
                            }));
                return btRegister;
            }
        }

        
        private MyCommand btChooseJpj;
        public MyCommand BtChooseJpj
        {
            get
            {
                if (btChooseJpj == null)
                    btChooseJpj = new MyCommand(
                        new Action<object>(
                            o =>
                            {
                                Winform.OpenFileDialog openFileDialog = new Winform.OpenFileDialog();
                                openFileDialog.ShowDialog();

                                MessageBox.Show(openFileDialog.FileName);
                         
                            }));
                return btChooseJpj;
            }
        }

        private MyCommand<PasswordBox> pbPassword;
        public MyCommand<PasswordBox> PbPassword
        {
            get
            {
                if (pbPassword == null)
                    pbPassword = new MyCommand<PasswordBox>(
                            password =>
                            {
                                //拿到密码框密码
                                this.PassWord = password.Password;
                            });
                return pbPassword;
            }
        }

        private MyCommand<PasswordBox> pbSPassword;
        public MyCommand<PasswordBox> PbSPassword
        {
            get
            {
                if (pbSPassword == null)
                    pbSPassword = new MyCommand<PasswordBox>(
                            password =>
                            {
                                //拿到确认密码框密码
                                this.sPassWord = password.Password;
                            });
                return pbSPassword;
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

            boUserName = false;
            boPassWord = false;
            boSPassWord = false;
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

                                boUserName = false;
                                boPassWord = false;
                                boSPassWord = false;
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

        private MyCommand<TextBlock> tbUserName;
        public MyCommand<TextBlock> TbUserName
        {
            get
            {
                if (tbUserName == null)
                    tbUserName = new MyCommand<TextBlock>(
                            para =>
                            {
                                String splist = "";
                                para.Inlines.Clear();
                                //确认UserName
                                if (Encoding.Default.GetByteCount(UserName) < 6)
                                {
                                    boUserName = false;
                                    //MessageBox.Show(UserName);
                                    splist = "用户名长度在6-20字节之间";
                                }
                                else if (Encoding.Default.GetByteCount(UserName) > 20)
                                {
                                    boUserName = false;
                                    splist = "用户名长度在6-20字节之间";
                                }
                                else
                                {
                                    boUserName = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.RigistButtonCheck();
                            });
                return tbUserName;
            }
        }

        private MyCommand<TextBlock> tbPassWord;
        public MyCommand<TextBlock> TbPassWord
        {
            get
            {
                if (tbPassWord == null)
                    tbPassWord = new MyCommand<TextBlock>(
                            para =>
                            {
                                Regex regex = new Regex(@"^[a-zA-Z]\w{5,19}$");
                                bool isOK = regex.IsMatch(PassWord);
                                String splist = "";
                                para.Inlines.Clear();

                                if (!isOK)
                                {
                                    boPassWord = false;
                                    splist = "以字母开头，长度在6-20之间";
                                }
                                else 
                                {
                                    boPassWord = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.RigistButtonCheck();
                            });
                return tbPassWord;
            }
        }

        private MyCommand<TextBlock> tbSPassWord;
        public MyCommand<TextBlock> TbSPassWord
        {
            get
            {
                if (tbSPassWord == null)
                    tbSPassWord = new MyCommand<TextBlock>(
                            para =>
                            {
                               
                                String splist = "";
                                para.Inlines.Clear();

                                if (!PassWord.Equals(sPassWord))
                                {
                                    boSPassWord = false;
                                    //MessageBox.Show(PassWord + "--" + sPassWord);
                                    splist = "密码不一致";
                                }
                                else
                                {
                                    boSPassWord = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.RigistButtonCheck();
                            });
                return tbSPassWord;
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
                                    splist = "长度为1-5个字节，且只能为中文字符";
                                }
                                else if (Encoding.Default.GetByteCount(RealName) > 5)
                                {
                                    boRealName = false;
                                    splist = "长度为1-5个字节，且只能为中文字符";
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
