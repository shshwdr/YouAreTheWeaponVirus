using System;
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
    public GameObject teleportPrefab;
    public GameObject deathEffect;
    private LevelAsIcons hpBar;
    private float immunityTime = 2f;
    private float immunityTimer = 0;
    private float stunTime = 5;
    private float stunTimer = 10;
    public SpriteRenderer imunityRenderer;
    public bool isInfected = false;
    public CharacterInfo info;
    private CharacterRenderController characterRenderer;
    public GameObject ouline;
    public GameObject stunObject;
    private HumanAI ai;
    public bool isHuman => info.characterType == "human";
    public bool isBin => info.characterType == "bin";
    public bool isAnimal=> info.characterType == "bird" || info.characterType == "squirrel";
    
    
    private Vector3 lastPosition;
    private float staticTime = 1;
    private float staticTimer;

    private float touchTime = 1;
    private float touchTimer = 0;

    private float skillTime = 0.3f;
    private float skillTimer = 0;
    private bool isSkill = false;
    public bool isStuned => stunTimer <= stunTime;
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
        immunityTimer = immunityTime;
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

        var wasStunned = isStuned;
        immunityTimer+= Time.deltaTime;
        touchTimer += Time.deltaTime;
        
        stunTimer+= Time.deltaTime;

        if (wasStunned && !isStuned)
        {
            FinishedStun();
        }

        if (isStuned)
        {
            return;
        }

        if (isSkill)
        {
            return;
        }
        
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

    public void RecoverFromInfect()
    {
        
        if (isDead)
        {
            return;
        }
        isInfected = false;
        hpBar.gameObject.SetActive(true);
        GetComponent<CharacterRenderController>().GetInfected(0);
        
        buffManager.SetBuff("touch",0);
    }

    public void Stun(CardInfo info)
    {
        if (!isStuned)
        {
            
            stunTimer = 0;
            StopMoving();
            stunObject.SetActive(true);
        }
    }

    void FinishedStun()
    {
        RestartMoving();
        stunObject.SetActive(false);
    }
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
        characterRenderer.HideDetector();

    }

    public void Attack(Human human)
    {
        if (isDead)
        {
            return;
        }
        
        characterRenderer.skillSamuriAnimation.PlayOnce();
        StopMoving();
        human.StopMoving();
        characterRenderer.mainRenderer.gameObject.SetActive(false);
        characterRenderer.UpdateDir(Vector3.down);

        if (human.transform.position.x > transform.position.x)
        {
            var scale = characterRenderer.renderersParent.transform.localScale;
            scale.x = -1;
            characterRenderer.renderersParent.transform.localScale = scale;
        }
        
        isSkill = true;
        StartCoroutine(ienumeratorAttack(human));
    }

    private Vector3 samuraiSkillPosition;
    IEnumerator ienumeratorAttack(Human human)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_ninja_slice");

        yield return new WaitForSeconds(0.5f);
        human.Die();
        samuraiSkillPosition = characterRenderer.skillSamuriAnimation.transform.position;
        characterRenderer.skillSamuriAnimation.transform.DOMove(human.transform.position,0.6f);
        yield return new WaitForSeconds(0.6f);
        yield return new WaitForSeconds(0.4f);
        characterRenderer.skillSamuriAnimation.transform.DOMove(samuraiSkillPosition,0.3f);
        //human.RestartMoving();
        yield return new WaitForSeconds(0.6f);
        //if (human.transform.position.x > transform.position.x)
        {
            var scale = characterRenderer.renderersParent.transform.localScale;
            scale.x = 1;
            characterRenderer.renderersParent.transform.localScale = scale;
        }
        characterRenderer.mainRenderer.gameObject.SetActive(true);
        RestartMoving();
        isSkill = false;
    }
    public void HealOther(Human human)
    {
        characterRenderer.skillAnimation.PlayOnce();
        StopMoving();
        human.StopMoving();
        characterRenderer.mainRenderer.gameObject.SetActive(false);
        characterRenderer.UpdateDir(Vector3.down);
        isSkill = true;
        StartCoroutine(ienumeratorHeal(human));
    }
    public void Heal()
    {
        if (currentHp >= HP)
        {
            return;
        }
        characterRenderer.healAnimation.PlayOnce();
        if (currentHp <= 0)
        {
            currentHp = 0;
            RecoverFromInfect();
        }
        currentHp++;
        hpBar.Init(currentHp, HP);

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_doctor_spray");
    }

    IEnumerator ienumeratorHeal(Human human)
    {
        yield return new WaitForSeconds(0.5f);
        human.Heal();
        yield return new WaitForSeconds(0.6f);
        human.RestartMoving();
        yield return new WaitForSeconds(0.75f);
        characterRenderer.mainRenderer.gameObject.SetActive(true);
        RestartMoving();
        isSkill = false;
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

        characterRenderer.infectAnimation.PlayOnce();
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

    public bool isPausedMoving = false;
    public void Sneeze(CardInfo cardInfo)
    {
        if (isDead)
        {
            return;
        }
       // var go = Instantiate(sneezePrefab, transform.position, Quaternion.identity,GameRoundManager.Instance.tempTrans);
       StopMoving();
        
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_man_sneeze");
        characterRenderer.UpdateDir(Vector3.down);
        characterRenderer.sneeze.PlayOnce();
        StartCoroutine(SneezeEnumerator());
    }

    public void RestartMoving()
    {
        isPausedMoving = false;
        ai.FindNextPath();
    }
    public void StopMoving()
    {
        isPausedMoving = true;
        ai.StopSeekPath();
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
        StartCoroutine(ExplodeEnumerator());
        characterRenderer.explosion.PlayOnce();
        Die(false);
    }
    
    
    IEnumerator ExplodeEnumerator()
    {
        yield return new WaitForSeconds(0.5f);
        
        var go = Instantiate(explodePrefab, transform.position, Quaternion.identity,GameRoundManager.Instance.tempTrans);
        go.GetComponent<ExplodeArea>().Init(1);
        
        
        yield return new WaitForSeconds(1f);
        characterRenderer.ghost.PlayOnce();
    }

    public bool isDead = false;
    public void Die(bool showGhost = true)
    {
        
        isDead = true;
        ai.StopSeekPath();
        GetComponent<Collider2D>().enabled = false;
        deathEffect.transform.parent = GameRoundManager.Instance.tempTrans;
        if (showGhost)
        {
            characterRenderer.ghost.PlayOnce();
        }
        StartCoroutine(Hide());
    }

    public void Teleport(Vector3 target)
    {
        characterRenderer.teleportAnimation.PlayOnce();
        var go = Instantiate(teleportPrefab, target, Quaternion.identity, GameRoundManager.Instance.tempTrans);
       StopMoving();
       StartCoroutine(teleportEnumerator(target, go));
    }

    IEnumerator teleportEnumerator(Vector3 target,GameObject teleport)
    {
        yield return new WaitForSeconds(1.25f);
        var originPosition = transform.position;
        teleport.transform.position = originPosition;
        transform.position = target;
        
        characterRenderer.teleportAnimation.PlayReverseOnce();
        teleport.GetComponent<TeleportArea>().rendere.PlayReverseOnce();
        
        yield return new WaitForSeconds(1.25f);
        Destroy(teleport);
        RestartMoving();
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(0.5f);
        //characterRenderer.renderersParent.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    public bool canBeActioned()
    {
        return !isDead && !isPausedMoving && !isStuned;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (!isPausedMoving)
        {
            GetComponent<HumanAI>().RestartSeek();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead)
        {
            return;
        }
        // 获取碰撞到的物体的名称
       // Debug.Log("Collided with: " + other.gameObject.name);

        // 获取碰撞的接触点
        // foreach (var contact in other.contacts)
        // {
        //     Debug.Log("Contact Point: " + contact.point);
        // }
        var human = other.gameObject.GetComponent<Human>();

        if (isInfected )
        {
            
            if (human)
            {
            
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
        else
        {
            // if (info.identifier == "VIROLOGIST")
            // {
            //     if (human && human.isHuman && !human.isFullHealthy())
            //     {
            //         
            //         HealOther(human);
            //     }
            // }
        }

        // if (!isPausedMoving)
        // {
        //     
        //     GetComponent<HumanAI>().RestartSeek();
        // }
        
    }

    public bool isFullHealthy()
    {
        return currentHp >= HP && !isDead;
    }
}
