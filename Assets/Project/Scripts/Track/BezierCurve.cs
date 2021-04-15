using System;
using UnityEngine;

namespace Scripts.Track
{
    [Serializable]
    public class BezierCurve
    {
        public Vector3 startPoint;
        public Vector3 endPoint;
        public Vector3 firstControlPoint;
        public Vector3 secondControlPoint;

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

        public Vector3 GetFirstDerivative(float t)
        {
            t = Mathf.Clamp01(t);
            var oneMinusT = 1f - t;

            return 3f * oneMinusT * oneMinusT * (firstControlPoint - startPoint) +
                   6f * oneMinusT * t * (secondControlPoint - firstControlPoint) +
                   3f * t * t * (endPoint - secondControlPoint);
        }
    }
}
