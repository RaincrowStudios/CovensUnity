using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class CameraTargetController : MonoBehaviour, ICameraTargetController
    {
        private Vector3 _origin;
        private Vector3 _bounds;
        private int _moveTweenId = int.MinValue;

        public void Move(Vector3 movement)
        {
            LeanTween.cancel(_moveTweenId);

            transform.Translate(movement, Space.World);
            transform.position = GetPositionClamped(transform.position);
        }

        public void SetBounds(Vector3 worldOrigin, Vector3 worldBounds)
        {
            LeanTween.cancel(_moveTweenId);
            _origin = worldOrigin;
            _bounds = worldBounds;
            transform.position = GetPositionClamped(transform.position);
        }

        public int SetTargetPosition(Vector3 position, float speed)
        {
            LeanTween.cancel(_moveTweenId);

            Vector3 targetPosition = GetPositionClamped(position);
            float distance = Vector3.Distance(transform.position, targetPosition);

            _moveTweenId = LeanTween.move(gameObject, targetPosition, distance / speed).setEaseLinear().uniqueId;

            return _moveTweenId;
        }

        private Vector3 GetPositionClamped(Vector3 position)
        {
            float minX = _origin.x - _bounds.x;
            float maxX = _bounds.x + _origin.x;
            position.x = Mathf.Clamp(position.x, minX, maxX);

            float minY = _origin.y - _bounds.y;
            float maxY = _bounds.y + _origin.y;
            position.y = Mathf.Clamp(position.y, minY, maxY);

            float minZ = _origin.z - _bounds.z;
            float maxZ = _bounds.z + _origin.z;
            position.z = Mathf.Clamp(position.z, minZ, maxZ);

            return position;
        }
    }

    public interface ICameraTargetController
    {
        void SetBounds(Vector3 worldOrigin, Vector3 worldBounds);
        void Move(Vector3 movement);
        int SetTargetPosition(Vector3 position, float speed);
    }
}