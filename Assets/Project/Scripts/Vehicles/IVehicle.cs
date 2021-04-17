using UnityEngine;

namespace Scripts.Vehicles
{
    public interface IVehicle
    {
        GameObject GameObject { get; }
        Vector3 Velocity { get; }
        
        void Accelerate(float strength);
        void Rotate(float strength);
    }
}