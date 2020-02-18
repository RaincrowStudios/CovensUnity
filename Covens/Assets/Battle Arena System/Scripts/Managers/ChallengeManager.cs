using Raincrow.GameEventResponses;
using UnityEngine;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Controller;
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

                    List<ICharacterModel> characters = new List<ICharacterModel>();

                    foreach (GenericCharacterObjectServer character in battle.Participants)
                    {
                        characters.Add(character); // as ICharacterModel
                    }

                    IBattleModel battleModel = new BattleModel()
                    {
                        Id = battle.Id,
                        Grid = grid,
                        Characters = characters
                    };
                    StartCoroutine(StartBattle(battleModel));

                    LoadingOverlay.Hide();
                }
             );
        }

        private IEnumerator StartBattle(IBattleModel battleModel)
        {
            ServiceLocator serviceLocator = FindObjectOfType<ServiceLocator>();
            ILoadingView loadingView = serviceLocator.GetLoadingView();
            BattleController battleController = serviceLocator.GetBattleController();

            yield return StartCoroutine(loadingView.Show(0f, 1f));
            yield return StartCoroutine(battleController.StartBattle(battleModel.Id, battleModel.Grid, battleModel.Characters, loadingView));
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