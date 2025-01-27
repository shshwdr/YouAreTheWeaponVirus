using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageState : MonoBehaviour
{
    public int state;
public Image image;
    public Sprite[] stateSprites;

    public void SetState(int s)
    {
        if (s == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        state = s;
        image.sprite = stateSprites[s];
        image.SetNativeSize();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
