using Scripts.Track;
using Scripts.Track.Results;
using Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts.UI.Menus
{
    public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverMenu;
        
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private TimeManager timeManager;

        [SerializeField] private Text scoreText;
        [SerializeField] private Text timeText;

        private void Awake()
        {
            gameOverMenu.SetActive(false);
            TrackManager.Instance.OnTrackFinish += ShowResults;
        }

        private void ShowResults(Track.Track track)
        {
            gameOverMenu.SetActive(true);
            
            scoreText.text = scoreManager.Score.ToString();
            timeText.text = FormatHelper.FormatTime(timeManager.CurrentTime);
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitToMainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}