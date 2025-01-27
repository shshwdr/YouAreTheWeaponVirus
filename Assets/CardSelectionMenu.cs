using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionMenu : MenuBase
{
    public Transform cardsParent;

    private CardVisualize[] cards;
    // Start is called before the first frame update
    void Awake()
    {
        cards = cardsParent.GetComponentsInChildren<CardVisualize>();
    }

    public override void Show()
    {
        base.Show();
        
        HandsView.Instance.gameObject.SetActive(false);
        var cardsToPick = CSVLoader.Instance.cardDict.Values.Where(x => x.canDraw).ToList();
        for (int i = 0; i < 3; i++)
        {
            var pick = cardsToPick.PickItem();
            cards[i].Init(pick);
            cards[i].isDraggable = false;
            cards[i].GetComponent<Button>().enabled = true;
            var tmpi = i;
            cards[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    HandManager.Instance.AddCard(cards[tmpi].cardInfo);
                    Hide();
                    GameRoundManager.Instance.GoToNextLevel();
                }
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
