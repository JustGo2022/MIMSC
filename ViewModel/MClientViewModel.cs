using MISMC.Model;
using MISMC.Windows;
using MyMVVM;
using SocketAsyncEventArgsOfficeDemo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISMC.ViewModel 
{
    class MClientViewModel : NotifyObject
    {
        private MClientViewModel()
        {
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

        private String userPass;
        public String UserPass
        {
            get { return userPass; }
            set
            {
                if (userPass != value)
                {
                    userPass = value;
                    RaisePropertyChanged("UserPass");
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
                        new Action<object>(
                            o =>
                            {
                                Mclient.SendLogin(UserName, UserPass);
                            }));
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
                                RegisterWindow registerWindow = new RegisterWindow();
                                registerWindow.Show();
                            }));
                return btRegister;
            }
        }

        //注册界面的代码

    }
}
