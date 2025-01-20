using System.Collections;
using Clickbait.Utilities;
using KBCore.Refs;
using UnityEngine;

public class EnemyAI : ValidatedMonoBehaviour, IRobotInput
{
    public bool JumpDown { get; set; }
    public bool JumpHeld { get; set; }
    public Vector2 Move { get; set; }

    [SerializeField, Self] RobotController _robotController;
    [SerializeField] Transform _footPos;
    [SerializeField] Transform _spawnPoint;

    [SerializeField] EnemyDataSO _enemyData;
    [SerializeField] WeaponDataSO _pistolData;

    public Observer<bool> IsAttacking;
    Transform _player;

    Vector3 _startPos;
    public EnemyDataSO EnemyData => _enemyData;

    float _punchingRange;
    public float PunchingRange => _punchingRange;
    public void SetPunchingRange(float punchingRange) => _punchingRange = punchingRange;

    void Awake()
    {
        _startPos = transform.position;
    }

    void Start()
    {
        _player = Singleton.Instance.PlayerTransform;
        SetPunchingRange(_enemyData.PunchingRange);
    }

    void Update()
    {
        float playerDist = PlayerDist();

        if (playerDist > _enemyData.ChaseRange || playerDist < _punchingRange)
        {
            Move = Vector2.zero;
        } else
        {
            Vector2 direction = _player.position - transform.position;
            if (FarFromBase() && RunningAwayFromBase(direction.x))
            {
                Move = Vector2.zero;
            } else
            {
                Move = new Vector2(Mathf.Sign(direction.x), 0);
            }
        }

        if (!JumpHeld && Move.magnitude > 0)
        {
            Ray ray = new Ray(_footPos.position, _footPos.forward);
            if (Physics.Raycast(ray, out RaycastHit _, _enemyData.ObstacleDist, _enemyData.ObstacleLayer))
            {
                StartCoroutine(HandleJump());
            }
        }

        IsAttacking.Value = playerDist < _enemyData.AttackRange;
    }

    IEnumerator HandleJump()
    {
        JumpDown = true;
        JumpHeld = true;

        yield return null;

        JumpDown = false;

        yield return new WaitForSeconds(_enemyData.JumpDuration);

        JumpHeld = false;
    }

    float PlayerDist() => Vector3.Distance(transform.position, _player.position);
    bool FarFromBase() => Vector3.Distance(transform.position, _startPos) > _enemyData.TerritoryRange;

    bool RunningAwayFromBase(float dir)
    {
        Vector3 toCurrent = transform.position - _startPos;
        return Vector3.Dot(toCurrent.normalized, new Vector3(dir, 0, 0)) > 0;
    }
    
    void OnDrawGizmosSelected()
    {
        // Draw circles around the enemy for different ranges
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _enemyData.ChaseRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _enemyData.PunchingRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _enemyData.AttackRange);

        Gizmos.color = Color.green;
        Vector3 territoryCenter = Application.isPlaying ? _startPos : transform.position;
        Gizmos.DrawWireSphere(territoryCenter, _enemyData.TerritoryRange);

        // Draw a ray from _footPos for obstacle detection
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(_footPos.position, _footPos.forward * _enemyData.ObstacleDist);
        
        Gizmos.color = Color.cyan;
        Vector3 flatDirection = _spawnPoint.forward;
        flatDirection.z = 0;
        flatDirection.Normalize();
        Gizmos.DrawRay(_spawnPoint.position, flatDirection * 30);
    }
}