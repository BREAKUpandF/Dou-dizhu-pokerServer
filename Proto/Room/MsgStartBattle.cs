using System.Collections;
using System.Collections.Generic;


public class MsgStartBattle :MsgBase
{
   public MsgStartBattle()
    {
        protoName = "MsgStartBattle";
    }
    /// <summary>
    /// 0����ɹ� 1���������������ˣ� 2���������δ׼�� 3��ʾ���䲻����
    /// </summary>
    public int result;
}
