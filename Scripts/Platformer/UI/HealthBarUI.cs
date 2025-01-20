using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : ValidatedMonoBehaviour
{
    [SerializeField] Health _health;
    [SerializeField, Self] Slider _slider;

    void Start()
    {
        _health.CurrentHealth.AddListener(UpdateSlider);
    }

    void UpdateSlider(int health)
    {
        _slider.value = (float)health / _health.MaxHealth;
    }
}
