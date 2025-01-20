using UnityEngine;

public class PlayerDeath : Death
{
    [SerializeField] float _reloadSceneDelay = 3;
    
    public override void Die()
    {
        base.Die();
        Invoke(nameof(ReloadScene), _reloadSceneDelay);
    }
    
    void ReloadScene() => SceneLoader.ReloadScene();
}
