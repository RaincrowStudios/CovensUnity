using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    [System.Serializable]
    public class BattleArenaGridUIModel
    {
        // Serializable variables
        [SerializeField] private GameObject _cellPrefab; // Cell Prefab 
        [SerializeField] private Vector3 _cellLocalScale = Vector3.one; // Local Scale of each cell
        [SerializeField] private Vector2 _spacing = Vector2.zero; // width and length distance between each cell

        // Properties
        public GameObject CellPrefab { get => _cellPrefab; private set => _cellPrefab = value; }
        public Vector3 CellLocalScale { get => _cellLocalScale; set => _cellLocalScale = value; }
        public Vector2 Spacing { get => _spacing; private set => _spacing = value; }        
    }
}
