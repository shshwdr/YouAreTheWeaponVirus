using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMenu : MenuBase
{
    public TMP_Text text;
    private int index = -1;
    public GameObject characterInfoGo;
    public Button nextButton;
    public Button skipButton;
    public TMP_Text characterTitle;
    public TMP_Text characterDesc;
    public LevelAsIcons speed;
    public LevelAsIcons hp;
    public Image characterImage;
    public Image characterPortrait;
    protected override void Start()
    {
        //base.Awake();
        nextButton.onClick.AddListener(() =>
        {
            gotoNextDialogue();
        });
        
        skipButton.onClick.AddListener(() =>
        {
            hideDialogue();
        });
    }

    public override void Show()
    {
        base.Show();
        HandsView.Instance.gameObject.SetActive(false);
        GameHud.Instance.gameObject.SetActive(false);
        FindObjectOfType<WinLoseMenu>().Hide();
        var dialogue = CSVLoader.Instance.dialogueDict.GetValueOrDefault(GameRoundManager.Instance.currentLevelId, new List<DialogueInfo>());
        if (dialogue.Count == 0)
        {
            hideDialogue();
        }
        else
        {
            index = -1;
            gotoNextDialogue();
        }
    }

    public void hideDialogue()
    {
        GameHud.Instance.gameObject.SetActive(true);
        GameRoundManager.Instance.StartLevel();
        text.text = "";
        base.Hide();

        FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_ui_next_dialog");
    }
    public void gotoNextDialogue()
    {
        index++;
        if (index >= CSVLoader.Instance.dialogueDict[GameRoundManager.Instance.currentLevelId].Count)
        {
            hideDialogue();
        }
        else
        {
            var info = CSVLoader.Instance.dialogueDict[GameRoundManager.Instance.currentLevelId][index];
            text.text = info.text;
            if (info.characterInfo != null && info.characterInfo != "")
            {
                characterInfoGo.SetActive(true);
                var characterInfo = CSVLoader.Instance.characterDict[info.characterInfo];
                characterTitle.text = characterInfo.title;
                characterDesc.text = characterInfo.desc;
                characterImage.sprite = characterSprite(characterInfo);
                var portrait = Resources.Load<Sprite>("characterPortraite/" + characterInfo.portrait);
                if (portrait)
                {
                    characterPortrait.sprite = portrait;
                    characterPortrait.gameObject.SetActive(true);
                }
                else
                {
                    characterPortrait.gameObject.SetActive(false);
                }
                speed.Init(characterInfo.speed, characterInfo.speed);
                hp.Init(characterInfo.hp, characterInfo.hp);
            }
            else
            {
                characterInfoGo.SetActive(false);
            }

            FMODUnity.RuntimeManager.PlayOneShot("event:/SFX/sfx_ui_next_dialog");
        }
    }

    Sprite characterSprite(CharacterInfo info)
    {
            int row = 4;
            int col = 3;
            switch (info.characterType)
            {
                case "human":
                    break;
                case "squirrel":
                    row = 1;
                    col = 2;
                    break;
                case "bird":
                    row = 1;
                    col = 3;
                    break;
                case "bin":
                    row = 1;
                    col = 1;
                    break;
            }
            var texture = Resources.Load<Texture2D>("character/"+ info.sprite);

            // 分割 Sprite Sheet 成 4 行 3 列的精灵
            var walkSprites = new Sprite[row*col]; // 4行 * 3列 = 12帧
            int spriteIndex = 0;
            var i = 0;
            var j = 1;
            if (info.characterType == "bin")
            {
                j = 0;
            }
           // for (int i = 0; i < row; i++)  // 4 行
            {
              //  for (int j = 0; j < col; j++)  // 3 列
                {
                    // 计算每个精灵的坐标
                    float x = j * (1f / col);
                    float y = 1f - (i + 1) * (1f / row);
                    return Sprite.Create(texture, new Rect(x * texture.width, y * texture.height, texture.width / col, texture.height / row), new Vector2(0.5f, 0.5f));
                   // spriteIndex++;
                }
            }
         
    }

    public override void Hide()
    {
        base.Hide();
        var test=1;
    }
}
