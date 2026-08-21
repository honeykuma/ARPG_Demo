using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCtrl : BaseCtrl
{
    #region 基本參數
    private Controls _controls;
    
    [SerializeField]
    private int _airJumpCountMax = 1;
    private int _airJumpCount;
    [SerializeField]
    private float _dashSpeed = 16f;
    private float _dashDuration = 0.15f;
    #endregion 基本參數

    #region 公用參數
    /// <summary>
    /// 產生一組預設好的控制檔
    /// </summary>
    public Controls InputsCtrl => _controls ??= new Controls();

    /// <summary>
    /// 從輸入取得的方向向量
    /// </summary>
    public override Vector2 MoveInput => InputsCtrl.Play.Move.ReadValue<Vector2>();

    /// <summary>
    /// 角度補償(攝影機側轉量)
    /// </summary>
    public override Quaternion AngComp
    {
        get
        {
            return Quaternion.Euler(0f ,GameManager.cameraRota.y, 0f);
        }
    }    

    /// <summary>
    /// 是否可執行空中跳躍
    /// </summary>
    public bool CanAirJump => _airJumpCount > 0;
    #endregion 公用參數

    #region 生命週期
    private void OnEnable()
    {
        GameManager.SetCurrentPlayer(this);
        InputsCtrl.Play.Enable();
        //操作行為事件訂閱
        InputsCtrl.Play.Jump.performed += Jump;
        InputsCtrl.Play.Attack.performed += Attack;
        InputsCtrl.Play.Dash.performed += Dash;
    }

    private void OnDisable()
    {
        GameManager.SetCurrentPlayer(null);
        InputsCtrl.Play.Disable();
        //操作行為事件訂閱取消
        InputsCtrl.Play.Jump.performed -= Jump;
        InputsCtrl.Play.Attack.performed -= Attack;
        InputsCtrl.Play.Dash.performed -= Dash;
    }
    #endregion 生命週期

    #region 角色物理控制
    /// <summary>
    /// 增強角色跳躍功能
    /// </summary>
    protected override void Gravity()
    {
        base.Gravity();
        if (IsGrounded) _airJumpCount = _airJumpCountMax;
    }
    #endregion 角色物理控制

    #region 跳躍功能    
    /// <summary>
    /// 跳躍事件
    /// </summary>
    /// <param name="context">接收輸入</param>
    void Jump(InputAction.CallbackContext context)
    {
        if (state == State.Attack || state == State.Dash) return;
        
        if (IsGrounded)
        {
            JumpHandle();
        }
        else if(CanAirJump)
        {
            _airJumpCount--;
            _jumpPower = 0.5f;
            JumpHandle();
        }
    }
    
    #endregion 跳躍功能

    #region 攻擊功能
    
    private void Attack(InputAction.CallbackContext context)
    {
        if (state == State.Dash) return;
        if (IsAttacking && _inConboWindow)
        {
            Combo++;
            _inConboWindow = false;
            AttackHandle();
        }        
        else if (!IsAttacking)
        {//完全停止攻擊後；連擊重啟
            Combo = 1;
            AttackHandle();
        }
    }
    #endregion 攻擊功能

    #region 衝刺功能
    private void Dash(InputAction.CallbackContext context)
    {
        if (state == State.Attack || state == State.Dash) return;
        ChangeState(State.Dash);
        animaCtrl.SetTrigger(AniHash.DashTrigger);

        _ = DashHandle();
    }

    private async Task DashHandle()
    {
        charCtrl.transform.rotation = Quaternion.LookRotation(transform.forward);
        _velocity = transform.forward * _dashSpeed;
        _velocity.y = 0;
        //推進
        await Task.Delay(TimeSpan.FromSeconds(_dashDuration));

        if (state == State.Dash)
        {
            _velocity = Vector3.zero;
            ChangeState(IsGrounded ? State.Idle : State.Jump);
        }
    }
    #endregion 衝刺功能
}
