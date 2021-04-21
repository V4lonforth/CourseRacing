using System;
using UnityEngine;

namespace Scripts.Track.Trajectory
{
    [Serializable]
    public class BezierCurve : ITrajectory
    {
        public Vector3 StartPoint
        {
            get => startPoint;
            set
            {
                startPoint = value;
                UpdateTable();
            }
        }
        public Vector3 EndPoint
        {
            get => endPoint;
            set
            {
                endPoint = value;
                UpdateTable();
            }
        }
        public Vector3 FirstControlPoint
        {
            get => firstControlPoint;
            set
            {
                firstControlPoint = value;
                UpdateTable();
            }
        }
        public Vector3 SecondControlPoint
        {
            get => secondControlPoint;
            set
            {
                secondControlPoint = value;
                UpdateTable();
            }
        }

        public float Length => GetDistance(1f);

        [SerializeField] private Vector3 startPoint;
        [SerializeField] private Vector3 endPoint;
        [SerializeField] private Vector3 firstControlPoint;
        [SerializeField] private Vector3 secondControlPoint;

        private TrajectoryLengthCalculator _lengthCalculator;

        private const int SampleSize = 64;
        
        public BezierCurve(Vector3 startPoint, Vector3 endPoint, Vector3 firstControlPoint, Vector3 secondControlPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.firstControlPoint = firstControlPoint;
            this.secondControlPoint = secondControlPoint;

            _lengthCalculator = new TrajectoryLengthCalculator(this, SampleSize);
        }

        private void UpdateTable()
        {
            _lengthCalculator = new TrajectoryLengthCalculator(this, SampleSize); 
        }

        public float GetDistance(float t)
        {
            if (_lengthCalculator == null || _lengthCalculator.Empty) _lengthCalculator = new TrajectoryLengthCalculator(this, SampleSize);
            return _lengthCalculator.GetDistance(t);
        }

        public float GetT(float distance)
        {
            if (_lengthCalculator == null || _lengthCalculator.Empty) _lengthCalculator = new TrajectoryLengthCalculator(this, SampleSize);
            return _lengthCalculator.GetT(distance);
        }

        public Vector3 GetPosition(float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;

            return
                oneMinusT * oneMinusT * oneMinusT * StartPoint +
                3f * t * oneMinusT * oneMinusT * FirstControlPoint +
                3f * t * t * oneMinusT * SecondControlPoint +
                t * t * t * EndPoint;
        }

        public Vector3 GetTangent(float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;

            return (3f * oneMinusT * oneMinusT * (FirstControlPoint - StartPoint) +
                    6f * oneMinusT * t * (SecondControlPoint - FirstControlPoint) +
                    3f * t * t * (EndPoint - SecondControlPoint)).normalized;
        }

        public Vector3 GetNormal(float t) => GetNormal(t, Vector3.up);

        public Vector3 GetNormal(float t, Vector3 up)
        {
            var tangent = GetTangent(t);
            var binormal = Vector3.Cross(up, tangent).normalized;
            return Vector3.Cross(tangent, binormal);
        }

        public Quaternion GetOrientation(float t) => GetOrientation(t, Vector3.up);

        public Quaternion GetOrientation(float t, Vector3 up)
        {
            return Quaternion.LookRotation(GetTangent(t), GetNormal(t, up));
        }

        public float GetClosestPointT(Vector3 point, float sectionStart, float sectionEnd, float precision = 0.01f)
        {
            var firstDistance = (point - GetPosition(sectionStart)).sqrMagnitude;
            var secondDistance = (point - GetPosition(sectionEnd)).sqrMagnitude;

            while (sectionEnd - sectionStart > precision)
            {
                if (secondDistance <= firstDistance)
                {
                    sectionStart = (sectionStart * 2f + sectionEnd) / 3f;
                    firstDistance = (point - GetPosition(sectionStart)).sqrMagnitude;
                }
                else
                {
                    sectionEnd = (sectionStart + sectionEnd * 2f) / 3f;
                    secondDistance = (point - GetPosition(sectionEnd)).sqrMagnitude;;
                }
            }

            return (sectionEnd + sectionStart) / 2f;
        }
    }
}