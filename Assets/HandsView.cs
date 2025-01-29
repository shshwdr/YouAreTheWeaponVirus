using System.Collections;
using System.Collections.Generic;
using Pool;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class HandsView : Singleton<HandsView>
{
    public Transform parent;

    public CardVisualize cardPrefab;
    private int drawTime = 10;
    private float drawTimer = 0;
    public Button drawButton;

    public TMP_Text drawTimeText;
    // Start is called before the first frame update
    void Start()
    {
        // foreach (var info in HandManager.Instance.hand)
        // {
        //     var go = Instantiate(cardPrefab.gameObject, parent);
        //     go.GetComponent<CardVisualize>().Init(info);
        // }
        EventPool.OptIn("DrawHand", UpdateHands);
        UpdateHands();
        drawButton.onClick.AddListener(() =>
        {
            DrawCard();
            FindObjectOfType<TutorialMenu>(). FinishUseRedraw();
        });
        drawButton.gameObject.SetActive(false);
    }

    public void showRedrawButton()
    {
        drawButton.gameObject.SetActive(true);
    }

    public void DrawCard()
    {
        if (drawTimer > 0)
        {
            return;
        }
        drawTimer = drawTime;
        HandManager.Instance.DrawHand();
        UpdateHands();

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_draw_card");
    }
    
    public void UpdateHands()
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
        foreach (var info in HandManager.Instance.handInBattle)
        {
            var go = Instantiate(cardPrefab.gameObject, parent);
            go.GetComponent<CardVisualize>().Init(info);
        }
    }

    // Update is called once per frame
    void Update()
    {
        drawTimer-= Time.deltaTime;
        drawButton.interactable = drawTimer <= 0;
        drawTimeText.gameObject.SetActive(drawTimer > 0);
        if (drawTimer > 0)
        {
            drawTimeText.text = (math.ceil(drawTimer)).ToString();
        }
    }
}
