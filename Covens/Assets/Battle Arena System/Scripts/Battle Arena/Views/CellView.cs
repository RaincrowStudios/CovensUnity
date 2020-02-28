using Raincrow.BattleArena.Model;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Views
{
    public class CellView : MonoBehaviour
    {
        private class CellClickEvent : UnityEvent<CellView> { }

        // Private serialized variables
        [SerializeField] private CellClickEvent _cellClickEvent = new CellClickEvent();
        [SerializeField] private Renderer _renderer;

        public bool IsEmpty { get { return string.IsNullOrEmpty(CellModel.ObjectId); } }

        public ICellModel CellModel { get; private set; }

        public void Init(ICellModel cellModel, Vector2 cellScale, UnityAction<CellView> callback)
        {
            CellModel = cellModel;
            _cellClickEvent.AddListener(callback);

            Vector3 localScale = _renderer.transform.localScale;
            localScale.x = cellScale.x;
            localScale.y = cellScale.y;
            _renderer.transform.localScale = localScale;
        }

        protected virtual void OnMouseUpAsButton()
        {
            _cellClickEvent.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            _cellClickEvent.RemoveAllListeners();
        }
    }
}