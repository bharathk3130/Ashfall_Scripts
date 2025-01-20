using Clickbait.Utilities;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    [SerializeField] float _speed = 5;
    [SerializeField] float _groundDownwardForce = 5;
    [SerializeField] float _startDelay = 3;

    CountDownTimer _startDelayTimer;
    
    void Start()
    {
        _startDelayTimer = new CountDownTimer(_startDelay);
        _startDelayTimer.Start();
    }
    
    void Update()
    {
        if (_startDelayTimer.IsRunning)
        {
            _startDelayTimer.Tick(Time.deltaTime);
            return;
        }
        
        transform.Translate(Vector3.right * (_speed * Time.deltaTime), Space.World);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Ground"))
        { 
            col.GetComponent<Rigidbody>().isKinematic = false;
        } else if(col.gameObject.TryGetComponent(out Health health))
        {
            health.TakeDamage(500);
        }
    }
}
