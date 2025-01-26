using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopySpriteRenderer : MonoBehaviour
{
    public SpriteRenderer copied;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().sprite = copied.sprite;
    }
}
