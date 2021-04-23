using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Track.Results
{
    [Serializable]
    public class Leaderboard
    {
        [SerializeField] private List<TrackResult> trackResults;

        public Leaderboard()
        {
            trackResults = new List<TrackResult>();
        }

        public void AddResult(TrackResult trackResult)
        {
            trackResults.Add(trackResult);
        }

        public void RemoveResult(TrackResult trackResult)
        {
            trackResults.Remove(trackResult);
        }

        public List<TrackResult> SortByTime() => trackResults.OrderBy(r => r.time).ToList();
        public List<TrackResult> SortByScore() => trackResults.OrderByDescending(r => r.score).ToList();

        public List<TrackResult> SortByScoreAndTime() =>
            trackResults.OrderByDescending(r => r.score).ThenBy(r => r.time).ToList();
    }
}