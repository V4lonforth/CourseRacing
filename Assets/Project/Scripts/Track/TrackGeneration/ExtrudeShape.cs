using System;

namespace Scripts.Track.TrackGeneration
{
    [Serializable]
    public struct ExtrudeShape
    {
        public ExtrudeVertex[] vertices;
        public int[] lines;
    }
}