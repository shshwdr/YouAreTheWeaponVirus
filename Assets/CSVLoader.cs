using System.Collections;
using System.Collections.Generic;
using Sinbad;
using UnityEngine;

public class CardInfo
{
    public string identifier;
    public List<string> actions;
}

public class LevelInfo
{
    public int identifier;
    public int time;

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
    public Dictionary<int, LevelInfo> levelDict = new Dictionary<int, LevelInfo>();
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
