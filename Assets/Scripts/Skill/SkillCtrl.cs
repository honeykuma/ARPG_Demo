using UnityEngine;
using Unity.Cinemachine;

public class SkillCtrl : MonoBehaviour
{
    #region 基礎元件
    public CinemachineImpulseSource _impulseSource;
    public CinemachineImpulseSource impulseSource
    {
        get
        {
            if( _impulseSource == null)
            {
                _impulseSource = GetComponent<CinemachineImpulseSource>();
                if( _impulseSource == null) 
                    _impulseSource = gameObject.AddComponent<CinemachineImpulseSource>();
            }
            return _impulseSource;
        }
    }
    #endregion 基礎元件

    public enum Target { None, Enemy, Player }
    [SerializeField]
    private Target target;
    [SerializeField]
    private GameObject _hitEffectObj;
    [SerializeField]
    private float _hitPower = 0f;
    private bool HitShock => _hitPower > 0f;
    [SerializeField]
    private float _distroyTime = 2f;
    private string Tag
    {
        get
        {
            switch (target)
            {
                case Target.Enemy: return "Enemy";
                case Target.Player: return "Player";

            }
            return string.Empty;
        }
    }

    void Start()
    {
        Destroy(gameObject, _distroyTime);
    }

    /// <summary>
    /// 物件上必須要有碰撞器，且勾上Trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == Tag)
        {
            _hitEffectObj.SetActive(true);
            if (HitShock) impulseSource.GenerateImpulseWithForce(_hitPower);
        }
    }
}
