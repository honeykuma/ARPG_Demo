using UnityEngine;
using UnityEngine.InputSystem;

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
    
    // ───【新增】新版輸入系統對應的Action欄位
    [Header("新版輸入系統對應 Action")]
    [SerializeField]
    [Tooltip("請拖入剛剛設定的 Look (Vector2) 動作")]
    private InputActionReference lookAction;
    [SerializeField]
    [Tooltip("請拖入剛剛設定的 RotateToggle (Button) 動作")]
    private InputActionReference rotateToggleAction;
    [SerializeField]
    [Tooltip("請拖入剛剛設定的 Zoom (Vector2) 動作")]
    private InputActionReference zoomAction;

    [Header("滑鼠控制靈敏度")]
    [SerializeField] private float xSpeed = 0.1f; //新版滑鼠Delta數值較大，速度建議調小
    [SerializeField] private float ySpeed = 0.1f;
    [SerializeField] private float zoomSpeed = 0.01f;
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
        // ───【新增】啟用輸入動作
        if (lookAction != null) lookAction.action.Enable();
        if (rotateToggleAction != null) rotateToggleAction.action.Enable();
        if (zoomAction != null) zoomAction.action.Enable();
        // ───
    }

    private void OnDisable()
    {
        // ───【新增】停用輸入動作
        if (lookAction != null) lookAction.action.Disable();
        if (rotateToggleAction != null) rotateToggleAction.action.Disable();
        if (zoomAction != null) zoomAction.action.Disable();
        // ───
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
    /// 讀取新版輸入系統資料，更新角度與距離
    /// </summary>
    private void HandleMouseInput()
    {
        if (!GotTarget) return;

        // 1. 處理滑鼠滾輪縮放距離 (讀取 Scroll 的 Vector2，通常 y 軸代表滾動方向)
        if (zoomAction != null)
        {
            Vector2 scrollDelta = zoomAction.action.ReadValue<Vector2>();
            distance -= scrollDelta.y * zoomSpeed;
            distance = Mathf.Clamp(distance, 0f, 20f); // 限制縮放範圍，對應你 distance 的 Range(0, 20)
        }

        // 2. 判斷是否按住滑鼠右鍵
        if (rotateToggleAction != null && rotateToggleAction.action.IsPressed())
        {
            // 隱藏滑鼠指標，並將滑鼠鎖定在螢幕中央，避免拖曳時指標跑出遊戲視窗
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (lookAction != null)
            {
                // 讀取滑鼠 Delta 的 Vector2 移動量(x 為水平，y 為垂直)
                Vector2 mouseDelta = lookAction.action.ReadValue<Vector2>();

                // 更新角度
                angY += mouseDelta.x * xSpeed;
                angX -= mouseDelta.y * ySpeed; //減法符合一般第三人稱鏡頭仰俯直覺

                // 限制 angX 的上下角度，防止鏡頭旋轉到翻轉或穿過地面 (對應你 angX 的設定範圍)
                angX = Mathf.Clamp(angX, -20f, 80f);

                // 讓 angY 的角度保持在 0 ~ 360 度之間，方便在 Inspector 觀察
                if (angY < 0f) angY += 360f;
                if (angY > 360f) angY -= 360f;
            }
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
