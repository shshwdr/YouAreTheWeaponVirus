using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pool;
using UnityEngine;

public class HandManager : Singleton<HandManager>
{
    public List<CardInfo> ownedCards = new List<CardInfo>();
    
    public List<CardInfo> deck = new List<CardInfo>();
    public List<CardInfo> handInBattle = new List<CardInfo>();
    public List<CardInfo> discardedInBattle = new List<CardInfo>();
    
    private int handMax = 3;
    public void InitDeck()
    {
        deck = ownedCards.ToList();
    }

    public void useCard(CardInfo info)
    {
        handInBattle.Remove(info);
        if (info.exhaust)
        {
            
        }
        else
        {
            discardedInBattle.Add(info);
        }
        EventPool.Trigger("DrawHand");
    }
    public void DrawHand()
    {
        discardedInBattle.AddRange(handInBattle);
        handInBattle.Clear();
        for (int i = 0; i < handMax; i++)
        {
            if (deck.Count == 0)
            {
                deck = discardedInBattle;
            }

            if (deck.Count == 0)
            {
                break;
            }

            var infect = CSVLoader.Instance.cardDict["infect"];
            if (deck.Contains(infect))
            {
                deck.Remove(infect);
                handInBattle.Add(infect);
            }
            else
            {
                
                handInBattle.Add(deck.PickItem());
            }
        }
        EventPool.Trigger("DrawHand");
    }

    public void ClearBattleHand()
    {
        handInBattle.Clear();
        discardedInBattle.Clear();
        EventPool.Trigger("DrawHand");
    }
    public void AddCard(CardInfo info)
    {
        ownedCards.Add(info);
    }
    public void Init()
    {
        ownedCards.Clear();
        handInBattle.Clear();
        discardedInBattle.Clear();
        foreach (var info in CSVLoader.Instance.cardDict.Values)
        {
            for (int i = 0; i < info.start; i++)
            {
                ownedCards.Add(info);
            }
            
        }
    }
}
