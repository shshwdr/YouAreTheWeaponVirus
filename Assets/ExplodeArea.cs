using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeArea : MonoBehaviour
{
    public float radius = 1f;
    public CharacterRenderer renderer;
    public void Init(float radius)
    {
        this.radius = radius;
       // transform.localScale = new Vector3(radius * 2, radius * 2, 1);
    }
    // Start is called before the first frame update
    void Start()
    {
        Collider2D[] results = new Collider2D[20]; // 假设最多检测 10 个碰撞体

        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;  // 允许触发器参与检测
        int count = GetComponent<Collider2D>().OverlapCollider(contactFilter, results);

        renderer.Init(3);
        renderer.PlayOnce();
        Destroy(gameObject,1);
        for (int i = 0; i < count; i++)
        {
            Debug.Log($"Overlapping with: {results[i].name}");
            if (results[i].GetComponent<Human>())
            {
                var human = results[i].GetComponent<Human>();
                human.Infect(null);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
