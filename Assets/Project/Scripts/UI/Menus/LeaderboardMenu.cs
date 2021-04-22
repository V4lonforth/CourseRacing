using Scripts.Track.Results;
using UnityEngine;

namespace Scripts.UI.Menus
{
    public class LeaderboardMenu : MonoBehaviour
    {
        [SerializeField] private GameObject mainMenu;

        [SerializeField] private Transform resultsTransform;
        [SerializeField] private GameObject resultPrefab;

        private void Awake()
        {
            ShowLeaderboard(LeaderboardManager.Load());
        }

        private void ShowLeaderboard(Leaderboard leaderboard)
        {
            if (leaderboard == null) return;

            foreach (var trackResult in leaderboard.SortByScoreAndTime())
            {
                Instantiate(resultPrefab, resultsTransform).GetComponent<ResultDisplay>().DisplayResult(trackResult);
            }
        }

        public void ReturnToMainMenu()
        {
            gameObject.SetActive(false);
            mainMenu.SetActive(true);
        }
    }
}