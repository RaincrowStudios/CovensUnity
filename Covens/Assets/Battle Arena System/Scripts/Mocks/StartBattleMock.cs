using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Model;
using Raincrow.Loading.View;
using Raincrow.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.Mocks
{
    public class StartBattleMock : MonoBehaviour
    {
#if !RELEASE
        
        [SerializeField] private ServiceLocator _serviceLocator;

        protected virtual void OnEnable()
        {
            DownloadManager.DownloadAssets(() =>
            {
                LoginAPIManager.Login((result, response) =>
                {
                    LoginAPIManager.GetCharacter(null);
                    StartCoroutine(CreateGrid());
                });
            });
        }

        private IEnumerator CreateGrid()
        {
            // Construct grid builder
            GridBuilder gridBuilder;
            {
                gridBuilder = new GridBuilder()
                {
                    MaxCellsPerRow = 5,
                    MaxCellsPerColumn = 5,
                };

                gridBuilder.CellBuilders = new CellBuilder[gridBuilder.MaxCellsPerRow, gridBuilder.MaxCellsPerColumn];

                for (int i = 0; i < gridBuilder.MaxCellsPerRow; i++)
                {
                    for (int j = 0; j < gridBuilder.MaxCellsPerColumn; j++)
                    {
                        gridBuilder.CellBuilders[i, j] = new CellBuilder();
                    }
                }
            }
            IGridModel gridModel = new GridModel(gridBuilder); // Create grid model

            // Create characters
            IWitchModel witchModel = new WitchModel()
            {
                Id = "emanuel",
                ObjectType = ObjectType.Witch,
                Degree = 0,
                Name = "Emanuel",
                Level = 1
            };

            InventoryApparelModel equip = new InventoryApparelModel()
            {
                Id = "cosmetic_m_A_B",
                Position = InventoryApparelPosition.BaseBody,
                Assets = new string[1] { "m_A_B" }
            };
            witchModel.Inventory.Equipped.Add(equip);

            equip = new InventoryApparelModel()
            {
                Id = "cosmetic_m_E_B",
                Position = InventoryApparelPosition.BaseBody,
                Assets = new string[1] { "m_E_B" }
            };
            witchModel.Inventory.Equipped.Add(equip);

            equip = new InventoryApparelModel()
            {
                Id = "cosmetic_m_O_B",
                Position = InventoryApparelPosition.BaseBody,
                Assets = new string[1] { "m_O_B" }
            };
            witchModel.Inventory.Equipped.Add(equip);

            equip = new InventoryApparelModel()
            {
                Id = "cosmetic_m_A_H",
                Position = InventoryApparelPosition.BaseBody,
                Assets = new string[2] { "m_A_H_Relaxed", "m_A_H_Censer" }
            };
            witchModel.Inventory.Equipped.Add(equip);

            ISpiritModel spiritModel = new SpiritModel()
            {
                ObjectType = ObjectType.Spirit,
                Id = "Crackudo",
                Texture = "spirit_moonSnake",
                BaseEnergy = 200,
                Energy = 80
            };

            // place characters in grid model
            gridModel.Cells[0, 0].ObjectId = witchModel.Id;
            gridModel.Cells[0, 1].ObjectId = spiritModel.Id;

            // Add all characters
            List<IWitchModel> witchModels = new List<IWitchModel> { witchModel };
            List<ISpiritModel> spiritModels = new List<ISpiritModel> { spiritModel };

            // Battle Id
            string battleId = System.Guid.NewGuid().ToString();

            // Show Loading
            ILoadingView loadingView = _serviceLocator.GetLoadingView();            
            yield return StartCoroutine(loadingView.Show(0.1f, 1f));

            BattleController battleController = _serviceLocator.GetBattleController();
            yield return StartCoroutine(battleController.StartBattle(battleId, gridModel, witchModels, spiritModels, loadingView));            
            StartCoroutine(loadingView.Hide(1f));
        }
#endif
    }
}