using Scripts.Track.Results;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private Text text;
        
        private void Awake()
        {
            scoreManager.OnScoreChanged += DisplayText;
        }

        private void DisplayText(int value)
        {
            text.text = value.ToString();
        }
    }
}