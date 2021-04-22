using UnityEngine;

namespace Scripts.Track.Results
{
    public class ResultManager : MonoBehaviour
    {
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private TimeManager timeManager;
        
        private void Awake()
        {
            TrackManager.Instance.OnTrackFinish += SaveResults;
        }

        private void SaveResults(Track track)
        {
            var trackResult = new TrackResult(timeManager.CurrentTime, scoreManager.Score);
            var leaderboard = LeaderboardManager.Load() ?? new Leaderboard();
            leaderboard.AddResult(trackResult);
            LeaderboardManager.Save(leaderboard);
        }
    }
}