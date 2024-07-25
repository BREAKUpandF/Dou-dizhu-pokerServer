using System.Collections;
using System.Collections.Generic;


public class MsgGetRoomInfo : MsgBase
{
  public MsgGetRoomInfo()
    {
        protoName = "MsgGetRoomInfo";
    }
   public  PlayerInfo[] players;
}
