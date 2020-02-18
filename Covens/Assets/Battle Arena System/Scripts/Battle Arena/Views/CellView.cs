using Raincrow.BattleArena.Model;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.View
{
    public class CellView : MonoBehaviour
    {
        private class CellClickEvent : UnityEvent<ICellModel> { }

        // Private serialized variables
        [SerializeField] private CellClickEvent _cellClickEvent = new CellClickEvent();

        // Private variables
        private ICellModel _cellModel;

        public void Init(ICellModel cellModel, UnityAction<ICellModel> callback)
        {
            _cellModel = cellModel;
            _cellClickEvent.AddListener(callback);            
        }

        protected virtual void OnMouseUpAsButton()
        {
            _cellClickEvent.Invoke(_cellModel);
        }

        protected virtual void OnDestroy()
        {
            _cellClickEvent.RemoveAllListeners();
        }
    }
}