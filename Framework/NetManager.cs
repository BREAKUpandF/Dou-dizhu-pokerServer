using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
public static class NetManager
{
    /// <summary>
    /// 服务器socket
    /// </summary>
    public static Socket listenfd;
    /// <summary>
    ///  客户端字典
    /// </summary>
  public static Dictionary<Socket, ClientState> clients = new Dictionary<Socket, ClientState>();

    /// <summary>
    /// 消息链表
    /// </summary>
     private static List<Socket> sockets = new List<Socket>();

    public static long pingInterval = 5;
    /// <summary>
    /// 连接地址和端口号
    /// </summary>
   static int xxx = 0;
    private static long lastTime;
    public static void  Connect(string ip,int port)
    {
        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listenfd.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(ip), port));
        listenfd.Listen(0);
        Console.WriteLine("[服务器]已启动");
        clients.Clear();
     
        while (true)
        {
            sockets.Clear();
            sockets.Add(listenfd);
            foreach(ClientState s in clients.Values)
            {
                sockets.Add(s.clientSocket);
            }
            Socket.Select(sockets, null, null, 1000);
            for(int i=0;i<sockets.Count;i++)
            {
                Socket s = sockets[i];
                if (GetTimeStamp() - lastTime >= 5)
                {
                    Timer();
                    lastTime = GetTimeStamp();
                }
                if (s == listenfd)
                {
                    Console.WriteLine("[服务器]有新的服务器发送连接请求");
                    //有新的服务器发送连接请求
                    Accept(listenfd);
                }
                else 
                {

                    if (s.Poll(1000, SelectMode.SelectRead))
                    {
                        Console.WriteLine("[服务器]有客户端发送消息过来");
                      
                        Receive(s);
                       
                    }
                }
            }
        }
    }
    public static void Accept(Socket listenfd)
    {
        try
        {
            //接受客户端的连接请求
            Socket clientSocket = listenfd.Accept();
            Console.WriteLine("[服务器]客户端已连接+" + clientSocket.RemoteEndPoint.ToString()  );
            //将客户端的socket添加到链表中
            ClientState state = new ClientState(clientSocket, new ByteArray());
            state.lastPingTime=GetTimeStamp();
            clients.Add(clientSocket, state);

        }
        catch (SocketException e)
        {
            Console.WriteLine("[服务器]连接异常" + e.ToString());
        }
    }
    public static void Receive(Socket clientSocket)
    {
        ClientState state=clients[clientSocket];
        //接收客户端发送的消息
        ByteArray readBuff = state.readBuffer;
        int count = 0;

        if (readBuff.Remain <= 0)
        {
            readBuff.MoveBytes();
        }
        if (readBuff.Remain <= 0)
        {
            Console.WriteLine("接收失败数组长度不足");
            Close(state);
            return;
        }
        try
        {
            count=clientSocket.Receive(readBuff.bytes, readBuff.writeIndex, readBuff.Remain, SocketFlags.None);
            Console.WriteLine("[服务器]接收数据" + count);
          
        }
        catch (SocketException e)
        {
            Console.WriteLine("[服务器]接收异常" + e.ToString());
        }
        if(count<=0)
        {
            Console.WriteLine("[服务器]客户端已断开连接");
            Close(state);
            return;
        }
        readBuff.writeIndex += count;
        //解码信息
        OnReceiveData(state);
        readBuff.MoveBytes();
    }
    public static void  Close(ClientState state)
    {
        MethodInfo mei=typeof(EventHandle).GetMethod("OnDisconnect");
        mei.Invoke(null, new object[] { state});
        state.clientSocket.Close();
        clients.Remove(state.clientSocket);
    }
    public static void  OnReceiveData(ClientState state)
    {
        ByteArray readBuff = state.readBuffer;
        byte[] bytes=readBuff.bytes;
        while (readBuff.Length <= 2)
        {
            return;
        }
        short bodyLength = (short)(bytes[readBuff.readIndex + 1] * 256 + bytes[readBuff.readIndex]);
        if (readBuff.Length < bodyLength + 2)
        {
            return;
        }
        readBuff.readIndex += 2;
        //解析协议名
        
        int nameCount = 0;
        string protoName = MsgBase.DecodeName(readBuff.bytes, readBuff.readIndex, out nameCount);
       
        if (protoName=="")
        {

            Console.WriteLine("解析协议名失败");
            Close(state);
            return;
        }
        //解析协议体

        readBuff.readIndex += nameCount;
        int bodyCount = bodyLength-nameCount;
       
       MsgBase msgBase  = MsgBase.DecodeMsg(protoName, readBuff.bytes, readBuff.readIndex, bodyCount);
      readBuff.readIndex += bodyCount;
        
        //readBuff.MoveBytes();
       //处理协议
        MethodInfo mei = typeof(MsgHandler).GetMethod(protoName);
        if (mei != null)
        {
            mei.Invoke(null, new object[] { state, msgBase });
        }
        else
        {
            Console.WriteLine("[服务器]未处理协议" + protoName);
        }
        //分发消息
        readBuff.MoveBytes();

     //递归处理未完成的消息
     if(readBuff.Length>2)
       {
         
            OnReceiveData(state);
        }
     

        
    } 
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="state"></param>
    /// <param name="msgBase"></param>
    public static void  Send(ClientState state,MsgBase msgBase)
    {
        Console.WriteLine("[服务器]发送pong");
        if (state == null || !state.clientSocket.Connected)
        {
            return;
        }
        byte[] namebytes = MsgBase.EncodeName(msgBase);
        byte[] bodybytes = MsgBase.EncodeMsg(msgBase);
        int len=namebytes.Length+bodybytes.Length;
        byte[] bytes = new byte[len+2];
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        Array.Copy(namebytes, 0, bytes, 2, namebytes.Length);
        Array.Copy(bodybytes, 0, bytes, 2 + namebytes.Length, bodybytes.Length);

        try
        {
            Console.WriteLine("[服务器]发送数据");
            state.clientSocket.Send(bytes,0,bytes.Length,SocketFlags.None);


        }
        catch (SocketException e)
        {
            Console.WriteLine("[服务器]发送异常" + e.ToString());
        }

    }

    /// <summary>
    /// 超时处理，心跳机制
    /// </summary>
    private static void Timer()
    {
        MethodInfo mInfo = typeof(EventHandle).GetMethod("OnTimer");

        object[] objects = new object[] { };

        mInfo.Invoke(null, objects);
    }
    /// <summary>
    /// 获取时间戳
    /// </summary>
    public static long GetTimeStamp()
    {
      TimeSpan ts= DateTime.UtcNow-new DateTime(1970, 1, 1, 0, 0, 0);

        return Convert.ToInt64(ts.TotalSeconds);
    }

}

