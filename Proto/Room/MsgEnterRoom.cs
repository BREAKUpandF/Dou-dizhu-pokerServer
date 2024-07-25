using System.Collections;
using System.Collections.Generic;


public class MsgEnterRoom : MsgBase
{
    public MsgEnterRoom()
    {
        protoName = "MsgEnterRoom";
    }
    public string id; //房间id
    public bool result; //是否成功
  
}
