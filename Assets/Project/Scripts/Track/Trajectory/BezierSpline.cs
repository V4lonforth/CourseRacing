using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Track.Trajectory
{
    [Serializable]
    public class BezierSpline : ITrajectory
    {
        public List<BezierCurve> trajectories;
        public float Length => GetDistance(1f);

        public BezierSpline(List<BezierCurve> trajectories)
        {
            this.trajectories = trajectories;
        }

        public float GetDistance(float t)
        {
            GetT(t, out var number, out var newT);

            var distance = 0f;
            for (var i = 0; i < number; i++)
                distance += trajectories[i].Length;

            return distance + trajectories[number].GetDistance(newT);
        }

        public float GetT(float distance)
        {
            var accumDistance = 0f;
            int i;
            for (i = 0; i < trajectories.Count; i++)
            {
                var segmentLength = trajectories[i].Length;
                if (accumDistance + segmentLength >= distance)
                {
                    distance -= accumDistance;
                    break;
                }

                accumDistance += segmentLength;
            }

            return i == trajectories.Count ? 1f : GetT(trajectories[i].GetT(distance), i);
        }

        public Vector3 GetPosition(float t)
        {
            GetT(t, out var number, out var newT);
            return trajectories[number].GetPosition(newT);
        }

        public Vector3 GetTangent(float t)
        {
            GetT(t, out var number, out var newT);
            return trajectories[number].GetTangent(newT);
        }

        public Vector3 GetNormal(float t) => GetNormal(t, Vector3.up);

        public Vector3 GetNormal(float t, Vector3 up)
        {
            GetT(t, out var number, out var newT);
            return trajectories[number].GetNormal(newT, up);
        }

        public Quaternion GetOrientation(float t) => GetOrientation(t, Vector3.up);

        public Quaternion GetOrientation(float t, Vector3 up)
        {
            GetT(t, out var number, out var newT);
            return trajectories[number].GetOrientation(newT, up);
        }

        public float GetClosestPointT(Vector3 point, float sectionStart, float sectionEnd, float precision = 0.01f)
        {
            GetT(sectionStart, out var firstTrajectory, out var firstT);
            GetT(sectionEnd, out var secondTrajectory, out var secondT);

            if (firstTrajectory == secondTrajectory)
            {
                return GetT(trajectories[firstTrajectory].GetClosestPointT(point, firstT, secondT, precision), firstTrajectory);
            }

            var minT = trajectories[firstTrajectory].GetClosestPointT(point, firstT, 1f, precision);
            var minDistance = (trajectories[firstTrajectory].GetPosition(minT) - point).sqrMagnitude;
            var minNumber = firstTrajectory;
            
            firstTrajectory++;
            float t;
            float distance;
            while (firstTrajectory < secondTrajectory)
            {
                t = trajectories[firstTrajectory].GetClosestPointT(point, 0f, 1f, precision);
                distance = (trajectories[firstTrajectory].GetPosition(t) - point).sqrMagnitude;
                
                if (distance <= minDistance)
                {
                    minT = t;
                    minDistance = distance;
                    minNumber= firstTrajectory;
                }
                
                firstTrajectory++;
            }
            
            t = trajectories[secondTrajectory].GetClosestPointT(point, 0f, secondT, precision);
            distance = (trajectories[secondTrajectory].GetPosition(t) - point).sqrMagnitude;

            return distance <= minDistance ? GetT(t, secondTrajectory) : GetT(minT, minNumber);
        }

        private void GetT(float t, out int number, out float newT)
        {
            t *= trajectories.Count;
            number = Mathf.FloorToInt(t);
            if (number < 0) number = 0;
            else if (number >= trajectories.Count) number = trajectories.Count - 1;
            newT = t - number;
        }

        private float GetT(float t, int number)
        {
            return (t + number) / trajectories.Count;
        }
    }
}