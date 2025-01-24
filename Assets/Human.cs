using System.Collections;
using System.Collections.Generic;
using Pool;
using Unity.VisualScripting;
using UnityEngine;

public class Human : MonoBehaviour
{
    [HideInInspector]
    public BuffManager buffManager;
    
    public SpriteRenderer renderer;

    public GameObject sneezePrefab;

    public bool isInfected = false;
    public CharacterInfo info;

    public int HP => info.hp;
    public int currentHp = 0;
    
    float[] speedAdjust = new float[]
    {
        0,1,1.5f,2,2.5f
    };
    public void Init(string type)
    {
        info = CSVLoader.Instance.characterDict[type];
        GetComponent<HumanAI>().speed *= speedAdjust[info.speed];
        currentHp = HP;
        GetComponent<CharacterRenderController>().Init(info);
    }
    // Start is called before the first frame update
    void Awake()
    {
        buffManager = new BuffManager();
    }

    // Update is called once per frame
    void Update()
    {
        if (lastPosition == transform.position)
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

    public void Infect(CardInfo cardInfo)
    {
       // renderer.color = Color.green;
       GetComponent<CharacterRenderController>().GetInfected(1);
        isInfected = true;
        EventPool.Trigger("Infect");

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_human_infected");
    }

    public void Sneeze(CardInfo cardInfo)
    {
        var go = Instantiate(sneezePrefab, transform.position, Quaternion.identity,GameRoundManager.Instance.tempTrans);
        
    }

    private Vector3 lastPosition;
    private float staticTime = 1;
    private float staticTimer;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 获取碰撞到的物体的名称
        Debug.Log("Collided with: " + collision.gameObject.name);

        // 获取碰撞的接触点
        foreach (var contact in collision.contacts)
        {
            Debug.Log("Contact Point: " + contact.point);
        }

        if (isInfected)
        {
            
            if (collision.gameObject.GetComponent<Human>())
            {
            
                var human = collision.gameObject.GetComponent<Human>();
                if (!human.isInfected)
                {
                    if (buffManager.GetBuffValue("touch") > 0)
                    {
                        buffManager.AddBuff("touch", -1);
                        human.Infect(null);
                    }
                }
            }
        }

        GetComponent<HumanAI>().RestartSeek();
    }
}
