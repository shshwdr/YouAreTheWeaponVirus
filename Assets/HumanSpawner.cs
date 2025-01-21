using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    private Human[] humans;
    // Start is called before the first frame update
    void Start()
    {
        humans = GetComponentsInChildren<Human>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAllAffected())
        {
            FindObjectOfType<WinLoseMenu>().Show();
        }
    }

    bool isAllAffected()
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
}
