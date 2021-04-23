using UnityEngine;

namespace Scripts.Track
{
    [CreateAssetMenu(fileName = "Data", menuName = "TrackInfo")]
    public class TrackInfo : ScriptableObject
    {
        public GameObject vehiclePrefab;
        public GameObject trackPrefab;
    }
}