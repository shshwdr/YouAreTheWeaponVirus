using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private float moveSpeed = 5f;  // 移动速度
    [SerializeField] private float zoomSpeed = 2f;  // 缩放速度
    [SerializeField] private float minZoom = 5f;  // 最小缩放
    [SerializeField] private float maxZoom = 20f;  // 最大缩放

    void Start()
    {
        if (camera == null) camera = Camera.main;  // 如果没有指定相机，则使用主相机
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private int cameraMoveUsed = 0;
    private int cameraScrollUsed = 0;
    // 处理镜头移动
    void HandleMovement()
    {
        float moveX = 0f;
        float moveY = 0f;

        // 监听键盘输入（AWSD）
        if (Input.GetKey(KeyCode.W))
        {
            moveY += 1f;
            cameraMoveUsed ++;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveY -= 1f;
            cameraMoveUsed ++;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveX -= 1f;
            cameraMoveUsed ++;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveX += 1f;
            cameraMoveUsed ++;
        }

        if (cameraMoveUsed >= 2)
        {
            FindObjectOfType<TutorialMenu>().FinishMove();
        }

        // 根据输入移动镜头
        Vector3 move = new Vector3(moveX, moveY, 0f) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    // 处理镜头缩放
    void HandleZoom()
    {
        // 获取鼠标滚轮输入
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        camera.orthographicSize -= scrollInput * zoomSpeed;
        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, minZoom, maxZoom);

        if (cameraMoveUsed >= 2)
        {
            
            if (scrollInput > 0)
            {
                cameraScrollUsed++;
            }
            else if (scrollInput < 0)
            {
                cameraScrollUsed++;
            }
            if (cameraScrollUsed>=2)
            {
                FindObjectOfType<TutorialMenu>().FinishScroll();
            }
        }
    }
}