using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Track
{
    public class BezierSplineBuilder : MonoBehaviour
    {
        public BezierCurve this[int i]
        {
            get => curves[i];
            set => curves[i] = value;
        }

        public List<BezierCurve> curves;

        public BezierCurve AddCurve(bool isLast)
        {
            BezierCurve curve;
            var index = isLast ? curves.Count : 0;
            
            if (curves.Count == 0)
            {
                curve = new BezierCurve
                {
                    startPoint = Vector3.forward * 1f,
                    firstControlPoint = Vector3.forward * 2f,
                    secondControlPoint = Vector3.forward * 3f,
                    endPoint = Vector3.forward * 4f
                };
            }
            else
            {
                if (isLast)
                {
                    var lastCurve = curves.Last();

                    curve = new BezierCurve
                    {
                        startPoint = lastCurve.endPoint,
                        firstControlPoint = MirrorControlPoint(lastCurve.endPoint, lastCurve.secondControlPoint, 1f),
                        secondControlPoint = MirrorControlPoint(lastCurve.endPoint, lastCurve.secondControlPoint, 2f),
                        endPoint = MirrorControlPoint(lastCurve.endPoint, lastCurve.secondControlPoint, 3f)
                    };
                }
                else
                {
                    var firstCurve = curves.First();

                    curve = new BezierCurve
                    {
                        endPoint = firstCurve.startPoint,
                        secondControlPoint = MirrorControlPoint(firstCurve.startPoint, firstCurve.firstControlPoint, 1f),
                        firstControlPoint = MirrorControlPoint(firstCurve.startPoint, firstCurve.firstControlPoint, 2f),
                        startPoint = MirrorControlPoint(firstCurve.startPoint, firstCurve.firstControlPoint, 3f)
                    };
                }
            }
                
            curves.Insert(index, curve);
            return curve;
        }
        
        public bool RemoveCurve(BezierCurve curve)
        {
            var previousCurve = FindPrevious(curve);
            var nextCurve = FindNext(curve);

            if (previousCurve == null || nextCurve == null)
            {
                return curves.Remove(curve);
            }
            
            var curveMiddle = curve.GetPosition(0.5f);
            var derivative = curve.GetFirstDerivative(0.5f);
            
            previousCurve.endPoint = curveMiddle;
            previousCurve.secondControlPoint = curveMiddle - derivative;

            nextCurve.startPoint = curveMiddle;
            nextCurve.firstControlPoint = curveMiddle + derivative;

            return curves.Remove(curve);
        }

        public BezierCurve Subdivide(BezierCurve curve)
        {
            var middlePoint = curve.GetPosition(0.5f);
            var derivative = curve.GetFirstDerivative(0.5f).normalized;

            var newCurve = new BezierCurve()
            {
                startPoint = middlePoint,
                endPoint = curve.endPoint,
                firstControlPoint = middlePoint + derivative,
                secondControlPoint = (curve.secondControlPoint - curve.endPoint) / 2f + curve.endPoint
            };
            
            curves.Insert(curves.IndexOf(curve) + 1, newCurve);

            curve.firstControlPoint = (curve.firstControlPoint - curve.startPoint) / 2f + curve.startPoint;
            curve.endPoint = middlePoint;
            curve.secondControlPoint = middlePoint - derivative;

            return newCurve;
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
        
        public void SetStartPoint(BezierCurve curve, Vector3 point)
        {
            curve.startPoint = point;

            var previousCurve = FindPrevious(curve);
            if (previousCurve != null)
                previousCurve.endPoint = point;
        }

        public void SetEndPoint(BezierCurve curve, Vector3 point)
        {
            curve.endPoint = point;

            var nextCurve = FindNext(curve);
            if (nextCurve != null)
                nextCurve.startPoint = point;
        }

        public void SetFirstControlPoint(BezierCurve curve, Vector3 point)
        {
            curve.firstControlPoint = point;

            var previousCurve = FindPrevious(curve);
            if (previousCurve != null)
                previousCurve.secondControlPoint = MirrorControlPoint(curve.startPoint, curve.firstControlPoint,
                    (previousCurve.endPoint - previousCurve.secondControlPoint).magnitude); 
        }

        public void SetSecondControlPoint(BezierCurve curve, Vector3 point)
        {
            curve.secondControlPoint = point;

            var nextCurve = FindNext(curve);
            if (nextCurve != null)
                nextCurve.firstControlPoint = MirrorControlPoint(curve.endPoint, curve.secondControlPoint,
                    (nextCurve.startPoint - nextCurve.firstControlPoint).magnitude); 
        }
    }
}
