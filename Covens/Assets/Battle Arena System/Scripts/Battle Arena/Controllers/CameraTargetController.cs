using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class CameraTargetController : MonoBehaviour, ICameraTargetController
    {
        private Vector3 _origin;
        private Vector3 _bounds;
        private IEnumerator _moveTo;
        //private int _moveTweenId = int.MinValue;

        public void Move(Vector3 movement)
        {
            //LeanTween.cancel(_moveTweenId);
            if (_moveTo != null)
            {
                StopCoroutine(_moveTo);
            }

            transform.Translate(movement, Space.World);
            transform.position = GetPositionClamped(transform.position);
        }

        public void SetPosition(Vector3 position)
        {
            if (_moveTo != null)
            {
                StopCoroutine(_moveTo);
            }

            transform.position = GetPositionClamped(position);
        }

        public void SetBounds(Vector3 worldOrigin, Vector3 worldBounds)
        {
            //LeanTween.cancel(_moveTweenId);
            if (_moveTo != null)
            {
                StopCoroutine(_moveTo);
            }

            _origin = worldOrigin;
            _bounds = worldBounds;
            transform.position = GetPositionClamped(transform.position);
        }

        public IEnumerator MoveTo(Vector3 position, float speed)
        {
            if (_moveTo != null)
            {
                StopCoroutine(_moveTo);
            }

            Vector3 targetPosition = GetPositionClamped(position);
            float distance = Vector3.Distance(transform.position, targetPosition);
            float totalTime = distance / speed;

            _moveTo = InnerMoveTo(targetPosition, totalTime);
            yield return StartCoroutine(_moveTo);
        }

        public IEnumerator MoveBy(Vector3 position, float time)
        {
            if (_moveTo != null)
            {
                StopCoroutine(_moveTo);
            }

            Vector3 targetPosition = GetPositionClamped(position);
            _moveTo = InnerMoveTo(targetPosition, time);
            yield return StartCoroutine(_moveTo);
        }

        private IEnumerator InnerMoveTo(Vector3 position, float totalTime)
        {
            Vector3 startPosition = transform.position;
            for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
            {
                transform.position = Vector3.Lerp(startPosition, position, elapsedTime / totalTime);
                yield return null;
            }

            _moveTo = null;
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
            return _moveTo != null;
        }
    }

    public interface ICameraTargetController
    {
        void SetBounds(Vector3 worldOrigin, Vector3 worldBounds);
        void Move(Vector3 movement);
        void SetPosition(Vector3 position);
        IEnumerator MoveTo(Vector3 position, float speed);
        IEnumerator MoveBy(Vector3 position, float time);
        bool IsMoving();
    }
}