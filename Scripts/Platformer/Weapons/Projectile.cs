using Clickbait.Utilities;
using KBCore.Refs;
using UnityEngine;

public class Projectile : ValidatedMonoBehaviour
{
    [SerializeField, Self] Rigidbody _rb;

    WeaponDataSO _weaponData;
    string _targetTag = "Enemy";

    public void Init(WeaponDataSO weaponData, string targetTag)
    {
        _weaponData = weaponData;
        _targetTag = targetTag;

        _rb.velocity = transform.forward.With(z: 0).normalized * _weaponData.BulletSpeed;
        Destroy(gameObject, _weaponData.BulletLifetime);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag(_targetTag))
        {
            col.gameObject.GetComponent<Health>().TakeDamage(_weaponData.Damage);
        }
        
        Destroy(gameObject);
    }
}