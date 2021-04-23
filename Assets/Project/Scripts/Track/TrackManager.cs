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

        [SerializeField] private TrackInfo trackInfo;
        [SerializeField] private Track track;

        private void Start()
        {
            StartTrack();
        }

        public void StartTrack()
        {
            var playerVehicle = Instantiate(trackInfo.vehiclePrefab).GetComponent<IVehicle>();

            if (playerVehicle == null)
            {
                Debug.LogError("Player vehicle prefab doesn't have IVehicle script");
                return;
            }

            if (track == null)
                track = Instantiate(trackInfo.trackPrefab).GetComponent<Track>();
            
            track.StartTrack(playerVehicle);
            OnTrackStart?.Invoke(track);
        }

        public void FinishTrack()
        {
            OnTrackFinish?.Invoke(track);
        }
    }
}