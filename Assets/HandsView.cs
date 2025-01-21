using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsView : MonoBehaviour
{
    public Transform parent;

    public CardVisualize cardPrefab;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var info in CSVLoader.Instance.cardDict.Values)
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
