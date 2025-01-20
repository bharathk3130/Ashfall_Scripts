using System;
using Clickbait.Utilities;
using KBCore.Refs;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class RobotController : ValidatedMonoBehaviour, IPlayerController
{
    [SerializeField] ScriptableStats _stats;
    
    IRobotInput _robotInput;
    
    Rigidbody _rb;
    CapsuleCollider _col;
    FrameInput _frameInput;
    Vector2 _frameVelocity;
    bool _cachedQueryStartInColliders;

    Death _death;
    
    #region Interface

    public Vector2 FrameInput => _frameInput.Move;
    public event Action<bool, float> GroundedChanged;
    public event Action Jumped;
    public Observer<float> MoveDir = new(0);

    #endregion

    float _time;
    bool _isDead;
    bool _facingRight = true;

    public void SetFacingRight(bool facingRight) => _facingRight = facingRight;
    public bool IsFacingRight => _facingRight;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _col = GetComponent<CapsuleCollider>();
        _robotInput = GetComponent<IRobotInput>();
        _death = GetComponent<Death>();

        _cachedQueryStartInColliders = Physics2D.queriesStartInColliders;
        
        Physics.IgnoreCollision(_col, GetComponent<BoxCollider>());
    }

    void Start()
    {
        _death.OnDeath += () =>
        {
            _isDead = true;
            _frameInput = new FrameInput
            {
                JumpDown = false,
                JumpHeld = false,
                Move = Vector2.zero
            };
        };
    }

    void LateUpdate()
    {
        if (_isDead) return;
        
        _time += Time.deltaTime;
        GatherInput();
    }

    void GatherInput()
    {
        _frameInput = new FrameInput
        {
            JumpDown = _robotInput.JumpDown,
            JumpHeld = _robotInput.JumpHeld,
            Move = _robotInput.Move
        };
        
        MoveDir.Value = _frameInput.Move.x;

        if (_stats.SnapInput)
        {
            _frameInput.Move.x = Mathf.Abs(_frameInput.Move.x) < _stats.HorizontalDeadZoneThreshold
                ? 0
                : Mathf.Sign(_frameInput.Move.x);
            _frameInput.Move.y = Mathf.Abs(_frameInput.Move.y) < _stats.VerticalDeadZoneThreshold
                ? 0
                : Mathf.Sign(_frameInput.Move.y);
        }

        if (_frameInput.JumpDown)
        {
            _jumpToConsume = true;
            _timeJumpWasPressed = _time;
        }
    }

    void FixedUpdate()
    {
        if (_isDead) return;
        
        CheckCollisions();

        HandleJump();
        HandleDirection();
        HandleGravity();

        ApplyMovement();
    }

    #region Collisions

    float _frameLeftGrounded = float.MinValue;
    bool _grounded;

    public bool IsGrounded => _grounded;

    void CheckCollisions()
    {
        Physics2D.queriesStartInColliders = false;
        
        // Ground and Ceiling
        bool groundHit = Physics.SphereCast(_col.bounds.center, _col.radius, Vector3.down, out RaycastHit hit, 
            _stats.GrounderDistance, ~_stats.PlayerLayer);
        bool ceilingHit = Physics.SphereCast(_col.bounds.center, _col.radius, Vector3.up, out RaycastHit hit2, 
            _stats.GrounderDistance, ~_stats.PlayerLayer);
        
        // Hit a Ceiling
        if (ceilingHit) _frameVelocity.y = Mathf.Min(0, _frameVelocity.y);

        // Landed on the Ground
        if (!_grounded && groundHit)
        {
            _grounded = true;
            _coyoteUsable = true;
            _bufferedJumpUsable = true;
            _endedJumpEarly = false;
            GroundedChanged?.Invoke(true, Mathf.Abs(_frameVelocity.y));
        }
        // Left the Ground
        else if (_grounded && !groundHit)
        {
            _grounded = false;
            _frameLeftGrounded = _time;
            GroundedChanged?.Invoke(false, 0);
        }

        Physics2D.queriesStartInColliders = _cachedQueryStartInColliders;
    }

    #endregion


    #region Jumping

    bool _jumpToConsume;
    bool _bufferedJumpUsable;
    bool _endedJumpEarly;
    bool _coyoteUsable;
    float _timeJumpWasPressed;

    bool HasBufferedJump => _bufferedJumpUsable && _time < _timeJumpWasPressed + _stats.JumpBuffer;
    bool CanUseCoyote => _coyoteUsable && !_grounded && _time < _frameLeftGrounded + _stats.CoyoteTime;

    void HandleJump()
    {
        if (!_endedJumpEarly && !_grounded && !_frameInput.JumpHeld && _rb.velocity.y > 0) _endedJumpEarly = true;

        if (!_jumpToConsume && !HasBufferedJump) return;

        if (_grounded || CanUseCoyote) ExecuteJump();

        _jumpToConsume = false;
    }

    void ExecuteJump()
    {
        _endedJumpEarly = false;
        _timeJumpWasPressed = 0;
        _bufferedJumpUsable = false;
        _coyoteUsable = false;
        _frameVelocity.y = _stats.JumpPower;
        Jumped?.Invoke();
    }

    #endregion

    #region Horizontal

    void HandleDirection()
    {
        if (_frameInput.Move.x == 0)
        {
            var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, 0, deceleration * Time.fixedDeltaTime);
        } else
        {
            _frameVelocity.x = Mathf.MoveTowards(_frameVelocity.x, _frameInput.Move.x * _stats.MaxSpeed,
                _stats.Acceleration * Time.fixedDeltaTime);
        }
    }

    #endregion

    #region Gravity

    void HandleGravity()
    {
        if (_grounded && _frameVelocity.y <= 0f)
        {
            _frameVelocity.y = _stats.GroundingForce;
        } else
        {
            var inAirGravity = _stats.FallAcceleration;
            if (_endedJumpEarly && _frameVelocity.y > 0) inAirGravity *= _stats.JumpEndEarlyGravityModifier;
            _frameVelocity.y =
                Mathf.MoveTowards(_frameVelocity.y, -_stats.MaxFallSpeed, inAirGravity * Time.fixedDeltaTime);
        }
    }

    #endregion

    void ApplyMovement() => _rb.velocity = _frameVelocity;

#if UNITY_EDITOR
    void OnValidate()
    {
        if (_stats == null)
            Debug.LogWarning("Please assign a ScriptableStats asset to the Player Controller's Stats slot", this);
    }
#endif
}

public struct FrameInput
{
    public bool JumpDown;
    public bool JumpHeld;
    public Vector2 Move;
}

public interface IPlayerController
{
    public event Action<bool, float> GroundedChanged;

    public event Action Jumped;
    public Vector2 FrameInput { get; }
}