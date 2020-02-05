using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    [System.Serializable]
    public class GridUIModel
    {
        // Serializable variables
        [SerializeField] private GameObject _cellPrefab; // Cell Prefab 
        [SerializeField] private Vector2 _spacing = Vector2.zero; // width and length distance between each cell

        // Properties
        public GameObject CellPrefab { get => _cellPrefab; private set => _cellPrefab = value; }
        public Vector2 Spacing { get => _spacing; private set => _spacing = value; }        
    }
}
