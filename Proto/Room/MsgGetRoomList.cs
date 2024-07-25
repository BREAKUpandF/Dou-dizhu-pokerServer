using System.Collections;
using System.Collections.Generic;


public class MsgGetRoomList :MsgBase
{
   public MsgGetRoomList()
    {
      protoName = "MsgGetRoomList";
    }
    public RoomInfo[] rooms = null;
}
