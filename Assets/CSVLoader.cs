using System.Collections;
using System.Collections.Generic;
using Sinbad;
using UnityEngine;

public class CardInfo
{
    public string identifier;
    public List<string> actions;
    public int selectType;
    public int start;
    public string title;
    public string desc;
    public bool canDraw;
    public bool exhaust;
    public int unlockAt;
}

public class LevelInfo
{
    public int identifier;
    public string id;
    public int time;

}

public class CharacterInfo
{
    public string identifier;
    public int speed;
    public int hp;
    public string sprite;
    public string sneezeSprite;
    public string abilitySprite;
    
    public string characterType;
    public string prefab;
    public string title;
    public string portrait;
    public string desc;

}

public class DialogueInfo
{
    public string level;
    public string text;
    public string characterInfo;

}
public class LevelDesignInfo
{
    public string identifier;
    public string type;
    public string spawn;
    public List<string> move;

}
public class CSVLoader : Singleton<CSVLoader>
{
    public Dictionary<string, CardInfo> cardDict = new Dictionary<string, CardInfo>();
    public Dictionary<string, CharacterInfo> characterDict = new Dictionary<string, CharacterInfo>();
    public Dictionary<int, LevelInfo> levelDict = new Dictionary<int, LevelInfo>();
    public Dictionary<string, List<DialogueInfo>> dialogueDict = new Dictionary<string, List<DialogueInfo>>();
    
public Dictionary<string, List<LevelDesignInfo>> levelDesignDict = new Dictionary<string, List<LevelDesignInfo>>();
    public void Init()
    {
        var heroInfos =
            CsvUtil.LoadObjects<CardInfo>(GetFileNameWithABTest("card"));
        foreach (var info in heroInfos)
        {
            cardDict[info.identifier]=info;
        }
        var levelInfos =
            CsvUtil.LoadObjects<LevelInfo>(GetFileNameWithABTest("level"));
        int i = 1;
        foreach (var info in levelInfos)
        {
            info.identifier = i;
            levelDict[info.identifier]=info;
            GameRoundManager.Instance.maxLevel = Mathf.Max(info.identifier,GameRoundManager.Instance.maxLevel);
            i++;
        }
        var levelDesignInfos =
            CsvUtil.LoadObjects<LevelDesignInfo>(GetFileNameWithABTest("levelDesign"));
        foreach (var info in levelDesignInfos)
        {
            if (!levelDesignDict.ContainsKey(info.identifier))
            {
                levelDesignDict[info.identifier] = new List<LevelDesignInfo>();
            }
            levelDesignDict[info.identifier].Add(info);
        }
        var dialogueInfos =
            CsvUtil.LoadObjects<DialogueInfo>(GetFileNameWithABTest("dialogue"));
        foreach (var info in dialogueInfos)
        {
            if (!dialogueDict.ContainsKey(info.level))
            {
                dialogueDict[info.level] = new List<DialogueInfo>();
            }
            dialogueDict[info.level].Add(info);
        }
        var characterInfos =
            CsvUtil.LoadObjects<CharacterInfo>(GetFileNameWithABTest("character"));
        foreach (var info in characterInfos)
        {
            characterDict[info.identifier]=info;
        }
    }
    
    string GetFileNameWithABTest(string name)
    {
        // if (ABTestManager.Instance.testVersion != 0)
        // {
        //     var newName = $"{name}_{ABTestManager.Instance.testVersion}";
        //     //check if file in resource exist
        //      
        //     var file = Resources.Load<TextAsset>("csv/" + newName);
        //     if (file)
        //     {
        //         return newName;
        //     }
        // }
        return name;
    }
}
