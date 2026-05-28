using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 靜態(唯一)資料管理腳本
/// </summary>
public static class GameManager
{
    /// <summary>
    /// 當前正在操作角色的索引號碼
    /// </summary>
    public static int playerIndex;

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
