using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject stonePrefab;
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

        var radius = float.Parse(cardInfo.actions[1]);
        selectionCircle.transform.localScale = Vector3.one * radius;
        selectionCircle.SetActive(true);
        PlayerControllerManager.Instance.StartDragging(selectionCircle,this);
    }

    public bool CanPlace()
    {
        var res = true;

        if (cardInfo.actions[0] == "summonStone")
        {
            Collider2D[] results = new Collider2D[20]; // 假设最多检测 10 个碰撞体

            // 检测重叠
            ContactFilter2D contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = true; // 允许触发器参与检测
            int count = selectionCircle.GetComponent<Collider2D>().OverlapCollider(contactFilter, results);
            for (int i = 0; i < count; i++)
            {
                //Debug.Log($"Overlapping with: {results[i].name}");
                if (results[i].GetComponent<Human>())
                {
                    var human = results[i].GetComponent<Human>();
                    if (human && !human.isDead)
                    {
                        return false;
                    }
                }
            }
        }

        return res;
    }
    public void OnPlace()
    {

        FindObjectOfType<TutorialMenu>().StartCoroutine( FindObjectOfType<TutorialMenu>().FinishUseCard());
       // Debug.LogError("place");
        Collider2D[] results = new Collider2D[20]; // 假设最多检测 10 个碰撞体

        // 检测重叠
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;  // 允许触发器参与检测
        int count = selectionCircle.GetComponent<Collider2D>().OverlapCollider(contactFilter, results);

        HandManager.Instance.useCard(cardInfo);
        bool foundTarget = false;
        for (int i = 0; i < count; i++)
        {
            //Debug.Log($"Overlapping with: {results[i].name}");
            if (results[i].GetComponent<Human>())
            {
                var human = results[i].GetComponent<Human>();
                switch (cardInfo.actions[0])
                {
                    case "infect":
                        if (!human.isInfected)
                        {
                            
                        foundTarget = true;
                        //Debug.LogError("infect");
                        human.Infect(cardInfo);
                        }
                        break;
                    case "infectNonHuman":
                        if (human.isAnimal || human.isBin)
                        {
                            human.Infect(cardInfo);
                        }
                        break;
                    case "infectInanimated":
                        if (human.isBin)
                        {
                            human.Infect(cardInfo);

                            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_bin_contamination");
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
                            FindObjectOfType<TutorialMenu>().FinishSneeze();
                        }
                        break;
                    
                    case "stun":
                        if (human.isHuman)
                        {
                            
                            results[i].GetComponent<Human>().Stun(cardInfo);

                            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_stunt");
                        }
                        break;
                    
                    case "explodeNonHuman":
                        
                        if (!human.isHuman &&  human.isInfected)
                        {
                            foundTarget = true;
                            results[i].GetComponent<Human>().Explode(cardInfo, float.Parse(cardInfo.actions[1]));
                            //human.Hide();
                        }
                        break;
                    case "explodeBin":
                        
                        if (human.isBin &&  human.isInfected)
                        {
                            
                            results[i].GetComponent<Human>().Explode(cardInfo, float.Parse(cardInfo.actions[1]));
                            //human.Hide();

                            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_bin_explosion");
                        }
                        break;
                    case "explodeHuman":
                        
                        if (human.isHuman &&  human.isInfected)
                        {
                            foundTarget = true;
                            results[i].GetComponent<Human>().Explode(cardInfo, float.Parse(cardInfo.actions[1]));
                            //human.Hide();

                            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_human_explosion");
                        }
                        break;
                    case "explodeAnimal":
                        
                        if (human.isAnimal &&  human.isInfected)
                        {
                            results[i].GetComponent<Human>().Explode(cardInfo, float.Parse(cardInfo.actions[1]));
                            //human.Hide();

                            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_animal_explosion");
                        }
                        break;
                    case "touch":

                        if (human.isInfected)
                        {
                            results[i].GetComponent<Human>().buffManager
                                .SetBuff("touch", int.Parse(cardInfo.actions[1]));

                            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_touch_infection");
                        }

                        break;
                    case "summonStone":
                        foundTarget = true;
                        break;
                        
                }

                if (foundTarget && cardInfo.selectType == 1)
                {
                    break;
                }
            }
        }

        if (cardInfo.actions[0] == "teleport")
        {
            var infectedHuman = HumanSpawner.Instance.humans.Where(h => h.isInfected && !h.isDead).ToList();
            if (infectedHuman.Count > 0)
            {
                var infected = infectedHuman.PickItem();
                infected.Teleport(selectionCircle.transform.position);

                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_teleport");
            }
        }

        if (cardInfo.actions[0] == "summonStone")
        {
            if (!foundTarget)
            {
                
                var go = Instantiate(stonePrefab, selectionCircle.transform.position, Quaternion.identity, GameRoundManager.Instance.tempTrans);
                
                Physics2D.SyncTransforms();

                FindObjectOfType<LevelController>().RescanAndReSeek();

                FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_create_rock");

            }
        }
        
        foreach (var human in HumanSpawner.Instance.humans)
        {
            human.DrawOutline(false);
        }
        
        selectionCircle.SetActive(false);
        ExitCard();
        //Destroy(gameObject);
    }

    public void Cancel()
    {
        foreach (var human in HumanSpawner.Instance.humans)
        {
            human.DrawOutline(false);
        }
        
        selectionCircle.SetActive(false);
        ExitCard();
    }

    public bool OnDrag()
    {
        Collider2D[] results = new Collider2D[20]; // 假设最多检测 10 个碰撞体

        // 检测重叠
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;  // 允许触发器参与检测
        int count = selectionCircle.GetComponent<Collider2D>().OverlapCollider(contactFilter, results);


        foreach (var human in HumanSpawner.Instance.humans)
        {
            human.DrawOutline(false);
        }
        bool foundTarget = false;
        for (int i = 0; i < count; i++)
        {
            //Debug.Log($"Overlapping with: {results[i].name}");
            if (results[i].GetComponent<Human>())
            {
                var human = results[i].GetComponent<Human>();
                switch (cardInfo.actions[0])
                {
                    case "infect":
                        if (!human.isInfected)
                        {
                            foundTarget = true;
                            human.DrawOutline(true);
                        }
                        break;
                    // case "infectInanimated":
                    //     if (human.isBin && !human.isInfected)
                    //     {
                    //         foundTarget = true;
                    //         human.DrawOutline(true);
                    //     }
                    //
                    //     break;
                    case "infectNonHuman":
                        if (!human.isHuman && !human.isInfected)
                        {
                             foundTarget = true;
                            human.DrawOutline(true);
                        }
                        break;
                    // case "infectAnimal":
                    //     if (human.isAnimal)
                    //     {
                    //         human.DrawOutline(true);
                    //     }
                    //     break;
                    case "sneeze":
                        if (human.isHuman &&  human.isInfected)
                        {
                            foundTarget = true;
                            human.DrawOutline(true);
                        }
                        break;
                    
                    case "stun":
                        
                        if (human.isHuman)
                        {
                            foundTarget = true;
                            human.DrawOutline(true);
                        }
                        break;
                    
                    case "explodeNonHuman":
                        
                        if (!human.isHuman &&  human.isInfected)
                        {
                            foundTarget = true;
                            human.DrawOutline(true);
                            //human.Hide();
                        }
                        break;
                    // case "explodeBin":
                    //     
                    //     if (human.isBin &&  human.isInfected)
                    //     {
                    //         
                    //         human.DrawOutline(true);
                    //     }
                    //     break;
                    case "explodeHuman":
                        
                        if (human.isHuman &&  human.isInfected)
                        {
                            
                            foundTarget = true;
                            human.DrawOutline(true);
                        }
                        break;
                    // case "explodeAnimal":
                    //     
                    //     if (human.isAnimal &&  human.isInfected)
                    //     {
                    //         human.DrawOutline(true);
                    //     }
                    //     break;
                    case "touch":

                        if (human.isInfected)
                        {
                            foundTarget = true;
                            human.DrawOutline(true);
                        }

                        break;
                    case "summonStone":
                    {
                        
                        foundTarget = true;
                        human.DrawOutline(true);
                    }
                        break;
                }
            }
            
            if (foundTarget && cardInfo.selectType == 1)
            {
                break;
            }
        }

        if (cardInfo.actions[0] == "teleport")
        {
            return HumanSpawner.Instance.infectedAnythingCount() > 0;
        }
        else if (cardInfo.actions[0] == "summonStone")
        {
            return !foundTarget;
        }

        return foundTarget;

        //Destroy(gameObject);
    }

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!setPosition|| !isDraggable)
        {
            return;
        }
        transform.position = hoverPos;

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_mouse_over_card");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!setPosition || !isDraggable)
        {
            return;
        }

        if (PlayerControllerManager.Instance.currentDraggingCell == this)
        {
            
        }
        else
        {
            ExitCard();
        }
        
    }

    public void ExitCard()
    {
        if (this && transform)
        {
            
            transform.position = startPos;
        }
    }
}
