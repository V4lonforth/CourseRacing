using Scripts.Track.ControlPoints;
using Scripts.Track.Trajectory;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor
{
    [CustomEditor(typeof(ControlPointsBuilder))]
    public class ControlPointsBuilderInspector : UnityEditor.Editor
    {
        private ControlPointsBuilder _controlPointsBuilder;
        private Transform _trackTransform;
        private Quaternion _trackRotation;

        private ITrajectory _trajectory;

        private ITrajectory Trajectory => _trajectory ?? _controlPointsBuilder.splineBuilder.GetTrajectory();

        private void OnEnable() { ((ControlPointsBuilder) target).splineBuilder.OnChanged += UpdateTrajectory; }
        private void OnDisable() { ((ControlPointsBuilder) target).splineBuilder.OnChanged -= UpdateTrajectory; }

        private void UpdateTrajectory(BezierSpline bezierSpline)
        {
            _trajectory = bezierSpline;
            UpdateControlPoints();
        }

        private void UpdateControlPoints()
        {
            _controlPointsBuilder.GenerateControlPoints();
        }
        
        private void OnSceneGUI()
        {
            _controlPointsBuilder = target as ControlPointsBuilder;
            if (_controlPointsBuilder == null) return;
            _trackTransform = _controlPointsBuilder.transform;
            _trackRotation = _trackTransform.rotation;
            
            foreach (var controlPoint in _controlPointsBuilder.controlPointsData)
            {
                DrawPoint(controlPoint);
            }
        }

        private void DrawPoint(ControlPointData controlPointData)
        {
            var point = _trackTransform.TransformPoint(Trajectory.GetPosition(controlPointData.trajectoryPosition));
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, _trackRotation);
            point = _trackTransform.InverseTransformPoint(point);
            
            controlPointData.trajectoryPosition = Trajectory.GetClosestPointT(point, 0f, 1f, 0.001f);
            UpdateControlPoints();

            if (!EditorGUI.EndChangeCheck()) return;

            Undo.RecordObject(_controlPointsBuilder, "Move Control Point");
            EditorUtility.SetDirty(_controlPointsBuilder);
        }
    }
}