using UnityEngine;

namespace Scripts.Track.ControlPoints
{
    [RequireComponent(typeof(ControlPoint))]
    public class FinishLine : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<ControlPoint>().OnPassed += PassFinishLine;
        }

        private void PassFinishLine(ControlPoint controlPoint)
        {
            TrackManager.Instance.FinishTrack();
        }
    }
}