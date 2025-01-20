using UnityEngine;

[CreateAssetMenu(menuName = "Create WeaponDataSO", fileName = "WeaponDataSO")]
public class WeaponDataSO : ScriptableObject
{
    [Header("Gun Settings")]
    public float FireInterval = 0.1f;
    public int Ammo = 30;
    public float ReloadTime = 1.5f;
    
    [Header("Projectile Settings")]
    public Projectile Bullet;
    public LayerMask ShooterLayer;
    public string TargetTag;
    public int Damage = 30;
    public float BulletSpeed = 10;
    public int BulletLifetime = 5;
}