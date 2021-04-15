using System;
using UnityEngine;

namespace Scripts.Track.Trajectory
{
    [Serializable]
    public class BezierCurve : ITrajectory
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public Vector3 firstControlPoint;
        public Vector3 secondControlPoint;

        public BezierCurve(Vector3 startPoint, Vector3 endPoint, Vector3 firstControlPoint, Vector3 secondControlPoint)
        {
            this.startPoint = startPoint;
            this.endPoint = endPoint;
            this.firstControlPoint = firstControlPoint;
            this.secondControlPoint = secondControlPoint;
        }

        public Vector3 GetPosition(float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;

            return
                oneMinusT * oneMinusT * oneMinusT * startPoint +
                3f * t * oneMinusT * oneMinusT * firstControlPoint +
                3f * t * t * oneMinusT * secondControlPoint +
                t * t * t * endPoint;
        }

        public Vector3 GetTangent(float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;

            return (3f * oneMinusT * oneMinusT * (firstControlPoint - startPoint) +
                   6f * oneMinusT * t * (secondControlPoint - firstControlPoint) +
                   3f * t * t * (endPoint - secondControlPoint)).normalized;
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

    }
}
