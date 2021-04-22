using Scripts.Managers;
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
            text.text = $"{value / 60f:00}:{value % 60f:00.000}";
        }
    }
}