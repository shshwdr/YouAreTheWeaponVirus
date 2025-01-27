using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class CardVisualize : MonoBehaviour, IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler
{

    public Image image;

    public bool isDraggable = true;
    public TMP_Text text;
    public TMP_Text desc;
    
    private GameObject selectionCircle;
    public GameObject selectionCirclePrefab;


    public Transform hoverTrans;
    Vector3 startPos;
    private Vector3 hoverPos;

    public CardInfo cardInfo;
    public bool setPosition;

    public void Init(CardInfo info)
    {
        cardInfo = info;
        text.text = cardInfo.title;
        desc.text = cardInfo.desc;
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(setposition());
    }

    IEnumerator setposition()
    {
        yield return new WaitForSeconds(0.01f);
        setPosition = true;
        startPos = transform.position;
        hoverPos = hoverTrans.position;
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

        HandManager.Instance.useCard(cardInfo);
        
        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Overlapping with: {results[i].name}");
            if (results[i].GetComponent<Human>())
            {
                var human = results[i].GetComponent<Human>();
                switch (cardInfo.actions[0])
                {
                    case "infect":
                        
                        human.Infect(cardInfo);
                        break;
                    case "infectInanimated":
                        if (human.isBin)
                        {
                            human.Infect(cardInfo);
                        }

                        break;
                    case "infectAnimal":
                        if (human.isAnimal)
                        {
                            human.Infect(cardInfo);
                        }
                        break;
                    case "sneeze":
                        if (human.isHuman &&  human.isInfected)
                        {
                            
                            results[i].GetComponent<Human>().Sneeze(cardInfo);
                        }
                        break;
                    case "explodeBin":
                        
                        if (human.isBin &&  human.isInfected)
                        {
                            
                            results[i].GetComponent<Human>().Explode(cardInfo, float.Parse(cardInfo.actions[1]));
                        }
                        break;
                    case "touch":

                        if (human.isInfected)
                        {
                            results[i].GetComponent<Human>().buffManager
                                .SetBuff("touch", int.Parse(cardInfo.actions[1]));
                        }

                        break;
                }
            }
        }
        
        
        
        selectionCircle.SetActive(false);
        //Destroy(gameObject);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!setPosition|| !isDraggable)
        {
            return;
        }
        transform.position = hoverPos;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!setPosition || !isDraggable)
        {
            return;
        }
         transform.position = startPos;
    }
}
