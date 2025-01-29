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
        
         StartCoroutine(generate(level));
    }

    IEnumerator generate(Transform level)
    {
        yield return new WaitForSeconds(0.01f);
        
        HumanSpawner.Instance.Init(level);
        AstarPath pathfinder = AstarPath.active;
        pathfinder.Scan();
    }
}
