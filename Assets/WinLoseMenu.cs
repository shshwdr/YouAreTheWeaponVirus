using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinLoseMenu : MenuBase
{

    public TMP_Text title;

    public Button restartLevel;

    public Button restartGame;

    public TMP_Text dialogue;

    public Button nextLevel;

    public void ShowWin()
    {
        GameRoundManager.Instance. isFinished = true;
        title.text = "You Win!";
        dialogue.text = "How Nice. You are my best weapon.";

        if (!GameRoundManager.Instance.isMaxLevel)
        {
            nextLevel.gameObject.SetActive(true);
        }
        else
        {
            nextLevel.gameObject.SetActive(false);
        }
        
        Show();
    }
    public void ShowLose()
    {
        GameRoundManager.Instance. isFinished = true;
        nextLevel.gameObject.SetActive(false);
        title.text = "You Lose!";
        dialogue.text = "You Lose? What a pity... Try again?";
        Show();
    }
    // Start is called before the first frame update
    void Start()
    {
        restartLevel.onClick.AddListener(() => GameRoundManager.Instance.RestartLevel());
        nextLevel. onClick.AddListener(() => GameRoundManager.Instance.GoToNextLevel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
