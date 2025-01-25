using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pool;
using UnityEngine;

public class HandManager : Singleton<HandManager>
{
    public List<CardInfo> hand = new List<CardInfo>();
    
    public List<CardInfo> deck = new List<CardInfo>();
    public List<CardInfo> handInBattle = new List<CardInfo>();

    public void InitDeck()
    {
        deck = hand.ToList();
    }
    public void DrawHand()
    {
        handInBattle = deck.ToList();
        EventPool.Trigger("DrawHand");
    }
    public void AddCard(CardInfo info)
    {
        hand.Add(info);
    }
    public void Init()
    {
        hand.Clear();
        foreach (var info in CSVLoader.Instance.cardDict.Values)
        {
            for (int i = 0; i < info.start; i++)
            {
                hand.Add(info);
            }
            
        }
    }
}
