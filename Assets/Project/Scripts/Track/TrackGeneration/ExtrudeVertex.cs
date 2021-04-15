using System;
using UnityEngine;

namespace Scripts.Track.TrackGeneration
{
    [Serializable]
    public class ExtrudeVertex
    {
        public Vector2 position;
        public Vector2 normal;
        public float uCoord;

        public ExtrudeVertex(Vector2 position, Vector2 normal, float uCoord)
        {
            this.position = position;
            this.normal = normal;
            this.uCoord = uCoord;
        }
    }
}