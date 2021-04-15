using System;
using System.Linq;
using Scripts.Track;
using Scripts.Track.Trajectory;
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

            if (GUILayout.Button("Add Curve First"))
            {
                Undo.RecordObject(_bezierSplineBuilder, "Add Curve");
                _selectedCurve = _bezierSplineBuilder.AddCurve(false);
                EditorUtility.SetDirty(_bezierSplineBuilder);
            }

            if (GUILayout.Button("Add Curve Last"))
            {
                Undo.RecordObject(_bezierSplineBuilder, "Add Curve");
                _selectedCurve = _bezierSplineBuilder.AddCurve(true);
                EditorUtility.SetDirty(_bezierSplineBuilder);
            }

            if (_bezierSplineBuilder.curves.Count > 0 && GUILayout.Button("Remove Curve"))
            {
                Undo.RecordObject(_bezierSplineBuilder, "Remove Curve");
                _bezierSplineBuilder.RemoveCurve(_selectedCurve);
                _selectedCurve = _bezierSplineBuilder.curves.Count == 0 ? null : _bezierSplineBuilder.curves[0];
                EditorUtility.SetDirty(_bezierSplineBuilder);
            }

            if (_selectedCurve != null && GUILayout.Button("Subdivide Curve"))
            {
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
                _selectedCurve = _bezierSplineBuilder.curves.First();

            return true;
        }

        private void DrawCurvePoints(BezierCurve bezierCurve)
        {
            var transformedCurve = new BezierCurve(startPoint: _trackTransform.TransformPoint(bezierCurve.startPoint),
                endPoint: _trackTransform.TransformPoint(bezierCurve.endPoint),
                firstControlPoint: _trackTransform.TransformPoint(bezierCurve.firstControlPoint),
                secondControlPoint: _trackTransform.TransformPoint(bezierCurve.secondControlPoint));

            Handles.DrawBezier(transformedCurve.startPoint, transformedCurve.firstControlPoint,
                transformedCurve.startPoint, transformedCurve.firstControlPoint, Color.gray, null, LineWidth);
            Handles.DrawBezier(transformedCurve.endPoint, transformedCurve.secondControlPoint,
                transformedCurve.endPoint, transformedCurve.secondControlPoint, Color.gray, null, LineWidth);

            if (DrawPoint(bezierCurve.startPoint, out var newPoint))
                _bezierSplineBuilder.SetStartPoint(bezierCurve, newPoint);

            if (DrawPoint(bezierCurve.endPoint, out newPoint))
                _bezierSplineBuilder.SetEndPoint(bezierCurve, newPoint);

            if (DrawPoint(bezierCurve.firstControlPoint, out newPoint))
                _bezierSplineBuilder.SetFirstControlPoint(bezierCurve, newPoint);

            if (DrawPoint(bezierCurve.secondControlPoint, out newPoint))
                _bezierSplineBuilder.SetSecondControlPoint(bezierCurve, newPoint);
        }

        private void DrawCurve(BezierCurve bezierCurve, Color color)
        {
            var transformedCurve = new BezierCurve(startPoint: _trackTransform.TransformPoint(bezierCurve.startPoint),
                endPoint: _trackTransform.TransformPoint(bezierCurve.endPoint),
                firstControlPoint: _trackTransform.TransformPoint(bezierCurve.firstControlPoint),
                secondControlPoint: _trackTransform.TransformPoint(bezierCurve.secondControlPoint));

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

        private bool DrawPoint(Vector3 point, out Vector3 result)
        {
            point = _trackTransform.TransformPoint(point);
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, _trackRotation);
            point = _trackTransform.InverseTransformPoint(point);

            result = point;

            if (!EditorGUI.EndChangeCheck()) return false;

            Undo.RecordObject(_bezierSplineBuilder, "Move Point");
            EditorUtility.SetDirty(_bezierSplineBuilder);

            return true;
        }

        private void DrawCurveInspector(BezierCurve bezierCurve)
        {
            GUILayout.Label("Selected Curve");

            if (DrawPointInspector(bezierCurve.startPoint, "First Point", out var newPoint))
                _bezierSplineBuilder.SetStartPoint(bezierCurve, newPoint);

            if (DrawPointInspector(bezierCurve.endPoint, "Second Point", out newPoint))
                _bezierSplineBuilder.SetEndPoint(bezierCurve, newPoint);

            if (DrawPointInspector(bezierCurve.firstControlPoint, "First Control Point", out newPoint))
                _bezierSplineBuilder.SetFirstControlPoint(bezierCurve, newPoint);

            if (DrawPointInspector(bezierCurve.secondControlPoint, "Second Control Point", out newPoint))
                _bezierSplineBuilder.SetSecondControlPoint(bezierCurve, newPoint);
        }

        private bool DrawPointInspector(Vector3 point, string label, out Vector3 result)
        {
            EditorGUI.BeginChangeCheck();
            point = EditorGUILayout.Vector3Field(label, point);
            result = point;

            if (!EditorGUI.EndChangeCheck()) return false;

            Undo.RecordObject(_bezierSplineBuilder, "Move Point");
            EditorUtility.SetDirty(_bezierSplineBuilder);

            return true;
        }
    }
}