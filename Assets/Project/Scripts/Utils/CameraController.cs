using Scripts.Track;
using UnityEngine;

namespace Scripts.Utils
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float scaleSpeed;
        
        [SerializeField] private float minCameraDistance;
        [SerializeField] private float maxCameraDistance;

        private float _cameraDistance = 30f;
        
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
                transform.position = _playerVehicleTransform.position - transform.forward * _cameraDistance;
            }
        }

        public void Rotate(Vector2 direction)
        {
            direction *= rotationSpeed;
            if (transform.eulerAngles.x + direction.y <= 5f || transform.eulerAngles.x + direction.y >= 89f)
                direction.y = 0f;
            
            transform.eulerAngles = new Vector3(transform.eulerAngles.x + direction.y, transform.eulerAngles.y - direction.x, 0f);
            transform.position = _playerVehicleTransform.position - transform.forward * _cameraDistance;
        }

        public void Scale(float value)
        {
            _cameraDistance = Mathf.Clamp(_cameraDistance + value * scaleSpeed, minCameraDistance, maxCameraDistance);
        }
    }
}