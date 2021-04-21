using Scripts.Track.Trajectory;
using Scripts.Vehicles;
using UnityEngine;

namespace Scripts.Track
{
    public class Track : MonoBehaviour
    {
        [HideInInspector] public BezierSpline trajectory;
        [HideInInspector] public int value;
        public IVehicle PlayerVehicle { get; set; }

        public void StartTrack(IVehicle playerVehicle)
        {
            PlayerVehicle = playerVehicle;
            PlayerVehicle.GameObject.transform.position = trajectory.GetPosition(0f);
            PlayerVehicle.GameObject.transform.rotation = trajectory.GetOrientation(0f);
        }
    }
}