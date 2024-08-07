
public class MsgCall :MsgBase
{
    public MsgCall()
    {
        protoName = "MsgCall";
    }
    //谁叫地主了
    public string id;
    public bool call;

    /// <summary>
    /// 叫地主的结果  0表示继续叫地主 1表示抢地主  2表示三家都不要地主要求重新发牌
    /// </summary>
    public int result;
}
