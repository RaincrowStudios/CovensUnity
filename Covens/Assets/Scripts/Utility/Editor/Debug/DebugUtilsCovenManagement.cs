using Raincrow.Team;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        /// <summary>
        /// Coven Management Scene cache
        /// </summary>
        private Scene _covenManagementScene;
        
        /// <summary>
        /// Foldout flag to show coven members
        /// </summary>
        public bool ExpandMembers
        {
            get { return EditorPrefs.GetBool("DebugUtils.ExpandMembers", false); }
            set { EditorPrefs.SetBool("DebugUtils.ExpandMembers", value); }
        }

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
                UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

                GameObject[] gameObjects = scene.GetRootGameObjects();
                foreach (var gameObject in gameObjects)
                {
                    TeamManagerUI teamManagerUI = gameObject.GetComponentInChildren<TeamManagerUI>();
                    if (teamManagerUI != null)
                    {
                        _teamManagerUI = teamManagerUI;
                        break;
                    }
                }

                if (_teamManagerUI == null)
                {
                    EditorGUILayout.HelpBox("Could not find a TeamManagerUI in the active scene!", MessageType.Warning);
                }
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
            bool disableScope = true;
            if (Application.isPlaying)
            {
                // Login
                if (!LoginAPIManager.accountLoggedIn)
                {
                    LoginAPIManager.Login((loginResult, loginResponse) =>
                    {
                        if (loginResult != HttpResponseSuccess)
                        {
                            Debug.LogErrorFormat("[DebugUtils] Could not login in the game: [Response Code - {0}] - {1}", loginResult, loginResponse);
                        }
                    });
                }
                else if (!LoginAPIManager.characterLoggedIn)
                {
                    //the player is logged in, get the character
                    LoginAPIManager.GetCharacter((charResult, charResponse) =>
                    {
                        if (charResult == HttpResponseSuccess)
                        {
                            Debug.LogFormat("[DebugUtils] Character Logged in: [Response Code - {0}] - {1}", charResult, charResponse);
                        }
                        else
                        {
                            Debug.LogErrorFormat("[DebugUtils] Could not retrieve character: [Response Code - {0}] - {1}", charResult, charResponse);
                        }
                    });
                }
                // Force Start TeamManagerUI button should only appear in the CovenManagement scene, if we have found a TeamManagerUI gameobject
                else if (_teamManagerUI != null && !_teamManagerUI.isActiveAndEnabled)
                {
                    if (_covenManagementScene.name != "CovenManagement")
                    {
                        _covenManagementScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName("CovenManagement");
                    }

                    if (_covenManagementScene.isLoaded)
                    {
                        disableScope = false;
                    }
                }
            }

            using (new EditorGUI.DisabledGroupScope(disableScope))
            {
                if (GUILayout.Button("Force Start TeamManagerUI"))
                {
                    ForceStartTeamManagerUI();
                }
            }            
        }

        private void ForceStartTeamManagerUI()
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
                    EditorGUILayout.SelectableLabel(storedUsername, guiStyle, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                }

                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Password:", EditorStyles.boldLabel, GUILayout.Width(100), GUILayout.Height(EditorGUIUtility.singleLineHeight));

                    GUIStyle guiStyle = new GUIStyle(EditorStyles.label);
                    string storedUserPassword = LoginAPIManager.StoredUserPassword;
                    if (string.IsNullOrWhiteSpace(storedUserPassword))
                    {
                        guiStyle.normal.textColor = Color.yellow;
                        storedUserPassword = "Add a password to Current Users in the 'Users' tab";
                    }
                    EditorGUILayout.SelectableLabel(storedUserPassword, guiStyle, GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
                }

                RequestStartCovenManagement();                
            }
        }       

        private void DisplayCurrentCovenBox()
        {
            // we have a player data
            if (PlayerDataManager.playerData != null)
            {
                if (PlayerDataManager.playerData.covenInfo != null)
                {
                    string covenId = PlayerDataManager.playerData.covenInfo.coven;
                    using (new BoxScope("Coven Information"))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Coven Id: ", EditorStyles.boldLabel, GUILayout.Width(100));
                            DisplaySelectableLabel(covenId);
                        }

                        if (!string.IsNullOrWhiteSpace(_teamData.Name))
                        {
                            DisplayCovenBoxComplete();
                        }
                        // we do not have a team data, let's request one
                        else
                        {
                            using (new GUILayout.HorizontalScope())
                            {
                                EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));
                                DisplaySelectableLabel("Unknown (Refresh)");
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
                // display create coven
                else
                {
                    using (new BoxScope("Create Coven"))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));
                            _covenNameTextField = EditorGUILayout.TextField(_covenNameTextField, GUILayout.ExpandWidth(true));
                        }

                        if (GUILayout.Button("Create Coven"))
                        {
                            TeamManagerRequestHandler.CreateCoven(_covenNameTextField, (teamData, responseCode) =>
                            {
                                if (responseCode == HttpResponseSuccess)
                                {
                                    _teamData = teamData;
                                    Debug.LogFormat("[DebugUtils] Created Coven - Response Code: {0}", responseCode);
                                }
                                else
                                {
                                    Debug.LogErrorFormat("[DebugUtils] Could not create a coven - Response Code: {0}", responseCode);
                                }
                            });
                        }
                    }
                }
            }            
        }

        private void DisplayCovenBoxComplete()
        {
            // Coven Name
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(_teamData.Name);
            }

            // Founder Name
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Founder Id: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(_teamData.CreatedBy);
            }

            // Motto
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Motto: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(_teamData.Motto);
            }

            // School
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("School: ", EditorStyles.boldLabel, GUILayout.Width(100));

                string school = Utilities.GetSchool(_teamData.School);
                DisplaySelectableLabel(school);
            }

            // World Rank
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("World Rank: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(_teamData.WorldRank);
            }

            // Dominion Rank
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Dominion Rank: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(_teamData.DominionRank);
            }

            // Dominion
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Dominion: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(_teamData.Dominion);
            }

            // Total Silver
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Total Silver: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(_teamData.TotalSilver);
            }

            // Total Gold
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Total Gold: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(_teamData.TotalGold);
            }

            // Total Energy
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Total Energy: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(_teamData.TotalEnergy);
            }

            // Created On
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Created On: ", EditorStyles.boldLabel, GUILayout.Width(100));
                string createdOn = Utilities.ShowDateTimeWithCultureInfo(_teamData.CreatedOn);
                DisplaySelectableLabel(createdOn);
            }

            // Members
            if (_teamData.Members != null && _teamData.Members.Length > 0)
            {
                ExpandMembers = Foldout(ExpandMembers, "Members");
                if (ExpandMembers)
                {
                    foreach (TeamMemberData teamMember in _teamData.Members)
                    {
                        // if the team member is a founder, background color changes to cyan 
                        if (teamMember.Id == _teamData.CreatedBy)
                        {
                            Color defaultColor = GUI.color;
                            GUI.color = Color.cyan;
                            DisplayTeamMemberData(teamMember);
                            GUI.color = defaultColor;
                        }
                        else
                        {
                            DisplayTeamMemberData(teamMember);
                        }                     
                    }
                }
            }            
        }

        private void DisplaySelectableLabel(object obj)
        {
            string text = obj?.ToString();
            EditorGUILayout.SelectableLabel(text, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
        }

        private void DisplayTeamMemberData(TeamMemberData teamMember)
        {
            string title = string.Concat(teamMember.Name, " (", teamMember.Id, ")");
            using (new BoxScope(title))
            {
                // Title
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Title: ", EditorStyles.boldLabel, GUILayout.Width(100));
                    DisplaySelectableLabel(teamMember.Title);
                }

                // Level
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Level: ", EditorStyles.boldLabel, GUILayout.Width(100));
                    DisplaySelectableLabel(teamMember.Level);
                }

                // Role
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Role: ", EditorStyles.boldLabel, GUILayout.Width(100));

                    string role = string.Empty;
                    switch (teamMember.Role)
                    {
                        case TeamRole.Admin:
                            role = LocalizeLookUp.GetText("team_member_admin_role");
                            break;                        
                        case TeamRole.Moderator:
                            role = LocalizeLookUp.GetText("team_member_moderator_role");
                            break;
                        default:
                            role = LocalizeLookUp.GetText("team_member_member_role");
                            break;
                    }

                    DisplaySelectableLabel(role);
                }

                // Degree & School
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Degree: ", EditorStyles.boldLabel, GUILayout.Width(100));                    
                    DisplaySelectableLabel(Utilities.WitchTypeControlSmallCaps(teamMember.Degree));
                }

                // Joined On
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Degree: ", EditorStyles.boldLabel, GUILayout.Width(100));
                    string joinedOn = Utilities.ShowDateTimeWithCultureInfo(teamMember.JoinedOn);
                    DisplaySelectableLabel(joinedOn);
                }

                // Last Active On
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField("Degree: ", EditorStyles.boldLabel, GUILayout.Width(100));
                    string lastActiveOn = Utilities.ShowDateTimeWithCultureInfo(teamMember.LastActiveOn);
                    DisplaySelectableLabel(lastActiveOn);
                }
            }
        }
    }
}
