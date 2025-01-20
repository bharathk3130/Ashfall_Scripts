using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance { get; private set; }
    
    [field: SerializeField] public Transform SpriteMaskTransform { get; private set; }
    [field: SerializeField] public Transform PlayerTransform { get; private set; }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
