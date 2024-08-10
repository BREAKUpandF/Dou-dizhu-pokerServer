using System;
using System.Collections.Generic;
public enum Suit
{
    None, Club, Square, Heart, Spade
}
public enum Rank
{
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    One,
    Two,
    SJoker,
    LJoker
}
public class Card
{
    public Suit suit;
    public Rank rank;
    public Card(Suit suit, Rank rank)
    {
        this.suit = suit; this.rank = rank;
    }
    public Card(int suit, int rank)
    {
        this.suit = (Suit)suit; this.rank = (Rank)rank;
    }
    public CardInfo GetCardInfo()
    {
        CardInfo info = new CardInfo();
        info.suit = (int)suit;
        info.rank = (int)rank;
        return info;
    }
}
