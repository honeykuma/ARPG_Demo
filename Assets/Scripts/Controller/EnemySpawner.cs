using System;
using Unity.Multiplayer.PlayMode;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    #region 基礎元件
    [SerializeField]
    private GameObject _enemyPrefab;
    #endregion 基礎元件

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        Instantiate(_enemyPrefab, transform.position, transform.rotation);
    }
}
