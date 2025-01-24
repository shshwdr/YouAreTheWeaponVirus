using UnityEngine;

public class CharacterRenderController : MonoBehaviour
{
    public string spriteSheetPath = "character/BASE-Walk-SpriteSheet";  // Resources 文件夹中的精灵图路径
    private Sprite[] walkSprites;  // 存储精灵帧的数组

    public SpriteRenderer spriteRenderer;
    private int currentDirection = 0;  // 0 - Down, 1 - Left, 2 - Right, 3 - Up
    private int currentFrame = 0;     // 当前帧

    private float swapFrameTime = 0.3f;

    private float swapFrameTimer = 0;
    // 在 Start 方法中初始化 spriteRenderer 和加载图片
    void Start()
    {

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

    private Vector2 lastPosition;
    // 更新人物状态（根据按键方向改变精灵）
    private Vector2 lastDir = Vector2.zero;  // 上一个方向

    void Update()
    {
        // 获取当前位置与上次位置的差向量
        var dir = (Vector2)transform.position - lastPosition;

        // 计算当前方向
        Vector2 currentDir = Vector2.zero;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))  // 水平方向优先
        {
            currentDir = dir.x > 0 ? Vector2.right : Vector2.left; // 向右或向左
        }
        else  // 垂直方向
        {
            currentDir = dir.y > 0 ? Vector2.up : Vector2.down; // 向上或向下
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

            ChangeSprite(currentDirection);  // 切换精灵
        }

        // 更新上次位置
        lastPosition = transform.position;
    }

    // 根据方向切换精灵帧
    void ChangeSprite(int direction)
    {
        // 计算当前精灵的索引
        int spriteIndex = direction * 3 + currentFrame;

        // 设置 Sprite Renderer 的精灵
        spriteRenderer.sprite = walkSprites[spriteIndex];

    }
}
