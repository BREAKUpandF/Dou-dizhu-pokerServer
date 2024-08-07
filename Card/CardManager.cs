using System;
using System.Collections.Generic;

public class CardManager 
{
    public static List<Card> cards = new List<Card>();
    public static List<Card> Shuffle()
    {
        //初始化牌
        cards.Clear();
        cards.Add(new Card(Suit.None,Rank.SJoker));
        cards.Add(new Card(Suit.None, Rank.LJoker));
        for(int i = 1; i < 5; i++)
        {
            for(int j = 0;j < 13; j++)
            {
                cards.Add(new Card(i, j)); // i = suit, j = rank;
            }
        }
        //插入大小王
        
        //洗牌算法
        Queue<Card> queue = new Queue<Card>();
        Random random = new Random();
        for(int i = 0; i < 54; i++)
        {
            
            int index = random.Next(0, cards.Count);
            queue.Enqueue(cards[index]);
            cards.RemoveAt(index);
        }
        for (int i = 0; i < 54; i++)
        {
            cards.Add(queue.Dequeue());
        }
        return cards;
    }
}
