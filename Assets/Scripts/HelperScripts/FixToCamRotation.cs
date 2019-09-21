using ControllerScripts;
using UnityEngine;

namespace HelperScripts
{
    public class FixToCamRotation : MonoBehaviour
    {
        // Start is called before the first frame update
        private Quaternion _rotation;
        private Transform _tramMenuTransform;
        private Transform _mainCameraTransform;
        
        private void Awake()
        {
            _tramMenuTransform = transform;
            if (Camera.main != null) _mainCameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            var camRotation = _mainCameraTransform.rotation;
            var rotation = _tramMenuTransform.rotation;
            _tramMenuTransform.rotation = Quaternion.Euler(rotation.x, camRotation.y, camRotation.z);
        }
    }
}