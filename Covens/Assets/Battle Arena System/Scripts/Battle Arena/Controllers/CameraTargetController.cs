using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class CameraTargetController : MonoBehaviour, ICameraTargetController
    {
        private Vector3 _origin;
        private Vector3 _bounds;
        private IEnumerator _moveToTargetPosition;
        //private int _moveTweenId = int.MinValue;

        public void Move(Vector3 movement)
        {
            //LeanTween.cancel(_moveTweenId);
            if (_moveToTargetPosition != null)
            {
                StopCoroutine(_moveToTargetPosition);
            }

            transform.Translate(movement, Space.World);
            transform.position = GetPositionClamped(transform.position);
        }

        public void SetBounds(Vector3 worldOrigin, Vector3 worldBounds)
        {
            //LeanTween.cancel(_moveTweenId);
            if (_moveToTargetPosition != null)
            {
                StopCoroutine(_moveToTargetPosition);
            }

            _origin = worldOrigin;
            _bounds = worldBounds;
            transform.position = GetPositionClamped(transform.position);
        }

        public void SetTargetPosition(Vector3 position, float speed)
        {
            if (_moveToTargetPosition != null)
            {
                StopCoroutine(_moveToTargetPosition);
            }

            _moveToTargetPosition = MoveToTargetPosition(position, speed);
            StartCoroutine(_moveToTargetPosition);

        }

        private IEnumerator MoveToTargetPosition(Vector3 position, float speed)
        {
            Vector3 startPosition = transform.position;
            Vector3 targetPosition = GetPositionClamped(position);
            float distance = Vector3.Distance(transform.position, targetPosition);
            float totalTime = distance / speed;

            for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / totalTime);
                yield return null;
            }

            _moveToTargetPosition = null;
        }

        private Vector3 GetPositionClamped(Vector3 position)
        {
            float minX = _origin.x - _bounds.x;
            float maxX = _bounds.x + _origin.x;
            position.x = Mathf.Clamp(position.x, minX, maxX);

            //float minY = _origin.y - _bounds.y;
            //float maxY = _bounds.y + _origin.y;
            //position.y = Mathf.Clamp(position.y, minY, maxY);

            float minZ = _origin.z - _bounds.z;
            float maxZ = _bounds.z + _origin.z;
            position.z = Mathf.Clamp(position.z, minZ, maxZ);

            return position;
        }

        public bool IsMoving()
        {
            return _moveToTargetPosition != null;
        }
    }

    public interface ICameraTargetController
    {
        void SetBounds(Vector3 worldOrigin, Vector3 worldBounds);
        void Move(Vector3 movement);
        void SetTargetPosition(Vector3 position, float speed);
        bool IsMoving();
    }
}