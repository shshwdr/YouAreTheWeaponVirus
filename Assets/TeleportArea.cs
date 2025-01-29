using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportArea : MonoBehaviour
{
    public CharacterRenderer rendere;
    // Start is called before the first frame update
    void Start()
    {
        rendere.Init(5);
        rendere.PlayOnce();
    }

    void Hide()
    {
        rendere.PlayReverseOnce();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
