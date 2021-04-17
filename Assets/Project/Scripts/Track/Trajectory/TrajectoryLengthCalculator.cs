using System;
using System.Linq;
using UnityEngine;

namespace Scripts.Track.Trajectory
{
    public class TrajectoryLengthCalculator
    {
        private readonly ITrajectory _trajectory;
        private readonly int _sampleSize;
        
        private float[] _distances;

        public bool Empty => _trajectory == null;
        
        public TrajectoryLengthCalculator(ITrajectory trajectory, int sampleSize = 8)
        {
            _trajectory = trajectory;
            _sampleSize = sampleSize;
            
            CalculateDistances(sampleSize);
        }

        private void CalculateDistances(int sampleSize = 8)
        {
            _distances = new float[sampleSize];
            _distances[0] = 0f;
            var totalLength = 0f;
            
            var previousPosition = _trajectory.GetPosition(0f);
            
            for (var i = 1; i < sampleSize; i++)
            {
                var t = (float) i / (sampleSize - 1);
                var nextPosition = _trajectory.GetPosition(t);
                var distance = (nextPosition - previousPosition).magnitude;
                
                totalLength += distance;
                _distances[i] = totalLength;
                previousPosition = nextPosition;
            }
        }

        public float GetDistance(float t)
        {
            if (_sampleSize <= 0)
            {
                Debug.LogError("Unable to sample trajectory length");
                return 0f;
            }

            if (_sampleSize == 1)
                return _distances.First();

            t *= _distances.Length - 1;
            var lowerIndex = Mathf.FloorToInt(t);
            var upperIndex = Mathf.FloorToInt(t + 1f);

            if (upperIndex >= _distances.Length) return _distances.Last();
            if (lowerIndex < 0) return _distances.First();
            return Mathf.Lerp(_distances[lowerIndex], _distances[upperIndex], t - lowerIndex);
        }   

        public float GetT(float distance)
        {
            if (_sampleSize <= 0)
            {
                Debug.LogError("Unable to sample trajectory length");
                return 0f;
            }

            if (_sampleSize == 1)
                return 0f;

            var index = Array.BinarySearch(_distances, distance);

            if (index >= 0)
                return (float)index / (_sampleSize - 1);

            index = ~index;
            if (index >= _sampleSize)
                return 1f;

            if (index == 0 && distance < _distances[0])
                return 0f;

            return (index - 1 + Mathf.InverseLerp(_distances[index - 1], _distances[index], distance)) / (_sampleSize - 1);
        }     
    }
}