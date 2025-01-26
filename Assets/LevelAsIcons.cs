using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAsIcons : MonoBehaviour
{
 ToggleCell[] cells;
    public void Init(int level,int maxLevel)
    {
        cells = GetComponentsInChildren<ToggleCell>(true);

        foreach (var cell in cells)
        {
            cell.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < maxLevel; i++)
        {
            cells[i].gameObject.SetActive(true);
        }
        
        if (level == 0)
        {
            cells[0].transform.parent.gameObject.SetActive(false);
            return;
        }
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Toggle(false);
        }
        for (int i = 0; i < level; i++)
        {
            cells[i].Toggle(true);
        }
    }
}
