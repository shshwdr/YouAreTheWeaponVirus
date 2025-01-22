using System.Collections;
using System.Collections.Generic;
using Pool;
using TMPro;
using UnityEngine;

public class GameHud : MonoBehaviour
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
    }

    public void UpdateLevel()
    {
         day.text = "Day " + GameManager.Instance.level;
    }
    // Update is called once per frame
    void Update()
    {
        timeRemain.text = GameRoundManager.Instance.timer.ToString("0")+" seconds remain";
        progressBar.SetHP(GameRoundManager.Instance.timer, GameRoundManager.Instance.levelTime);
    }

    public void UpdateInfect()
    {
        infectedCount.text = HumanSpawner.Instance.infectedCount() + "/" + HumanSpawner.Instance.humans.Length;
    }
}
