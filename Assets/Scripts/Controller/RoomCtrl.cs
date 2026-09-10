using UnityEngine;
using Unity.Cinemachine;

public class RoomCtrl : MonoBehaviour
{
    #region 基礎元件
    [SerializeField]
    private CinemachineCamera cinemachineCamera;
    #endregion 基礎元件

    private const string Tag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag))
        {
            cinemachineCamera.Priority.Value = 100;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tag))
        {
            cinemachineCamera.Priority.Value = 0;
        }
    }

}
