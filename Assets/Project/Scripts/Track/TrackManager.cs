using System;
using Scripts.Utils;
using Scripts.Vehicles;
using UnityEngine;

namespace Scripts.Track
{
    public class TrackManager : Singleton<TrackManager>
    {
        public Action<Track> OnTrackStart { get; set; }
        public Action<Track> OnTrackFinish { get; set; }

        [SerializeField] private Track track;
        [SerializeField] private GameObject playerVehiclePrefab;

        private void Start()
        {
            StartTrack();
        }

        public void StartTrack()
        {
            var playerVehicle = Instantiate(playerVehiclePrefab).GetComponent<IVehicle>();

            if (playerVehicle == null)
            {
                Debug.LogError("Player vehicle prefab doesn't have IVehicle script");
                return;
            }
            
            track.StartTrack(playerVehicle);
            OnTrackStart?.Invoke(track);
        }

        public void FinishTrack()
        {
            OnTrackFinish?.Invoke(track);
        }
    }
}