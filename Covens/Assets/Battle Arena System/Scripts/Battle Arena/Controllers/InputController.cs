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
        [SerializeField] private float _zoomTouchSensitivity = 1f;
        [SerializeField] private float _zoomScrollSensitivity = 50f;

        // Variables
        private Camera _battleCamera;


        private bool _wasZoomingLastFrame;
        private Vector2[] _lastZoomPositions;

        // Properties
        public Ray? Touch { get; private set; }
        public Vector2? DragVelocity { get; private set; }

        public float ZoomVelocity { get; private set; }

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
            ZoomVelocity = 0;
        }

        #region Events

        public void OnDrag(PointerEventData eventData)
        {
            //Start Touch Position
            Vector2 startScreenDrag = eventData.position - eventData.delta;
            Vector2 endScreenDrag = eventData.position;

            Vector3? startWorldDrag = ScreenPointToWorldPointInRectangle(_rectTransform, startScreenDrag, eventData.pressEventCamera);
            Vector3? endWorldDrag = ScreenPointToWorldPointInRectangle(_rectTransform, endScreenDrag, eventData.pressEventCamera);

            if (Input.touchCount < 2)
            {
                DragVelocity = endWorldDrag - startWorldDrag;
            }
            else
            {
                DragVelocity = Vector2.zero;
            }
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

        private void Update()
        {
            if (!Input.touchSupported)
            {
                ZoomVelocity = -Input.GetAxis("Mouse ScrollWheel") * _zoomScrollSensitivity;
            }
            else
            {
                HandleZoomTouch();
            }
        }

        void HandleZoomTouch()
        {
            switch (Input.touchCount)
            {
                case 1:
                    _wasZoomingLastFrame = false;
                    break;

                case 2: // Zooming
                    Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };
                    if (!_wasZoomingLastFrame)
                    {
                        _lastZoomPositions = newPositions;
                        _wasZoomingLastFrame = true;
                    }
                    else
                    {
                        // Zoom based on the distance between the new positions compared to the 
                        // distance between the previous positions.
                        float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                        float oldDistance = Vector2.Distance(_lastZoomPositions[0], _lastZoomPositions[1]);
                        float offset = newDistance - oldDistance;

                        ZoomVelocity = -offset * _zoomTouchSensitivity;

                        _lastZoomPositions = newPositions;
                    }
                    break;

                default:
                    _wasZoomingLastFrame = false;
                    break;
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
        float ZoomVelocity { get; }
        void SetActive(bool value);
    }
}