using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 public  class EventHandle
{
    /// <summary>
    /// 掉线处理
    /// </summary>
    /// <param name="clentState"></param>
    public static void OnDisconnect(ClientState clientState)
    {
        if(clientState.player!=null)
        {
            Console.WriteLine("玩家"+clientState.player.id+"掉线了");
            string roomId = clientState.player.roomId;
            if( roomId!="-1")
            {
                Room room = RoomManager.GetRoom(roomId);
                room.RemovePlayer(clientState.player.id);

            }
            DbManager.UpdatePlayerData(clientState.player.id,clientState.player.playerData);
            PlayerManager.RemovePlayer(clientState.player.id);

        }
    }
   /// <summary>
   /// 超时处理
   /// </summary>
    public static void OnTimer()
    {
        CheckPing();
    }
    /// <summary>
    /// 检测心跳机制ping是否超时
    /// </summary>
    public static void CheckPing()
    {
        //遍历所有客户端
        List<ClientState> closeList = new List<ClientState>();
        foreach (ClientState s in NetManager.clients.Values)
        {
            if (NetManager.GetTimeStamp() - s.lastPingTime >4* NetManager.pingInterval)
            {
                Console.WriteLine("心跳机制超时"+s.clientSocket.RemoteEndPoint.ToString());
                closeList.Add(s);
                return;
            }
        }
        //关闭超时的客户端
        foreach (ClientState s in closeList)
        {
            NetManager.Close(s);
        }
    }
   
}

