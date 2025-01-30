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


    public void ShowWin()
    {
        GameRoundManager.Instance. isFinished = true;
        title.text = "You Win!";
        dialogue.text = "Awesome. You are the smartest weapon around.";

        // if (!GameRoundManager.Instance.isMaxLevel)
        // {
        //     nextLevel.gameObject.SetActive(true);
        // }
        // else
        // {
        //     nextLevel.gameObject.SetActive(false);
        // }

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_ui_level_win");

        Show();
    }
    public void ShowLose()
    {
        GameRoundManager.Instance. isFinished = true;
        title.text = "You Lose!";
        dialogue.text = "You lost? What a pity... What? You expect me to do something? Go and try it again!";
        Show();

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_ui_level_lose");
    }
    // Start is called before the first frame update
    void Start()
    {
        restartLevel.onClick.AddListener(() => GameRoundManager.Instance.RestartLevel());
        restartGame. onClick.AddListener(() => GameManager.Instance.RestartGame());
    }

    public override void Show()
    {
        base.Show();
        HandsView.Instance.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
