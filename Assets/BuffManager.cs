using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pool;
using UnityEngine;

public enum BuffType
{
    heroMaxHP, heroAttack, price,heroCriticalRate, cooldownFood, shield, explodeRange, attackTool,healAfterBattle,criticalDamage,shieldReflect,allAttackSplit,allAttackStealHP, attackExplode,dodgeRate,energyCriticalRate
    
}
public class BuffManager 
{
    public Dictionary<string, int> buffs = new Dictionary<string, int>();
    private Human human;
public BuffManager(Human human)
{
    this.human = human;
}

    // public void RemoveBuff(string type, int value)
    // {
    //     if (!buffs[type].Contains(value))
    //     {
    //         Debug.LogError("BuffManager: remove a non-existent buff");
    //     }
    //     buffs[type].Remove(value);
    // }
    public void SetBuff(string type, int value)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_touch_infection");

        if (!buffs.ContainsKey(type))
        {
            buffs[type] = 0;
        }
        buffs[type]=(value);

        if (type == "touch")
        {
            human.touchState.SetState(buffs[type]);
        }
    }
    public void AddBuff(string type, int value)
    {
        if (!buffs.ContainsKey(type))
        {
            buffs[type] = 0;
        }
        buffs[type]+=(value);

        if (type == "touch")
        {
            human.touchState.SetState(buffs[type]);
        }

        //EventPool.Trigger(EventPoolNames.UpdateBuff);
    }
    public void ClearBuff()
    {
        buffs.Clear();
    }

    public List<string> buffTypes()
    {
        return buffs.Keys.ToList();
    }

    // public void RemoveDebuff()
    // {
    //     foreach (var type in new List<BuffType>() {BuffType.heroMaxHP, BuffType.heroAttack})
    //     {
    //         if (buffs.ContainsKey(type))
    //         {
    //             for (int i = 0; i < buffs[type].Count(); i++)
    //             {
    //                 if (buffs[type][i] < 0)
    //                 {
    //                     buffs[type].RemoveAt(i); 
    //                     i--;
    //                 }
    //             }
    //         }
    //         
    //     }
    //     EventPool.Trigger(EventPoolNames.UpdateBuff);
    // }

    public int GetBuffValue(string type)
    {
        if (buffs.ContainsKey(type))
        {
            return buffs[type];
        }

        return 0;
    }
}
