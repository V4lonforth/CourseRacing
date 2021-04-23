using System.Collections.Generic;
using Scripts.Track.Trajectory;
using UnityEngine;

namespace Scripts.Track.ControlPoints
{
    public class ControlPointsBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject controlPointPrefab;
        [SerializeField] private GameObject finishLinePrefab;
        [SerializeField] private Track track;

        public BezierSplineBuilder splineBuilder;

        public List<ControlPointData> controlPointsData;
        public ControlPointData finishLineData;

        [SerializeField] [HideInInspector] private List<ControlPoint> scoreControlPoints = new List<ControlPoint>();
        [SerializeField] [HideInInspector] private ControlPoint finishLine;

        private void OnValidate()
        {
            GenerateControlPoints();
        }

        public void GenerateControlPoints()
        {
            for (var i = 0; i < controlPointsData.Count; i++)
            {
                ControlPoint controlPoint;
                if (i >= scoreControlPoints.Count)
                {
                    controlPoint = Instantiate(controlPointPrefab, track.transform).GetComponent<ControlPoint>();
                    scoreControlPoints.Add(controlPoint);
                }
                else if (scoreControlPoints[i] == null)
                {
                    controlPoint = Instantiate(controlPointPrefab, track.transform).GetComponent<ControlPoint>();
                    scoreControlPoints[i] = controlPoint;
                }
                else
                {
                    controlPoint = scoreControlPoints[i];
                }

                controlPoint.transform.position =
                    track.trajectory.GetPosition(controlPointsData[i].trajectoryPosition);
                controlPoint.transform.rotation =
                    track.trajectory.GetOrientation(controlPointsData[i].trajectoryPosition);
            }

            UnityEditor.EditorApplication.delayCall += () =>
            {
                while (controlPointsData.Count < scoreControlPoints.Count)
                {
                    DestroyImmediate(scoreControlPoints[scoreControlPoints.Count - 1].gameObject);
                    scoreControlPoints.RemoveAt(scoreControlPoints.Count - 1);
                }
            };

            if (finishLine == null)
            {
                finishLine = Instantiate(finishLinePrefab, track.transform).GetComponent<ControlPoint>();
            }

            finishLine.transform.position = track.trajectory.GetPosition(finishLineData.trajectoryPosition);
            finishLine.transform.rotation = track.trajectory.GetOrientation(finishLineData.trajectoryPosition);

            track.trackControlPoints = new TrackControlPoints(scoreControlPoints, finishLine);
        }
    }
}