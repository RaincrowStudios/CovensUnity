using Raincrow.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Raincrow.BattleArena.Controllers
{
    public class InputController : MonoBehaviour, IInputController, IDragHandler, IPointerUpHandler, IPointerDownHandler
    {
        // Serialized Variables
        [SerializeField] private ServiceLocator _serviceLocator;

        // Variables
        private Camera _battleCamera;

        // Properties
        public Ray? Touch { get; private set; }
        public Vector3? DragVelocity { get; private set; }

        #region MonoBehaviour

        protected virtual void OnEnable()
        {
            if (_battleCamera == null)
            {
                _battleCamera = _serviceLocator.GetBattleCamera();
            }

            DragVelocity = null;
            Touch = null;
        }

        #endregion

        #region Events

        public void OnDrag(PointerEventData eventData)
        {
            DragVelocity = _battleCamera.transform.TransformDirection(eventData.delta);
        }

        public void OnPointerDown(PointerEventData eventData) { }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!DragVelocity.HasValue)
            {
                Touch = _battleCamera.ScreenPointToRay(eventData.position);
            }

            DragVelocity = null;
        }

        protected virtual void LateUpdate()
        {
            Touch = null;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        #endregion
    }

    public interface IInputController
    {
        Ray? Touch { get; } 
        Vector3? DragVelocity { get; }
        void SetActive(bool value);
    }
}