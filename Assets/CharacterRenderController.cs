using System;
using UnityEngine;

public class CharacterRenderController : MonoBehaviour
{

    private int currentDirection = 0;  // 0 - Down, 1 - Left, 2 - Right, 3 - Up
    private int currentFrame = 0;     // 当前帧
    

    private float swapFrameTime = 0.3f;

    private float swapFrameTimer = 0;

    CharacterRenderer[] renderers;
    // 在 Start 方法中初始化 spriteRenderer 和加载图片
    

    private Vector2 lastPosition;
    // 更新人物状态（根据按键方向改变精灵）
    private Vector2 lastDir = Vector2.zero;  // 上一个方向

    private void Awake()
    {
        renderers = GetComponentsInChildren<CharacterRenderer>();
    }

    void updateFrame()
    {
        
        swapFrameTimer += Time.deltaTime;
        if (swapFrameTimer >= swapFrameTime)
        {
            swapFrameTimer -= swapFrameTime;
            currentFrame = (currentFrame + 1) % 4;
        }
    }
    void Update()
    {
        updateFrame();
        ChangeSprite(currentDirection);  // 切换精灵
    }
    public void UpdateDir(Vector3 dir)
    {
        // 获取当前位置与上次位置的差向量
       // var dir = (Vector2)transform.position - lastPosition;
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

        }
Debug.Log("currentDirection: " + currentDirection + " lastDir " + lastDir +  " currentFrame " + transform.position);
        // 更新上次位置
        lastPosition = transform.position;
    }

    private int[] index = new[] { 0, 1, 2, 1 };
    // 根据方向切换精灵帧
    void ChangeSprite(int direction)
    {
        // 计算当前精灵的索引
        int spriteIndex = direction * 3 + index[ currentFrame];
        foreach (var renderer in renderers)
        {
            renderer.SetSprite(spriteIndex);
        }

    }
}
