using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISMC.Model
{
    //这个类是用来保存当前使用用户的信息的，因为时单例模式，所以在哪里都可以拿到这个类的唯一副本
    public class UserInfo
    {
        public String UserName { get; set; }
        public String PassWord { get; set; }
        public String NewPassword { get; set; }
        public String ReaLname { get; set; }
        public String Sex { get; set; }
        public String BirthDay { get; set; }
        public String AddRess { get; set; }
        public String Email { get; set; }
        public String PhoneNumber { get; set; }
        public String Remarks { get; set; }
        public String IpAddress { get; set; }
        public String Port { get; set; }
        public String Group { get; set; }

        private static UserInfo userInfo = null;
        public static UserInfo CreateInstance()
        {
            if (userInfo == null)
            {
                userInfo = new UserInfo();
            }
            return userInfo;
        }
    }
}
