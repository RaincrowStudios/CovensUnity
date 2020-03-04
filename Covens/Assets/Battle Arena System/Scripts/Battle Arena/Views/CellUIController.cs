using Raincrow.BattleArena.Model;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Views
{
    public class CellUIController : MonoBehaviour, ICellUIModel
    {
        // Private serialized variables
        [SerializeField] private Renderer _renderer;

        // Variables
        private ICellModel _cellModel;

        // Properties
        public CellClickEvent OnCellClick { get; private set; } = new CellClickEvent();

        public Transform Transform => transform;

        public void Show(ICellModel cellModel, Vector2 cellScale)
        {
            _cellModel = cellModel;

            Vector3 localScale = _renderer.transform.localScale;
            localScale.x = cellScale.x;
            localScale.y = cellScale.y;
            _renderer.transform.localScale = localScale;
        }

        protected virtual void OnMouseUpAsButton()
        {
            OnCellClick?.Invoke(_cellModel);
        }
    }

    public class CellClickEvent : UnityEvent<ICellModel> { }    
}