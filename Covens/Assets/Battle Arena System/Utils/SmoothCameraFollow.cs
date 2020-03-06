// Smooth towards the target

using UnityEngine;

namespace Raincrow.Utils
{
    public class SmoothCameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothTime = 0.3F;
        [SerializeField] private float _cameraHeight = 5f;
        [SerializeField] private float _cameraDistance = 10f;

        private Vector3 velocity = Vector3.zero;

        protected void LateUpdate()
        {
            // Define a target position above and behind the target transform
            Vector3 targetPosition = _target.TransformPoint(new Vector3(0, _cameraHeight, -_cameraDistance));

            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, _smoothTime);
            
            // Look at target
            transform.LookAt(_target);
        }
    }
}