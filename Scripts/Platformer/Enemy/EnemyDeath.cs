using UnityEngine;
using KBCore.Refs;

public class EnemyDeath : Death
{
    [SerializeField, Self] Rigidbody _rb;
    
    [SerializeField] float _selfDestructDelay = 3;
    [SerializeField] float _explosionForce = 10;
    
    public override void Die()
    {
        base.Die();
        ThrowSelfOffPlatform();
        Invoke(nameof(DestroySelf), _selfDestructDelay);
    }

    void DestroySelf() => Destroy(gameObject);

    void ThrowSelfOffPlatform()
    {
        _rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        Vector3 randomDirection = new Vector3(0, 0.3f, Random.value > 0.5f ? 1 : -1).normalized;
        _rb.AddForce(randomDirection * _explosionForce, ForceMode.Impulse);
    }
}