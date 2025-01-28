using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfectArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject,10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Human")
        {
            Human human = other.gameObject.GetComponent<Human>();
            if (!human.isInfected)
            {
                human.Infect(null);
            }
        }
    }
}
