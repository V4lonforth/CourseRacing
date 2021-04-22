using System;
using UnityEngine;

namespace Scripts.Track.Results
{
    public class TimeManager : MonoBehaviour
    {
        public float CurrentTime { get; private set; }
        
        public Action<float> OnTimeChanged { get; set; }

        private bool _isTicking;
        
        private void Awake()
        {
            TrackManager.Instance.OnTrackStart += StartTrack;
            TrackManager.Instance.OnTrackFinish += FinishTrack;
        }

        private void Start()
        {
            OnTimeChanged?.Invoke(CurrentTime);
        }

        private void StartTrack(Scripts.Track.Track track)
        {
            _isTicking = true;
        }

        private void FinishTrack(Scripts.Track.Track track)
        {
            _isTicking = false;
        }

        private void Update()
        {
            if (!_isTicking) return;
            
            CurrentTime += Time.deltaTime;
            OnTimeChanged?.Invoke(CurrentTime);
        }
    }
}