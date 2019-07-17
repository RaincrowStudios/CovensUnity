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

        /// <summary>
        /// HTTP Response Code when request is successful.
        /// </summary>
        private const int HttpResponseSuccess = 200;

        private void ShowCovenDebug()
        {
            ValidateCovenManagementDebug();

            DisplayCurrentUserBox();

            DisplayCurrentCovenBox();          
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
                if (loginResult == HttpResponseSuccess)
                {
                    //the player is logged in, get the character
                    LoginAPIManager.GetCharacter((charResult, charResponse) =>
                    {
                        if (charResult == HttpResponseSuccess)
                        {
                            Debug.LogFormat("[DebugUtils] Logged in: {0} - {1}", charResult, loginResponse);
                            _teamManagerUI.gameObject.SetActive(true);
                        }
                        else
                        {
                            Debug.LogErrorFormat("[DebugUtils] Could not retrieve character: {0} - {1}", charResult, loginResponse);
                        }
                    });
                }
                else
                {
                    Debug.LogErrorFormat("[DebugUtils] Could not login in the game: {0} - {1}", loginResult, loginResponse);
                }
            });
        }

        private void DisplayCurrentUserBox()
        {
            // CURRENT USER
            using (new BoxScope("Current User"))
            {
                EditorGUILayout.HelpBox("Current User in the 'Users' option", MessageType.Info);

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Username:", EditorStyles.boldLabel, GUILayout.Width(100));

                    GUIStyle guiStyle = new GUIStyle(EditorStyles.label);
                    string storedUsername = LoginAPIManager.StoredUserName;
                    if (string.IsNullOrWhiteSpace(storedUsername))
                    {
                        guiStyle.normal.textColor = Color.yellow;
                        storedUsername = "Add a username to Current Users in the 'Users' tab";
                    }
                    EditorGUILayout.LabelField(storedUsername, guiStyle);
                }
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Password:", EditorStyles.boldLabel, GUILayout.Width(100));

                    GUIStyle guiStyle = new GUIStyle(EditorStyles.label);
                    string storedUserPassword = LoginAPIManager.StoredUserPassword;
                    if (string.IsNullOrWhiteSpace(storedUserPassword))
                    {
                        guiStyle.normal.textColor = Color.yellow;
                        storedUserPassword = "Add a password to Current Users in the 'Users' tab";
                    }
                    EditorGUILayout.LabelField(storedUserPassword, guiStyle);
                }

                bool disableStartCoven = _teamManagerUI == null || !Application.isPlaying;
                using (new EditorGUI.DisabledGroupScope(disableStartCoven))
                {
                    if (GUILayout.Button("Start Coven Management"))
                    {
                        StartCovenManagement();
                    }
                }
            }
        }

        private void DisplayCurrentCovenBox()
        {
            // CURRENT COVEN
            if (PlayerDataManager.playerData != null)
            {
                using (new BoxScope("Current Coven"))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));

                        GUIStyle guiStyle = new GUIStyle(EditorStyles.label);
                        string covenName = PlayerDataManager.playerData.coven;
                        if (string.IsNullOrWhiteSpace(covenName))
                        {
                            guiStyle.normal.textColor = Color.yellow;
                            covenName = "Not a member of any Coven";
                        }
                        EditorGUILayout.LabelField(covenName, guiStyle);
                    }
                }
            }
        }
    }
}
