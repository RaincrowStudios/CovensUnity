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
        
        public bool IsEmpty { get { return string.IsNullOrEmpty(CellModel.ObjectId); } }

        public ICellModel CellModel { get; private set; }

        public void Init(ICellModel cellModel, UnityAction<CellView> callback)
        {
            CellModel = cellModel;
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