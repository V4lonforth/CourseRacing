using System;
using Scripts.Track;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.UI
{
    public class VehicleSelector : MonoBehaviour
    {
        [SerializeField] private TrackInfo trackInfo;
        [SerializeField] private GameObject vehiclePrefab;

        private void Awake()
        {
            GetComponent<Toggle>().onValueChanged.AddListener(SelectCar);
        }

        private void SelectCar(bool isActive)
        {
            if (isActive)
            {
                trackInfo.vehiclePrefab = vehiclePrefab;
            }
        }
    }
}