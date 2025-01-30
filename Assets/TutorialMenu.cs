using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMenu : MenuBase
{
    private int currentTutorialId = -1;
   public TMP_Text text;

   public Button skipTutorial;

   public GameObject blockButton;
    List<string> tutorialsKeys = new List<string>()
    {
        "tutorialPan",
        "tutorialZoom",
        "tutorialInfect",
        "tutorialSneeze",
        "tutorialRedraw",
        "tutorialEnd",
    };
    private Dictionary<string, string> tutorials = new Dictionary<string, string>()
    {
        { "tutorialPan", "You can use awsd to pan camera." },
        { "tutorialZoom", "You can use mouse scroll to zoom in and out" },
        { "tutorialInfect", "Drag the infect card and use on human" },
        { "tutorialInfectFailed", "No, you need to drag the infect card and use on human, try it again." },
        
        { "tutorialSneeze", "Drag the sneeze card and use on the infected human" },
        { "tutorialSneezeFailed", "No, you need to drag the sneeze card and use on the infected human, try it again." },
        { "tutorialRedraw", "Click draw button to redraw your cards. Remember the Infect card would exhausted after usage" },
        { "tutorialEnd", "That's all you need to know, and infect all of them in time! (Click to continue)" },
    };

    public void reset()
    {
        currentTutorialId = -1;
        isFinished = false;
        text.text = "";
    }
    public override void Show()
    {
        base.Show();
        ShowNextTutorial();
        

    }
public bool isFinished = false;
    public void ShowNextTutorial()
    {
        if (isFinished)
        {
            
            Hide();
            return;
            //currentTutorialId = tutorialsKeys.Count();
        }
        currentTutorialId++;
        if (currentTutorialId < tutorialsKeys.Count)
        {
            text.text = tutorials[tutorialsKeys[currentTutorialId]];

            if (currentTutorialKey == "tutorialInfect")
            {
                HandManager.Instance.DrawSpecificCard("infect");
            }
            if (currentTutorialKey == "tutorialSneeze")
            {
                HandManager.Instance.DrawSpecificCard("sneeze");
            }
            if (currentTutorialKey == "tutorialRedraw")
            {
                HandsView.Instance.showRedrawButton();
            }
            
            if (currentTutorialKey == "tutorialEnd")
            {
                GameHud.Instance.StartProgressBar();
                blockButton.SetActive(true);
            }
        }
        else
        {

            FinishTutorial();
        }
    }

    public void FinishTutorial()
    {
        isFinished = true;
        blockButton.SetActive(false);
        HandManager.Instance.DrawHand();
        HandsView.Instance.showRedrawButton();
        
        GameHud.Instance.StartProgressBar();
        foreach (var human in HumanSpawner.Instance.humans)
        {
            human.RestartMoving();
        }
        Hide();
    }


    public void FinishMove()
    {
        if (currentTutorialId == 0)
        {
            ShowNextTutorial();
        }
    }
    
    public void FinishScroll()
    {
        if (currentTutorialId == 1)
        {
            ShowNextTutorial();
        }
    }

    public IEnumerator FinishUseCard()
    {
        
        yield return new WaitForSeconds(1f);
        if (currentTutorialKey == "tutorialInfect")
        {
            if (HumanSpawner.Instance.infectedCount() >= 1)
            {
                ShowNextTutorial();
            }
            else
            {
                text.text = tutorials["tutorialInfectFailed"];
                HandManager.Instance.DrawSpecificCard("infect",false);
            }
        }
        else if (currentTutorialKey == "tutorialSneeze")
        {
                text.text = tutorials["tutorialSneezeFailed"];
                HandManager.Instance.DrawSpecificCard("sneeze");
            
        }

    }

    public void FinishSneeze()
    {
        if (currentTutorialKey == "tutorialSneeze")
        {
                ShowNextTutorial();
        }
    }

    public void FinishUseRedraw()
    {
        if (currentTutorialKey == "tutorialRedraw")
        {
            ShowNextTutorial();
        }
    }

    protected override void Start()
    {
        base.Start();
        blockButton.SetActive(false);
        blockButton. GetComponent<Button>().onClick.AddListener(()=>
        {
            ShowNextTutorial();
        });
        skipTutorial.onClick.AddListener(()=>
        {
            FinishTutorial();
        });
    }

    public string currentTutorialKey=>isFinished ? "" : tutorialsKeys[currentTutorialId];
}
