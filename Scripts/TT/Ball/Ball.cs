using System;
using Clickbait.Utilities;
using KBCore.Refs;
using UnityEngine;

namespace TT
{
    public enum Side
    {
        None,
        Right,
        Left
    }

    public class Ball : ValidatedMonoBehaviour
    {
        [Header("REFERENCES")] [SerializeField, Self]
        Rigidbody2D _rb;

        [SerializeField] LayerMask _cvColliderLayer;
        [SerializeField] Transform _tableCorner;
        [SerializeField] Transform _screenCorner;
        [SerializeField] TTManager _ttManager;

        [Header("SETTINGS")] [SerializeField] float _speed = 10;
        [SerializeField] int _maxAngle = 30;

        Side _hitBy;
        IBallBehaviour _ballBehaviour;

        Vector2 _rbVel;

        void Awake()
        {
            _ballBehaviour = GetComponent<IBallBehaviour>();
        }

        void Update()
        {
            if (_hitBy != Side.None)
            {
                if (_rb.velocity.magnitude < _speed * 0.9f)
                {
                    _rb.velocity = _rbVel;
                }
            }
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("CVCollider"))
            {
                Vector2 dir = _rb.velocity.magnitude > 0 ? _rb.velocity : col.contacts[0].point - _rb.position;
                Vector2 behindPoint = -dir.normalized * 10;

                // Draw a raycast from behindPoint in the direction of dir and get the point where it hits something of Layer "CVCollider"
                RaycastHit2D hit = Physics2D.Raycast(behindPoint, dir, 100, _cvColliderLayer);
                Vector2 reflection = Vector2.Reflect(dir, hit.normal).normalized;

                if (Math.Sign(transform.position.x) == Math.Sign(reflection.x))
                {
                    // If the ball is on the right side of the screen, it's reflection should be left
                    reflection = reflection.Multiply(x: -1);
                }

                SetHitBy();
                _ballBehaviour.PlayerHitBall();
                _rbVel = reflection * _speed;
                _rb.velocity = _rbVel;
            }
        }

        public void ResetBall()
        {
            _hitBy = Side.None;
            _rb.velocity = Vector2.zero;
            _rbVel = Vector2.zero;
            _ballBehaviour.ResetBall();
        }

        public void PointLost()
        {
            _ttManager.PointLost(_hitBy);
        }

        public bool IsInTableBounds() => IsInBounds(_tableCorner.position);
        public bool IsInScreenBounds() => IsInBounds(_screenCorner.position);

        bool IsInBounds(Vector2 corner) => Mathf.Abs(transform.position.x) <= Mathf.Abs(corner.x) &&
                                           Mathf.Abs(transform.position.y) <= Mathf.Abs(corner.y);

        void SetHitBy() => _hitBy = Mathf.Sign(transform.position.x) > 0 ? Side.Right : Side.Left;

        Vector2 ClampVerticalComponent(Vector2 vector)
        {
            // Get the angle of the vector in degrees
            float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;

            // Clamp the angle to the range -30 to 30 degrees
            angle = Mathf.Clamp(angle, -_maxAngle, _maxAngle);

            // Convert the angle back to radians
            float angleRad = angle * Mathf.Deg2Rad;

            // Calculate the new x and y components based on the clamped angle
            float x = Mathf.Cos(angleRad);
            float y = Mathf.Sin(angleRad);

            // Create the new Vector2 with the clamped angle and normalize it
            Vector2 clampedAngle = new Vector2(x, y).normalized;

            return clampedAngle;
        }
    }
}