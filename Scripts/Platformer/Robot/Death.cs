using System;
using UnityEngine;

public abstract class Death : MonoBehaviour
{
    public event Action OnDeath = delegate { };
    
    [SerializeField] MonoBehaviour[] _componentsToDisable;
    
    public virtual void Die()
    {
        OnDeath?.Invoke();
        Array.ForEach(_componentsToDisable, c => c.enabled = false);
    }
}