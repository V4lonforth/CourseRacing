using System.Collections.Generic;
using Scripts.Track.Trajectory;
using UnityEngine;

namespace Scripts.Track.ControlPoints
{
    public class ControlPointsBuilder : MonoBehaviour
    {
        [SerializeField] private GameObject controlPointPrefab;
        [SerializeField] private Track track;

        public BezierSplineBuilder splineBuilder;
        
        public List<ControlPointData> controlPointsData;

        [SerializeField] [HideInInspector] private List<ControlPoint> controlPoints = new List<ControlPoint>();
        
        public void GenerateControlPoints()
        {
            for (var i = 0; i < controlPointsData.Count; i++)
            {
                if (i >= controlPoints.Count)
                {
                    var controlPoint = Instantiate(controlPointPrefab, track.transform).GetComponent<ControlPoint>();
                    controlPoint.transform.position =
                        track.trajectory.GetPosition(controlPointsData[i].trajectoryPosition);
                    
                    controlPoints.Add(controlPoint);
                } 
                else if (controlPoints[i] == null)
                {
                    var controlPoint = Instantiate(controlPointPrefab, track.transform).GetComponent<ControlPoint>();
                    controlPoint.transform.position =
                        track.trajectory.GetPosition(controlPointsData[i].trajectoryPosition);

                    controlPoints[i] = controlPoint;
                }
                else
                {
                    controlPoints[i].transform.position = 
                        track.trajectory.GetPosition(controlPointsData[i].trajectoryPosition);
                }
            }
        }
    }
}