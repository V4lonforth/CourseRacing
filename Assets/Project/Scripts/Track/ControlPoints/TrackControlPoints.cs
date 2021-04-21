using System;
using System.Collections.Generic;

namespace Scripts.Track.ControlPoints
{
    [Serializable]
    public class TrackControlPoints
    {
        public List<ControlPoint> scoreControlPoints;
        public ControlPoint finishLine;

        public TrackControlPoints(List<ControlPoint> scoreControlPoints, ControlPoint finishLine)
        {
            this.scoreControlPoints = scoreControlPoints;
            this.finishLine = finishLine;
        }
    }
}