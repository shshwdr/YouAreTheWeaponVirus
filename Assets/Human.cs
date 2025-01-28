using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pathfinding;
using Pool;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Human : MonoBehaviour
{
    [HideInInspector]
    public BuffManager buffManager;
    
    public SpriteRenderer renderer;

    public GameObject sneezePrefab;
    public GameObject explodePrefab;
    private LevelAsIcons hpBar;
    private float immunityTime = 2f;
    private float immunityTimer = 0;
    public SpriteRenderer imunityRenderer;
    public bool isInfected = false;
    public CharacterInfo info;
    private CharacterRenderController characterRenderer;
    public GameObject ouline;
    private HumanAI ai;
    public bool isHuman => info.characterType == "human";
    public bool isBin => info.characterType == "bin";
    public bool isAnimal=> info.characterType == "bird" || info.characterType == "squirrel";
    
    
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

    public void DrawOutline(bool show)
    {
        if (isDead)
        {
            return;
        }
        ouline.GetComponent<SpriteRenderer>().enabled = (show);
    }
    
    public void Init(LevelDesignInfo designInfo)
    {
        levelDesignInfo = designInfo;
        info = CSVLoader.Instance.characterDict[designInfo.type];
        GetComponent<HumanAI>().speed *= speedAdjust[info.speed];
        currentHp = HP;
        characterRenderer = GetComponent<CharacterRenderController>();
        characterRenderer.Init(info);
        hpBar = GetComponentInChildren<LevelAsIcons>(true);
        hpBar.Init(currentHp, HP);
        touchState.SetState(0);
        ai = GetComponent<HumanAI>();
    }
    // Start is called before the first frame update
    void Awake()
    {
        buffManager = new BuffManager(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
        {
            return;
        }
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
        if (isDead)
        {
            return;
        }
        isInfected = true;
        EventPool.Trigger("Infect");
        hpBar.gameObject.SetActive(false);
        // renderer.color = Color.green;
        GetComponent<CharacterRenderController>().GetInfected(1);
    }
    public void Infect(CardInfo cardInfo)
    {
        if (isDead)
        {
            return;
        }
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

    private bool isPausedMoving = true;
    public void Sneeze(CardInfo cardInfo)
    {
        if (isDead)
        {
            return;
        }
       // var go = Instantiate(sneezePrefab, transform.position, Quaternion.identity,GameRoundManager.Instance.tempTrans);

        ai.StopSeekPath();
        
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_man_sneeze");
        characterRenderer.UpdateDir(Vector3.down);
        characterRenderer.sneeze.PlayOnce();
        isPausedMoving = true;
        StartCoroutine(SneezeEnumerator());
    }

    IEnumerator SneezeEnumerator()
    {
        yield return new WaitForSeconds(0.5f);
        var go = Instantiate(sneezePrefab, transform.position, Quaternion.identity,GameRoundManager.Instance.tempTrans);
       
        isPausedMoving = false;
        ai.FindNextPath();
        
    }
    
    public void Explode(CardInfo cardInfo,float radius)
    {
        if (isDead)
        {
            return;
        }
        var go = Instantiate(explodePrefab, transform.position, Quaternion.identity,GameRoundManager.Instance.tempTrans);
        go.GetComponent<ExplodeArea>().Init(radius);
        characterRenderer.explosion.PlayOnce();
        Die();
    }

    public bool isDead = false;
    public void Die()
    {
        isDead = true;
        ai.StopSeekPath();
        StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(0.5f);
        characterRenderer.renderersParent.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead)
        {
            return;
        }
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

        if (!isPausedMoving)
        {
            GetComponent<HumanAI>().RestartSeek();
        }
    }
}
