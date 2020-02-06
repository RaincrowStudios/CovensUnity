using Raincrow.BattleArena.Builder;
using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class MockGridGameObjectFactory : AbstractGridGameObjectFactory
    {
        [SerializeField] private Transform _cellsParent;
        [SerializeField] private GridGameObjectModel _gridGameObjectModel;

        public override GameObject[,] Create()
        {
            // Construct grid builder
            GridBuilder gridBuilder;
            {
                gridBuilder = new GridBuilder()
                {
                    MaxCellsPerLine = 5,
                    MaxCellsPerColumn = 5,
                };

                gridBuilder.CellBuilders = new CellBuilder[gridBuilder.MaxCellsPerColumn, gridBuilder.MaxCellsPerLine];

                // create first column
                gridBuilder.CellBuilders[0, 0] = null;
                gridBuilder.CellBuilders[0, 1] = new CellBuilder();// { Height = 1 };
                gridBuilder.CellBuilders[0, 2] = new CellBuilder();
                gridBuilder.CellBuilders[0, 3] = new CellBuilder();
                gridBuilder.CellBuilders[0, 4] = new CellBuilder();

                // create second column
                gridBuilder.CellBuilders[1, 0] = new CellBuilder();
                gridBuilder.CellBuilders[1, 1] = null;
                gridBuilder.CellBuilders[1, 2] = new CellBuilder();
                gridBuilder.CellBuilders[1, 3] = new CellBuilder();
                gridBuilder.CellBuilders[1, 4] = new CellBuilder();

                // create third column
                gridBuilder.CellBuilders[2, 0] = new CellBuilder();
                gridBuilder.CellBuilders[2, 1] = new CellBuilder();
                gridBuilder.CellBuilders[2, 2] = null;
                gridBuilder.CellBuilders[2, 3] = new CellBuilder();
                gridBuilder.CellBuilders[2, 4] = new CellBuilder();// { Height = 2 };

                // create fourth column
                gridBuilder.CellBuilders[3, 0] = new CellBuilder();
                gridBuilder.CellBuilders[3, 1] = new CellBuilder();
                gridBuilder.CellBuilders[3, 2] = new CellBuilder();
                gridBuilder.CellBuilders[3, 3] = null;
                gridBuilder.CellBuilders[3, 4] = new CellBuilder();

                // create fifth column
                gridBuilder.CellBuilders[4, 0] = new CellBuilder();// { Height = 3 };
                gridBuilder.CellBuilders[4, 1] = new CellBuilder();
                gridBuilder.CellBuilders[4, 2] = new CellBuilder();
                gridBuilder.CellBuilders[4, 3] = new CellBuilder();
                gridBuilder.CellBuilders[4, 4] = null;
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
                }
            }

            return gridGameObjects;
        }
    }
}