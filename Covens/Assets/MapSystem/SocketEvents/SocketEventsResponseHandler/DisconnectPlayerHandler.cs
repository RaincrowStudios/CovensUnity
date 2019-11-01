using UnityEngine;
using System.Collections;
using Raincrow.Maps;

namespace Raincrow.GameEventResponses
{
    public class DisconnectPlayerHandler : IGameEventHandler
    {
        public string EventName => "disconnect.player";

        public void HandleResponse(string eventData)
        {
            DisconnectPlayer();
        }

        public static void DisconnectPlayer()
        {
            UIGlobalPopup.ShowPopUp(() =>
            {
                Application.Quit();

#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif

                //LoadingOverlay.Show();
                //SceneManager.LoadSceneAsync(SceneManager.Scene.START, UnityEngine.SceneManagement.LoadSceneMode.Single, null, () => 
                //{
                //    LoadingOverlay.Hide();
                //    SocketClient.Instance?.DisconnectFromSocket();
                //    PlayerDataManager.playerData = null;
                //    LoginAPIManager.loginToken = null;
                //    LoginAPIManager.wssToken = null;
                //});
            },
            LocalizeLookUp.GetText("server_maintenance"));
        }
    }
}