using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class CellUIController : MonoBehaviour, ICellUIModel
    {
        // Private serialized variables
        [SerializeField] private Renderer _renderer;
        [SerializeField] private Material _defaultMaterial;
        [SerializeField] private Material _selectedMaterial;
        [SerializeField] private BoxCollider _boxCollider;
        
        // Properties
        public Transform Transform => transform;
        public bool IsSelected { get; set; }
        public ICellModel CellModel { get; private set; }

        public void SetIsSelected(bool value)
        {
            IsSelected = value;
            _renderer.material = IsSelected ? _selectedMaterial : _defaultMaterial;
        }

        public void Show(ICellModel cellModel, Vector2 cellScale)
        {
            CellModel = cellModel;

            Vector3 localScale = _renderer.transform.localScale;
            localScale.x = cellScale.x;
            localScale.y = cellScale.y;

            _boxCollider.size = new Vector3(cellScale.x, 0.05f, cellScale.y);

            _renderer.transform.localScale = localScale;
        }
    }
}