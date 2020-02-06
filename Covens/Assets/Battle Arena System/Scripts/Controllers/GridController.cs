using Raincrow.BattleArena.Factory;
using System.Collections;
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
        /// Array with all characters
        /// </summary>
        private GameObject[] _characters = new GameObject[0];

        /// <summary>
        /// Number of characters instantiated inside the characters array
        /// </summary>
        private int _numCharacters = 0;

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
            _characters = new GameObject[maxCellsPerColumn * maxCellsPerLine];

            for (int i = 0; i < maxCellsPerLine; i++)
            {
                for (int j = 0; j < maxCellsPerColumn; j++)
                {
                    GameObject cellGameObject = _grid[i, j];
                    if (cellGameObject != null)
                    {
                        Coroutine<GameObject> createCharacter = this.StartCoroutine<GameObject>(_characterFactory.Create(cellGameObject.transform));
                        yield return createCharacter;

                        // add a character
                        _characters[_numCharacters] = createCharacter.ReturnValue;
                        _numCharacters += 1;
                    }
                }
            }
        }

        protected void DestroyGrid()
        {
            // Destroy characters
            for (int i = _numCharacters - 1; i >= 0; i--)
            {
                Destroy(_characters[i]);

                _numCharacters -= 1;
            }
            _characters = new GameObject[0];

            // Destroy grid 
            for (int i = 0; i < _cellsTransform.childCount; i++)
            {
                Destroy(_cellsTransform.GetChild(i).gameObject);
            }
            _grid = new GameObject[0, 0];
        }
    }
}