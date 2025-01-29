using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRenderer : MonoBehaviour
{
    public Texture2D texture;
    public string spriteSheetPath = "character/BASE-Walk-SpriteSheet";  // Resources 文件夹中的精灵图路径
    private Sprite[] walkSprites;  // 存储精灵帧的数组

    private int currentFrame = 0;     // 当前帧
    

    private float swapFrameTime = 0.25f;

    private float swapFrameTimer = 0;
    public SpriteRenderer spriteRenderer;
    public int frameMax;
    public void SetSprite(int spriteIndex)
    {
        if (walkSprites != null)
        {
            
            spriteRenderer = GetComponent<SpriteRenderer>();
            // 设置 Sprite Renderer 的精灵
            spriteRenderer.sprite = walkSprites[spriteIndex];
        }
    }

    private bool isPlaying = false;
    public void PlayOnce()
    {
        gameObject.SetActive(true);
        currentFrame = 0;
        isPlaying = true;
    }

    private bool reversePlay = false;

    public void PlayReverseOnce()
    {
        
        gameObject.SetActive(true);
        currentFrame = frameMax-1;
        reversePlay = true;
        isPlaying = true;
    }
    
    void updateFrame()
    {
        swapFrameTimer += Time.deltaTime;
        if (swapFrameTimer >= swapFrameTime)
        {
            swapFrameTimer -= swapFrameTime;
            if (reversePlay)
            {
                currentFrame = (currentFrame - 1);
            }
            else
            {
                
                currentFrame = (currentFrame + 1);
            }
        }

        bool finishedPlay = false;
        if (reversePlay)
        {
            finishedPlay = currentFrame < 0;
        }
        else
        {
            finishedPlay = currentFrame >= frameMax;
        }
        if (finishedPlay)
        {
            isPlaying = false;
            gameObject.SetActive(false);
        }
        else
        {
            SetSprite(currentFrame);
        }
    }
    void Update()
    {
        if (isPlaying)
        {
            updateFrame();
        }
    }

    public void Init(CharacterInfo info)
    {
        if (info.characterType == "bin")
        {
            return;
        }
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
        }

        setSprites(row, col);
    }

    public void Init(int col)
    {
         setSprites(1,col);
         frameMax = col;
    }

    void setSprites(int row,int col)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 从 Resources 文件夹加载完整的 Sprite Sheet
        if (texture == null)
        {
            texture = Resources.Load<Texture2D>(spriteSheetPath);
        }

        // 分割 Sprite Sheet 成 4 行 3 列的精灵
        walkSprites = new Sprite[row*col]; // 4行 * 3列 = 12帧
        int spriteIndex = 0;

        for (int i = 0; i < row; i++)  // 4 行
        {
            for (int j = 0; j < col; j++)  // 3 列
            {
                // 计算每个精灵的坐标
                float x = j * (1f / col);
                float y = 1f - (i + 1) * (1f / row);
                walkSprites[spriteIndex] = Sprite.Create(texture, new Rect(x * texture.width, y * texture.height, texture.width / col, texture.height / row), new Vector2(0.5f, 0.5f));
                spriteIndex++;
            }
        }
    }

}
