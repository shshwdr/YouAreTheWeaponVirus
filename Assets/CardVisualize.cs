using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class CardVisualize : MonoBehaviour, IPointerDownHandler
{

    public Image image;

    public bool isDraggable = true;
    public TMP_Text text;
    
    private GameObject selectionCircle;
    public GameObject selectionCirclePrefab;

    private CardInfo cardInfo;

    public void Init(CardInfo info)
    {
        cardInfo = info;
        text.text = cardInfo.identifier;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        if (!isDraggable)
        {
            return;
        }

        if (selectionCircle == null)
        {
            
            selectionCircle = Instantiate(selectionCirclePrefab);
        }
        
        selectionCircle.SetActive(true);
        PlayerControllerManager.Instance.StartDragging(selectionCircle,this);
    }

    public void OnPlace()
    {
        Collider2D[] results = new Collider2D[10]; // 假设最多检测 10 个碰撞体

        // 检测重叠
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;  // 允许触发器参与检测
        int count = selectionCircle.GetComponent<Collider2D>().OverlapCollider(contactFilter, results);

        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Overlapping with: {results[i].name}");
            if (results[i].GetComponent<Human>())
            {
                var human = results[i].GetComponent<Human>();
                switch (cardInfo.actions[0])
                {
                    case "infect":
                        
                        results[i].GetComponent<Human>().Infect(cardInfo);
                        break;
                    case "sneeze":
                        if (human.isInfected)
                        {
                            
                            results[i].GetComponent<Human>().Sneeze(cardInfo);
                        }
                        break;
                    case "touch":
                        
                        results[i].GetComponent<Human>().buffManager.AddBuff("touch",int.Parse(cardInfo.actions[1]));
                        break;
                }
            }
        }
        
        
        
        selectionCircle.SetActive(false);
        //Destroy(gameObject);
    }

    
}
