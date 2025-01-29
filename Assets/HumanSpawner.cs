using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanSpawner : Singleton<HumanSpawner>
{
    public List<Human> humans = new List<Human>();

    //public GameObject humanPrefab;
    public Transform levelParent;
    public Transform area;
    
    public Vector3 GetPoint(string index)
    {
        var points = levelParent.Find("points");
        return points.Find(index).transform.position;
    }

    public void clear()
    {
        
        foreach (var human in humans)
        {
            Destroy(human.gameObject);
        }
        humans.Clear();
    }
    // Start is called before the first frame update
    public void Init(Transform levelParent)
    {
        clear();
        this.levelParent = levelParent;
        area = levelParent.Find("areas");
        var points = levelParent.Find("points");

        foreach (var levelDesignInfo in CSVLoader.Instance.levelDesignDict[GameRoundManager.Instance.currentLevelId])
        {
            var  info = CSVLoader.Instance.characterDict[levelDesignInfo.type];
            Vector2 position;
            if (info.characterType == "bin")
            {
                position = points.Find(levelDesignInfo.spawn).position;
            }
            else
            {
                
                position =GetRandomPointInBoxCollider(area.Find(levelDesignInfo.spawn).GetComponent<BoxCollider2D>());
            }
            
            
           var humanPrefab = Resources.Load<GameObject>("characterPrefab/" + info.prefab);
            var human = Instantiate(humanPrefab, position, Quaternion.identity,transform);
            human.GetComponent<Human>().Init(levelDesignInfo);
            humans.Add(human.GetComponent<Human>());
        }
        
        //humans = GetComponentsInChildren<Human>();
    }

    static public Vector2 GetRandomPointInBoxCollider(BoxCollider2D boxCollider)
    {
        if (boxCollider == null)
        {
            throw new System.Exception("BoxCollider2D not assigned.");
        }

        // 获取 BoxCollider2D 的 bounds（边界）
        Bounds bounds = boxCollider.bounds;

        // 在 bounds 的范围内生成一个随机点
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }
    public void InfectAll()
    {
        foreach (var human in humans)
        {
            human.InfectFull();
        }
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
            if (human.isInfected && human.isHuman)
            {
                count++;
            }
        }
        return count;
    }

    public int humanCount()
    {
        
        int count = 0;
        foreach (var human in humans)
        {
            if (human.isHuman)
            {
                count++;
            }
        }
        return count;
    }
}
