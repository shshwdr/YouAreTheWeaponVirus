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

    protected override void Start()
    {
        base.Start();
        for (int i = 0; i < 3; i++)
        {
            var tmpi = i;
            cards[i].GetComponent<Button>().onClick.AddListener(() =>
                {
                    HandManager.Instance.AddCard(cards[tmpi].cardInfo);
                    Hide();
                    GameRoundManager.Instance.GoToNextLevel();

                    FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_select_new_card");

                }
            );
        }
    }

    public override void Show()
    {
        base.Show();
        HumanSpawner.Instance.clear();
        GameHud.Instance.gameObject.SetActive(false);
        HandsView.Instance.gameObject.SetActive(false);
        
        var cardsToPick = CSVLoader.Instance.cardDict.Values.Where(x => x.canDraw&&x.unlockAt<GameRoundManager.Instance.currentLevel).ToList();
var cardsCurrentLeve = CSVLoader.Instance.cardDict.Values.Where(x => x.canDraw&&x.unlockAt==GameRoundManager.Instance.currentLevel).ToList();
int i = 0;
foreach (var info in cardsCurrentLeve)
{
    
    cards[i].Init(info);
    cards[i].isDraggable = false;
    cards[i].GetComponent<Button>().enabled = true;
    i++;
}        

for (; i < 3; i++)
        {
            var pick = cardsToPick.PickItem();
            cards[i].Init(pick);
            cards[i].isDraggable = false;
            cards[i].GetComponent<Button>().enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
