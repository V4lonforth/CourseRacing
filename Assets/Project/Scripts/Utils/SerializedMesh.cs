using UnityEngine;

namespace Scripts.Utils
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshFilter))]
    public class SerializedMesh : MonoBehaviour
    {
        [HideInInspector] [SerializeField] private Vector2[] uv;
        [HideInInspector] [SerializeField] private Vector3[] vertices;
        [HideInInspector] [SerializeField] private Vector3[] normals;
        [HideInInspector] [SerializeField] private int[] triangles;
        [HideInInspector] [SerializeField] private bool serialized;

        private void Awake()
        {
            if (serialized)
            {
                GetComponent<MeshFilter>().mesh = Rebuild();
            }
        }

        private void Start()
        {
            if (serialized) return;
 
            Serialize();
        }
 
        public void Serialize()
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
 
            uv = mesh.uv;
            vertices = mesh.vertices;
            triangles = mesh.triangles;
            normals = mesh.normals;
 
            serialized = true;
        }
 
        public Mesh Rebuild()
        {
            var mesh = new Mesh {vertices = vertices, triangles = triangles, uv = uv, normals = normals};

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
 
            return mesh;
        }
    }
}