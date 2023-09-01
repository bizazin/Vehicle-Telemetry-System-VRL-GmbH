using Behaviours;
using Core.Abstracts;
using UnityEngine;
using VehiclePhysics;

namespace Views
{
    public class VehicleView : View
    {
        [SerializeField] private FieldOfViewBehaviour _fieldOfView;
        [SerializeField] private VehicleBase _vehicleBase;
        
        public VehicleBase Base => _vehicleBase;
        public FieldOfViewBehaviour FieldOfView => _fieldOfView;

        public void SetupFieldOfView(float radius) => _fieldOfView.Setup(radius);
    }
}