using Raincrow.BattleArena.Model;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.View
{
    public class CellView : MonoBehaviour
    {
        private class CellClickEvent : UnityEvent<CellView> { }

        // Private serialized variables
        [SerializeField] private CellClickEvent _cellClickEvent = new CellClickEvent();

        // Private variables
        private ICellModel _cellModel;

        public bool IsEmpty { get { return string.IsNullOrEmpty(_cellModel.ObjectId); } }
        public ICellModel CellModel { get => _cellModel;}

        public void Init(ICellModel cellModel, UnityAction<CellView> callback)
        {
            _cellModel = cellModel;
            _cellClickEvent.AddListener(callback);            
        }

        protected virtual void OnMouseUpAsButton()
        {
            _cellClickEvent.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            _cellClickEvent.RemoveAllListeners();
        }
    }
}