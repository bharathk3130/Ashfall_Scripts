using UnityEngine;
using Clickbait.Utilities;

public class Mask : MonoBehaviour
{
    [SerializeField] PolygonCollider2D _polygonCollider;
    [SerializeField] Transform _camTransform;
    [SerializeField] Vector2 _offset;

    Vector3 _colliderScale;
    float _zPos;

    Vector2 _camStartPos;

    void Awake()
    {
        _zPos = transform.position.z;
    }

    void Start()
    {
        _camStartPos = _camTransform.position;
        _colliderScale = _polygonCollider.transform.lossyScale;
    }

    void Update()
    {
        Vector2 movingOffset = (Vector2)_camTransform.position - _camStartPos;
        transform.position = GetCentroid(_polygonCollider.points).With(z: _zPos) + (Vector3)movingOffset;
    }

    Vector3 GetCentroid(Vector2[] points)
    {
        Vector3 centroid = Vector3.zero;
        foreach (Vector3 point in points)
        {
            centroid += point.Multiply(_colliderScale.x, _colliderScale.y, _colliderScale.z);
        }
        
        if (points.Length > 1)
            centroid /= points.Length;

        centroid += (Vector3)_offset;

        return centroid;
    }
}