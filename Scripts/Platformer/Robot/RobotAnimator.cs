using System;
using Clickbait.Utilities;
using KBCore.Refs;
using UnityEngine;

public class RobotAnimator : ValidatedMonoBehaviour
{
    static readonly int _idleHash = Animator.StringToHash("Idle");
    static readonly int _runHash = Animator.StringToHash("Run_Guard");
    static readonly int _jumpHash = Animator.StringToHash("Jump");
    static readonly int _dieHash = Animator.StringToHash("Die");
    static readonly int _reloadHash = Animator.StringToHash("Reload");

    [SerializeField, Child] Animator _anim;
    [SerializeField, Self] RobotController _robotController;
    [SerializeField, Self] Death _death;
    [SerializeField] Gun _gun;

    [Header("Settings")] [SerializeField] float _crossFadeDuration = 0.1f;

    Transform _model;
    IRobotInput _input;

    bool _grounded = true;
    bool _isMoving;
    bool _facingRight = true;
    bool _isDead;

    CountDownTimer _holdCurrentAnimation;

    void Awake()
    {
        _model = _anim.transform;
        _holdCurrentAnimation = new CountDownTimer(1);
        _holdCurrentAnimation.OnTimerStop += () =>
        {
            if (_grounded)
            {
                PlayAnim(_isMoving ? _runHash : _idleHash);
            }
        };

        PlayAnim(_idleHash);
    }

    void Start()
    {
        _robotController.Jumped += OnJump;
        _robotController.GroundedChanged += OnGroundedChanged;
        // _gun.OnReload += Reload;
        
        _robotController.MoveDir.AddListener(OnMoveChange);
        _death.OnDeath += () =>
        {
            if (!_isDead)
            {
                _isDead = true;
                PlayAnim(_dieHash);
            }
        };
        
        _robotController.SetFacingRight(_facingRight);
    }

    void Update()
    {
        if (_holdCurrentAnimation.IsRunning)
        {
            _holdCurrentAnimation.Tick(Time.deltaTime);
        }
    }

    void Reload()
    {
        PlayAnim(_reloadHash);
        _holdCurrentAnimation.Reset(_gun.GetReloadTime());
        _holdCurrentAnimation.Start();
    }

    void OnJump()
    {
        PlayAnim(_jumpHash);
        _grounded = false;
    }

    void OnGroundedChanged(bool grounded, float _)
    {
        _grounded = grounded;
        if (grounded)
        {
            PlayAnim(_isMoving ? _runHash : _idleHash);
        }
    }

    void OnMoveChange(float moveDir)
    {
        _isMoving = moveDir != 0;
        if (_isMoving)
            HandlePlayerFlip(moveDir);

        if (!_grounded) return;

        PlayAnim(_isMoving ? _runHash : _idleHash);
    }

    void HandlePlayerFlip(float dir)
    {
        if (_facingRight && dir < 0)
        {
            FlipPlayer();
        } else if (!_facingRight && dir > 0)
        {
            FlipPlayer();
        }
    }

    void FlipPlayer()
    {
        Vector3 euler = _model.localEulerAngles;
        euler.y += 180;
        _model.localEulerAngles = euler;
        _facingRight = !_facingRight;
        _robotController.SetFacingRight(_facingRight);
    }

    void PlayAnim(int hash)
    {
        if (_holdCurrentAnimation.IsRunning) return;

        if (!_isDead || hash == _dieHash)
        {
            _anim.CrossFade(hash, _crossFadeDuration);
        }
    }
}