using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRoundManager : Singleton<GameRoundManager>
{
    bool isStarted = false;

    public float timer;
    public float levelTime = 3;
    private bool isFinished = false;
    public int currentLevel = 1;
    private LevelController levelController;

    public int maxLevel = 0;
    
    public bool isMaxLevel => currentLevel >= maxLevel;

    public Transform tempTrans;

    public void GoToNextLevel()
    {
        currentLevel++;
        StartLevel();
    }
    public void StartLevel()
    {
        isStarted = true;
        
        foreach (Transform trans in tempTrans)
        {
            Destroy(trans.gameObject);
        }
        var levelInfo = CSVLoader.Instance.levelDict[currentLevel];
        levelTime = levelInfo.time;
        timer = levelInfo.time;
        levelController = GetComponentInChildren<LevelController>();
        levelController.Init();
        FindObjectOfType<WinLoseMenu>().Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isStarted)
        {
            return;
        }
        if (isFinished)
        {
            return;
        }
            
        if (HumanSpawner.Instance.isAllAffected())
        {
            FindObjectOfType<WinLoseMenu>().ShowWin();
            return;
        }
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            FindObjectOfType<WinLoseMenu>().ShowLose();
        }
        
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    
}
