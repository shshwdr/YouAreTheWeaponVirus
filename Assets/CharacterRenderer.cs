using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRenderer : MonoBehaviour
{
    public string spriteSheetPath = "character/BASE-Walk-SpriteSheet";  // Resources 文件夹中的精灵图路径
    private Sprite[] walkSprites;  // 存储精灵帧的数组

    public SpriteRenderer spriteRenderer;
    public void SetSprite(int spriteIndex)
    {
        if (walkSprites != null)
        {
            
            spriteRenderer = GetComponent<SpriteRenderer>();
            // 设置 Sprite Renderer 的精灵
            spriteRenderer.sprite = walkSprites[spriteIndex];
        }
    }

    public void Init(CharacterInfo info)
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
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 从 Resources 文件夹加载完整的 Sprite Sheet
        Texture2D texture = Resources.Load<Texture2D>(spriteSheetPath);

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
