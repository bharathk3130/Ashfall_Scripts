using System;
using Clickbait.Utilities;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] protected WeaponDataSO _gunData;
    [SerializeField] protected Transform _spawnPoint;

    public event Action OnShoot = delegate { };
    public event Action OnReload = delegate { };
    
    [SerializeField] CountDownTimer _shootTimer;
    [SerializeField] CountDownTimer _reloadTimer;

    string _targetTag = "Enemy";

    int _bulletsLeft;
    
    public int BulletsLeft => _bulletsLeft;

    void Awake()
    {
        _bulletsLeft = _gunData.Ammo;
        
        _shootTimer = new CountDownTimer(_gunData.FireInterval);
        _reloadTimer = new CountDownTimer(_gunData.ReloadTime);
        _reloadTimer.OnTimerStart += () => OnReload.Invoke();
        _reloadTimer.OnTimerStop += () => _bulletsLeft = _gunData.Ammo;
    }
    
    void Update()
    {
        if (_shootTimer.IsRunning)
        {
            _shootTimer.Tick(Time.deltaTime);
        }

        if (_reloadTimer.IsRunning)
        {
            _reloadTimer.Tick(Time.deltaTime);
        }
    }

    public void Shoot()
    {
        if (_shootTimer.IsRunning || _reloadTimer.IsRunning) return;
        
        Projectile projectile = Instantiate(_gunData.Bullet, _spawnPoint.position, _spawnPoint.rotation);
        projectile.Init(_gunData, _targetTag);
        _shootTimer.Start();
        OnShoot.Invoke();

        if (--_bulletsLeft <= 0)
        {
            Reload();
        }
    }

    public void Reload() => _reloadTimer.Start();

    public bool IsShootingPathFree()
    {
        float radius = _gunData.Bullet.GetComponent<CapsuleCollider>().radius;
        if (Physics.SphereCast(_spawnPoint.position, radius, _spawnPoint.forward, out RaycastHit hit,
                100, ~_gunData.ShooterLayer))
        {
            if (hit.transform.CompareTag(_gunData.TargetTag))
            {
                return true;
            }
        }

        return false;
    }

    public void SetIsPlayer(bool isPlayer)
    {
        _targetTag = isPlayer ? "Enemy" : "Player";
    }

    public WeaponDataSO GetGunData() => _gunData;
    public float GetReloadTime() => _gunData.ReloadTime;
}