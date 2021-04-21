using System;
using UnityEngine;

namespace Scripts.Track.ControlPoints
{
    public class ControlPoint : MonoBehaviour
    {
        public Action<ControlPoint> OnPassed { get; set; }
        
        [SerializeField] private LayerMask targetMask;
        
        private void OnTriggerEnter(Collider other)
        {
            if (targetMask != (targetMask | (1 << other.gameObject.layer))) return;
            
            OnPassed?.Invoke(this);
            Remove();
        }

        private void Remove()
        {
            gameObject.SetActive(false);
        }
    }
}