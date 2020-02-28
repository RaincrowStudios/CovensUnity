using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.Services;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.BattleArena.Factory
{
    public class GridGameObjectFactory : AbstractGridGameObjectFactory
    {
        [SerializeField] private Transform _cellsParent;
        [SerializeField] private GridGameObjectModel _gridGameObjectModel;
        [SerializeField] private ServiceLocator _serviceLocator;

        // private variables        
        private ObjectPool _objectPool;

        protected virtual void OnEnable()
        {
            if (_objectPool == null)
            {
                _objectPool = _serviceLocator.GetObjectPool();
            }
        }

        public override IEnumerator<GameObject[,]> Create(IGridModel gridModel, UnityAction<CellView> cellClickCallback)
        {
            // Create GameObjects grid
            GameObject[,] gridGameObjects = new GameObject[gridModel.MaxCellsPerColumn, gridModel.MaxCellsPerRow];

            Vector2 cellScale = _gridGameObjectModel.CellScale;

            float startX = (gridModel.MaxCellsPerColumn - 1) * (cellScale.x * 0.5f);
            startX += _gridGameObjectModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float endX = (gridModel.MaxCellsPerColumn - 1) * (cellScale.x * -0.5f);
            endX -= _gridGameObjectModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float startZ = (gridModel.MaxCellsPerRow - 1) * (cellScale.y * 0.5f);
            startZ += _gridGameObjectModel.Spacing.y * (gridModel.MaxCellsPerRow - 1) * 0.5f;

            float endZ = (gridModel.MaxCellsPerRow - 1) * (cellScale.y * -0.5f);
            endZ -= _gridGameObjectModel.Spacing.y * (gridModel.MaxCellsPerRow - 1) * 0.5f;

            for (int i = 0; i < gridModel.MaxCellsPerColumn; i++)
            {
                for (int j = 0; j < gridModel.MaxCellsPerRow; j++)
                {
                    if (gridModel.Cells[i, j] != null)
                    {
                        Vector3 cellPosition = new Vector3
                        {
                            x = Mathf.Lerp(startX, endX, i / (gridModel.MaxCellsPerColumn - 1f)),
                            y = gridModel.Cells[i, j].Height,
                            z = Mathf.Lerp(startZ, endZ, j / (gridModel.MaxCellsPerRow - 1f)),
                        };

                        cellPosition = _cellsParent.TransformPoint(cellPosition);

                        // Create CellView                        
                        CellView cellInstance = _objectPool.Spawn(_gridGameObjectModel.CellPrefab, _cellsParent, cellPosition, _cellsParent.rotation);
                        cellInstance.Init(gridModel.Cells[i, j], cellScale, cellClickCallback);

                        gridGameObjects[i, j] = cellInstance.gameObject;
                    }

                    yield return null;
                }
            }

            yield return gridGameObjects;
        }
    }
}