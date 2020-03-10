using Raincrow.BattleArena.Controllers;
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

        protected virtual void OnEnable()
        {
            if (_serviceLocator == null)
            {
                _serviceLocator = FindObjectOfType<ServiceLocator>();
            }

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
            GridModel.Builder gridBuilder;
            {
                gridBuilder = new GridModel.Builder()
                {
                    MaxCellsPerRow = _maxCellsPerRow,
                    MaxCellsPerColumn = _maxCellsPerColumn,
                };

                gridBuilder.CellBuilders = new CellModel.Builder[gridBuilder.MaxCellsPerRow, gridBuilder.MaxCellsPerColumn];

                for (int i = 0; i < gridBuilder.MaxCellsPerRow; i++)
                {
                    for (int j = 0; j < gridBuilder.MaxCellsPerColumn; j++)
                    {
                        gridBuilder.CellBuilders[i, j] = new CellModel.Builder()
                        {
                            X = i,
                            Y = j
                        };
                    }
                }
            }

            IGridModel gridModel = gridBuilder.Build(); // Create grid model

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

            SpiritModel witchsSpiritModel = new SpiritModel()
            {
                ObjectType = ObjectType.Spirit,
                Id = "spirit2",
                Texture = "spirit_dapperSkeleton",
                OwnerId = "witch1",
                BaseEnergy = 500,
                Energy = 300
            };

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
            //gridModel.Cells[0, 0].ObjectId = witchModel.Id;
            gridModel.SetObjectToGrid(witchModel, 0, 0);
            witchModels.Add(witchModel);

            // add witch's spirit
            //gridModel.Cells[0, 1].ObjectId = spiritModel.Id;
            gridModel.SetObjectToGrid(witchsSpiritModel, 4, 2);
            spiritModels.Add(witchsSpiritModel);

            // add spirit
            //gridModel.Cells[4, 4].ObjectId = spiritModel.Id;
            gridModel.SetObjectToGrid(spiritModel, 4, 4);
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
            yield return StartCoroutine(battleController.StartBattle(battleId, witchModel.Id, gridModel, witchModels, spiritModels, loadingView));            
            StartCoroutine(loadingView.Hide(1f));
        }
#endif
    }
}