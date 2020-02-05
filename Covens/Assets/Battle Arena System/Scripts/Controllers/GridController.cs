using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Model;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Controller
{
    public class GridController : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private Transform _cellsTransform;
        [SerializeField] private GridUIModel _gridUIModel;
        [SerializeField] private AbstractGridModelFactory _gridFactory; // Factory class responsible for creating our Grid

        [Header("Character Settings")]
        [SerializeField] private AbstractCharacterModelFactory _characterFactory; // Factory class responsible for creating our Characters
        [SerializeField] private CharacterUIModel _characterUIModel;

        protected virtual IEnumerator Start()
        {
            IGridModel gridModel = _gridFactory.Create(_characterFactory);
            yield return StartCoroutine(CreateGridUI(gridModel));
        }

        /// <summary>
        /// Creates the Battle Arena Grid UI from an IBattleArenaGridModel gridModel
        /// </summary>
        /// <param name="gridModel">IBattleArenaGridModel that defines how the grid should look</param>
        /// <returns>A CreateBattleArenaGridUI coroutine</returns>
        public IEnumerator CreateGridUI(IGridModel gridModel)
        {
            Vector3 cellLocalScale = _gridUIModel.CellPrefab.transform.localScale;

            float startX = (gridModel.MaxCellsPerColumn - 1) * (cellLocalScale.x * 0.5f);
            startX += _gridUIModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float endX = (gridModel.MaxCellsPerColumn - 1) * (cellLocalScale.x * -0.5f);
            endX -= _gridUIModel.Spacing.x * (gridModel.MaxCellsPerColumn - 1) * 0.5f;

            float startZ = (gridModel.MaxCellsPerLine - 1) * (cellLocalScale.z * 0.5f);
            startZ += _gridUIModel.Spacing.y * (gridModel.MaxCellsPerLine - 1) * 0.5f;

            float endZ = (gridModel.MaxCellsPerLine - 1) * (cellLocalScale.z * -0.5f);
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

                        if (gridModel.Cells[i, j].CharacterModel != null)
                        {
                            CreateCharacterUI(gridModel.Cells[i, j].CharacterModel, cellInstance);
                        }
                    }
                    yield return null;
                }
            }
        }

        private void CreateCharacterUI(ICharacterModel characterModel, GameObject cellInstance)
        {
            Instantiate(_characterUIModel.CharacterPrefab, cellInstance.transform);
        }
    }
}