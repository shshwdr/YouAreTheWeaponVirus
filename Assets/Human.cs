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
    // Start is called before the first frame update
    void Awake()
    {
        buffManager = new BuffManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Infect(CardInfo cardInfo)
    {
        renderer.color = Color.green;
        isInfected = true;
        EventPool.Trigger("Infect");
    }

    public void Sneeze(CardInfo cardInfo)
    {
        var go = Instantiate(sneezePrefab, transform.position, Quaternion.identity);
        
    }

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
