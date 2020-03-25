using Raincrow.GameEventResponses;
using UnityEngine;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Controllers;
using System.Collections.Generic;
using Raincrow.Services;
using Raincrow.Loading.View;
using System.Collections;
using BattleArena;

namespace Raincrow.BattleArena.Manager
{
    public class ChallengeManager : MonoBehaviour
    {
        void Awake()
        {
            BattleOpenHandler.OnBattleOpen += BattleOpen;
        }

        void Start()
        {
            bool inBattle = false;
            string battleId = "5e73da9d4c2f3b78dfabf588"; //In really i'm sending a spirit id and initializing a new battle, needs change when finish on backend
            if (inBattle){
                UIGlobalPopup.ShowPopUp(()=>ReturnToBattle(battleId), () => { }, LocalizeLookUp.GetText("battle_text_return"));
            }
        }

        private void ReturnToBattle(string battleID)
        {
            ChallengeRequests.Challenge(battleID);
        }

        private void BattleOpen(BattleObjectServer battle)
        {
            UIMain.Instance.HideBattleWaitScreen(0.3f);
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

                    StartCoroutine(StartBattle(battle.Id, PlayerDataManager.playerData.instance, grid, witches, spirits));

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
    }
}