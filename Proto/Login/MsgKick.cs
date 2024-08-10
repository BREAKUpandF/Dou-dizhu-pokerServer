using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 玩家下线
/// </summary>
public class MsgKick : MsgBase
{
    public MsgKick()
    {
       protoName = "MsgKick";
    }
    public bool isKick = true;


}
