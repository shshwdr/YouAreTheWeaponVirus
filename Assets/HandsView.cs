using System.Collections;
using System.Collections.Generic;
using Pool;
using UnityEngine;

public class HandsView : MonoBehaviour
{
    public Transform parent;

    public CardVisualize cardPrefab;
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
        
    }
}
