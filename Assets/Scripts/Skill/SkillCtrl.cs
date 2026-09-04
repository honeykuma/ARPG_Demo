using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Threading.Tasks;

public class SkillCtrl : MonoBehaviour
{
    #region 基礎元件
    private CinemachineImpulseSource _impulseSource;
    private CinemachineImpulseSource impulseSource
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

    #region 基本參數
    /// <summary>
    /// [列舉]目標類型
    /// </summary>
    [SerializeField]
    private Target target;
    public enum Target { None, Enemy, Player }
    /// <summary>
    /// 實際運作的對象目標標籤
    /// </summary>
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
    /// <summary>
    /// 基本傷害
    /// </summary>
    [SerializeField]
    private float _damage = 10f;
    /// <summary>
    /// 撞擊效果
    /// </summary>
    [SerializeField]
    private GameObject _hitEffectObj;
    /// <summary>
    /// [二段]次級效果
    /// </summary>
    [SerializeField]
    private GameObject _secondHitObj;
    /// <summary>
    /// [二段]觸發延遲
    /// </summary>
    [SerializeField]
    private float _secondHitDelay = 0f;
    /// <summary>
    /// 銷毀時間
    /// </summary>
    [SerializeField]
    private float _distroyTime = 2f;
    [SerializeField]
    private bool _UseTargetPos = true;
    #endregion 基本參數

    #region 運作方式
    /// <summary>
    /// [鏡頭震動]打擊感的威力
    /// </summary>
    [SerializeField]
    private float _hitPower = 0f;
    /// <summary>
    /// [鏡頭震動]是否啟用
    /// </summary>
    private bool HitShock => _hitPower > 0f;
    /// <summary>
    /// [位移]飛射速度
    /// </summary>
    [SerializeField]
    private float _flySpeed = 0f;
    private float FlySpeed => _flySpeed * Time.deltaTime; 
    /// <summary>
    /// [位移]是否啟用
    /// </summary>
    private bool Canfly => _flySpeed > 0f;
    /// <summary>
    /// [二段打擊]是否啟用
    /// </summary>
    private bool UseSecondHit => _secondHitObj != null;
    /// <summary>
    /// [二段打擊延遲觸發]是否啟用
    /// </summary>
    private bool DelaySecondHit => _secondHitDelay > 0f;
    #endregion 運作方式

    #region 生命週期
    void Start()
    {
        Destroy(gameObject, _distroyTime);
    }

    private void Update()
    {
        Fly();
    }

    /// <summary>
    /// 物件上必須要有碰撞器，且勾上Trigger
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag))
        {
            BaseCtrl actor = other.GetComponent<BaseCtrl>();
            if(!actor) return;

            actor.TakeDamage(_damage);

            SecondHit(_UseTargetPos? other.transform : transform);

            if (_hitEffectObj) _hitEffectObj.SetActive(true);
            if (HitShock) impulseSource.GenerateImpulseWithForce(_hitPower);
        }
    }
    #endregion 生命週期

    #region 運行手段
    /// <summary>
    /// 飛行功能
    /// </summary>
    private void Fly()
    {
        if (!Canfly) return;
        transform.Translate(Vector3.forward * FlySpeed); 
    }
    /// <summary>
    /// 二段傷害
    /// </summary>
    /// <param name="hitTarget">觸發目標</param>
     private void SecondHit(Transform hitTarget)
    {
        if (!UseSecondHit) return;
        if (DelaySecondHit) _ = SecondHitDelay(hitTarget);
        else Instantiate(_secondHitObj, hitTarget.position, Quaternion.identity);
    }

    /// <summary>
    /// [延遲]二段傷害
    /// </summary>
    /// <param name="hitTarget">觸發目標</param>
    /// <returns></returns>
    private async Task SecondHitDelay(Transform hitTarget)
    {
        await Task.Delay(TimeSpan.FromSeconds(_secondHitDelay));
         Instantiate(_secondHitObj, hitTarget.position, Quaternion.identity);
}
    #endregion 運行手段
}
