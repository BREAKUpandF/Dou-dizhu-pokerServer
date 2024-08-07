using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class MsgHandler
{
    #region HeatBeat
    public static void MsgPing(ClientState state, MsgBase msgBase)
    {
        state.lastPingTime = NetManager.GetTimeStamp();
        //给客户端发送一个Pong消息
        MsgPong msgPong = new MsgPong();
        NetManager.Send(state, msgPong);
    }
    #endregion
    #region Login
    /// <summary>
    /// 注册消息
    /// </summary>
    /// <param name="state"></param>
    /// <param name="msgBase"></param>
    public static void MsgRegister(ClientState state, MsgBase msgBase)
    {
        MsgRegister msg = msgBase as MsgRegister;
        if (DbManager.Register(msg.id, msg.pw))
        {
            Console.WriteLine(msg.id + "注册成功");
            DbManager.CreadtePlayer(msg.id);
            msg.result = true;
        }
        else
        {
            Console.WriteLine(msg.id + "注册失败");
            msg.result = false;
        }
        //发回给客户端
        NetManager.Send(state, msg);
    }
    public static void MsgLogin(ClientState state, MsgBase msgBase)
    {
        MsgLogin msg = msgBase as MsgLogin;
        if (!DbManager.CheckPassword(msg.id, msg.pw))
        {
            Console.WriteLine(msg.id + "登录失败密码错误");
            msg.result = false;
            NetManager.Send(state, msg);
            return;

        }
        //判断玩家是否已经在线
        if (state.player != null)
        {
            Console.WriteLine(msg.id + "已经登录了");
            msg.result = false;
            NetManager.Send(state, msg);
            return;
        }
        //判断是否在其他地方已经登录
        if (PlayerManager.IsOnline(msg.id))
        {
            Player otherPlayer = PlayerManager.GetPlayer(msg.id);
            MsgKick msgKick = new MsgKick();
            otherPlayer.Send(msgKick);
            NetManager.Close(otherPlayer.state);

        }
        //玩家登录
        PlayerData playerData = DbManager.GetPlayerData(msg.id);
        if (playerData == null)
        {
            Console.WriteLine(msg.id + "没有数据");
            msg.result = false;
            NetManager.Send(state, msg);
            return;
        }
        Player player = new Player(state);
        player.id = msg.id;
        player.playerData = playerData;
        PlayerManager.AddPlayer(player);
        state.player = player;
        msg.result = true;
        NetManager.Send(state, msg);
    }
    #endregion
    #region Room
    public static void MsgCreateRoom(ClientState state, MsgBase msgBase)
    {
        MsgCreateRoom msg = msgBase as MsgCreateRoom;
        if (state.player == null) return;
        if (state.player.roomId != "-1")
        {
            Console.WriteLine("已经在房间内了" + state.player.id);
            msg.result = false;
            NetManager.Send(state, msg);
            return;
        }
        Room room = RoomManager.AddRoom();
        if (room != null)
        {
            room.AddPlayer(state.player.id);
            msg.result = true;
        }
        NetManager.Send(state, msg);
    }
    public static void MsgEnterRoom(ClientState state, MsgBase msgBase)
    {
        MsgEnterRoom msg = msgBase as MsgEnterRoom;
        if (state.player == null) return;
        if (state.player.roomId != "-1")
        {
            Console.WriteLine("已经在房间内了" + state.player.id);
            msg.result = false;
            NetManager.Send(state, msg);
            return;
        }
        Room room = RoomManager.GetRoom(msg.id);
        if (room != null)
        {
            msg.result = room.AddPlayer(state.player.id);
        }
        else
        {
            msg.result = false;
            Console.WriteLine("房间不存在" + msg.id);
        }
        NetManager.Send(state, msg);

    }
    public static void MsgGetAchieve(ClientState state, MsgBase msgBase)
    {
        MsgGetAchieve msg = msgBase as MsgGetAchieve;
        Player player = state.player;
        if (player != null)
        {
            msg.bean = player.playerData.bean;
            NetManager.Send(state, msg);
        }

    }
    //获取房间内玩家信息
    public static void MsgGetRoomInfo(ClientState state, MsgBase msgBase)
    {
        MsgGetRoomInfo msg = msgBase as MsgGetRoomInfo;
        Player player = state.player;
        if (player == null)
            return;
        if (player.roomId == "-1")
            return;
        Room room = RoomManager.GetRoom(player.roomId);
        PlayerInfo[] playerInfo = new PlayerInfo[room.playerList.Count];
        if (room == null)
        {
            NetManager.Send(state, msg);
            return;
        }
        NetManager.Send(state, room.ToMsg());
    }
    public static void MsgGetRoomList(ClientState state, MsgBase msgBase)
    {
        MsgGetRoomList msg = msgBase as MsgGetRoomList;
        Player player = state.player;
        if (player == null) return;
        NetManager.Send(state, RoomManager.ToMsg());

    }
    public static void MsgLeaveRoom(ClientState state, MsgBase msgBase)
    {
        MsgLeaveRoom msg = msgBase as MsgLeaveRoom;
        Player player = state.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            msg.result = false;
            NetManager.Send(state, msg);
        }
        else
        {

            msg.result = room.RemovePlayer(player.id);
            NetManager.Send(state, msg);

        }
    }
    public static void MsgPrepare(ClientState state, MsgBase msgBase)
    {
        MsgPrepare msg = msgBase as MsgPrepare;
        //msg=new MsgPrepare();
        if (state.player == null) return;
        Player player = state.player;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            Console.WriteLine("房间不存在" + player.roomId);
            msg.isPrepare = false;
            player.isPrepare = false;
            NetManager.Send(state, msg);
            return;
        }
        msg.isPrepare = room.Prepare(player.id);


        NetManager.Send(state, msg);
        //重新给房间里每一个人发送房间信息
        room.Broadcast(room.ToMsg());

    }
    public static void MsgStartBattle(ClientState state, MsgBase msgBase)
    {
        MsgStartBattle msg = msgBase as MsgStartBattle;
        Player player = state.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            msg.result = 3;
            NetManager.Send(state, msg);
            return;
        }
        if (room.playerList.Count <= 2)
        {
            msg.result = 1;
            NetManager.Send(state, msg);
            return;
        }
        for (int i = 0; i < room.playerList.Count; i++)
        {
            string id = room.playerList[i];
            if (id == room.hostId) continue;
            else
            {
                if (!room.playerDic.ContainsKey(id) || room.playerDic[id] == false)
                {
                    msg.result = 2;
                    NetManager.Send(state, msg);
                    return;
                }
            }
        }
        //处理房间内的手牌信息
        room.Start();
        msg.result = 0;
        room.Broadcast(msg);//广播所有玩家游戏开始
    }

    #endregion
    #region Battle
    public static void MsgGetCardList(ClientState state, MsgBase msgBase)
    {
        MsgGetCardList msg = msgBase as MsgGetCardList;
        if (state.player == null) return;
        Player player = state.player;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        CardInfo[] playCardInfo = room.GetPlayCardInfoArray(player.id);
        msg.cardInfos = playCardInfo;
        Console.WriteLine("玩家" + player.id + "获取手牌信息" + playCardInfo);
        NetManager.Send(state, msg);
    }
    public static void MsgGetStartPlayer(ClientState state, MsgBase msgBase)
    {
        MsgGetStartPlayer msg = msgBase as MsgGetStartPlayer;
        if (state.player == null) return;
        Player player = state.player;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        msg.id = room.currentPlayerId;
        //我认为不需要广播，因为所有玩家进入battlePanel后都会向服务端请求
        NetManager.Send(state, msg);
    }
    public static void MsgSwitchTurn(ClientState state, MsgBase msgBase)
    {
        MsgSwitchTurn msg = msgBase as MsgSwitchTurn;
        if (state.player == null) return;
        Player player = state.player;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        room.index = (room.index + 1) % 3;
        room.currentPlayerId = room.playerList[room.index];
        msg.id = room.currentPlayerId;
        room.Broadcast(msg);//广播给所有玩家
    }

    //处理叫地主
    public static void MsgCall(ClientState state, MsgBase msgBase)
    {
        MsgCall msg = msgBase as MsgCall;
        if (state.player == null) return;
        Player player = state.player;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        msg.id = player.id;
        if (msg.call)
        {
            room.callId = player.id;
            room.landLordRank[player.id] += 2;
            if (room.CheckCall())
            {
                msg.result = 3;
                room.Broadcast(msg);
                return;
            }
            else
            {
                msg.result = 1;
                room.Broadcast(msg);
                return;
            }
        }
        else
        {
            room.landLordRank[player.id]+=1;
            if (room.CheckAllNotCall())
            {
                msg.result=2;//重新洗牌
            }
            else
            {
                msg.result = 0;//继续叫地主
            }
            room.Broadcast(msg);
            return;

        }
    }
    /// <summary>
    /// 都没有叫地主重新开始
    /// </summary>
    /// <param name="state"></param>
    /// <param name="msgBase"></param>
    public static void MsgReStart(ClientState state, MsgBase msgBase)
    {
        MsgReStart msg = msgBase as MsgReStart;
        if (state.player == null) return;
        Player player = state.player;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        room.Start();
        NetManager.Send(state, msg);
    }
    //获取战斗开始收文件信息
    #endregion
}

