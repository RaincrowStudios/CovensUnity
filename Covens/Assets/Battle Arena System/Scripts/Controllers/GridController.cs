using Raincrow.BattleArena.Factory;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class GridController : MonoBehaviour
    {
        [Header("Factories")]
        [SerializeField] private AbstractGridGameObjectFactory _gridFactory; // Factory class responsible for creating our Grid        
        [SerializeField] private AbstractCharacterGameObjectFactory _characterFactory; // Factory class responsible for creating our Characters        

        /// <summary>
        /// Grid with all the game objects inserted
        /// </summary>
        private GameObject[,] _grid = new GameObject[0, 0];

        public virtual void OnEnable()
        {
            //_grid = _gridFactory.Create();
            StartCoroutine(CreateGrid());
        }

        protected IEnumerator CreateGrid()
        {
            Coroutine<GameObject[,]> createGrid = this.StartCoroutine<GameObject[,]>(_gridFactory.Create());
            yield return createGrid;
            _grid = createGrid.ReturnValue;

            // Create characters
            int maxCellsPerLine = _grid.GetLength(0);
            int maxCellsPerColumn = _grid.GetLength(1);

            for (int i = 0; i < maxCellsPerLine; i++)
            {
                for (int j = 0; j < maxCellsPerColumn; j++)
                {
                    GameObject cellGameObject = _grid[i, j];
                    if (cellGameObject != null)
                    {
                        yield return this.StartCoroutine<GameObject>(_characterFactory.Create(cellGameObject));
                    }
                }
            }
        }
    }
}