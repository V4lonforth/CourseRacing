using Scripts.Track.TrackGeneration;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor
{
    [CustomEditor(typeof(TrackGenerator))]
    public class TrackGeneratorInspector : UnityEditor.Editor
    {
        private void OnEnable() { ((TrackGenerator) target).splineBuilder.OnChanged += BuildTrack; }
        private void OnDisable() { ((TrackGenerator) target).splineBuilder.OnChanged -= BuildTrack; }

        private void BuildTrack()
        {
            if (target is TrackGenerator trackGenerator)
                trackGenerator.GenerateTrack();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var trackGenerator = target as TrackGenerator;
            if (trackGenerator == null) return;
            
            if (GUILayout.Button("Generate Track")) {
                trackGenerator.GenerateTrack();
            }
            
            if (GUILayout.Button("Destroy Track")) {
                trackGenerator.DestroyTrack();
            }
        }
        
        
    }
}