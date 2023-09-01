using System;
using System.Collections.Generic;
using UnityEngine;

namespace Behaviours
{
    [RequireComponent(typeof(Collider))]
    public class FieldOfViewBehaviour : MonoBehaviour
    {
        [SerializeField] private float _radius = 20;
        private readonly List<OtherCar> _detectedVehicles = new();

        public event Action<float> OnClosestVehicleDistM;

        private void OnTriggerEnter(Collider other)
        {
            var vehicle = other.GetComponent<OtherCar>();
            if (vehicle != null)
            {
                _detectedVehicles.Add(vehicle);
                PerformRaycast();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            var vehicle = other.GetComponent<OtherCar>();
            if (vehicle != null) 
                PerformRaycast();
        }

        private void OnTriggerExit(Collider other)
        {
            var vehicle = other.GetComponent<OtherCar>();
            if (vehicle != null)
            {
                _detectedVehicles.Remove(vehicle);
                PerformRaycast();
            }
        }

        private void PerformRaycast()
        {
            var minDistance = _radius;

            foreach (var vehicle in _detectedVehicles)
            {
                if (Physics.Raycast(transform.position, vehicle.transform.position - transform.position, out var hit))
                {
                    var hitVehicle = hit.collider.GetComponent<OtherCar>();
                    if (hitVehicle != null && hitVehicle == vehicle)
                    {
                        var distance = Vector3.Distance(transform.position, hit.point);
                        if (distance < minDistance) 
                            minDistance = distance;
                    }
                }
            }

            OnClosestVehicleDistM?.Invoke(minDistance);
        }
    }
}