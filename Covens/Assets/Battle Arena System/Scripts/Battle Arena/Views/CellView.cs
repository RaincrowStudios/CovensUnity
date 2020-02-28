using Raincrow.BattleArena.Model;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Views
{
    public class CellView : MonoBehaviour
    {
        private class CellClickEvent : UnityEvent<ICellModel> { }

        // Private serialized variables
        [SerializeField] private CellClickEvent _cellClickEvent = new CellClickEvent();
        [SerializeField] private Renderer _renderer;

        // Variables
        private ICellModel _cellModel;

        public void Show(ICellModel cellModel, Vector2 cellScale, UnityAction<ICellModel> callback)
        {
            _cellModel = cellModel;
            _cellClickEvent.AddListener(callback);

            Vector3 localScale = _renderer.transform.localScale;
            localScale.x = cellScale.x;
            localScale.y = cellScale.y;
            _renderer.transform.localScale = localScale;
        }

        protected virtual void OnMouseUpAsButton()
        {
            _cellClickEvent.Invoke(_cellModel);
        }

        protected virtual void OnDisable()
        {
            _cellClickEvent.RemoveAllListeners();
        }
    }
}