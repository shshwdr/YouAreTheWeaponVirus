using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCell : MonoBehaviour
{
    public GameObject onItem;
    
    public void Toggle(bool isOn)
    {
        onItem.SetActive(isOn);
    }
}
