using Raincrow.Team;
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
        /// Coven name in the 'Create Coven' dialog
        /// </summary>
        private string _covenNameTextField = string.Empty;

        /// <summary>
        /// Team Data that we request to server
        /// </summary>
        private TeamData _teamData = new TeamData();

        /// <summary>
        /// Flag that tells if we are refreshing the TeamData
        /// </summary>
        private bool _isRefreshingTeamData;

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
                if (scene != null && scene.isLoaded)
                {
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
                else
                {
                    EditorGUILayout.HelpBox("This tab only works in the CovenManagement scene!", MessageType.Warning);
                }
            }            

            // After finding it, we disable it, but only if the scene is not being played.
            if (_teamManagerUI != null && _teamManagerUI.isActiveAndEnabled && !Application.isPlaying)
            {
                _teamManagerUI.gameObject.SetActive(false);
                Debug.Log("[DebugUtils] Disabling Coven Management GameObject!");
            }           

            if (!Application.isPlaying)
            {
                _isRefreshingTeamData = false;
                _teamData = new TeamData();
                _covenNameTextField = string.Empty;
                PlayerDataManager.playerData = null;
            }
        }

        private void RequestStartCovenManagement()
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
                            StartCovenManagement();
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

        private void StartCovenManagement()
        {
            _teamManagerUI.gameObject.SetActive(true);
            _teamManagerUI.RequestShow(PlayerDataManager.playerData.covenInfo);
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
                        RequestStartCovenManagement();
                    }
                }
            }
        }       

        private void DisplayCurrentCovenBox()
        {
            // we have a player data
            if (PlayerDataManager.playerData != null && PlayerDataManager.playerData.covenInfo != null)
            {
                string covenId = PlayerDataManager.playerData.covenInfo.coven;
                using (new BoxScope("Coven Information"))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Coven Id: ", EditorStyles.boldLabel, GUILayout.Width(100));
                        EditorGUILayout.LabelField(covenId, GUILayout.ExpandWidth(true));
                    }
                    
                    if (!string.IsNullOrWhiteSpace(_teamData.Name))
                    {
                        // Coven Name
                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));
                            EditorGUILayout.LabelField(_teamData.Name, GUILayout.ExpandWidth(true));
                        }
                    }
                    // we do not have a team data, let's request one
                    else
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));
                            EditorGUILayout.LabelField("Unknown (Refresh)", GUILayout.ExpandWidth(true));
                        }
                    }

                    using (new EditorGUI.DisabledGroupScope(_isRefreshingTeamData))
                    {
                        // when you click on this button, Team Data is requested to server and, if successful, it is updated
                        if (GUILayout.Button("Refresh Team Data"))
                        {
                            _isRefreshingTeamData = true;
                            TeamManagerRequestHandler.GetCoven(covenId, (teamData, responseCode) =>
                            {
                                if (responseCode == HttpResponseSuccess)
                                {
                                    _teamData = teamData;                                    
                                }

                                _isRefreshingTeamData = false;
                                Debug.LogFormat("[DebugUtils] Refresh Team Data Request Response Code: {0}", responseCode);                                
                            });
                        }
                    }
                }                    
            }
            // we do not have a player data, we must request it first
            else
            {

            }

            //if  && _teamData == null && Application.isPlaying)
            //{
            //    // get a coven info to request a coven
            //    CovenInfo covenInfo = PlayerDataManager.playerData.covenInfo;

            //    using (new BoxScope("Coven Creation"))
            //    {
            //        using (new GUILayout.HorizontalScope())
            //        {
            //            EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));

            //            string covenName = coveninf

            //            EditorGUILayout.LabelField(, GUILayout.ExpandWidth(true));
            //            //_covenNameTextField = EditorGUILayout.TextField(_covenNameTextField, GUILayout.ExpandWidth(true));
            //        }
            //    }
            //    TeamManagerRequestHandler.GetCoven(covenInfo.coven, (teamData, responseCode) =>
            //    {                    
            //        if (responseCode == HttpResponseSuccess)
            //        {
            //            _teamData = teamData;
            //        }
            //    });

            //}
            //// clean up Team Data from memory if we are on play mode
            //else if (_teamData != null && !Application.isPlaying)
            //{
            //    _teamData = null;
            //}

            //if (PlayerDataManager.playerData != null && Application.isPlaying)
            //{
            //    // CURRENT COVEN
            //    if (PlayerDataManager.playerData.covenInfo == null)
            //    {
            //        // CREATE COVEN
            //        using (new BoxScope("Coven Creation"))
            //        {
            //            using (new GUILayout.HorizontalScope())
            //            {
            //                EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));
            //                _covenNameTextField = EditorGUILayout.TextField(_covenNameTextField, GUILayout.ExpandWidth(true));
            //            }

            //            using (new EditorGUI.DisabledGroupScope(string.IsNullOrWhiteSpace(_covenNameTextField)))
            //            {
            //                if (GUILayout.Button("Create Coven"))
            //                {
            //                    TeamManagerRequestHandler.CreateCoven(_covenNameTextField, (teamData, responseCode) =>
            //                    {
            //                        Debug.LogFormat("[DebugUtils] Create Coven Response: {0}", responseCode);
            //                    });
            //                }
            //            }
            //        }

            //        //using (new BoxScope("Current Coven"))
            //        //{
            //        //    using (new GUILayout.HorizontalScope())
            //        //    {
            //        //        string covenId = PlayerDataManager.playerData.covenInfo.coven;
            //        //        string covenName = PlayerDataManager.playerData.covenInfo.;
            //        //        EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));
            //        //        EditorGUILayout.LabelField(, EditorStyles.label);
            //        //    }
            //        //}
            //    }
            //    else
            //    {
                   
            //    }
            //}
        }
    }
}
