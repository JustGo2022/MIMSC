using MISMC.Model;
using MyMVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISMC.ViewModel 
{
    class MClientViewModel : NotifyObject
    {
        public MClientViewModel()
        {
            Mclient = new MClient();
            Mclient.Connect();
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
    }
}
