using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 靜態(唯一)資料管理腳本
/// </summary>
public static class GameManager
{
    #region 玩家相關資訊    
    /// <summary>
    /// 當前正在操作角色的索引號碼
    /// </summary>
    public static int playerIndex;

    /// <summary>
    /// 當前正在操作角色
    /// </summary>
    public static PlayerCtrl playerCtrl { get; private set; }

    /// <summary>
    /// 當前玩家座標定位
    /// </summary>
    public static Vector3 playerGPS
    {
        get
        {
            return playerCtrl != null
                ? playerCtrl.transform.position
                : Vector3.zero;
        }
    }

    /// <summary>
    /// 設定(初始化)當前操作角色
    /// </summary>
    /// <param name="ctrl">角色控制器</param>
    public static void SetCurrentPlayer(PlayerCtrl ctrl)
    {
        playerCtrl = ctrl;
    }

    /// <summary>
    /// 連結HPBarUI的動作
    /// </summary>
    public static Action<float, float> UpdatePlayerHPBar { get; private set; }

    public static void SetPlayerHPBar(Action<float, float> action)
    {
        UpdatePlayerHPBar += action;
        //玩家已存在的話立刻刷新一次
        if (playerCtrl) UpdatePlayerHPBar(playerCtrl.CurrentHP,playerCtrl.MaxHP);
    }

    public static void RemovePlayerHPBar(Action<float, float> action)
    {
        UpdatePlayerHPBar -= action;
    }

    public static void ClearPlayerHPBox()
    {
        UpdatePlayerHPBar = null;
    }
    #endregion 玩家相關資訊

    #region 主攝影機相關
    /// <summary>
    /// 當前運作中的攝影機
    /// </summary>
    public static CameraManger mainCamera { get; private set; }
    /// <summary>
    /// 攝影機的旋轉參數
    /// </summary>
    public static Vector3 mainCameraRota
    {
        get
        {
            return mainCamera != null
                ? mainCamera.transform.rotation.eulerAngles
                : Vector3.zero;
        }
    }
    /// <summary>
    /// 設定(初始化)當前操作鏡頭
    /// </summary>
    /// <param name="main">鏡頭控制器</param>
    public static void SetMainCamera(CameraManger main)
    {
        mainCamera = main;
    }
    #endregion 主攝影機相關    

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
