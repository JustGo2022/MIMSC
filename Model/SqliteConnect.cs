using MISMC.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            //先获得一个数据库连接
            SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
            sQLiteConnection.Open();
            try
            {
                
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
            sQLiteConnection.Close();
        }

        //保存好友的详细信息
        public static void SaveFriendInfo(String id, String friendgroup, String username, String realname, String sex, String birthday, String address, String email, String phonenumber, String reamarks)
        {
            //先获得一个数据库连接
            SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
            sQLiteConnection.Open();
            try
            {
                
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
            sQLiteConnection.Close();
        }

        //保存与好友的聊天信息，每个好友都有一个对应的消息表,默认type = 0,即好友发送过来的
        public static void SaveMessage(String friendid, String message, String messagedate, int type = 0)
        {
            // 先获得一个数据库连接
            SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
            sQLiteConnection.Open();
            try
            { 
                SQLiteCommand qLiteCommand = sQLiteConnection.CreateCommand();

                String tablename ="F" + friendid + "message";

                //这个好友的表存不存在
                qLiteCommand.CommandText = "SELECT COUNT(*) FROM sqlite_master where type = 'table' and name = @table";
                qLiteCommand.Parameters.AddWithValue("@table", tablename);

                if (0 == Convert.ToInt32(qLiteCommand.ExecuteScalar()))
                {
                    Console.WriteLine(tablename + "表不存在，那么创建它");
                    qLiteCommand.CommandText = "create table " + tablename + "( friendid INT NOT NULL,"
                                                                              + " message varchar(255) NOT NULL, "
                                                                              + " messagedate datatime NOT NULL, "
                                                                              + " type int(2) NOT NULL "
                                                                              + ")";
                    qLiteCommand.ExecuteNonQuery();
                }

                //将信息插入
                qLiteCommand.CommandText = "insert into " + tablename +" (friendid, message, messagedate, type) " +
                                           "values(@friendid, @message, @messagedate, @type)";
                qLiteCommand.Parameters.AddWithValue("@friendid", friendid);
                qLiteCommand.Parameters.AddWithValue("@message", message);
                qLiteCommand.Parameters.AddWithValue("@messagedate", messagedate);
                qLiteCommand.Parameters.AddWithValue("@type", type);
                qLiteCommand.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveFriendInfo Error : " + ex);
            }
            sQLiteConnection.Close();
        }

        //查询自己的详细信息
        public static void  QueryUserInfo(out String id, out String username, out String realname, out String sex, out String birthday, out String address, out String email, out String phonenumber, out String remarks)
        {
            //先获得一个数据库连接
            SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
            sQLiteConnection.Open();
            try
            {
                SQLiteCommand qLiteCommand = sQLiteConnection.CreateCommand();
                qLiteCommand.CommandText = "select * from userinformation";
                //获得查询结果
                SQLiteDataReader sqlitreader = qLiteCommand.ExecuteReader();
                sqlitreader.Read();

                id = sqlitreader[0].ToString();
                username = sqlitreader[1].ToString();
                realname = sqlitreader[2].ToString();
                sex = sqlitreader[3].ToString();
                birthday = sqlitreader[4].ToString();
                address = sqlitreader[5].ToString();
                email = sqlitreader[6].ToString();
                phonenumber = sqlitreader[7].ToString();
                remarks = sqlitreader[8].ToString();

                sqlitreader.Close();

            }
            catch (Exception ex)
            {
                id = "ERROR";
                username = "ERROR";
                realname = "ERROR";
                sex = "ERROR";
                birthday = "ERROR";
                address = "ERROR";
                email = "ERROR";
                phonenumber = "ERROR";
                remarks = "ERROR";
                Console.WriteLine("SaveFriendInfo Error : " + ex);
            }
            sQLiteConnection.Close();
        }

        //查询所有好友的详细信息
        public static void QueryFriendInfo(ref ObservableCollection<FriendGroup> friendCollection)
        {
            //先获得一个数据库连接
            Console.WriteLine("好友信息查询开始");
            SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
            sQLiteConnection.Open();
            try
            {
       
                SQLiteCommand qLiteCommand = sQLiteConnection.CreateCommand();
            

                //先分组，再排序
                qLiteCommand.CommandText = "select * from friendinformation order by friendgroup desc";
                //获得查询结果集
                SQLiteDataReader sqlitreader = qLiteCommand.ExecuteReader();


                //循环改变friendcollection的值
                //保存上一个分组
                FriendGroup friendgroup = null;
                FriendEntity friendEntity;
                while (sqlitreader.Read())
                {
                    //TODO已经被删除的好友，想办法从分组中删除
                    //如果这个分组与上个分组不一致
                    //还要看分组列表中是不是已经有这个列表了
                    //如果有，就从分组列表中取出这个分组进行操作
                    //还要看分组中是不是已经有好友信息了，如果已经有了，那么只需要进行更新就行

                    //检查分组List中有没有此分组,有这个分组，返回这个分组。没有，则新建分组，返回新建的分组
                    friendgroup = FriendGroup.InGroupListAdd(ref friendCollection, sqlitreader[1].ToString());

                    //检查好友List中有没有此好友，有这个好友，返回这个好友。没有，那么新建好友，返回新建的好友
                    friendEntity = FriendEntity.InGroupAdd(ref friendgroup, sqlitreader[2].ToString());
                    //获得好友信息
                    friendEntity.Id = sqlitreader[0].ToString();
                    friendEntity.Name = sqlitreader[2].ToString();
                    friendEntity.RealName = sqlitreader[3].ToString();
                    friendEntity.Sex = sqlitreader[4].ToString();
                    friendEntity.BirthDay = sqlitreader[5].ToString();
                    friendEntity.Address = sqlitreader[6].ToString();
                    friendEntity.Email = sqlitreader[7].ToString();
                    friendEntity.PhoneNumber = sqlitreader[8].ToString();
                    friendEntity.Remarks = sqlitreader[9].ToString();
                }
                sqlitreader.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveFriendInfo Error : " + ex);
            }
            sQLiteConnection.Close();
            Console.WriteLine("好友信息查询结束");
        }



        //查询对应好友的聊天信息
        public static void QueryMessage(String Username, String FriendName, String Id, ref ObservableCollection<MessageMix> messageMixGroup)
        {
            Console.WriteLine(Id + "聊天消息查询开始");
            //该好友对应的消息表的表名字
            String table = "F" + Id + "message";
            //先获得一个数据库连接
            SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
            sQLiteConnection.Open();
            try
            {
                SQLiteCommand qLiteCommand = sQLiteConnection.CreateCommand();
                //这里要先看看数据库有没有变化
                //目前可行方法 ①:查询数据库消息总数，如果消息数没有改变，那么就直接跳出，不作变化
                //             ②:数据库有没有可行的变化与否的方法
                //TODO 添加倒数条数指示，用于向前向后翻页。添加总数指示，用于数据库变化方法①

                //如果记录条数为0，说明是第一次读数据，所以使用这个语句
                if (messageMixGroup.Count == 0)
                {
                    qLiteCommand.CommandText = "select * from " + table + " order by messagedate desc limit 30";                 
                }
                else
                {
                    //如果不是第一次读取数据，那么在数据表中寻找日期大于最后一条已装入信息的记录
                    qLiteCommand.CommandText = "select * from " + table + "  where messagedate > @messagedate order by messagedate desc";
                    qLiteCommand.Parameters.AddWithValue("@messagedate", messageMixGroup.Last().MessageDate);
                }
                //获得查询结果集
                SQLiteDataReader sqlitreader = qLiteCommand.ExecuteReader();

                int messcount = messageMixGroup.Count;

                MessageMix message = null;
                while (sqlitreader.Read())
                {
                    message = new MessageMix();
                    //将数据插入到最前面，因为在查询是是按日期从大到小排的，而日期小的应该在前面
                    messageMixGroup.Insert(messcount, message);
                    //获得好友信息
                    message.FriendId = sqlitreader[0].ToString();
                    message.Message = sqlitreader[1].ToString();
                    message.MessageDate = sqlitreader[2].ToString();
                    message.FriendName = FriendName;
                    message.UserName = Username;
                    if (int.Parse(sqlitreader[3].ToString()) == 0)
                    {
                        //好友发过来的消息
                        message.Type = "Left";
                    }
                    else
                    {
                        //自己发给好友的消息
                        message.Type = "Right";
                    }


                }
                sqlitreader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveFriendInfo Error : " + ex);
            }
            sQLiteConnection.Close();
            Console.WriteLine("好友信息查询结束");
        }

        //保存聊天信息
        public static void SaveChat(String Id, String message, String messageDate)
        {
            Console.WriteLine(Id + "插入聊天消息");
            //该好友对应的消息表的表名字
            String table = "F" + Id + "message";
            //先获得一个数据库连接
            SQLiteConnection sQLiteConnection = SqliteConnect.GetSqliteConnect();
            sQLiteConnection.Open();
            try
            {
                SQLiteCommand command = sQLiteConnection.CreateCommand();

                command.CommandText = "insert into " + table + "(friendid, message, messagedate, type) values(@friendid, @message, @messagedate, @type)";
                command.Parameters.AddWithValue("@friendid", Id);
                command.Parameters.AddWithValue("@message", message);
                command.Parameters.AddWithValue("@messagedate", messageDate);
                //因为是自己发给别人的信息，所有这里的type=1
                command.Parameters.AddWithValue("@type", "1");
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine("SaveFriendInfo Error : " + ex);
            }
            sQLiteConnection.Close();

        }
    }
}
