using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public  static class RoomManager
    {
    //房间管理器，用于管理房间
    private static int currentRoomId = 0;
    public static Dictionary<string, Room> rooms = new Dictionary<string, Room>();

    //获取房间是否存在
    public static Room GetRoom(string id)
    {
        return rooms.ContainsKey(id)?rooms[id]: null;
    }
    public static Room AddRoom()
    {
        currentRoomId++;
        Room room = new Room();
        room.id = currentRoomId.ToString();
        rooms.Add(room.id, room);
        return room; 

    }
    public static void RemoveRoom(string id)
    {
        if( rooms.ContainsKey(id))
        {
            rooms.Remove(id);
            Console.WriteLine("房间已删除" + id);
        }
        else
        {
            Console.WriteLine("房间不存在" + id);
        }
    }
    public static MsgBase ToMsg()
    {
         MsgGetRoomList msg= new MsgGetRoomList();
        msg.rooms = new RoomInfo[rooms.Count];
        for( int i = 0; i < rooms.Count; i++)
        {
            RoomInfo roomInfo = new RoomInfo();
            roomInfo.id = rooms.ElementAt(i).Value.id;
            roomInfo.count = rooms.ElementAt(i).Value.playerList.Count;
            roomInfo.isPrepare = rooms.ElementAt(i).Value.status == Room.Status.prepare? true:false;
            roomInfo.hostId = rooms.ElementAt(i).Value.hostId;
            msg.rooms[i] = roomInfo;
        }
        return msg;
    }
}

