using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokerServer
{
   public class MainClass
    {
        public static void Main()    // connect to localhost on port 1337");
        {
            //连接数据库  要在连接服务器端口号之前
            if (!DbManager.Conenect("Game", "127.0.0.1", 3306, "root", "123456"))
                return;
           
            //账号验证+账号注册+用户信息获取添加更新  成功
         
         
   
            NetManager.Connect("127.0.0.1", 8888); 
        }
    }
}
