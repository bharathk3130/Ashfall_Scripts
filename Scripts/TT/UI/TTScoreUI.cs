using TMPro;
using UnityEngine;

namespace TT
{
    public class TTScoreUI : MonoBehaviour
    {
        [SerializeField] TTManager _ttManager;
        [SerializeField] TextMeshProUGUI _leftScoreText;
        [SerializeField] TextMeshProUGUI _rightScoreText;

        void OnEnable()
        {
            _ttManager.LeftScore.AddListener(UpdateLeftScore);
            _ttManager.RightScore.AddListener(UpdateRightScore);
        }

        void OnDisable()
        {
            _ttManager.LeftScore.AddListener(UpdateLeftScore);
            _ttManager.RightScore.AddListener(UpdateRightScore);
        }

        void UpdateLeftScore(int score) => _leftScoreText.text = score.ToString();
        void UpdateRightScore(int score) => _rightScoreText.text = score.ToString();
    }
}