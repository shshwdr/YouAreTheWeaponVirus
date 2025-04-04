using System.Collections;
using System.Collections.Generic;
using Pool;
using TMPro;
using UnityEngine;

public class GameHud : Singleton<GameHud>
{
    public TMP_Text day;
    public TMP_Text infectedCount;

    public TMP_Text timeRemain;

    public HPBar progressBar;
    // Start is called before the first frame update
    void Start()
    {
        EventPool.OptIn("UpdateLevel", UpdateLevel);
        EventPool.OptIn("Infect", UpdateInfect);
        UpdateInfect();
        UpdateLevel();
        progressBar.gameObject.SetActive(false);
    }
    
    public void StartProgressBar()
    {
        progressBar.gameObject.SetActive(true);
    }

    public void UpdateAll()
    {
        UpdateLevel();
        UpdateInfect();
    }
    public void UpdateLevel()
    {
         day.text = "Day " + GameRoundManager.Instance.currentLevel;
    }
    // Update is called once per frame
    void Update()
    {
        timeRemain.text = GameRoundManager.Instance.timer.ToString("0")+" seconds remain";
        progressBar.SetHP(GameRoundManager.Instance.timer, GameRoundManager.Instance.levelTime);
    }

    public void UpdateInfect()
    {
        infectedCount.text = HumanSpawner.Instance.infectedCount() + "/" + HumanSpawner.Instance.humanCount();
    }
}
