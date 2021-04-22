using System;

namespace Scripts.Track.Results
{
    [Serializable]
    public class TrackResult
    {
        public float time;
        public int score;

        public TrackResult(float time, int score)
        {
            this.time = time;
            this.score = score;
        }
    }
}