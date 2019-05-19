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
    class ChangePassWordViewModel : NotifyObject
    {

        //用来操作mClientViewModel中mClient的注册信息发送函数
        public MClientViewModel mClientViewModel;

        bool boPassWord;
        bool boSPassWord;
        bool boSSPassWord;


        //RegisterViewModel的构造函数
        public ChangePassWordViewModel()
        {
            mClientViewModel = MClientViewModel.CreateInstance();
            this.PassWord = "";
            this.sPassWord = "";
            this.ssPassWord = "";

            boPassWord = false;
            boSPassWord = false;
            boSSPassWord = false;
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

        private String sspassword;
        public String ssPassWord
        {
            get { return sspassword; }
            set
            {
                if (sspassword != value)
                {
                    sspassword = value;
                    RaisePropertyChanged("ssPassWord");
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
            if (boPassWord && boSPassWord && boSSPassWord)
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
                                    obj["PassWord"] = this.PassWord;
                                    obj["SPassWord"] = this.sPassWord;
                                    String str = obj.ToString();
                                    //发送给服务端
                                    mClientViewModel.Mclient.SendModePassWordMessage(str);
                                    //关闭窗口
                                    window.Close();
                                }

                            });
                return btRegister;
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
                                //拿到旧密码框密码
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
                                //拿到新密码框密码
                                this.sPassWord = password.Password;
                            });
                return pbSPassword;
            }
        }

        private MyCommand<PasswordBox> pbSSPassword;
        public MyCommand<PasswordBox> PbSSPassword
        {
            get
            {
                if (pbSSPassword == null)
                    pbSSPassword = new MyCommand<PasswordBox>(
                            password =>
                            {
                                //拿到确认密码框密码
                                this.ssPassWord = password.Password;
                            });
                return pbSSPassword;
            }
        }



        public void Resset()
        {
            //重置所有框中的文本

            PassWord = "";
            sPassWord = "";
            ssPassWord = "";


            boPassWord = false;
            boSPassWord = false;
            boSSPassWord = false;
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
                                this.Resset();
                            }));
                return btResetting;
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
                                Regex regex = new Regex(@"^[a-zA-Z]\w{5,19}$");
                                bool isOK = regex.IsMatch(sPassWord);
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
                return tbSPassWord;
            }
        }

        private MyCommand<TextBlock> tbSSPassWord;
        public MyCommand<TextBlock> TbSSPassWord
        {
            get
            {
                if (tbSSPassWord == null)
                    tbSSPassWord = new MyCommand<TextBlock>(
                            para =>
                            {

                                String splist = "";
                                para.Inlines.Clear();

                                if (!ssPassWord.Equals(sPassWord))
                                {
                                    boSSPassWord = false;
                                    //MessageBox.Show(PassWord + "--" + sPassWord);
                                    splist = "密码不一致";
                                }
                                else
                                {
                                    boSSPassWord = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.RigistButtonCheck();
                            });
                return tbSSPassWord;
            }
        }
    }
}
