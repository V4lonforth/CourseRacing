using Scripts.Track.Results;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class TimeDisplay : MonoBehaviour
    {
        [SerializeField] private TimeManager timeManager;
        [SerializeField] private Text text;
        
        private void Awake()
        {
            timeManager.OnTimeChanged += DisplayText;
        }

        private void DisplayText(float value)
        {
            text.text = FormatHelper.FormatTime(value);
        }
    }
}