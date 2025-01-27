using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pool;
using Unity.VisualScripting;
using UnityEngine;

public class Human : MonoBehaviour
{
    [HideInInspector]
    public BuffManager buffManager;
    
    public SpriteRenderer renderer;

    public GameObject sneezePrefab;
    private LevelAsIcons hpBar;
    private float immunityTime = 2f;
    private float immunityTimer = 0;
    public SpriteRenderer imunityRenderer;
    public bool isInfected = false;
    public CharacterInfo info;
    
    
    private Vector3 lastPosition;
    private float staticTime = 1;
    private float staticTimer;

    private float touchTime = 1;
    private float touchTimer = 0;

    public int HP => info.hp;
    public int currentHp = 0;
    public LevelDesignInfo levelDesignInfo;
    public ImageState touchState;
    public bool isRandomMove=>levelDesignInfo.move == null || levelDesignInfo.move.Count==0 ||levelDesignInfo.move.Contains("random");
    float[] speedAdjust = new float[]
    {
        0,1,1.5f,2,2.5f,3,3.5f
    };
    
    public void Init(LevelDesignInfo designInfo)
    {
        levelDesignInfo = designInfo;
        info = CSVLoader.Instance.characterDict[designInfo.type];
        GetComponent<HumanAI>().speed *= speedAdjust[info.speed];
        currentHp = HP;
        GetComponent<CharacterRenderController>().Init(info);
        hpBar = GetComponentInChildren<LevelAsIcons>(true);
        hpBar.Init(currentHp, HP);
        touchState.SetState(0);
    }
    // Start is called before the first frame update
    void Awake()
    {
        buffManager = new BuffManager(this);
    }

    // Update is called once per frame
    void Update()
    {
        immunityTimer+= Time.deltaTime;
        touchTimer += Time.deltaTime;
        if (isRandomMove && lastPosition == transform.position)
        {
            staticTimer += Time.deltaTime;
            if (staticTimer >= staticTime)
            {
                
                GetComponent<HumanAI>().RestartSeek();
                staticTimer = 0;
            }
        }
        lastPosition = transform.position;
        
    }
    
    public bool canBeInfect=>immunityTimer>=immunityTime;

    public void InfectFull()
    {
        isInfected = true;
        EventPool.Trigger("Infect");
        hpBar.gameObject.SetActive(false);
        // renderer.color = Color.green;
        GetComponent<CharacterRenderController>().GetInfected(1);
    }
    public void Infect(CardInfo cardInfo)
    {
        if (immunityTimer < immunityTime)
        {
            return;
        }

        if (isInfected)
        {
            return;
        }

       currentHp -= 1;
       hpBar.Init(currentHp, HP);
       immunityTimer = 0;
       var color = imunityRenderer.color;
       color.a = 0.5f;
       imunityRenderer.color = color;
       imunityRenderer.DOFade(1, immunityTime/4f).SetLoops(4, LoopType.Yoyo);
       StartCoroutine(resetImmunityRenderer());
       if (currentHp <= 0)
       {

           InfectFull();
       }
       
        EventPool.Trigger("Infect");

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_human_infected");
    }

    IEnumerator resetImmunityRenderer()
    {
        yield return new WaitForSeconds(immunityTime);
        var color = imunityRenderer.color;
        color.a = 0f;
        imunityRenderer.color = color;
    }
    
    public void Sneeze(CardInfo cardInfo)
    {
        var go = Instantiate(sneezePrefab, transform.position, Quaternion.identity,GameRoundManager.Instance.tempTrans);

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_man_sneeze");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 获取碰撞到的物体的名称
        Debug.Log("Collided with: " + other.gameObject.name);

        // 获取碰撞的接触点
        // foreach (var contact in other.contacts)
        // {
        //     Debug.Log("Contact Point: " + contact.point);
        // }

        if (isInfected )
        {
            
            if (other.gameObject.GetComponent<Human>())
            {
            
                var human = other.gameObject.GetComponent<Human>();
                if (!human.isInfected && human.canBeInfect)
                {
                    if (buffManager.GetBuffValue("touch") > 0&& touchTimer>touchTime)
                    {
                        touchTimer = 0;
                        buffManager.AddBuff("touch", -1);
                        human.Infect(null);
                    }
                }
            }
        }

        GetComponent<HumanAI>().RestartSeek();
    }
}
