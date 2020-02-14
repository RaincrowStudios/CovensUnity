using Raincrow.GameEventResponses;
using UnityEngine;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Controller;
using System.Collections.Generic;

namespace Raincrow.BattleArena.Manager
{
    public class ChallengeManager : MonoBehaviour
    {
        void Awake()
        {
            BattleOpenHandler.OnBattleOpen += BattleOpen;
        }

        private void BattleOpen(BattleObjectServer battle)
        {
            LoadingOverlay.Show();
            UIMain.SetActive(false);
            UIQuickCast.SetActive(false);

            SceneManager.LoadSceneAsync(SceneManager.Scene.ARENA, UnityEngine.SceneManagement.LoadSceneMode.Additive,
                null,
                () => {
                    MapsAPI.Instance.HideMap(true);
                    BattleController gridController = (BattleController)FindObjectOfType(typeof(BattleController));
                    if (!gridController.isActiveAndEnabled)
                    {
                        gridController.gameObject.SetActive(true);
                    }

                    IGridModel grid = new GridModel(battle.grid.MaxCellsPerColumn, battle.grid.MaxCellsPerLine, battle.grid.Cells);

                    List<ICharacterModel> characters = new List<ICharacterModel>();

                    foreach (GenericCharacterObjectServer character in battle.participants)
                    {
                        if (string.Equals(character.CharacterType, CharacterType.Spirit))
                        {
                            characters.Add(character as ISpiritModel);
                        }
                        else
                        {
                            characters.Add(character as IWitchModel);
                        }
                    }

                    StartCoroutine(gridController.StartBattle(battle._id, grid, characters));
                    
                    LoadingOverlay.Hide();
                }
             );
        }

        private void BattleClose()
        {
            LoadingOverlay.Show();
            UIMain.SetActive(true);
            UIQuickCast.SetActive(true);
            SceneManager.UnloadScene(SceneManager.Scene.ARENA,
                null,
                () => {
                    MapsAPI.Instance.HideMap(false);
                    LoadingOverlay.Hide();
                }
             );
        }
    }
}