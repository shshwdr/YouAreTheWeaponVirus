using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDetector : MonoBehaviour
{
    public Human thisHuman;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (thisHuman.isDead)
        {
            return;
        }

        if (!thisHuman.canBeActioned())
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
        var human = other.gameObject.GetComponent<Human>();

        if (thisHuman.isInfected )
        {
            
        }
        else
        {
            if (human && human.canBeActioned())
            {
                
                if (thisHuman.info.identifier == "VIROLOGIST")
                {
                    if (human && human.isHuman && !human.isFullHealthy())
                    {
                    
                        thisHuman.HealOther(human);
                    }
                }
                else if (thisHuman.info.identifier == "samuri")
                {
                
                    if (human && human.isHuman && human.isInfected && !human.isDead)
                    {
                    
                        thisHuman.Attack(human);
                    }
                }
            }
        }

    }

}
