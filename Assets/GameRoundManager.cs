using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameRoundManager : Singleton<GameRoundManager>
{
    bool isStarted = false;

    public float timer;
    public float levelTime = 3;
    public bool isFinished = false;
    public int currentLevel = 1;
    public string currentLevelId => CSVLoader.Instance.levelDict[currentLevel].id;
    public int startLevel = 1;
    private LevelController levelController;

    public int maxLevel = 0;
    
    public bool isMaxLevel => currentLevel >= maxLevel;

    public Transform tempTrans;

    public void GoToNextLevel()
    {
        currentLevel++;
        isStarted = false;
        isFinished = false;
        ShowDialogue();
    }

    public void RestartGame()
    {
        currentLevel = startLevel;
        //HandManager.Instance.Init();
        ShowDialogue();
    }

    public void ShowDialogue()
    {
        
        FindObjectOfType<DialogueMenu>(true).Show();
    }
    
    public void StartLevel()
    {
        HandsView.Instance.gameObject.SetActive(true);
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
        StartCoroutine(waitToStart());
    }

    IEnumerator waitToStart()
    {
         yield return new WaitForSeconds(0.1f);
        isStarted = true;
        GameHud.Instance.UpdateAll();
    }

    void Cheat()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            HumanSpawner.Instance.InfectAll();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartGame();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (isMaxLevel)
            {
                ShowDialogue();
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
                ShowDialogue();
            }
        }
    }

    IEnumerator FinishLevel()
    {
        yield return new WaitForSeconds(2);
        if (isMaxLevel)
        {
                
            FindObjectOfType<WinLoseMenu>().ShowWin();
        }
        else
        {
            FindObjectOfType<CardSelectionMenu>().Show();
        }
            
            
        HandManager.Instance.ClearBattleHand();
    }
    // Update is called once per frame
    void Update()
    {
        Cheat();
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

            StartCoroutine(FinishLevel());
            return;
        }
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            FindObjectOfType<WinLoseMenu>().ShowLose();
            HandManager.Instance.ClearBattleHand();
        }
        
        
        
    }

    public void RestartLevel()
    {
        isFinished = false;
        isStarted = false;
        ShowDialogue();
    }
    
    
}
