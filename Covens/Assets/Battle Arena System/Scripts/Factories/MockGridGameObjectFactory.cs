using Raincrow.BattleArena.Builder;
using Raincrow.BattleArena.Model;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class MockGridGameObjectFactory : AbstractGridGameObjectFactory
    {
        [SerializeField] private Transform _cellsParent;
        [SerializeField] private GridGameObjectModel _gridGameObjectModel;

        public override IEnumerator Create()
        {
            // Construct grid builder
            GridBuilder gridBuilder;
            {
                gridBuilder = new GridBuilder()
                {
                    MaxCellsPerLine = 25,
                    MaxCellsPerColumn = 25,
                };

                gridBuilder.CellBuilders = new CellBuilder[gridBuilder.MaxCellsPerLine, gridBuilder.MaxCellsPerColumn];

                for (int i = 0; i < gridBuilder.MaxCellsPerLine; i++)
                {
                    for (int j = 0; j < gridBuilder.MaxCellsPerColumn; j++)
                    {
                        gridBuilder.CellBuilders[i, j] = new CellBuilder();
                    }
                }
            }

            IGridModel gridModel = new GridModel(gridBuilder);

            // Create GameObjects grid
            GameObject[,] gridGameObjects = new GameObject[gridModel.MaxCellsPerColumn, gridModel.MaxCellsPerLine];

            Vector3 cellLocalScale = _gridGameObjectModel.CellPrefab.transform.localScale;

            float startX = (gridModel.MaxCellsPerColumn - 1) * (cellLocalScale.x * 0.5f);
            startX += _gridGameObjectModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float endX = (gridModel.MaxCellsPerColumn - 1) * (cellLocalScale.x * -0.5f);
            endX -= _gridGameObjectModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float startZ = (gridModel.MaxCellsPerLine - 1) * (cellLocalScale.z * 0.5f);
            startZ += _gridGameObjectModel.Spacing.y * (gridModel.MaxCellsPerLine - 1) * 0.5f;

            float endZ = (gridModel.MaxCellsPerLine - 1) * (cellLocalScale.z * -0.5f);
            endZ -= _gridGameObjectModel.Spacing.y * (gridModel.MaxCellsPerLine - 1) * 0.5f;

            for (int i = 0; i < gridModel.MaxCellsPerColumn; i++)
            {
                for (int j = 0; j < gridModel.MaxCellsPerLine; j++)
                {
                    if (gridModel.Cells[i, j] != null)
                    {
                        Vector3 cellPosition = new Vector3
                        {
                            x = Mathf.Lerp(startX, endX, i / (gridModel.MaxCellsPerColumn - 1f)),
                            y = gridModel.Cells[i, j].Height,
                            z = Mathf.Lerp(startZ, endZ, j / (gridModel.MaxCellsPerLine - 1f)),
                        };

                        GameObject cellInstance = null;
                        if (_cellsParent != null)
                        {
                            cellPosition = _cellsParent.TransformPoint(cellPosition);
                            cellInstance = Instantiate(_gridGameObjectModel.CellPrefab, cellPosition, _cellsParent.rotation, _cellsParent);
                        }                        
                        else
                        {
                            cellInstance = Instantiate(_gridGameObjectModel.CellPrefab, cellPosition, Quaternion.identity);
                        }                        

                        gridGameObjects[i, j] = cellInstance;
                    }

                    yield return null;
                }
            }

            yield return gridGameObjects;
        }
    }
}