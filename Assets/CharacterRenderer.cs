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
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 从 Resources 文件夹加载完整的 Sprite Sheet
        Texture2D texture = Resources.Load<Texture2D>(spriteSheetPath);

        // 分割 Sprite Sheet 成 4 行 3 列的精灵
        walkSprites = new Sprite[12]; // 4行 * 3列 = 12帧
        int spriteIndex = 0;

        for (int row = 0; row < 4; row++)  // 4 行
        {
            for (int col = 0; col < 3; col++)  // 3 列
            {
                // 计算每个精灵的坐标
                float x = col * (1f / 3f);
                float y = 1f - (row + 1) * (1f / 4f);
                walkSprites[spriteIndex] = Sprite.Create(texture, new Rect(x * texture.width, y * texture.height, texture.width / 3, texture.height / 4), new Vector2(0.5f, 0.5f));
                spriteIndex++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
