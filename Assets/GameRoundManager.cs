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
        
        isFinished = false;
        StartLevel();
    }

    public void Restart()
    {
        currentLevel = 1;
        //HandManager.Instance.Init();
        StartLevel();
    }
    public void StartLevel()
    {
        isStarted = true;
        isFinished = false;
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
        FindObjectOfType<CardSelectionMenu>().Hide();

        FindObjectOfType<GameHud>().UpdateAll();
        HandManager.Instance.InitDeck();
        HandManager.Instance.DrawHand();
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
            isFinished = true;
            if (isMaxLevel)
            {
                
                FindObjectOfType<WinLoseMenu>().ShowWin();
            }
            else
            {
                FindObjectOfType<CardSelectionMenu>().Show();
            }
            
            
            HandManager.Instance.ClearBattleHand();
            return;
        }
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            FindObjectOfType<WinLoseMenu>().ShowLose();
            HandManager.Instance.ClearBattleHand();
        }
        
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            HumanSpawner.Instance.InfectAll();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (isMaxLevel)
            {
                StartLevel();
            }
            else
            {
                GoToNextLevel();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLevel > 1)
            {
                currentLevel--;
                StartLevel();
            }
        }
    }
    public void RestartLevel()
    {
        GameRoundManager.Instance.Restart();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    
}
