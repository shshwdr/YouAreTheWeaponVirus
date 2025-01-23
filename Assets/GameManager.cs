using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{// Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        CSVLoader.Instance.Init();
        GameRoundManager.Instance.StartLevel();
    }
    // Update is called once per frame
    void Update()
    {
    }

}
