using Scripts.Track;
using Scripts.Track.Trajectory;
using UnityEngine;

namespace Scripts.Vehicles
{
    public class VehicleController : MonoBehaviour
    {
        [SerializeField] private float maxTrackOffset = 3f;
        [SerializeField] private float maxAngleToMaxRotation;
        [SerializeField] private GameObject tracker;

        private IVehicle _vehicle;
        private ITrajectory _trajectory;

        private float _currentTrackDistance;

        private const float TrajectoryPositionPrecision = 0.0001f;

        private void Awake()
        {
            TrackManager.Instance.OnTrackStart += StartTrack;
        }

        private void StartTrack(Track.Track track)
        {
            _vehicle = track.PlayerVehicle;
            _trajectory = track.trajectory;
        }

        private void Update()
        {
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

            tracker.transform.position = _trajectory.GetPosition(trajectoryT);
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