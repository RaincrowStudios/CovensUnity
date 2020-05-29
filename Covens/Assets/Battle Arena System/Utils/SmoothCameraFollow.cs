// Smooth towards the target

using UnityEngine;

namespace Raincrow.Utils
{
    public class SmoothCameraFollow : MonoBehaviour, ISmoothCameraFollow
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothTime = 0.3F;
        [SerializeField] private float _zoomSmoothTime = 0.3F;
        [SerializeField] private float _cameraHeight = 5f;
        [SerializeField] private float _cameraMaxHeight = 350f;
        [SerializeField] private float _cameraMinHeight = 225f;
        [SerializeField] private float _cameraDistance = 10f;

        //PrivateVariables
        private Vector3 velocity = Vector3.zero;
        private float zoomVelocity = 0;

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
            float smoothCameraHeight = Mathf.SmoothDamp(transform.position.y, _cameraHeight, ref zoomVelocity, _zoomSmoothTime);

            // Define a target position above and behind the target transform
            Vector3 targetPosition = _target.TransformPoint(new Vector3(0, smoothCameraHeight, -_cameraDistance));

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

            cameraHeight = Mathf.Clamp(cameraHeight, _cameraMinHeight, _cameraMaxHeight);

            float distance = Mathf.Abs(_cameraHeight - cameraHeight);
            float speed = distance / timeAnimation;

            _animationCameraHeightID = LeanTween.value(_cameraHeight, cameraHeight, speed).setOnUpdate((value)=> {
                _cameraHeight = value;
            }).uniqueId;

            return _animationCameraHeightID;
        }

        public int ResetCameraHeight(float timeAnimation)
        {
            LeanTween.cancel(_animationCameraHeightID);

            float distance = Mathf.Abs(_cameraHeight - _initialCameraHeight);
            float speed = distance / timeAnimation;

            _animationCameraHeightID = LeanTween.value(_cameraHeight, _initialCameraHeight, speed).setOnUpdate((value) => {
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

        public void SetCameraZoom(float speedMove)
        {
            _cameraHeight = Mathf.Clamp(_cameraHeight + speedMove, _cameraMinHeight, _cameraMaxHeight);
        }
    }

    public interface ISmoothCameraFollow
    {
        int SetCameraHeight(float cameraHeight, float timeAnimation);
        void SetCameraZoom(float speedMove);
        int ResetCameraHeight(float timeAnimation);
        int SetCameraDistance(float cameraDistance, float timeAnimation);
        int ResetCameraDistance(float timeAnimation);
    }
}