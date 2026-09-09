using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    #region 鏡頭設定
    [SerializeField]
    private Vector3 offset;
    [SerializeField]
    [Range(0f,20f)]
    private float distance;
    [SerializeField]
    [Range(10f,80f)]
    private float angX;
    [SerializeField]
    [Range(0f, 360f)]
    private float angY;

    // ───【新增】旋轉與縮放的速度設定
    [Header("滑鼠控制設定")]
    [SerializeField]
    [Tooltip("滑鼠水平旋轉速度")]
    private float xSpeed = 200.0f;
    [SerializeField]
    [Tooltip("滑鼠垂直旋轉速度")]
    private float ySpeed = 120.0f;
    [SerializeField]
    [Tooltip("滑鼠滾輪縮放速度")]
    private float zoomSpeed = 3.0f;
    // ───
    #endregion 鏡頭設定

    #region 公用參數
    /// <summary>
    /// 角色定位+偏移修正後的最終位置
    /// </summary>
    private Vector3 GPS => GameManager.playerGPS + offset;

    /// <summary>
    /// 是否取得跟隨目標對象
    /// </summary>
    private bool GotTarget => GPS != Vector3.zero;
    #endregion 公用參數

    #region 生命週期    
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // ───【新增】在追隨前，先偵測玩家的滑鼠輸入 
        HandleMouseInput();
        // ───

        Fallow();
    }
    #endregion 生命週期
    // ───【新增】處理滑鼠右鍵旋轉與滾輪縮放的函式
    /// <summary>
    /// 處理滑鼠輸入（右鍵拖曳旋轉、滾輪縮放遠近）
    /// </summary>
    private void HandleMouseInput()
    {
        if (!GotTarget) return;

        // 1. 處理滑鼠滾輪縮放距離 (Input.GetAxis 會取得滾輪滾動量，往後滾為負，往前滾為正)
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, 0f, 20f); // 限制縮放範圍，對應你 distance 的 Range(0, 20)

        // 2. 判斷是否按住滑鼠右鍵（1 代表滑鼠右鍵）
        if (Input.GetMouseButton(1))
        {
            // 隱藏滑鼠指標，並將滑鼠鎖定在螢幕中央，避免拖曳時指標跑出遊戲視窗
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // 根據滑鼠的物理移動量（Mouse X / Y）來改變目前的 angY 與 angX 數值
            angY += Input.GetAxis("Mouse X") * xSpeed * Time.deltaTime;
            angX -= Input.GetAxis("Mouse Y") * ySpeed * Time.deltaTime; // 減法符合一般第三人稱鏡頭仰俯直覺

            // 限制 angX 的上下角度，防止鏡頭旋轉到翻轉或穿過地面 (對應你 angX 的設定範圍)
            angX = Mathf.Clamp(angX, -20f, 80f);

            // 讓 angY 的角度保持在 0 ~ 360 度之間，方便在 Inspector 觀察
            if (angY < 0f) angY += 360f;
            if (angY > 360f) angY -= 360f;
        }
        else
        {
            // 放開右鍵時，恢復滑鼠指標的顯示與自由移動
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    // ───

    private void Fallow()
    {
        if (!GotTarget) return;
        transform.position = GPS + Angle() * Distance();
        //transform.LookAt(GPS);
    }

    /// <summary>
    /// 組合角度
    /// </summary>
    /// <returns>四元素運算結果</returns>
    private Quaternion Angle()
    {
        return transform.rotation = 
            Quaternion.Euler(angX, angY, 0);
    }

    /// <summary>
    /// 方向向量(距離)
    /// </summary>
    /// <returns>後退距離</returns>
    private Vector3 Distance()
    {
        return Vector3.back * distance;
    }

}
