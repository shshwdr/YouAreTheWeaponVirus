using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Transform levelParent;
    public void Init()
    {
        foreach (Transform trans in levelParent)
        {
            trans.gameObject.SetActive(false);
        }
        var level = levelParent.Find("level"+GameRoundManager.Instance.currentLevelId);
        level .gameObject.SetActive(true);
        level.transform.position = Vector3.zero;
        Physics2D.SyncTransforms();
        Generate(level);
        //StartCoroutine(generate(level));
    }

    
    

    void Generate(Transform level)
    {
        HumanSpawner.Instance.Init(level);
        Rescan();
    }

    public void Rescan()
    {
        
        AstarPath pathfinder = AstarPath.active;
        pathfinder.Scan();
    }

    public void RescanAndReSeek()
    {
        Rescan();
        
        foreach (var human in HumanSpawner.Instance.humans)
        {
            if (!human.isDead && !human.isPausedMoving)
            {
                human.RestartMoving();
            }
        }
    }
}
