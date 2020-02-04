using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Model;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class BattleArenaGridController : MonoBehaviour
    {
        [SerializeField] private BattleArenaGridUIModel _gridUIModel;
        [SerializeField] private Transform _cellsTransform;
        [SerializeField] private AbstractBattleArenaGridFactory _gridFactory; // Factory class responsible for creating our Grid

        protected virtual IEnumerator Start()
        {
            IBattleArenaGridModel gridModel = _gridFactory.Create();
            yield return StartCoroutine(CreateBattleArenaGridUI(gridModel));
        }

        /// <summary>
        /// Creates the Battle Arena Grid UI from an IBattleArenaGridModel gridModel
        /// </summary>
        /// <param name="gridModel">IBattleArenaGridModel that defines how the grid should look</param>
        /// <returns>A CreateBattleArenaGridUI coroutine</returns>
        private IEnumerator CreateBattleArenaGridUI(IBattleArenaGridModel gridModel)
        {
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
                    yield return null;
                }
            }
        }        
    }
}