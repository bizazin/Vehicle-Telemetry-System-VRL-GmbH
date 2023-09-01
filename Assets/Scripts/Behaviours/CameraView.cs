using UnityEngine;
using VehiclePhysics;
using Views;
using Zenject;

namespace Behaviours
{
    [RequireComponent(typeof(VPCameraController))]
    public class CameraView : MonoBehaviour
    {
        [SerializeField] private VPCameraController _cameraController;

        [Inject]
        private void Construct
        (
            VehicleView vehicleView
        )
        {
            _cameraController.target = vehicleView.transform;
        }

    }
}