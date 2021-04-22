using System;
using Scripts.Track;
using Scripts.Track.ControlPoints;
using UnityEngine;

namespace Scripts.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public int Score { get; private set; }
        public Action<int> OnScoreChanged { get; set; }
        
        private void Awake()
        {
            TrackManager.Instance.OnTrackStart += StartTrack;
        }

        private void Start()
        {
            OnScoreChanged?.Invoke(Score);
        }
        
        private void StartTrack(Track.Track track)
        {
            foreach (var scoreControlPoint in track.trackControlPoints.scoreControlPoints)
            {
                scoreControlPoint.OnPassed += AddScore;
            }
        }

        private void AddScore(ControlPoint controlPoint)
        {
            Score += controlPoint.GetComponent<ScoreReward>().score;
            OnScoreChanged?.Invoke(Score);
        }
    }
}