using UnityEngine;

public class EnemyGunAim : MonoBehaviour
{
    [SerializeField] EnemyAI _enemyAI;
    [SerializeField] Transform _arm;
    [SerializeField] RobotController _robotController;

    [SerializeField, Range(30, 100)] float _facingLeftAngleError = 41;
    [SerializeField, Range(30, 100)] float _facingRightAngleError = 50;

    Transform _player;
    
    bool _inAttackingRange;

    void Start()
    {
        _player = Singleton.Instance.PlayerTransform;
        
        _enemyAI.IsAttacking.AddListener(isAttacking => _inAttackingRange = isAttacking);
    }

    void LateUpdate()
    {
        if (!_inAttackingRange) return;
        
        Vector3 direction = _player.position - _arm.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float error = _robotController.IsFacingRight ? _facingRightAngleError : _facingLeftAngleError;
        _arm.rotation = Quaternion.Euler(0, 180, -(angle + error));
    }
}