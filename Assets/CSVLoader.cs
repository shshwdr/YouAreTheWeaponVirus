using System.Collections;
using System.Collections.Generic;
using Sinbad;
using UnityEngine;

public class CardInfo
{
    public string identifier;
    public List<string> actions;
    public int start;
    public string title;
    public string desc;
    public bool canDraw;

}

public class LevelInfo
{
    public int identifier;
    public int time;

}

public class CharacterInfo
{
    public string identifier;
    public int speed;
    public int hp;
    public string sprite;
    public string characterType;
    public string prefab;
    public string title;
    public string desc;

}

public class DialogueInfo
{
    public int level;
    public string text;
    public string characterInfo;

}
public class LevelDesignInfo
{
    public int identifier;
    public string type;
    public string spawn;
    public List<string> move;

}
public class CSVLoader : Singleton<CSVLoader>
{
    public Dictionary<string, CardInfo> cardDict = new Dictionary<string, CardInfo>();
    public Dictionary<string, CharacterInfo> characterDict = new Dictionary<string, CharacterInfo>();
    public Dictionary<int, LevelInfo> levelDict = new Dictionary<int, LevelInfo>();
    public Dictionary<int, List<DialogueInfo>> dialogueDict = new Dictionary<int, List<DialogueInfo>>();
    
public Dictionary<int, List<LevelDesignInfo>> levelDesignDict = new Dictionary<int, List<LevelDesignInfo>>();
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
        foreach (var info in levelInfos)
        {
            levelDict[info.identifier]=info;
            GameRoundManager.Instance.maxLevel = Mathf.Max(info.identifier,GameRoundManager.Instance.maxLevel);
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
