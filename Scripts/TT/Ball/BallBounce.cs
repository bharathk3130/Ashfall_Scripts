using System;
using KBCore.Refs;
using UnityEngine;

namespace TT
{
    [RequireComponent(typeof(Ball))]
    public class BallBounce : ValidatedMonoBehaviour, IBallBehaviour
    {
        [Header("REFERENCES")] [SerializeField, Self]
        Ball _ball;

        [Header("SETTINGS")] [SerializeField] float _bottomSize = 0.4f;
        [SerializeField] float _clearNetSize = 0.53f;
        [SerializeField] float _halfBounceDuration = 1f;
        float _timeElapsed = 0f;

        float _topSize;
        float _ballSize;
        int _bounceCount;
        bool _isHighBallLayerMask = true;

        Action _bounceAction = delegate { };

        int _highBallLayer;
        int _normalBallLayer;

        void Awake()
        {
            _highBallLayer = LayerMask.NameToLayer("HighBall");
            _normalBallLayer = LayerMask.NameToLayer("Ball");

            _ballSize = transform.localScale.x;
            _topSize = _ballSize;
            gameObject.layer = _highBallLayer;
        }

        void Update()
        {
            _bounceAction.Invoke();
        }

        public void PlayerHitBall()
        {
            _bounceCount = 0;
            _timeElapsed = 0;
            _bounceAction = BounceDown;
            gameObject.layer = _highBallLayer;
            _isHighBallLayerMask = true;
        }

        void BounceUp()
        {
            float completionPercentage = UpdateElapsedTimeAndGetCompletion();
            SetBallSize(Mathf.Lerp(_ballSize, _topSize, completionPercentage));

            if (!_isHighBallLayerMask && _ballSize > _clearNetSize)
            {
                gameObject.layer = _highBallLayer;
                _isHighBallLayerMask = true;
            }

            if (completionPercentage >= 1f)
            {
                _bounceAction = BounceDown;
                _timeElapsed = 0;
            }
        }

        void BounceDown()
        {
            float completionPercentage = UpdateElapsedTimeAndGetCompletion();
            SetBallSize(Mathf.Lerp(_ballSize, _bottomSize, completionPercentage));

            if (_isHighBallLayerMask && _ballSize < _clearNetSize)
            {
                gameObject.layer = _normalBallLayer;
                _isHighBallLayerMask = false;
            }

            if (completionPercentage >= 1f)
            {
                SetBallSize(_bottomSize);
                _timeElapsed = 0;
                _bounceCount++;

                // Ball is at table level

                if (_ball.IsInTableBounds())
                {
                    _bounceAction = BounceUp;
                } else
                {
                    // Lose point
                    _ball.PointLost();
                }
            }
        }

        float UpdateElapsedTimeAndGetCompletion()
        {
            _timeElapsed += Time.deltaTime;
            return Mathf.Clamp01(_timeElapsed / _halfBounceDuration);
        }

        void SetBallSize(float size)
        {
            transform.localScale = Vector3.one * size;
            _ballSize = size;
        }

        public void ResetBall()
        {
            _bounceAction = delegate { };
            SetBallSize(_topSize);
            gameObject.layer = _highBallLayer;
            _isHighBallLayerMask = true;
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.CompareTag("Net"))
            {
                Invoke(nameof(CallPointLost), 0.75f);
            }
        }

        void CallPointLost() => _ball.PointLost();
    }
}