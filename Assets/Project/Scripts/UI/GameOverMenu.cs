using Scripts.Managers;
using Scripts.Track;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Scripts.UI
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
            timeText.text = $"{timeManager.CurrentTime / 60f:00}:{timeManager.CurrentTime % 60f:00.000}";
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