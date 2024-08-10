using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Player
{
    public string id = "";
    public ClientState state;
    public PlayerData playerData;
    /// <summary>
    /// /// 玩家是否是房主
    /// </summary>
    public bool isHost = false;

    public bool isPrepare= false;
    /// <summary>
    /// 房间号
    /// -1表示不在任何房间
    /// </summary>
    /// <param name="state"></param>
    public string roomId = "-1";
    public Player(ClientState state)
    {
        this.state = state;
    }
    public void Send(MsgBase msgBase)
    {
        NetManager.Send(state, msgBase);
    }
   
}

