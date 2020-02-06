using Raincrow.BattleArena.Factory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private Transform _cellsTransform;    
        [SerializeField] private AbstractGridGameObjectFactory _gridFactory; // Factory class responsible for creating our Grid        
        [SerializeField] private AbstractCharacterGameObjectFactory _characterFactory; // Factory class responsible for creating our Characters        

        /// <summary>
        /// Grid with all the game objects inserted
        /// </summary>
        private GameObject[,] _grid = new GameObject[0, 0];

        /// <summary>
        /// List with all characters
        /// </summary>
        private List<GameObject> _characters = new List<GameObject>();

        public virtual void OnEnable()
        {
            StartCoroutine(InstantiateGrid());
        }

        public virtual void OnDisable()
        {
            DestroyGrid();
        }

        protected IEnumerator InstantiateGrid()
        {
            // Create grid
            Coroutine<GameObject[,]> createGrid = this.StartCoroutine<GameObject[,]>(_gridFactory.Create());
            yield return createGrid;
            _grid = createGrid.ReturnValue;

            // Create characters
            int maxCellsPerLine = _grid.GetLength(0);
            int maxCellsPerColumn = _grid.GetLength(1);

            // Initialize list of characters
            _characters = new List<GameObject>(maxCellsPerColumn * maxCellsPerLine);

            for (int i = 0; i < maxCellsPerLine; i++)
            {
                for (int j = 0; j < maxCellsPerColumn; j++)
                {
                    GameObject cellGameObject = _grid[i, j];
                    if (cellGameObject != null && Random.Range(0f, 1f) < 0.1f)
                    {
                        Coroutine<GameObject> createCharacter = this.StartCoroutine<GameObject>(_characterFactory.Create(cellGameObject.transform));
                        yield return createCharacter;

                        // add a character
                        _characters.Add(createCharacter.ReturnValue);
                    }
                }
            }
        }

        protected void DestroyGrid()
        {
            // Destroy characters
            for (int i = _characters.Count - 1; i >= 0; i--)
            {
                Destroy(_characters[i]);
            }
            _characters.Clear();

            // Destroy grid 
            for (int i = 0; i < _cellsTransform.childCount; i++)
            {
                Destroy(_cellsTransform.GetChild(i).gameObject);
            }
            _grid = new GameObject[0, 0];
        }
    }
}