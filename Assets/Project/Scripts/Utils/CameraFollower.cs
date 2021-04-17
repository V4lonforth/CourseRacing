using Scripts.Track;
using UnityEngine;

namespace Scripts.Utils
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] private Vector3 offset;
        
        private Transform _playerVehicleTransform;
        
        private void Awake()
        {
            TrackManager.Instance.OnTrackStart += OnTrackStart; 
        }

        private void OnTrackStart(Track.Track track)
        {
            _playerVehicleTransform = track.PlayerVehicle.GameObject.transform;
        }

        private void Update()
        {
            if (_playerVehicleTransform != null)
            {
                transform.position = _playerVehicleTransform.position + offset;
            }
        }
    }
}