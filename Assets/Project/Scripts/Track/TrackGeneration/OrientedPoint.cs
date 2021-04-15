using UnityEngine;

namespace Scripts.Track.TrackGeneration
{
    public readonly struct OrientedPoint
    {
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public OrientedPoint(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Vector3 LocalToWorld(Vector3 point) => Position + Rotation * point;
        public Vector3 WorldToLocal(Vector3 point) => Quaternion.Inverse(Rotation) * (point - Position);
        public Vector3 LocalToWorldDirection(Vector3 direction) => Rotation * direction;
    }
}