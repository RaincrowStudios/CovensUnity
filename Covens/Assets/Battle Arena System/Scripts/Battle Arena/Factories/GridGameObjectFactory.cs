using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class GridGameObjectFactory : AbstractGridGameObjectFactory
    {
        [SerializeField] private Transform _cellsParent;
        [SerializeField] private GridGameObjectModel _gridGameObjectModel;

        public override IEnumerator<GameObject[,]> Create(IGridModel gridModel)
        {
            // Create GameObjects grid
            GameObject[,] gridGameObjects = new GameObject[gridModel.MaxCellsPerColumn, gridModel.MaxCellsPerRow];

            Vector3 cellLocalScale = _gridGameObjectModel.CellPrefab.transform.localScale;

            float startX = (gridModel.MaxCellsPerColumn - 1) * (cellLocalScale.x * 0.5f);
            startX += _gridGameObjectModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float endX = (gridModel.MaxCellsPerColumn - 1) * (cellLocalScale.x * -0.5f);
            endX -= _gridGameObjectModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float startZ = (gridModel.MaxCellsPerRow - 1) * (cellLocalScale.z * 0.5f);
            startZ += _gridGameObjectModel.Spacing.y * (gridModel.MaxCellsPerRow - 1) * 0.5f;

            float endZ = (gridModel.MaxCellsPerRow - 1) * (cellLocalScale.z * -0.5f);
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
                        GameObject cellInstance = Instantiate(_gridGameObjectModel.CellPrefab, cellPosition, _cellsParent.rotation, _cellsParent);
                        gridGameObjects[i, j] = cellInstance;
                    }

                    yield return null;
                }
            }

            yield return gridGameObjects;
        }
    }
}