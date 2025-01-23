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
        var level = levelParent.Find("level"+GameRoundManager.Instance.currentLevel);
        level .gameObject.SetActive(true);
        
        
        HumanSpawner.Instance.Init(level);
    }
}
