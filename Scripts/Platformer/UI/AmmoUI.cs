using TMPro;
using UnityEngine;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] Gun _gun;
    [SerializeField] TextMeshProUGUI _ammoText;

    void Start()
    {
        _gun.OnShoot += UpdateAmmoUI;
        _gun.OnReload += UpdateAmmoUI;
    }

    void UpdateAmmoUI()
    {
        _ammoText.text = $"{_gun.BulletsLeft}/{_gun.GetGunData().Ammo}";
    }
}
