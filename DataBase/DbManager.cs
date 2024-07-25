using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using System.Text.RegularExpressions;
using Newtonsoft.Json;//正则表达式

public class DbManager
    {
    /// <summary>
    /// 数据库连接对象
    /// </summary>
    public static MySqlConnection mysql;

    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    /// <param name="db">数据表</param>
    /// <param name="ip">ip地址</param>
    /// <param name="port">端口号</param>
    /// <param name="user">用户名</param>
    /// <param name="pw">密码</param>
    /// <returns></returns>
    public static bool Conenect(string db,string ip,int port ,string user,string pw)
    {
        mysql = new MySqlConnection();
        string s=string .Format("Database={0};Data Source={1};port={2};User Id={3};Password={4};",db,ip,port,user,pw);
        mysql.ConnectionString=s;
        try
        {
            mysql.Open();
            Console.WriteLine("[数据库]连接成功");
            return true;
        }
        catch (MySqlException e)
        {
            Console.WriteLine("[数据库]连接失败"+e.Message);
            return false;
        }
    }
    /// <summary>
    /// 判断字符串是否安全
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    private static bool IsSafaString(string str)
    {
        return  !Regex.IsMatch(str, @"[-|\;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\'|#|$|\']");
    }
     

    /// <summary>
    /// 判断账号是否存在
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static bool IsAccountExist(string id)
    {
        if(!IsSafaString(id))
        { return true; 
        }
        string s=string.Format("select * from account where id='{0}'",id);

        try
        {
            using (MySqlCommand cmd = new MySqlCommand(s, mysql)) //创建执行脚本对象
            {
  
                MySqlDataReader dataReader = cmd.ExecuteReader();//执行脚本
                
                bool result= dataReader.HasRows; 
                dataReader.Close();  
                return result;
            }
        }
        catch (MySqlException e)
        {

            Console.WriteLine("[数据库]ISsAcountExist失败"+e.Message);
            return true;//出错了是再注册的时候要返回true，确保无法注册
        }
    }
    /// <summary>
    /// 注册账号
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public static bool Register(string id,string pw)
    {
        //防止SQL注入 
        if (!IsSafaString(id)){
            Console.WriteLine("[数据库]Register失败，账号含有非法字符");
            return false;
        }
        if(!IsSafaString(pw))
        {
            Console.WriteLine("[数据库]Register失败，密码含有非法字符");
            return false;
        }
        if (IsAccountExist(id))
        {
            Console.WriteLine("[数据库]Register失败，账号已存在");
            return false;
        }
        string s=string.Format("insert into account(Id,Pw) values('{0}','{1}')",id,pw);
        try
        {
            using (MySqlCommand cmd = new MySqlCommand(s, mysql)) // 使用 using 确保资源释放
            {
                cmd.ExecuteNonQuery(); // 执行脚本
            }
            Console.WriteLine("[数据库]Register成功");

            return true;
        }
        catch (MySqlException e)
        {
            Console.WriteLine("[数据库]Register失败"+e.Message);
            return false;
        }
    }
    /// <summary>
    /// 创建玩家
    /// </summary>
    /// <param name="id"></param>
    public static bool CreadtePlayer(string id)
    {
        if(!IsSafaString(id))
        {
            Console.WriteLine("[数据库]CreadtePlayer失败，id含有非法字符");
            return false;
        }

        PlayerData playerData = new PlayerData();
        string data=JsonConvert.SerializeObject(playerData);
        string s=string.Format("insert into player(Id,Data) values('{0}','{1}')",id,data);
        try
        {
            using (MySqlCommand cmd = new MySqlCommand(s, mysql)) // 使用 using 确保资源释放
            {
                cmd.ExecuteNonQuery(); // 执行脚本
            }
            Console.WriteLine("[数据库]" + id + "创建成功");

            return true;
        }
        catch (MySqlException e)
        {
            Console.WriteLine("[数据库]" + id + "创建失败" + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 检查密码
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pw"></param>
    /// <returns></returns>
    public static bool CheckPassword(string id,string pw)
    {
        if (!IsSafaString(id))
        {
            Console.WriteLine("[数据库]CheckPassword失败，账号含有非法字符");
            return false;
        }
        if (!IsSafaString(pw))
        {
            Console.WriteLine("[数据库]CheckPassword失败，密码含有非法字符");
            return false;
        }
        string s=string.Format("select * from account where Id='{0}' and Pw='{1}'",id,pw);
        try
        {
            using (MySqlCommand cmd = new MySqlCommand(s, mysql)) // 使用 using 确保资源释放
            {
                MySqlDataReader dataReader = cmd.ExecuteReader(); // 执行脚本
              bool result= dataReader.HasRows;
              dataReader.Close();
                return result;

            }

        }
        catch (MySqlException e)
        {
            Console.WriteLine("[数据库]CheckPassword失败"+e.Message);
            return false;
        }
    }
    /// <summary>
    /// 获取玩家数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static PlayerData GetPlayerData(string id)
    {
        if (!IsSafaString(id))
        {
            Console.WriteLine("[数据库]GetPlayerData失败，账号含有非法字符");
            return null;     
        }
        string s=string.Format("select * from player where Id='{0}'",id);
        try
        {
            using (MySqlCommand cmd = new MySqlCommand(s, mysql))// 使用 using 确保资源释放
            {
                using (MySqlDataReader dataReader = cmd.ExecuteReader()) // 执行脚本{
                {
                  bool result= dataReader.HasRows;
                    if (result)
                    {
                        dataReader.Read();
                        int dataIndex = dataReader.GetOrdinal("Data");
                        string data = dataReader.GetString(dataIndex);
                        PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(data);
                        return  playerData;
                    }
                    else
                    {
                        Console.WriteLine("[数据库]GetPlayerData失败，未找到该玩家");
                        return null;
                    }
                }
            }
        }
        catch (MySqlException e)
        {
            Console.WriteLine("[数据库]GetPlayerData失败"+e.Message);
            return null;

        }
    }
    public static bool UpdatePlayerData(string id,PlayerData playerData)
    {
        if (!IsSafaString(id))
        {
            Console.WriteLine("[数据库]UpdatePlayerData失败，账号含有非法字符");
            return false;
        } 
        string data = JsonConvert.SerializeObject(playerData);
            string s = string.Format("update player set Data='{0}' where Id='{1}'", data, id);
        try
        {
           using (MySqlCommand cmd = new MySqlCommand(s, mysql))
            {
                cmd.ExecuteNonQuery();// 执行脚本没有返回值，使用可以不用using
                return true;
            }
        }
        catch (MySqlException e)
        {
            Console.WriteLine("[数据库]UpdatePlayerData失败"+e.Message);
            return false;
        }
    }
}

