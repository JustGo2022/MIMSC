using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISMC.Model
{
    class SqliteConnect
    {
        static String userid;
        static String databaseName;

        public SqliteConnect(String userid)
        {
            SqliteConnect.userid = userid;
            SqliteConnect.databaseName = "./DataBase/" + userid + "DB";
        }

        //用于外部直接初始化Sqlite数据库
        public static void  SqliteInit(String userid)
        {
            SqliteConnect.userid = userid;
            SqliteConnect.databaseName = "./DataBase/" + "U" + userid + "DB" + ".sqlite";
        }

        //获取一个SQLiteConnection类型的数据库连接
        public static SQLiteConnection GetSqliteConnect()
        {
            SQLiteConnection sqliteConnect = new SQLiteConnection("data source=" + SqliteConnect.databaseName);
            return sqliteConnect;
        }

        //在本地创建自己的信息表，好友信息表         单个好友的消息表后面再处理
        //每次正确登陆后，应该执行一次这个方法
        public static void  CreateTable()
        {
            //先获得一个数据库连接
            SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
            sQLiteConnection.Open();
            SQLiteCommand qLiteCommand = sQLiteConnection.CreateCommand();

            try
            {
                if (!Directory.Exists("./DataBase"))
                {
                    Directory.CreateDirectory("./DataBase");
                }

                //Learn  在执行AddWithValue插入时，插入的数据格式为'userinformation'，所以占位符两边就不用包含''了
                qLiteCommand.CommandText = "SELECT COUNT(*) FROM sqlite_master where type = 'table' and name = @table";
                qLiteCommand.Parameters.AddWithValue("@table", "userinformation");

                if (0 == Convert.ToInt32(qLiteCommand.ExecuteScalar()))
                {
                    Console.WriteLine("userinformation表不存在，那么创建它");
                    qLiteCommand.CommandText = "create table userinformation" + "( id INT NOT NULL,"
                                                                              + " username varchar(20) NOT NULL, "
                                                                              + " realname varchar(20) NOT NULL, "
                                                                              + " sex varchar(2) NOT NULL, "
                                                                              + " birthday data NOT NULL, "
                                                                              + " address varchar(50) NOT NULL, "
                                                                              + " email varchar(50) NOT NULL, "
                                                                              + " phonenumber varchar(11) NOT NULL, "
                                                                              + " remarks varchar(255) NOT NULL "
                                                                              + ")";
                    qLiteCommand.ExecuteNonQuery();
                }

                //下面开始检查好友信息列表
                qLiteCommand.CommandText = "SELECT COUNT(*) FROM sqlite_master where type = 'table' and name = 'friendinformation'";
                //qLiteCommand.Parameters.AddWithValue("@table", "friendinformation");

                if (0 == Convert.ToInt32(qLiteCommand.ExecuteScalar()))
                {
                    Console.WriteLine("friendinformation表不存在，那么创建它");
                    qLiteCommand.CommandText = "create table friendinformation" + "( id INT NOT NULL,"
                                                                              + " friendgroup varchar(20) NOT NULL, "
                                                                              + " username varchar(20) NOT NULL, "
                                                                              + " realname varchar(20) NOT NULL, "
                                                                              + " sex varchar(2) NOT NULL, "
                                                                              + " birthday data NOT NULL, "
                                                                              + " address varchar(50) NOT NULL, "
                                                                              + " email varchar(50) NOT NULL, "
                                                                              + " phonenumber varchar(11) NOT NULL, "
                                                                              + " remarks varchar(255) NOT NULL "
                                                                              + ")";
                    qLiteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateTable Error : " + ex);
            }
            

            sQLiteConnection.Close();
        }

        //将自己的信息保存在这个表中
        public static void SaveUserInfo(String id, String username, String realname, String sex, String birthday, String address, String email, String phonenumber, String reamarks)
        {
            try
            {
                //先获得一个数据库连接
                SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
                sQLiteConnection.Open();
                SQLiteCommand qLiteCommand = sQLiteConnection.CreateCommand();

                qLiteCommand.CommandText = "SELECT COUNT(*) FROM userinformation";

                if (0 == Convert.ToInt32(qLiteCommand.ExecuteScalar()))
                {
                    Console.WriteLine("插入了用户信息");
                    //如果信息不存在，那么就插入
                    qLiteCommand.CommandText = "insert into userinformation (id,username,realname,sex,birthday,address,email,phonenumber,remarks) " +
                                           "values(@id,@username,@realname,@sex,@birthday,@address,@email,@phonenumber,@remarks)";
                    qLiteCommand.Parameters.AddWithValue("@id", id);
                    qLiteCommand.Parameters.AddWithValue("@username", username);
                    qLiteCommand.Parameters.AddWithValue("@realname", realname);
                    qLiteCommand.Parameters.AddWithValue("@sex", sex);
                    qLiteCommand.Parameters.AddWithValue("@birthday", birthday);
                    qLiteCommand.Parameters.AddWithValue("@address", address);
                    qLiteCommand.Parameters.AddWithValue("@email", email);
                    qLiteCommand.Parameters.AddWithValue("@phonenumber", phonenumber);
                    qLiteCommand.Parameters.AddWithValue("@remarks", reamarks);
                    qLiteCommand.ExecuteNonQuery();
                }
                else
                {
                    Console.WriteLine("更新了用户信息");
                    //如果信息存在，那么就更新
                    qLiteCommand.CommandText = "update userinformation set username = @username,realname = @realname,sex = @sex,birthday = @birthday,address = @address,email = @email,phonenumber = @phonenumber,remarks = @remarks " +
                                                "where id = @id";
                    qLiteCommand.Parameters.AddWithValue("@id", id);
                    qLiteCommand.Parameters.AddWithValue("@username", username);
                    qLiteCommand.Parameters.AddWithValue("@realname", realname);
                    qLiteCommand.Parameters.AddWithValue("@sex", sex);
                    qLiteCommand.Parameters.AddWithValue("@birthday", birthday);
                    qLiteCommand.Parameters.AddWithValue("@address", address);
                    qLiteCommand.Parameters.AddWithValue("@email", email);
                    qLiteCommand.Parameters.AddWithValue("@phonenumber", phonenumber);
                    qLiteCommand.Parameters.AddWithValue("@remarks", reamarks);
                    qLiteCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveUserInfo Error : " + ex);
            }
        }

        public static void SaveFriendInfo(String id, String friendgroup, String username, String realname, String sex, String birthday, String address, String email, String phonenumber, String reamarks)
        {
            try
            {
                //先获得一个数据库连接
                SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
                sQLiteConnection.Open();
                SQLiteCommand qLiteCommand = sQLiteConnection.CreateCommand();

                qLiteCommand.CommandText = "SELECT COUNT(*) FROM friendinformation where id = @id";
                qLiteCommand.Parameters.AddWithValue("@id", id);

                Console.WriteLine(qLiteCommand.CommandText.ToString());
                Console.WriteLine("查询该好友信息是否已经存在 = " + Convert.ToInt32(qLiteCommand.ExecuteScalar()));
                Console.WriteLine("该好友的id为 = " + id);

                if (0 == Convert.ToInt32(qLiteCommand.ExecuteScalar()))
                {
                    Console.WriteLine("插入了好友信息");
                    //如果不存在该好友的详细信息，就插入
                    qLiteCommand.CommandText = "insert into friendinformation (id,friendgroup,username,realname,sex,birthday,address,email,phonenumber,remarks) " +
                                           "values(@friendid,@friendgroup,@username,@realname,@sex,@birthday,@address,@email,@phonenumber,@remarks)";
                    qLiteCommand.Parameters.AddWithValue("@friendid", id);
                    qLiteCommand.Parameters.AddWithValue("@friendgroup", friendgroup);
                    qLiteCommand.Parameters.AddWithValue("@username", username);
                    qLiteCommand.Parameters.AddWithValue("@realname", realname);
                    qLiteCommand.Parameters.AddWithValue("@sex", sex);
                    qLiteCommand.Parameters.AddWithValue("@birthday", birthday);
                    qLiteCommand.Parameters.AddWithValue("@address", address);
                    qLiteCommand.Parameters.AddWithValue("@email", email);
                    qLiteCommand.Parameters.AddWithValue("@phonenumber", phonenumber);
                    qLiteCommand.Parameters.AddWithValue("@remarks", reamarks);
                    qLiteCommand.ExecuteNonQuery();
                }
                else
                {
                    Console.WriteLine("更新了好友信息");
                    //如果存在，就更新
                    qLiteCommand.CommandText = "update friendinformation set friendgroup = @friendgroup, username = @username,realname = @realname,sex = @sex,birthday = @birthday,address = @address,email = @email,phonenumber = @phonenumber,remarks = @remarks " +
                                                "where id = @friendid";
                    qLiteCommand.Parameters.AddWithValue("@friendid", id);
                    qLiteCommand.Parameters.AddWithValue("@friendgroup", friendgroup);
                    qLiteCommand.Parameters.AddWithValue("@username", username);
                    qLiteCommand.Parameters.AddWithValue("@realname", realname);
                    qLiteCommand.Parameters.AddWithValue("@sex", sex);
                    qLiteCommand.Parameters.AddWithValue("@birthday", birthday);
                    qLiteCommand.Parameters.AddWithValue("@address", address);
                    qLiteCommand.Parameters.AddWithValue("@email", email);
                    qLiteCommand.Parameters.AddWithValue("@phonenumber", phonenumber);
                    qLiteCommand.Parameters.AddWithValue("@remarks", reamarks);
                    qLiteCommand.ExecuteNonQuery();
                }
  

            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveFriendInfo Error : " + ex);
            }
        }

        //保存与好友的聊天信息，每个好友都有一个对应的消息表,默认type = 0,即好友发送过来的
        public static void SaveMessage(String friendid, String message, String messagedate, int type = 0)
        {
            try
            {
                //先获得一个数据库连接
                SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
                sQLiteConnection.Open();
                SQLiteCommand qLiteCommand = sQLiteConnection.CreateCommand();

                String tablename = friendid + "message";

                //这个好友的表存不存在
                qLiteCommand.CommandText = "SELECT COUNT(*) FROM sqlite_master where type = 'table' and name = @table";
                qLiteCommand.Parameters.AddWithValue("@table", tablename);

                if (0 == Convert.ToInt32(qLiteCommand.ExecuteScalar()))
                {
                    Console.WriteLine(tablename + "表不存在，那么创建它");
                    qLiteCommand.CommandText = "create table " + tablename + "( friendid INT NOT NULL,"
                                                                              + " message varchar(255) NOT NULL, "
                                                                              + " messsagedate datatime NOT NULL, "
                                                                              + " type int(2) NOT NULL, "
                                                                              + ")";
                    qLiteCommand.ExecuteNonQuery();
                }

                //将信息插入
                qLiteCommand.CommandText = "insert into " + tablename +" (friendid, message, messagedate, type) " +
                                           "values(@friendid, @message, @messagedate, @type)";
                qLiteCommand.Parameters.AddWithValue("@riendid", friendid);
                qLiteCommand.Parameters.AddWithValue("@message", message);
                qLiteCommand.Parameters.AddWithValue("@messagedate", messagedate);
                qLiteCommand.Parameters.AddWithValue("@type", type);
                qLiteCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveFriendInfo Error : " + ex);
            }
        }
    }
}
