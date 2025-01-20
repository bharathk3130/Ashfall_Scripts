using System;
using Clickbait.Utilities;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] int _maxHealth = 100;
    [SerializeField] int _deathDelay = 3;

    public Observer<int> CurrentHealth;
    public int MaxHealth => _maxHealth;
    
    Death _death;

    void Awake()
    {
        _death = GetComponent<Death>();

        CurrentHealth = new Observer<int>(_maxHealth);
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth.Value -= damage;
        
        if (CurrentHealth.Value <= 0)
        {
            _death.Die();
        }
    }
}