using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.UI.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private GameObject leaderboardMenu;
        
        public void StartLevel()
        {
            SceneManager.LoadScene("Level");
        }

        public void OpenLeaderboard()
        {
            gameObject.SetActive(false);
            leaderboardMenu.SetActive(true);
        }
        
        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}