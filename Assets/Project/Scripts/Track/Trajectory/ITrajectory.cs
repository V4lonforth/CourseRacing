using UnityEngine;

namespace Scripts.Track.Trajectory
{
    public interface ITrajectory
    {
        Vector3 GetPosition(float t);
        Vector3 GetTangent(float t);
        Vector3 GetNormal(float t);
        Vector3 GetNormal(float t, Vector3 up);
        Quaternion GetOrientation(float t);
        Quaternion GetOrientation(float t, Vector3 up);
    }
}