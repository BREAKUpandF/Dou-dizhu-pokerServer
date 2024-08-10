using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    public static class PlayerManager
    {
    private static Dictionary<string, Player> players = new Dictionary<string, Player>();
    public static bool IsOnline(string id)
    {
        return players.ContainsKey(id);//判断玩家是否在线
    }

    public static Player GetPlayer(string id)
    {
        if (players.ContainsKey(id))
            return players[id];
        else return null;
    }
    /// <summary>
    /// 添加玩家到在线列表
    /// </summary>
    /// <param name="player"></param>
    public static void AddPlayer(Player player)
    {
        players.Add(player.id, player);

    }
    public static void RemovePlayer(string id)
    {
        players.Remove(id);
    }
}

