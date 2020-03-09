// Smooth towards the target

using UnityEngine;

namespace Raincrow.Utils
{
    public class SmoothCameraFollow : MonoBehaviour, ISmoothCameraFollow
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothTime = 0.3F;
        [SerializeField] private float _cameraHeight = 5f;
        [SerializeField] private float _cameraDistance = 10f;

        //PrivateVariables
        private Vector3 velocity = Vector3.zero;

        private float _initialCameraHeight;
        private float _initialCameraDistance;

        private int _animationCameraDistanceID;
        private int _animationCameraHeightID;

        private void Start()
        {
            _initialCameraDistance = _cameraDistance;
            _initialCameraHeight = _cameraHeight;
        }

        protected void LateUpdate()
        {
            // Define a target position above and behind the target transform
            Vector3 targetPosition = _target.TransformPoint(new Vector3(0, _cameraHeight, -_cameraDistance));

            // Smoothly move the camera towards that target position
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, _smoothTime);
            
            // Look at target
            transform.LookAt(_target);
        }

        public int SetCameraDistance(float cameraDistance, float timeAnimation)
        {
            LeanTween.cancel(_animationCameraDistanceID);
            _animationCameraDistanceID = LeanTween.value(_cameraDistance, cameraDistance, timeAnimation).setOnUpdate((value) => {
                _cameraDistance = value;
            }).uniqueId;

            return _animationCameraDistanceID;
        }

        public int SetCameraHeight(float cameraHeight, float timeAnimation)
        {
            LeanTween.cancel(_animationCameraHeightID);
            _animationCameraHeightID = LeanTween.value(_cameraHeight, cameraHeight, timeAnimation).setOnUpdate((value)=> {
                _cameraHeight = value;
            }).uniqueId;

            return _animationCameraHeightID;
        }

        public int ResetCameraHeight(float timeAnimation)
        {
            LeanTween.cancel(_animationCameraHeightID);
            _animationCameraHeightID = LeanTween.value(_cameraHeight, _initialCameraHeight, timeAnimation).setOnUpdate((value) => {
                _cameraHeight = value;
            }).uniqueId;

            return _animationCameraHeightID;
        }

        public int ResetCameraDistance(float timeAnimation)
        {
            LeanTween.cancel(_animationCameraDistanceID);
            _animationCameraDistanceID = LeanTween.value(_cameraDistance, _initialCameraDistance, timeAnimation).setOnUpdate((value) => {
                _cameraDistance = value;
            }).uniqueId;

            return _animationCameraDistanceID;
        }
    }

    public interface ISmoothCameraFollow
    {
        int SetCameraHeight(float cameraHeight, float timeAnimation);
        int ResetCameraHeight(float timeAnimation);
        int SetCameraDistance(float cameraDistance, float timeAnimation);
        int ResetCameraDistance(float timeAnimation);
    }
}