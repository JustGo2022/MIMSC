using MISMC.Model;
using MISMC.Windows;
using MyMVVM;
using SocketAsyncEventArgsOfficeDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MISMC.ViewModel 
{
    class MClientViewModel : NotifyObject
    {
        private MClientViewModel()
        {
            boUserName = false;
            boPassWord = false;
            UserName = "";
            PassWord = "";
            isLand = "false";
            Mclient = MClient.CreateInstance("127.0.0.1", "5730");
            Mclient.ConnectServer();
        }

        private static MClientViewModel mClientViewModel = null;
        public static MClientViewModel CreateInstance()
        {
            if (mClientViewModel == null)
            {
                Console.WriteLine("mClientViewModel创建开始");
                mClientViewModel = new MClientViewModel();
                Console.WriteLine("mClientViewModel创建完毕");
            }
            return mClientViewModel;
        }

        //注册窗口实例
        public RegisterWindow registerWindow { get; set; }

        bool boUserName;
        bool boPassWord;

        public void LandButtonCheck()
        {
            if (boUserName && boPassWord)
            {
                isLand = "true";
            }
            else
            {
                isLand = "false";
            }
        }

        private MClient mclient;
        public MClient Mclient
        {
            get { return mclient; }
            set
            {
                if (mclient != value)
                {
                    mclient = value;
                    RaisePropertyChanged("Mclient");
                }
            }
        }

        private String userName;
        public String UserName
        {
            get { return userName; }
            set
            {
                if (userName != value)
                {
                    userName = value;
                    RaisePropertyChanged("UserName");
                }
            }
        }

        private String passWord;
        public String PassWord
        {
            get { return passWord; }
            set
            {
                if (passWord != value)
                {
                    passWord = value;
                    RaisePropertyChanged("PassWord");
                }
            }
        }

        private String island;
        public String isLand
        {
            get { return island; }
            set
            {
                if (island != value)
                {
                    island = value;
                    RaisePropertyChanged("isLand");
                }
            }
        }

        private MyCommand btLogin;
        public MyCommand BtLogin
        {
            get
            {
                if (btLogin == null)
                    btLogin = new MyCommand(
                            password =>
                            {
                                //发送用户名和密码
                                Mclient.SendLogin(UserName, PassWord);
                            });
                return btLogin;
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
                                //新建一个注册窗口对象
                                registerWindow = new RegisterWindow();
                                //隐藏主窗口
                                APPlication.Current.MainWindow.Hide();
                                //显示注册窗口并阻塞
                                registerWindow.ShowDialog();
                                //显示主窗口
                                APPlication.Current.MainWindow.Show();
                            }));
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
                                //拿到密码框密码
                                this.PassWord = password.Password;
                            });
                return pbPassword;
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
                                    splist = "长度为6-20字节";
                                }
                                else if (Encoding.Default.GetByteCount(UserName) > 20)
                                {
                                    boUserName = false;
                                    splist = "长度为6-20字节";
                                }
                                else
                                {
                                    boUserName = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.LandButtonCheck();
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
                                    splist = "字母开头，长度为6-20字节";
                                }
                                else
                                {
                                    boPassWord = true;
                                    splist = "";
                                }
                                para.Inlines.Add(new Run(splist) { Foreground = Brushes.Red });
                                this.LandButtonCheck();
                            });
                return tbPassWord;
            }
        }
    }
}
