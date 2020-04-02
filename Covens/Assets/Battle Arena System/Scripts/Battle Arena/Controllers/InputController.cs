using Raincrow.Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Raincrow.BattleArena.Controllers
{
    public class InputController : MonoBehaviour, IInputController, IDragHandler, IPointerUpHandler, IEndDragHandler, IPointerDownHandler
    {
        // Serialized Variables
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private RectTransform _rectTransform;

        // Variables
        private Camera _battleCamera;

        // Properties
        public Ray? Touch { get; private set; }
        public Vector2? DragVelocity { get; private set; }

        protected virtual void OnEnable()
        {          
            if (_serviceLocator == null)
            {
                _serviceLocator = FindObjectOfType<ServiceLocator>();
            }

            if (_battleCamera == null)
            {
                _battleCamera = _serviceLocator.GetBattleCamera();
            }

            DragVelocity = null;
            Touch = null;
        }

        #region Events

        public void OnDrag(PointerEventData eventData)
        {
            //Start Touch Position
            Vector2 startScreenDrag = eventData.position - eventData.delta;
            Vector2 endScreenDrag = eventData.position;

            Vector3? startWorldDrag = ScreenPointToWorldPointInRectangle(_rectTransform, startScreenDrag, eventData.pressEventCamera);
            Vector3? endWorldDrag = ScreenPointToWorldPointInRectangle(_rectTransform, endScreenDrag, eventData.pressEventCamera);

            DragVelocity = endWorldDrag - startWorldDrag;
            Debug.LogFormat("Dragging {0}:", DragVelocity);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!DragVelocity.HasValue)
            {                
                Touch = _battleCamera.ScreenPointToRay(eventData.position);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DragVelocity = null;
        }

        protected virtual void LateUpdate()
        {
            Touch = null;
            if (DragVelocity.HasValue)
            {
                DragVelocity = Vector2.zero;
            }
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        #endregion

        private Vector3? ScreenPointToWorldPointInRectangle(RectTransform rectTransform, Vector2 screenPoint, Camera cam)
        {
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, cam, out Vector3 worldPoint))
            {
                //Rect rect = rectTransform.rect;
                //return new Vector3
                //{
                //    x = Mathf.InverseLerp(0, rect.width, worldPoint.x),
                //    y = Mathf.InverseLerp(0, rect.height, worldPoint.y),
                //    z = 0f,
                //};
                return worldPoint;
            }
            return null;
        }
    }

    public interface IInputController
    {
        Ray? Touch { get; } 
        Vector2? DragVelocity { get; }
        void SetActive(bool value);
    }
}