using System.Linq;
using Scripts.Track;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor
{
    [CustomEditor(typeof(BezierSplineBuilder))]
    public class BezierSplineInspector : UnityEditor.Editor
    {
        private BezierSplineBuilder _bezierSplineBuilder;

        private Transform _trackTransform;
        private Quaternion _trackRotation;

        private BezierCurve _selectedCurve;

        private const float LineWidth = 3f;

        private const float HandleSize = 0.10f;
        private const float PickSize = 0.14f;

        public override void OnInspectorGUI()
        {
            if (!CheckTarget()) return;

            if (_selectedCurve != null)
                DrawCurveInspector(_selectedCurve);
            
            if (GUILayout.Button("Add Curve First")) {
                Undo.RecordObject(_bezierSplineBuilder, "Add Curve");
                _bezierSplineBuilder.AddCurve(false);
                _selectedCurve = _bezierSplineBuilder.curves.First();
                EditorUtility.SetDirty(_bezierSplineBuilder);
            }
            
            if (GUILayout.Button("Add Curve Last")) {
                Undo.RecordObject(_bezierSplineBuilder, "Add Curve");
                _bezierSplineBuilder.AddCurve(true);
                _selectedCurve = _bezierSplineBuilder.curves.Last();
                EditorUtility.SetDirty(_bezierSplineBuilder);
            }
            
            if (_bezierSplineBuilder.curves.Count > 0 && GUILayout.Button("Remove Curve")) {
                Undo.RecordObject(_bezierSplineBuilder, "Remove Curve");
                _bezierSplineBuilder.RemoveCurve(_selectedCurve);
                _selectedCurve = _bezierSplineBuilder.curves.Count == 0 ? null : _bezierSplineBuilder.curves[0];
                EditorUtility.SetDirty(_bezierSplineBuilder);
            }
            
            if (_selectedCurve != null && GUILayout.Button("Subdivide Curve")) {
                Undo.RecordObject(_bezierSplineBuilder, "Subdivide Curve");
                _selectedCurve = _bezierSplineBuilder.Subdivide(_selectedCurve);
                EditorUtility.SetDirty(_bezierSplineBuilder);
            }
        }

        private void OnSceneGUI()
        {
            if (!CheckTarget()) return;

            foreach (var curve in _bezierSplineBuilder.curves)
            {
                if (curve == _selectedCurve)
                {
                    DrawCurve(curve, Color.red);
                    DrawCurvePoints(curve);
                }
                else
                {
                    DrawCurve(curve, Color.white);
                    DrawCurveSelector(curve);
                }
            }
        }

        private bool CheckTarget()
        {
            _bezierSplineBuilder = target as BezierSplineBuilder;
            if (_bezierSplineBuilder == null) return false;

            _trackTransform = _bezierSplineBuilder.transform;
            _trackRotation = Tools.pivotRotation == PivotRotation.Local
                ? _trackTransform.rotation
                : Quaternion.identity;
            
            if (_selectedCurve == null && _bezierSplineBuilder.curves.Count > 0)
                _selectedCurve = _bezierSplineBuilder[0];
            
            return true;
        }

        private void DrawCurvePoints(BezierCurve bezierCurve)
        {
            var transformedCurve = new BezierCurve
            {
                startPoint = _trackTransform.TransformPoint(bezierCurve.startPoint),
                endPoint = _trackTransform.TransformPoint(bezierCurve.endPoint),
                firstControlPoint = _trackTransform.TransformPoint(bezierCurve.firstControlPoint),
                secondControlPoint = _trackTransform.TransformPoint(bezierCurve.secondControlPoint)
            };
            
            Handles.DrawBezier(transformedCurve.startPoint, transformedCurve.firstControlPoint,
                transformedCurve.startPoint, transformedCurve.firstControlPoint, Color.gray, null, LineWidth);
            Handles.DrawBezier(transformedCurve.endPoint, transformedCurve.secondControlPoint,
                transformedCurve.endPoint, transformedCurve.secondControlPoint, Color.gray, null, LineWidth);
            
            _bezierSplineBuilder.SetStartPoint(bezierCurve, DrawPoint(bezierCurve.startPoint));
            _bezierSplineBuilder.SetEndPoint(bezierCurve, DrawPoint(bezierCurve.endPoint));
            _bezierSplineBuilder.SetFirstControlPoint(bezierCurve, DrawPoint(bezierCurve.firstControlPoint));
            _bezierSplineBuilder.SetSecondControlPoint(bezierCurve, DrawPoint(bezierCurve.secondControlPoint));
        }

        private void DrawCurve(BezierCurve bezierCurve, Color color)
        {
            var transformedCurve = new BezierCurve
            {
                startPoint = _trackTransform.TransformPoint(bezierCurve.startPoint),
                endPoint = _trackTransform.TransformPoint(bezierCurve.endPoint),
                firstControlPoint = _trackTransform.TransformPoint(bezierCurve.firstControlPoint),
                secondControlPoint = _trackTransform.TransformPoint(bezierCurve.secondControlPoint)
            };

            Handles.DrawBezier(transformedCurve.startPoint, transformedCurve.endPoint,
                transformedCurve.firstControlPoint, transformedCurve.secondControlPoint,
                color, null, LineWidth);
        }

        private void DrawCurveSelector(BezierCurve bezierCurve)
        {
            var point = _trackTransform.TransformPoint(bezierCurve.GetPosition(0.5f));
            Handles.color = Color.white;

            var size = HandleUtility.GetHandleSize(point);
            if (!Handles.Button(point, _trackRotation, HandleSize * size, PickSize * size,
                Handles.DotHandleCap)) return;

            _selectedCurve = bezierCurve;
            Repaint();
        }

        private Vector3 DrawPoint(Vector3 point)
        {
            point = _trackTransform.TransformPoint(point);
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, _trackRotation);
            point = _trackTransform.InverseTransformPoint(point);

            if (!EditorGUI.EndChangeCheck()) return point;

            Undo.RecordObject(_bezierSplineBuilder, "Move Point");
            EditorUtility.SetDirty(_bezierSplineBuilder);

            return point;
        }

        private void DrawCurveInspector(BezierCurve bezierCurve)
        {
            GUILayout.Label("Selected Curve");

            _bezierSplineBuilder.SetStartPoint(bezierCurve, DrawPointInspector(bezierCurve.startPoint, "First Point"));
            _bezierSplineBuilder.SetEndPoint(bezierCurve, DrawPointInspector(bezierCurve.endPoint, "Second Point"));
            _bezierSplineBuilder.SetFirstControlPoint(bezierCurve,
                DrawPointInspector(bezierCurve.firstControlPoint, "First Control Point"));
            _bezierSplineBuilder.SetSecondControlPoint(bezierCurve,
                DrawPointInspector(bezierCurve.secondControlPoint, "Second Control Point"));
        }

        private Vector3 DrawPointInspector(Vector3 point, string label)
        {
            EditorGUI.BeginChangeCheck();
            point = EditorGUILayout.Vector3Field(label, point);

            if (!EditorGUI.EndChangeCheck()) return point;

            Undo.RecordObject(_bezierSplineBuilder, "Move Point");
            EditorUtility.SetDirty(_bezierSplineBuilder);

            return point;
        }
    }
}