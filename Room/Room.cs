using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


public  class Room
{
    public string id = "0";
    /// <summary>
    /// 玩家
    /// </summary>
    public List<string> playerList = new List<string>();
  public Dictionary<string, bool> playerDic = new Dictionary<string, bool>();
    /// <summary
    // 一个房间一套牌
    /// </summary>
    public List<Card> cardList = new List<Card>();

    /// <summary
    // 房间内的每个文件都有17张牌或20张牌
    /// </summary>
    public Dictionary<string, List<Card>> playerCards = new Dictionary<string, List<Card>>();

    /// <summary>
    /// 房间的最大玩家数量
    /// </summary>
    public int maxPlayer = 3;
    public string hostId = "";

    /// <summary>
    /// 当前操作的玩家id
    /// </summary>
    public string currentPlayerId;

    ///<summary>
    ///当前操作的玩家在list的索引
    ///</summary>
    public int index = 0;

    /// <summary>
    /// 房间状态
    /// </summary>
    public enum Status
    {
        prepare,
        start
    }
    public Room()
    {
        //初始化牌
        cardList = CardManager.Shuffle();
    }
    public Status status = Status.prepare;
    public bool AddPlayer(string id)
    {
        if (playerList.Count >= maxPlayer)
        {
            Console.WriteLine("Room.AddPlayer: 房间已满");
            return false;
        }
        Player player = PlayerManager.GetPlayer(id);
        if (player == null)
        {
            Console.WriteLine("Room.AddPlayer: 玩家为空");
            return false;
        }
        if (this.status == Status.start)
        {
            Console.WriteLine("Room.AddPlayer: 房间已经开始");
            return false;
        }
        if (playerList.Contains(id))
        {
            Console.WriteLine("Room.AddPlayer: 玩家已存在" + "玩家id:" + id);
            return false;
        }
        playerList.Add(id);
        player.roomId = this.id;
        if (playerList.Count == 1 || hostId == "")
        {
            hostId = player.id;
            player.isHost = true;
        }
        Console.WriteLine("房间人数" + playerList.Count);
        Broadcast(ToMsg());//广播其他人，某玩家加入房间
        return true;
    }
    public bool RemovePlayer(string id)
    {
        Player player = PlayerManager.GetPlayer(id);
        if (player == null)
        {
            Console.WriteLine("Room.RemovePlayer: 玩家为空" + "玩家id:" + id);
            return false;
        }
        if (playerList.Contains(id))
        {
            playerList.Remove(id);
            player.roomId = "-1";
            player.isPrepare = false;
            if(playerDic.ContainsKey(id))//如果玩家在准备字典中，移除
                playerDic.Remove(id);
            if(player.isHost)
            {
                player.isHost = false;
                if( playerList.Count== 0)
                RoomManager.RemoveRoom(this.id);
                else
                {
                    hostId = playerList[0];
                    PlayerManager.GetPlayer(hostId).isHost = true;
                }
            }
            Broadcast(ToMsg());//广播其他人，某玩家离开房间
            return true;
        }
        return false;
    }
    /// <summary>
    /// 广播其他玩家，更新房间内容
    /// </summary>
    /// <returns></returns>
    public void  Broadcast(MsgBase msg)
    {
       foreach( string id in playerList)
        {
            Player player = PlayerManager.GetPlayer(id);
            NetManager.Send(player.state, msg);
        }
    }
    public MsgGetRoomInfo ToMsg()
    {
        MsgGetRoomInfo msg = new MsgGetRoomInfo();
        PlayerInfo[] playerInfo = new PlayerInfo[playerList.Count];
        for (int i = 0; i < playerList.Count; i++)
        {
           
            Player p = PlayerManager.GetPlayer(playerList[i]);
            playerInfo[i] = new PlayerInfo();
            if (p == null)
            {
                Console.WriteLine("Room.ToMsg: 玩家为空" );
            }
            if (p.id != null)
            { Console.WriteLine("Room.ToMsg: 玩家id" + p.id);
                playerInfo[i].id = p.id;
              
            
            }
            if (p.playerData == null)
            {
                Console.WriteLine("Room.ToMsg: 玩家数据null");
               
            }
            else
            {
                Console.WriteLine("Room.ToMsg: 玩家数据" + p.playerData.bean);
            }
            playerInfo[i].bean = p.playerData.bean;
          
            playerInfo[i].isHost = p.isHost;

            playerInfo[i].isPrepare = p.isPrepare;
        }
        msg.players = playerInfo;
        Console.WriteLine(msg);
       
        return msg;
    }
    public bool  Prepare(string id)
    {
        Player player = PlayerManager.GetPlayer(id);
        if (player == null) {
            Console.WriteLine("Room.Prepare: 玩家为空" + "玩家id:" + id);
            return false; 
        }
        if (!playerList.Contains(id))
        {

            Console.WriteLine("Room.Prepare: 玩家不在房间内" + "玩家id:" + id);
            return false;
        }
      
        if (!playerDic.ContainsKey(id))
        {
          playerDic[id] = true;
            //return true;
        }
        else
        {
           playerDic[id] = !playerDic[id];
            //return true;
        }
        player.isPrepare = playerDic[id];
        return playerDic[id];
    }
    public  void Start()
    {
        Random rand = new Random();
       this.index = rand.Next(3);
        currentPlayerId = playerList[this.index];
        cardList = CardManager.Shuffle();
        //给每个玩家发牌
        int index = 0;
        foreach (string id in playerList)
        {
          playerCards[id] =  FireCards(index);
            index += 17;
        }
        //设置底牌
        List<Card> bottomCards = new List<Card>();
        for(int i =index; i < index + 3; i++)
        {
            bottomCards.Add(cardList[i]);
        }
        //设置""来存储底牌
        playerCards.Add("", bottomCards);
    }
    private List<Card> FireCards(int index)
    {
        List<Card> card = new List<Card>();
        for (int i = index; i <index + 17; i++)
        {
            card.Add(cardList[i]);
        }
        return card;
    }
    public CardInfo[] GetPlayCardInfoArray(string id)
    {
        List<Card> cards = playerCards[id];
        List<CardInfo> cardInfos = new List<CardInfo>();
        foreach (Card card in cards)
        {
            CardInfo cardInfo = card.GetCardInfo();
            cardInfos.Add(cardInfo);
        }
        return cardInfos.ToArray();
    }
}

