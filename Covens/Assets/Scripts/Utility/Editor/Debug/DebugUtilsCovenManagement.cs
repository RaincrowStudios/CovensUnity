using UnityEditor;
using UnityEngine;

namespace Raincrow.Test
{
    [ExecuteInEditMode]
    public partial class DebugUtils : EditorWindow
    {
        /// <summary>
        /// Class that we are going to use to handle all Coven Management features.
        /// </summary>
        private TeamManagerUI _teamManagerUI;

        private void ShowCovenDebug()
        {
            ValidateCovenManagementDebug();

            bool disableStartCoven = _teamManagerUI == null || !Application.isPlaying;
            using (new EditorGUI.DisabledGroupScope(disableStartCoven))
            {
                if (GUILayout.Button("Start Coven Management"))
                {
                    StartCovenManagement();
                }
            }
        }

        private void ValidateCovenManagementDebug()
        {
            // Let's find TeamManagerUI in the scene.
            if (_teamManagerUI == null) 
            {
                UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName("CovenManagement");
                GameObject[] gameObjects = scene.GetRootGameObjects();
                foreach (var gameObject in gameObjects)
                {
                    TeamManagerUI teamManagerUI = gameObject.GetComponentInChildren<TeamManagerUI>();
                    if (teamManagerUI != null)
                    {
                        Debug.Log("[DebugUtils] Found TeamManagerUI GameObject!");
                        _teamManagerUI = teamManagerUI;
                        break;
                    }
                }
            }            

            // After finding it, we disable it, but only if the scene is not being played.
            if (_teamManagerUI != null && _teamManagerUI.isActiveAndEnabled && !Application.isPlaying)
            {
                _teamManagerUI.gameObject.SetActive(false);
                Debug.Log("[DebugUtils] Disabling Coven Management GameObject!");
            }
        }

        private void StartCovenManagement()
        {
            // Login
            LoginAPIManager.Login((loginResult, loginResponse) =>
            {
                if (loginResult == 200)
                {
                    //the player is logged in, get the character
                    LoginAPIManager.GetCharacter((charResult, charResponse) =>
                    {
                        Debug.LogFormat("[DebugUtils] Logged in: {0} - {1}", loginResult, loginResponse);
                        _teamManagerUI.gameObject.SetActive(true);
                    });
                }
                else
                {
                    Debug.LogErrorFormat("[DebugUtils] Could not login in the game: {0} - {1}", loginResult, loginResponse);
                }
            });
        }
    }
}
