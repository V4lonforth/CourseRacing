using System;
using UnityEngine;

namespace Scripts.Track.Trajectory
{
    public interface ITrajectory
    {
        float Length { get; }
        float GetDistance(float t);
        float GetT(float distance);
        
        Vector3 GetPosition(float t);
        Vector3 GetTangent(float t);
        Vector3 GetNormal(float t);
        Vector3 GetNormal(float t, Vector3 up);
        Quaternion GetOrientation(float t);
        Quaternion GetOrientation(float t, Vector3 up);
        float GetClosestPointT(Vector3 point, float sectionStart, float sectionEnd, float precision);
    }
}