using Raincrow.GameEventResponses;
using UnityEngine;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Controller;

namespace Raincrow.BattleArena.Manager
{
    public class ChallengeManager : MonoBehaviour
    {
        void Awake()
        {
            BattleOpenHandler.OnBattleOpen += BattleOpen;
        }

        private void BattleOpen(IBattleModel arena)
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.ARENA, UnityEngine.SceneManagement.LoadSceneMode.Additive,
                null,
                () => {
                    MapsAPI.Instance.HideMap(true);
                    GridController gridController = (GridController)FindObjectOfType(typeof(GridController));
                    if (!gridController.isActiveAndEnabled)
                    {
                        gridController.gameObject.SetActive(true);
                    }
                    LoadingOverlay.Hide();
                }
             );
        }

        private void BattleClose()
        {
            LoadingOverlay.Show();
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