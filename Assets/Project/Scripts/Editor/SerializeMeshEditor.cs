using Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor
{
    [CustomEditor(typeof(SerializedMesh))]
    class SerializeMeshEditor : UnityEditor.Editor
    {
        private SerializedMesh _obj;

        private void OnSceneGUI()
        {
            _obj = (SerializedMesh)target;
        }
 
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
 
            if (GUILayout.Button("Rebuild"))
            {
                if (_obj)
                {
                    _obj.gameObject.GetComponent<MeshFilter>().mesh = _obj.Rebuild();
                }
            }
 
            if (GUILayout.Button("Serialize"))
            {
                if (_obj)
                {
                    _obj.Serialize();
                }
            }
        }
    }
}