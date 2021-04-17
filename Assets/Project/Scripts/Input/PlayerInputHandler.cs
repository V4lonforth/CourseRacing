using Scripts.Track;
using Scripts.Vehicles;
using UnityEngine;

namespace Scripts.Input
{
    public class PlayerInputHandler : BasicInputHandler
    {
        private IVehicle _vehicle;

        private void Awake()
        {
            TrackManager.Instance.OnTrackStart += StartTrack; 
        }

        private void StartTrack(Track.Track track)
        {
            _vehicle = track.PlayerVehicle;
        }

        protected override void HandlePress(Vector2 inputPosition)
        {
            if (_vehicle == null) return;
            
            if (inputPosition.x >= Screen.width / 2f)
            {
                _vehicle.Accelerate(1f);
            }
            else
            {
                _vehicle.Accelerate(-1f);
            }
        }

        protected override void HandleHold(Vector2 inputPosition)
        {
            HandlePress(inputPosition);
        }

        protected override void HandleRelease(Vector2 inputPosition)
        {
            HandlePress(inputPosition);
        }
    }
}