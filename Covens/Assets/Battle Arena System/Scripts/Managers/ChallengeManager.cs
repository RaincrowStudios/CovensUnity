using Raincrow.GameEventResponses;
using UnityEngine;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Controllers;
using System.Collections.Generic;
using Raincrow.Services;
using Raincrow.Loading.View;
using System.Collections;

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
                () =>
                {
                    MapsAPI.Instance.HideMap(true);                    

                    IGridModel grid = new GridModel(battle.Grid.MaxCellsPerColumn, battle.Grid.MaxCellsPerLine, battle.Grid.Cells);

                    IList<IWitchModel> witches = new List<IWitchModel>();
                    IList<ISpiritModel> spirits = new List<ISpiritModel>();
                    foreach (GenericCharacterObjectServer character in battle.Participants)
                    {
                        if (character.ObjectType == ObjectType.Spirit)
                        {
                            spirits.Add(character);
                        }
                        else if (character.ObjectType == ObjectType.Witch)
                        {
                            witches.Add(character);
                        }                        
                    }

                    StartCoroutine(StartBattle(battle.Id, PlayerDataManager.playerData.account, grid, witches, spirits));

                    LoadingOverlay.Hide();
                }
             );
        }

        private IEnumerator StartBattle(string id, string playerId, IGridModel grid, IList<IWitchModel> witches, IList<ISpiritModel> spirits)
        {
            ServiceLocator serviceLocator = FindObjectOfType<ServiceLocator>();
            ILoadingView loadingView = serviceLocator.GetLoadingView();
            BattleController battleController = serviceLocator.GetBattleController();

            yield return StartCoroutine(loadingView.Show(0f, 1f));
            yield return StartCoroutine(battleController.StartBattle(id, playerId, grid, witches, spirits, loadingView));
            yield return StartCoroutine(loadingView.Hide(1f));
        }

        private void BattleClose()
        {
            LoadingOverlay.Show();
            UIMain.SetActive(true);
            UIQuickCast.SetActive(true);
            SceneManager.UnloadScene(SceneManager.Scene.ARENA,
                null,
                () =>
                {
                    MapsAPI.Instance.HideMap(false);
                    LoadingOverlay.Hide();
                }
             );
        }
    }
}