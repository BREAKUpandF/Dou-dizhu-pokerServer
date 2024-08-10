using System.Collections;
using System.Collections.Generic;


public class MsgStartBattle :MsgBase
{
   public MsgStartBattle()
    {
        protoName = "MsgStartBattle";
    }
    /// <summary>
    /// 0代表成功 1代表人数不足三人， 2代表有玩家未准备 3表示房间不存在
    /// </summary>
    public int result;
}
