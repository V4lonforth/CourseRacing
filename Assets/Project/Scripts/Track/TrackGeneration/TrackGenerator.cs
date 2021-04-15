using Scripts.Track.Trajectory;
using UnityEngine;

namespace Scripts.Track.TrackGeneration
{
    public class TrackGenerator : MonoBehaviour
    {
        public BezierSplineBuilder splineBuilder;

        [SerializeField] private Track track;
        [SerializeField] private ExtrudeShape extrudeShape;

        [SerializeField] private int defaultSegmentSize = 150;

        public void DestroyTrack()
        {
            track.GetComponent<MeshFilter>().mesh = null;
        }

        public void GenerateTrack()
        {
            DestroyTrack();

            var trajectory = splineBuilder.GetTrajectory();
            track.Trajectory = trajectory;
            track.GetComponent<MeshFilter>().mesh = Extrude(trajectory);
        }

        private Mesh Extrude(ITrajectory trajectory)
        {
            var segmentsCount = defaultSegmentSize;
            var path = new OrientedPoint[segmentsCount + 1];

            for (var i = 0; i <= segmentsCount; i++)
            {
                var t = (float) i / segmentsCount;
                path[i] = new OrientedPoint(trajectory.GetPosition(t), trajectory.GetOrientation(t));
            }

            return Extrude(path, new TrajectoryLengthCalculator(trajectory, 1024));
        }

        private Mesh Extrude(OrientedPoint[] path, TrajectoryLengthCalculator lengthCalculator)
        {
            var verticesInShape = extrudeShape.vertices.Length;
            var segments = path.Length - 1;
            var edgeLoops = path.Length;
            var verticesCount = verticesInShape * edgeLoops;
            var trianglesCount = extrudeShape.lines.Length * segments;
            var triangleIndexCount = trianglesCount * 3;

            var triangleIndices = new int[triangleIndexCount];
            var vertices = new Vector3[verticesCount];
            var normals = new Vector3[verticesCount];
            var uvs = new Vector2[verticesCount];

            for (var i = 0; i < edgeLoops; i++)
            {
                var offset = i * verticesInShape;
                for (var j = 0; j < verticesInShape; j++)
                {
                    var index = offset + j;
                    vertices[index] = path[i].LocalToWorld(extrudeShape.vertices[j].position);
                    normals[index] = path[i].LocalToWorldDirection(extrudeShape.vertices[j].normal);
                    uvs[index] = new Vector2(extrudeShape.vertices[j].uCoord, lengthCalculator.GetDistance(i / (float) edgeLoops));
                }
            }

            var indexNumber = 0;
            for (var i = 0; i < segments; i++)
            {
                var offset = i * verticesInShape;
                for (var l = 0; l < extrudeShape.lines.Length; l += 2)
                {
                    var a = offset + extrudeShape.lines[l] + verticesInShape;
                    var b = offset + extrudeShape.lines[l];
                    var c = offset + extrudeShape.lines[l + 1];
                    var d = offset + extrudeShape.lines[l + 1] + verticesInShape;

                    triangleIndices[indexNumber++] = a;
                    triangleIndices[indexNumber++] = c;
                    triangleIndices[indexNumber++] = b;
                    triangleIndices[indexNumber++] = c;
                    triangleIndices[indexNumber++] = a;
                    triangleIndices[indexNumber++] = d;
                }
            }

            var mesh = new Mesh();
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangleIndices;
            mesh.normals = normals;
            mesh.uv = uvs;
            return mesh;
        }
    }
}