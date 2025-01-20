using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    [SerializeField] Transform _arm;
    [SerializeField] LayerMask _aimPlaneLayer;

    Camera _cam;

    void Start()
    {
        _cam = Camera.main;
    }

    void LateUpdate()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _aimPlaneLayer))
        {
            Vector3 direction = hit.point - _arm.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            _arm.rotation = Quaternion.Euler(0, 180, -(angle + 48.206f)); // Invert the angle
            
            // Add 180 to global y and -48.206 to global Z
        }
    }
}