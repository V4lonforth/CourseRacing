using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Track.Trajectory
{
    [Serializable]
    public class CompoundTrajectory : ITrajectory
    {
        public ITrajectory this[int i] => _trajectories[i];

        private List<ITrajectory> _trajectories;

        public CompoundTrajectory(List<ITrajectory> trajectories)
        {
            _trajectories = trajectories;
        }

        public Vector3 GetPosition(float t)
        {
            GetT(t, out var number, out var newT);
            return _trajectories[number].GetPosition(newT);
        }
        
        public Vector3 GetTangent(float t)
        {
            GetT(t, out var number, out var newT);
            return _trajectories[number].GetTangent(newT);
        }
        
        public Vector3 GetNormal(float t) => GetNormal(t, Vector3.up);
        public Vector3 GetNormal(float t, Vector3 up)
        {
            GetT(t, out var number, out var newT);
            return _trajectories[number].GetNormal(newT, up);
        }
        
        public Quaternion GetOrientation(float t) => GetOrientation(t, Vector3.up);
        public Quaternion GetOrientation(float t, Vector3 up)
        {
            GetT(t, out var number, out var newT);
            return _trajectories[number].GetOrientation(newT, up);
        }

        private void GetT(float t, out int number, out float newT)
        {
            t *= _trajectories.Count;
            number = Mathf.FloorToInt(t);
            if (number < 0) number = 0;
            else if (number >= _trajectories.Count) number = _trajectories.Count - 1;
            newT = t - number;
        }
    }
}