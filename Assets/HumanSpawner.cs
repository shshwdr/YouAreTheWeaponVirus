using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : Singleton<HumanSpawner>
{
    public Human[] humans;
    // Start is called before the first frame update
    public void Init()
    {
        humans = GetComponentsInChildren<Human>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isAllAffected()
    {
        foreach (var human in humans)
        {
            if (!human.isInfected)
            {
                return false;
            }
        }
        return true;
    }

    public int infectedCount()
    {
        int count = 0;
        foreach (var human in humans)
        {
            if (human.isInfected)
            {
                count++;
            }
        }
        return count;
    }
}
