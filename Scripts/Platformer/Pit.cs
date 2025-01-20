using UnityEngine;

public class Pit : MonoBehaviour
{
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Destroyer")) return;

        if (col.TryGetComponent(out Health health))
        {
            health.TakeDamage(500);
        } else
        {
            Destroy(col.gameObject);
        }
    }
}