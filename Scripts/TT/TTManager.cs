using Clickbait.Utilities;
using UnityEngine;

namespace TT
{
    public class TTManager : MonoBehaviour
    {
        [SerializeField] Ball _ball;
        [SerializeField] Transform _corner;

        Vector2 _ballOriginalPos;

        public Observer<int> LeftScore = new(0);
        public Observer<int> RightScore = new(0);

        void Awake()
        {
            _ballOriginalPos = _ball.transform.position;
        }

        void ResetBall()
        {
            _ball.transform.position = _ballOriginalPos;
            _ball.ResetBall();
        }

        public void PointLost(Side hitBy)
        {
            if (hitBy == Side.Right)
            {
                // Left gets a point
                LeftScore.Value++;
            } else
            {
                // Right gets a point
                RightScore.Value++;
            }

            ResetBall();
        }
    }
}