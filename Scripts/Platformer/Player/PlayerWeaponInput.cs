using KBCore.Refs;
using UnityEngine;

public class PlayerWeaponInput : ValidatedMonoBehaviour
{
    [SerializeField, Self] Gun _gun;

    void Start()
    {
        _gun.SetIsPlayer(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _gun.Reload();
        }
        
        if (Input.GetButton("Fire1"))
        {
            _gun.Shoot();
        }
    }
}