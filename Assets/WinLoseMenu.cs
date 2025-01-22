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

    public Button nextLevel;

    public void ShowWin()
    {
        title.text = "You Win!";
        Show();
    }
    public void ShowLose()
    {
        title.text = "You Lose!";
        Show();
    }
    // Start is called before the first frame update
    void Start()
    {
        restartLevel.onClick.AddListener(() => GameRoundManager.Instance.RestartLevel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
