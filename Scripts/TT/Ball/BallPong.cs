using System;
using KBCore.Refs;
using UnityEngine;

namespace TT
{
    [RequireComponent(typeof(Ball))]
    public class BallPong : ValidatedMonoBehaviour, IBallBehaviour
    {
        [SerializeField, Self] Ball _ball;
        [SerializeField] LayerMask _highBallLayerMask;

        void Awake()
        {
            gameObject.layer = _highBallLayerMask;
        }

        void Update()
        {
            if (!_ball.IsInScreenBounds())
            {
                _ball.PointLost();
            }
        }

        public void ResetBall() { }
        public void PlayerHitBall() { }
    }
}