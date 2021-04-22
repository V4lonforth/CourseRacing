using Scripts.Track.Results;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class ResultDisplay : MonoBehaviour
    {
        [SerializeField] private Text scoreText;
        [SerializeField] private Text timeText;

        public void DisplayResult(TrackResult trackResult)
        {
            scoreText.text = trackResult.score.ToString();
            timeText.text = FormatHelper.FormatTime(trackResult.time);
        }
    }
}
