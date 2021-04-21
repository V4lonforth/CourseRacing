using Scripts.Track.TrackGeneration;
using Scripts.Track.Trajectory;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor
{
    [CustomEditor(typeof(TrackGenerator))]
    public class TrackGeneratorInspector : UnityEditor.Editor
    {
        private void OnEnable() { ((TrackGenerator) target).splineBuilder.OnChanged += UpdateTrajectory; }
        private void OnDisable() { ((TrackGenerator) target).splineBuilder.OnChanged -= UpdateTrajectory; }

        private BezierSpline _lastBezierSpline;
        
        private void UpdateTrajectory(BezierSpline bezierSpline)
        {
            _lastBezierSpline = bezierSpline;
            BuildTrack();
        }
        
        private void BuildTrack()
        {
            if (target is TrackGenerator trackGenerator)
                trackGenerator.GenerateTrack(_lastBezierSpline);
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var trackGenerator = target as TrackGenerator;
            if (trackGenerator == null) return;
            
            if (GUILayout.Button("Generate Track")) {
                BuildTrack();
            }
            
            if (GUILayout.Button("Destroy Track")) {
                trackGenerator.DestroyTrack();
            }
        }
        
        
    }
}