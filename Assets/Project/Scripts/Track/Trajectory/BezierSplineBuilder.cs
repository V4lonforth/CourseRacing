using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Track.Trajectory
{
    public class BezierSplineBuilder : MonoBehaviour
    {
        public Action OnChanged { get; set; }

        public List<BezierCurve> curves;

        private void OnValidate()
        {
            OnChanged?.Invoke();
        }

        public BezierSpline GetTrajectory()
        {
            return curves.Count > 0 ? new BezierSpline(new List<BezierCurve>(curves)) : null;
        }

        public BezierCurve AddCurve(bool isLast)
        {
            BezierCurve curve;
            var index = isLast ? curves.Count : 0;

            if (curves.Count == 0)
            {
                curve = new BezierCurve(startPoint: Vector3.forward * 1f, firstControlPoint: Vector3.forward * 2f,
                    secondControlPoint: Vector3.forward * 3f, endPoint: Vector3.forward * 4f);
            }
            else
            {
                if (isLast)
                {
                    var lastCurve = curves.Last();

                    curve = new BezierCurve(startPoint: lastCurve.EndPoint,
                        firstControlPoint: MirrorControlPoint(lastCurve.EndPoint, lastCurve.SecondControlPoint, 1f),
                        secondControlPoint: MirrorControlPoint(lastCurve.EndPoint, lastCurve.SecondControlPoint, 2f),
                        endPoint: MirrorControlPoint(lastCurve.EndPoint, lastCurve.SecondControlPoint, 3f));
                }
                else
                {
                    var firstCurve = curves.First();

                    curve = new BezierCurve(endPoint: firstCurve.StartPoint,
                        secondControlPoint: MirrorControlPoint(firstCurve.StartPoint, firstCurve.FirstControlPoint, 1f),
                        firstControlPoint: MirrorControlPoint(firstCurve.StartPoint, firstCurve.FirstControlPoint, 2f),
                        startPoint: MirrorControlPoint(firstCurve.StartPoint, firstCurve.FirstControlPoint, 3f));
                }
            }

            curves.Insert(index, curve);
            OnChanged?.Invoke();
            return curve;
        }

        public bool RemoveCurve(BezierCurve curve)
        {
            var previousCurve = FindPrevious(curve);
            var nextCurve = FindNext(curve);

            if (previousCurve == null || nextCurve == null)
            {
                if (!curves.Remove(curve)) return false;
                OnChanged?.Invoke();
                return true;
            }

            var curveMiddle = curve.GetPosition(0.5f);
            var tangent = curve.GetTangent(0.5f);

            previousCurve.EndPoint = curveMiddle;
            previousCurve.SecondControlPoint = curveMiddle - tangent;

            nextCurve.StartPoint = curveMiddle;
            nextCurve.FirstControlPoint = curveMiddle + tangent;
            
            if (!curves.Remove(curve)) return false;
            OnChanged?.Invoke();
            return true;
        }

        public BezierCurve Subdivide(BezierCurve curve)
        {
            var middlePoint = curve.GetPosition(0.5f);
            var tangent = curve.GetTangent(0.5f);

            var newCurve = new BezierCurve(startPoint: middlePoint, endPoint: curve.EndPoint,
                firstControlPoint: middlePoint + tangent,
                secondControlPoint: (curve.SecondControlPoint - curve.EndPoint) / 2f + curve.EndPoint);

            curves.Insert(curves.IndexOf(curve) + 1, newCurve);

            curve.FirstControlPoint = (curve.FirstControlPoint - curve.StartPoint) / 2f + curve.StartPoint;
            curve.EndPoint = middlePoint;
            curve.SecondControlPoint = middlePoint - tangent;

            OnChanged?.Invoke();
            return newCurve;
        }

        public void SetStartPoint(BezierCurve curve, Vector3 point)
        {
            curve.StartPoint = point;

            var previousCurve = FindPrevious(curve);
            if (previousCurve != null)
                previousCurve.EndPoint = point;

            SetFirstControlPoint(curve, curve.FirstControlPoint);
            
            OnChanged?.Invoke();
        }

        public void SetEndPoint(BezierCurve curve, Vector3 point)
        {
            curve.EndPoint = point;

            var nextCurve = FindNext(curve);
            if (nextCurve != null)
                nextCurve.StartPoint = point;
            
            SetSecondControlPoint(curve, curve.SecondControlPoint);
            
            OnChanged?.Invoke();
        }

        public void SetFirstControlPoint(BezierCurve curve, Vector3 point)
        {
            curve.FirstControlPoint = point;

            var previousCurve = FindPrevious(curve);
            if (previousCurve != null)
                previousCurve.SecondControlPoint = MirrorControlPoint(curve.StartPoint, curve.FirstControlPoint,
                    (previousCurve.EndPoint - previousCurve.SecondControlPoint).magnitude);

            OnChanged?.Invoke();
        }

        public void SetSecondControlPoint(BezierCurve curve, Vector3 point)
        {
            curve.SecondControlPoint = point;

            var nextCurve = FindNext(curve);
            if (nextCurve != null)
                nextCurve.FirstControlPoint = MirrorControlPoint(curve.EndPoint, curve.SecondControlPoint,
                    (nextCurve.StartPoint - nextCurve.FirstControlPoint).magnitude);

            OnChanged?.Invoke();
        }

        private Vector3 MirrorControlPoint(Vector3 curvePoint, Vector3 controlPoint, float length = 0f)
        {
            if (length == 0f)
                return curvePoint + curvePoint - controlPoint;
            return curvePoint + (curvePoint - controlPoint).normalized * length;
        }

        private BezierCurve FindPrevious(BezierCurve curve)
        {
            var index = curves.IndexOf(curve);
            return index <= 0 ? null : curves[index - 1];
        }

        private BezierCurve FindNext(BezierCurve curve)
        {
            var index = curves.IndexOf(curve);
            return index == -1 || index == curves.Count - 1 ? null : curves[index + 1];
        }
    }
}