using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public  class ClientState
 {
    /// <summary>
    /// 客户端socket
    /// </summary>
        public Socket clientSocket;
    /// <summary>
    /// 客户端的字节数组
    /// </summary>
       public ByteArray readBuffer=new ByteArray();
     public ClientState(Socket socket,ByteArray readBuffer)
    {
         this.clientSocket = socket;
         this.readBuffer = readBuffer;
    }
   /// <summary>
   /// 最后一次发送ping的时间
   /// </summary>
    public long lastPingTime = 0;
    public Player player;
 }
