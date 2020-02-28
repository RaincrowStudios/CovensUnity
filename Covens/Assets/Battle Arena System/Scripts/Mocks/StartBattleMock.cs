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
        [SerializeField] private int _maxCellsPerRow = 5;
        [SerializeField] private int _maxCellsPerColumn = 5;

        protected virtual void OnValidate()
        {
            if (_serviceLocator == null)
            {
                _serviceLocator = FindObjectOfType<ServiceLocator>();
            }

            // Could not lazily initialize Service Locator
            if (_serviceLocator == null)
            {
                Debug.LogError("Could not find Service Locator!");
            }
        }

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
                    MaxCellsPerRow = _maxCellsPerRow,
                    MaxCellsPerColumn = _maxCellsPerColumn,
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
            WitchModel witchModel = new WitchModel()
            {
                Id = "witch1",
                ObjectType = ObjectType.Witch,
                Degree = 0,
                Name = "SHADOW THE HEDGEHOG",
                Level = 1,
                BaseEnergy = 200,
                Energy = 80
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

            SpiritModel spiritModel = new SpiritModel()
            {
                ObjectType = ObjectType.Spirit,
                Id = "spirit1",
                Texture = "spirit_moonSnake",
                BaseEnergy = 200,
                Energy = 80
            };

            // Add all characters
            List<IWitchModel> witchModels = new List<IWitchModel>();
            List<ISpiritModel> spiritModels = new List<ISpiritModel>();

            // add witch
            gridModel.Cells[0, 0].ObjectId = witchModel.Id;
            witchModels.Add(witchModel);

            // add spirit
            gridModel.Cells[4, 4].ObjectId = spiritModel.Id;
            spiritModels.Add(spiritModel);

            // place characters in grid model
            //bool addSpirit = false;
            //for (int i = 0; i < gridModel.MaxCellsPerRow; i++)
            //{
            //    for (int j = 0; j < gridModel.MaxCellsPerColumn; j++)
            //    {
            //        if (addSpirit)
            //        {
            //            ISpiritModel spiritModelClone = spiritModel.Clone();
            //            spiritModelClone.Id = System.Guid.NewGuid().ToString();
            //            gridModel.Cells[i, j].ObjectId = spiritModelClone.Id;
            //            spiritModels.Add(spiritModelClone);
            //        }
            //        else
            //        {
            //            IWitchModel witchModelClone = witchModel.Clone();
            //            witchModelClone.Id = System.Guid.NewGuid().ToString();
            //            gridModel.Cells[i, j].ObjectId = witchModelClone.Id;
            //            witchModels.Add(witchModelClone);
            //        }

            //        addSpirit = !addSpirit;
            //    }
            //}                        

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