using Raincrow.BattleArena.Builder;
using Raincrow.BattleArena.Model;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class BattleArenaGridController : MonoBehaviour
    {
        [SerializeField] private BattleArenaGridUIModel _gridUIModel;
        [SerializeField] private Transform _cellsTransform;

        protected virtual IEnumerator Start()
        {
            // Mock BattleArenaGrid
            BattleArenaGridModel gridModel = CreateMockBattleArenaGridModel();
            float startX = (gridModel.MaxCellsPerColumn - 1) * (_gridUIModel.CellLocalScale.x * 0.5f);
            startX += _gridUIModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float endX = (gridModel.MaxCellsPerColumn - 1) * (_gridUIModel.CellLocalScale.x * -0.5f);
            endX -= _gridUIModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float startZ = (gridModel.MaxCellsPerLine - 1) * (_gridUIModel.CellLocalScale.x * 0.5f);
            startZ += _gridUIModel.Spacing.y * (gridModel.MaxCellsPerLine - 1) * 0.5f;

            float endZ = (gridModel.MaxCellsPerLine - 1) * (_gridUIModel.CellLocalScale.x * -0.5f);
            endZ -= _gridUIModel.Spacing.y * (gridModel.MaxCellsPerLine - 1) * 0.5f;

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
                        cellPosition = _cellsTransform.TransformPoint(cellPosition);
                        GameObject cellInstance = Instantiate(_gridUIModel.CellPrefab, cellPosition, _cellsTransform.rotation, _cellsTransform);
                        cellInstance.transform.localScale = _gridUIModel.CellLocalScale;
                    }                    
                    yield return new WaitForSeconds(0.25f);
                }
            }
        }

        private BattleArenaGridModel CreateMockBattleArenaGridModel()
        {
            // Create a mock BattleArenaGridBuilder
            BattleArenaGridBuilder gridBuilder;
            {
                gridBuilder = new BattleArenaGridBuilder()
                {
                    MaxCellsPerLine = 5,
                    MaxCellsPerColumn = 5,
                };

                gridBuilder.CellBuilders = new BattleArenaCellBuilder[gridBuilder.MaxCellsPerColumn, gridBuilder.MaxCellsPerLine];

                // create first column
                gridBuilder.CellBuilders[0, 0] = null;
                gridBuilder.CellBuilders[0, 1] = new BattleArenaCellBuilder() { Height = 1 };
                gridBuilder.CellBuilders[0, 2] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[0, 3] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[0, 4] = new BattleArenaCellBuilder();

                // create second column
                gridBuilder.CellBuilders[1, 0] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[1, 1] = null;
                gridBuilder.CellBuilders[1, 2] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[1, 3] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[1, 4] = new BattleArenaCellBuilder();

                // create third column
                gridBuilder.CellBuilders[2, 0] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[2, 1] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[2, 2] = null;
                gridBuilder.CellBuilders[2, 3] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[2, 4] = new BattleArenaCellBuilder() { Height = 2 };

                // create fourth column
                gridBuilder.CellBuilders[3, 0] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[3, 1] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[3, 2] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[3, 3] = null;
                gridBuilder.CellBuilders[3, 4] = new BattleArenaCellBuilder();

                // create fifth column
                gridBuilder.CellBuilders[4, 0] = new BattleArenaCellBuilder() { Height = 3 };
                gridBuilder.CellBuilders[4, 1] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[4, 2] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[4, 3] = new BattleArenaCellBuilder();
                gridBuilder.CellBuilders[4, 4] = null;
            }

            return new BattleArenaGridModel(gridBuilder);
        }

        
    }
}