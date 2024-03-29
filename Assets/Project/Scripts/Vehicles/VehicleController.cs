﻿using Scripts.Track;
using Scripts.Track.Trajectory;
using UnityEngine;

namespace Scripts.Vehicles
{
    public class VehicleController : MonoBehaviour
    {
        [SerializeField] private float maxTrackOffset = 3f;
        [SerializeField] private float maxAngleToMaxRotation;

        private IVehicle _vehicle;
        private ITrajectory _trajectory;

        private float _currentTrackDistance;
        private bool _isControlling;

        private const float TrajectoryPositionPrecision = 0.0001f;

        private void Awake()
        {
            TrackManager.Instance.OnTrackStart += StartTrack;
            TrackManager.Instance.OnTrackFinish += FinishTrack;
        }

        private void StartTrack(Track.Track track)
        {
            _vehicle = track.PlayerVehicle;
            _trajectory = track.trajectory;

            _isControlling = true;
        }

        private void FinishTrack(Track.Track track)
        {
            _vehicle = null;
            _trajectory = null;
            
            _isControlling = false;
        }

        private void Update()
        {
            if (!_isControlling) return;
            
            CheckTrackPosition();
            ChangeDirection();
        }

        private void CheckTrackPosition()
        {
            var distanceOffset = _vehicle.Velocity.magnitude * Time.deltaTime * 2f + 1f;
            var furtherTrackT = _trajectory.GetT(_currentTrackDistance + distanceOffset);
            var nearTrackT = _trajectory.GetT(_currentTrackDistance - distanceOffset);
            var trajectoryT = _trajectory.GetClosestPointT(_vehicle.GameObject.transform.position, nearTrackT,
                furtherTrackT, TrajectoryPositionPrecision);
            _currentTrackDistance = _trajectory.GetDistance(trajectoryT);
        }

        private void ChangeDirection()
        {
            var t = _trajectory.GetT(_currentTrackDistance);
            var trackOffset = _trajectory.GetPosition(t) - _vehicle.GameObject.transform.position;
            var k = Mathf.Clamp01(trackOffset.magnitude / maxTrackOffset);
            var direction = (1 - k) * _trajectory.GetTangent(t) + k * trackOffset.normalized;

            var forward2D = new Vector2(_vehicle.GameObject.transform.forward.x, _vehicle.GameObject.transform.forward.z);
            var direction2D = new Vector2(direction.x, direction.z);
            
            var angle = Vector2.SignedAngle(direction2D, forward2D);
            var steerStrength = Mathf.Clamp(angle / maxAngleToMaxRotation, -1f, 1f);
            
            _vehicle.Rotate(steerStrength);
        }
    }
}