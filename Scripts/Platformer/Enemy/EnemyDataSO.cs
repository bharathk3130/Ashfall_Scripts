using UnityEngine;

[CreateAssetMenu(menuName = "Create EnemyDataSO", fileName = "EnemyDataSO", order = 0)]
public class EnemyDataSO : ScriptableObject
{
    [Header("Movement")]
    public float ChaseRange;
    public float PunchingRange;
    public float AttackRange;
    public float TerritoryRange;

    [Header("Jumping")]
    public float ObstacleDist;
    public LayerMask ObstacleLayer;
    public float JumpDuration = 1;
}