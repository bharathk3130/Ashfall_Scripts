using KBCore.Refs;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyWeaponInput : ValidatedMonoBehaviour
{
    [SerializeField] EnemyAI _enemyAI;
    [SerializeField, Self] Gun _gun;

    bool _inAttackingRange;
    float _punchingRange;

    void Start()
    {
        _punchingRange = _enemyAI.EnemyData.PunchingRange;
        
        _gun.SetIsPlayer(false);
        _enemyAI.IsAttacking.AddListener(isAttacking => _inAttackingRange = isAttacking);
    }

    void Update()
    {
        if (!_inAttackingRange) return;
        
        if (_gun.IsShootingPathFree())
        {
            if (!Mathf.Approximately(_enemyAI.PunchingRange, _punchingRange))
            {
                _enemyAI.SetPunchingRange(_punchingRange);
            }
            _gun.Shoot();
        } else
        {
            if (Mathf.Abs(_enemyAI.PunchingRange) > 0.2f)
            {
                _enemyAI.SetPunchingRange(0);
            }
        }
    }
}