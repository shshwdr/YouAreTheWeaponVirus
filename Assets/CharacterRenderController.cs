using System;
using UnityEngine;

public class CharacterRenderController : MonoBehaviour
{
    public string characterType;

    private int currentDirection = 0;  // 0 - Down, 1 - Left, 2 - Right, 3 - Up
    private int currentFrame = 0;     // 当前帧
    

    private float swapFrameTime = 0.3f;

    private float swapFrameTimer = 0;

    public CharacterRenderer[] renderers;

    public Transform renderersParent;

    public GameObject infectedObject;
    public CharacterRenderer mainRenderer;

    public CharacterRenderer explosion;
    public CharacterRenderer ghost;
    public CharacterRenderer skillDetector;
    public CharacterRenderer sneeze;

    public CharacterRenderer infectAnimation;
    public CharacterRenderer teleportAnimation;
    public CharacterRenderer healAnimation;
    public CharacterRenderer skillAnimation;
    public CharacterRenderer skillSamuriAnimation;
    // 在 Start 方法中初始化 spriteRenderer 和加载图片
    public void GetInfected(int state)
    {
        infectedObject.SetActive(state == 1);
    }

    private Vector2 lastPosition;
    // 更新人物状态（根据按键方向改变精灵）
    private Vector2 lastDir = Vector2.zero;  // 上一个方向
    private int[] index;

    public void HideDetector()
    {
        
        skillDetector.gameObject.SetActive(false);
    }
    public void Init(CharacterInfo info)
    {
        characterType = info.characterType;
        //renderers = GetComponentsInChildren<CharacterRenderer>(true);
        mainRenderer.spriteSheetPath ="character/"+ info.sprite;
        
        
        explosion.Init(9);
        ghost.Init(5);

        if (info.sneezeSprite != null && info.sneezeSprite != "")
        {
            sneeze.spriteSheetPath ="character/"+ info.sneezeSprite;
            sneeze.Init(5);
        }
        
        healAnimation.Init(3);
        if (info.identifier == "samuri" || info.identifier == "viro")
        {
            skillDetector.gameObject.SetActive(true);
            skillDetector.Init(3);
            skillDetector.PlayLoop();
        }
        infectAnimation.Init(3);
        teleportAnimation.Init(5);
        skillSamuriAnimation.Init(9);
        if (info.abilitySprite != null && info.abilitySprite != "")
        {
            skillAnimation.spriteSheetPath ="character/"+ info.abilitySprite;
            int count = 8;
            if (info.identifier == "samuri")
            {
                count = 9;
            }
            skillAnimation.Init(count);
        }
        
        switch (characterType)
        {
            case "human":
            case "bird":
                
            index = new[] { 0, 1, 2, 1 };
                break;
            case "squirrel":
                index = new[] { 0, 1};
                break;
        }
        foreach (var renderer in renderers)
        {
            renderer.Init(info);
        }
    }

    void updateFrame()
    {
        if (characterType == "bin")
        {
            return;
        }
        int frameMax = 4;
        switch (characterType)
        {
            case "human":

                break;
            case "squirrel":
                frameMax = 2;
                break;
            case "bird":
                frameMax = 4;
                break;
                
        }
        
        swapFrameTimer += Time.deltaTime;
        if (swapFrameTimer >= swapFrameTime)
        {
            swapFrameTimer -= swapFrameTime;
            currentFrame = (currentFrame + 1) % frameMax;
        }
        
    }
    void Update()
    {
        if (characterType == "bin")
        {
            return;
        }
        updateFrame();
        ChangeSprite(currentDirection);  // 切换精灵
        
        var thisHuman = GetComponent<Human>();
        if (thisHuman.isDead)
        {
            return;
        }

        if (thisHuman.isInfected )
        {
            return;
        }
        if (!thisHuman.canBeActioned())
        {
            return;
        }

        if (thisHuman.info.identifier == "viro" || thisHuman.info.identifier == "samuri")
        {
            foreach (var human in HumanSpawner.Instance.humans)
            {
                if (human == thisHuman)
                {
                    continue;
                }
                if (human && human.canBeActioned())
                {
                if (Vector2.Distance(human.transform.position, transform.position) <= 2)
                {
                  
                
                
                    if (thisHuman.info.identifier == "viro")
                    {
                        if (human && human.isHuman && !human.isFullHealthy())
                        {
                    
                            thisHuman.HealOther(human);
                            return;
                        }
                    }
                    else if (thisHuman.info.identifier == "samuri")
                    {
                
                        if (human && human.isHuman && human.isInfected && !human.isDead)
                        {
                    
                            thisHuman.Attack(human);
                            return;
                        }
                    }
                }  
                }
            }
        }
        {
            
        }
    }
    public void UpdateDir(Vector3 dir)
    {
        if (characterType == "bin")
        {
            return;
        }
        // 获取当前位置与上次位置的差向量
       // var dir = (Vector2)transform.position - lastPosition;
        // 计算当前方向
        Vector2 currentDir = Vector2.zero;

        switch (characterType)
        {
            case "human":
                
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))  // 水平方向优先
                {
                    currentDir = dir.x > 0 ? Vector2.right : Vector2.left; // 向右或向左
                }
                else  // 垂直方向
                {
                    currentDir = dir.y > 0 ? Vector2.up : Vector2.down; // 向上或向下
                }

                break;
            case "squirrel":
            case "bird":
                currentDir = dir.x > 0 ? Vector2.right : Vector2.left; // 向右或向左
                break;
        }

        // 如果当前方向与上一个方向不一样，才更新精灵
        if (currentDir != lastDir)
        {
            lastDir = currentDir;  // 更新方向
            // 根据新的方向切换精灵
            if (currentDir == Vector2.right)
                currentDirection = 2;  // 向右
            else if (currentDir == Vector2.left)
                currentDirection = 1;  // 向左
            else if (currentDir == Vector2.up)
                currentDirection = 3;  // 向上
            else
                currentDirection = 0;  // 向下

        }

        if (characterType != "human")
        {

            if (currentDirection == 2)
            {
                renderersParent.localScale = new Vector3(-1, 1, 1);
            }else if (currentDirection == 1)
            {
                renderersParent.localScale = new Vector3(1, 1, 1);
            }
    }
//Debug.Log("currentDirection: " + currentDirection + " lastDir " + lastDir +  " currentFrame " + transform.position);
        // 更新上次位置
        lastPosition = transform.position;
    }

    // 根据方向切换精灵帧
    void ChangeSprite(int direction)
    {
        if (characterType == "bin")
        {
            return;
        }
        // 计算当前精灵的索引
        int spriteIndex = 0;
        switch (characterType)
        {
             case "human":
                 spriteIndex = direction * 3 + index[ currentFrame];
                break;
             case "squirrel":
             case "bird":
                spriteIndex = index[ currentFrame];
                break;
        }
        
        
        
        foreach (var renderer in renderers)
        {
            renderer.SetSprite(spriteIndex);
        }

    }
}
