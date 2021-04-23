using Scripts.Track;
using Scripts.Vehicles;
using UnityEngine;

namespace Scripts.Input
{
    public class PlayerInputHandler : BasicInputHandler
    {
        private IVehicle _vehicle;
        private bool _isControlling;

        private void Awake()
        {
            TrackManager.Instance.OnTrackStart += StartTrack;
            TrackManager.Instance.OnTrackFinish += FinishTrack;
        }

        private void Update()
        {
            if (_vehicle != null && !_isControlling)
            {
                _vehicle.Accelerate(-1f);
            }
        }

        private void StartTrack(Track.Track track)
        {
            _vehicle = track.PlayerVehicle;
            _isControlling = true;
        }

        private void FinishTrack(Track.Track track)
        {
            _isControlling = false;
        }

        protected override bool HandlePress(Vector2 inputPosition)
        {
            if (!_isControlling || _vehicle == null) return false;

            if (inputPosition.x >= Screen.width / 2f)
            {
                _vehicle.Accelerate(1f);
            }
            else
            {
                _vehicle.Accelerate(-1f);
            }

            return true;
        }

        protected override bool HandleHold(Vector2 inputPosition)
        {
            return HandlePress(inputPosition);
        }

        protected override bool HandleRelease(Vector2 inputPosition)
        {
            return HandlePress(inputPosition);
        }
    }
}