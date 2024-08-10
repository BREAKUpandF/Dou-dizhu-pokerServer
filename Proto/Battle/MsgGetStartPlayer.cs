
using System.Collections;
public class MsgGetStartPlayer : MsgBase
{
   public MsgGetStartPlayer()
    {
        protoName = "MsgGetStartPlayer";
    }
    public string id;//代表开始叫地主玩家的id
}
