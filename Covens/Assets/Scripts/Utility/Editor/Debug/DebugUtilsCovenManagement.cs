using Raincrow.Team;
using Raincrow.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Raincrow.Test
{
    [ExecuteInEditMode]
    public partial class DebugUtils : EditorWindow
    {
        /// <summary>
        /// HTTP Response Code when request is successful.
        /// </summary>
        private const int HttpResponseSuccess = 200;

        /// <summary>
        /// Max Login Tries
        /// </summary>
        private const int MaxLoginTries = 3;

        /// <summary>
        /// Max get character tries
        /// </summary>
        private const int MaxGetCharacterTries = 3;

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
        /// Coven Management Scene cache
        /// </summary>
        private Scene _covenManagementScene;

        /// <summary>
        /// My User Role in the Coven
        /// </summary>
        private int? userRole;

        /// <summary>
        /// Flag that tells if we are sending a request to server
        /// </summary>
        private PadlockSet _padlockSet = new PadlockSet();

        /// <summary>
        /// Flag that indicates if our members are sorted
        /// </summary>
        private bool _membersAreSorted = false;

        /// <summary>
        /// Number of Login Tries
        /// </summary>
        private int _loginTries = 0;

        /// <summary>
        /// Number of Get Character Tries
        /// </summary>
        private int _getCharacterTries = 0;

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

            CovenInfo covenInfo = new CovenInfo();
            if (PlayerDataManager.playerData != null)
            {
                covenInfo = PlayerDataManager.playerData.covenInfo;
            }

            DisplayCurrentUserBox(LoginAPIManager.StoredUserName, LoginAPIManager.StoredUserPassword, _teamManagerUI, covenInfo);

            DisplayCurrentCovenBox();          
        }        

        private void ValidateCovenManagementDebug()
        {
            // Let's find TeamManagerUI in the scene.
            if (_teamManagerUI == null) 
            {
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

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
                _padlockSet = new PadlockSet();
                _teamData = new TeamData();
                _covenNameTextField = string.Empty;
                PlayerDataManager.playerData = null;
                _membersAreSorted = false;
                _loginTries = 0;
                _getCharacterTries = 0;
            }
        }

        private void RefreshTeamData(string covenId)
        {
            _padlockSet.AddPadlock("RefreshTeamData");
            TeamManagerRequestHandler.GetCoven(covenId, (teamData, responseCode) =>
            {
                if (responseCode == HttpResponseSuccess)
                {
                    _teamData = teamData;
                    _membersAreSorted = false;
                }                

                _padlockSet.RemovePadlock("RefreshTeamData");
                Debug.LogFormat("[DebugUtils] Refresh Team Data Request Response Code: {0}", responseCode);
            });
        }



        private void RequestStartCovenManagement(TeamManagerUI teamManagerUI, CovenInfo covenInfo)
        {
            bool disableScope = true;
            if (Application.isPlaying)
            {
                // Login
                if (!LoginAPIManager.accountLoggedIn && !_padlockSet.HasPadlocks() && _loginTries < MaxLoginTries)
                {
                    Debug.Log("trying to login");

                    _padlockSet.AddPadlock("login");

                    LoginAPIManager.Login((loginResult, loginResponse) =>
                    {
                        if (loginResult != HttpResponseSuccess)
                        {
                            Debug.LogErrorFormat("[DebugUtils] Could not login in the game: [Response Code - {0}] - {1}", loginResult, loginResponse);
                            _loginTries++;
                        }

                        _padlockSet.RemovePadlock("login");                     
                    });
                }
                else if (!LoginAPIManager.characterLoggedIn && !_padlockSet.HasPadlocks() && _getCharacterTries < MaxGetCharacterTries)
                {
                    Debug.Log("trying to get character");

                    _padlockSet.AddPadlock("GetCharacter");

                    //the player is logged in, get the character
                    LoginAPIManager.GetCharacter((charResult, charResponse) =>
                    {
                        if (charResult != HttpResponseSuccess)
                        {
                            Debug.LogErrorFormat("[DebugUtils] Could not retrieve character: [Response Code - {0}] - {1}", charResult, charResponse);
                            _getCharacterTries++;
                        }

                        _padlockSet.RemovePadlock("GetCharacter");
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
                    ForceStartTeamManagerUI(teamManagerUI, covenInfo);
                }
            }            
        }

        private void ForceStartTeamManagerUI(TeamManagerUI teamManagerUI, CovenInfo covenInfo)
        {
            teamManagerUI.gameObject.SetActive(true);
            teamManagerUI.RequestShow(covenInfo);
        }

        private void DisplayCurrentUserBox(string username, string password, TeamManagerUI teamManagerUI, CovenInfo covenInfo)
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

                RequestStartCovenManagement(teamManagerUI, covenInfo);                
            }
        }       

        private void DisplayCurrentCovenBox()
        {
            // we have a player data
            if (PlayerDataManager.playerData != null)
            {
                CovenInfo covenInfo = PlayerDataManager.playerData.covenInfo;
                if (covenInfo != null && !string.IsNullOrWhiteSpace(covenInfo.coven))
                {
                    using (new BoxScope("Coven Information"))
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            EditorGUILayout.LabelField("Coven Id: ", EditorStyles.boldLabel, GUILayout.Width(100));
                            DisplaySelectableLabel(covenInfo.coven);
                        }

                        if (!string.IsNullOrWhiteSpace(_teamData.Name))
                        {
                            DisplayCovenBoxComplete(_teamData);
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

                        // Draw Coven Buttons
                        using (new EditorGUI.DisabledGroupScope(_padlockSet.HasPadlocks()))
                        {
                            // when you click on this button, Team Data is requested to server and, if successful, it is updated
                            if (GUILayout.Button("Refresh Team Data"))
                            {
                                RefreshTeamData(covenInfo.coven);
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
                                    covenInfo = new CovenInfo
                                    {
                                        role = TeamRole.Admin,
                                        joinedOn = (long)Utilities.GetUnixTimestamp(System.DateTime.UtcNow),
                                        coven = _teamData.Id
                                    };                                    
                                    PlayerDataManager.playerData.covenInfo = covenInfo;
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

        private void DisplayCovenBoxComplete(TeamData teamData)
        {
            // Coven Name
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Coven Name: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(teamData.Name);
            }

            // Founder Name
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Founder Id: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(teamData.CreatedBy);
            }

            // Motto
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Motto: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(teamData.Motto);
            }

            // School
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("School: ", EditorStyles.boldLabel, GUILayout.Width(100));

                string school = Utilities.GetSchool(teamData.School);
                DisplaySelectableLabel(school);
            }

            // World Rank
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("World Rank: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(teamData.WorldRank);
            }

            // Dominion Rank
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Dominion Rank: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(teamData.DominionRank);
            }

            // Dominion
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Dominion: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(teamData.Dominion);
            }

            // Total Silver
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Total Silver: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(teamData.TotalSilver);
            }

            // Total Gold
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Total Gold: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(teamData.TotalGold);
            }

            // Total Energy
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Total Energy: ", EditorStyles.boldLabel, GUILayout.Width(100));
                DisplaySelectableLabel(teamData.TotalEnergy);
            }

            // Created On
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Created On: ", EditorStyles.boldLabel, GUILayout.Width(100));
                string createdOn = Utilities.ShowDateTimeWithCultureInfo(teamData.CreatedOn);
                DisplaySelectableLabel(createdOn);
            }                       

            // Members
            if (teamData.Members != null && teamData.Members.Length > 0)
            {
                if (!userRole.HasValue)
                {
                    userRole = TeamRole.Member;
                    foreach (TeamMemberData member in teamData.Members)
                    {
                        if (PlayerDataManager.playerData.name == member.Name)
                        {
                            userRole = member.Role;
                            break;
                        }
                    }
                }                

                ExpandMembers = Foldout(ExpandMembers, "Members");
                if (ExpandMembers)
                {
                    // sort members by role
                    TeamMemberData[] members = teamData.Members;

                    if (!_membersAreSorted)
                    {
                        System.Array.Sort(members);
                        _membersAreSorted = true;
                    }                    

                    foreach (TeamMemberData teamMember in members)
                    {
                        string title = string.Concat(teamMember.Name, " (", teamMember.Id, ")");

                        Color defaultColor = GUI.color;
                        GUI.color = GetRoleColor(teamData, teamMember);

                        using (new BoxScope(title))
                        {                            
                            DisplayTeamMemberData(teamMember);
                            GUI.color = defaultColor;

                            // I'm adding this check to prevent me from promoting myself, if i'm not a founder
                            if (PlayerDataManager.playerData.name != teamMember.Name)
                            {
                                DrawPromoteButtons(teamData.Id, userRole.Value, teamMember);
                                DrawDemoteButtons(teamData.Id, userRole.Value, teamMember);
                            }
                        }
                        
                        EditorGUILayout.Space();
                    }
                }
            }            
        }

        private Color GetRoleColor(TeamData teamData, TeamMemberData teamMember)
        {
            if (teamData.CreatedBy == teamMember.Id)
            {
                return Color.cyan;
            }

            switch (teamMember.Role)
            {
                case TeamRole.Admin:
                    return new Color(0.3f, 0.65f, 1f, 1f); // Cornflower Blue
                case TeamRole.Moderator:
                    return new Color(0.9f, 0.8f, 1f, 1f); // Pale Lavender
                default:
                    return Color.white;
            }
        }

        private void DisplaySelectableLabel(object obj)
        {
            string text = obj?.ToString();
            EditorGUILayout.SelectableLabel(text, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight));
        }

        private void DisplayTeamMemberData(TeamMemberData teamMember)
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
                EditorGUILayout.LabelField("Joined On: ", EditorStyles.boldLabel, GUILayout.Width(100));
                string joinedOn = Utilities.ShowDateTimeWithCultureInfo(teamMember.JoinedOn);
                DisplaySelectableLabel(joinedOn);
            }

            // Last Active On
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Last Active On: ", EditorStyles.boldLabel, GUILayout.Width(100));
                string lastActiveOn = Utilities.ShowDateTimeWithCultureInfo(teamMember.LastActiveOn);
                DisplaySelectableLabel(lastActiveOn);
            }
        }        

        private void DrawPromoteButtons(string teamDataId, int userRole, TeamMemberData teamMemberData)
        {
            // Cannot promote people if they have the same or a higher role than you
            if (userRole > teamMemberData.Role)
            {
                using (new GUILayout.HorizontalScope())
                {
                    string labelText = string.Concat("Promote to: ");
                    EditorGUILayout.LabelField(labelText, EditorStyles.boldLabel, GUILayout.Width(100));
                    using (new EditorGUI.DisabledGroupScope(_padlockSet.HasPadlocks()))
                    {
                        if (userRole >= TeamRole.Admin && teamMemberData.Role < TeamRole.Admin)
                        {
                            if (GUILayout.Button("Admin", GUILayout.ExpandWidth(true)))
                            {
                                _padlockSet.AddPadlock("PromoteAdmin");
                                TeamManagerRequestHandler.PromoteMember(teamMemberData.Id, TeamRole.Admin, (responseCode) =>
                                {
                                    if (responseCode == HttpResponseSuccess)
                                    {
                                        Debug.LogFormat("[DebugUtils] Promoted Member {0} to {1}. Response Code: {2}", teamMemberData.Id, "Admin", responseCode);
                                        RefreshTeamData(teamDataId);
                                    }
                                    else
                                    {
                                        Debug.LogErrorFormat("[DebugUtils] Could not promote Member {0} to {1}. Response Code: {2}", teamMemberData.Id, "Admin", responseCode);
                                    }
                                    _padlockSet.RemovePadlock("PromoteAdmin");
                                    RefreshTeamData(teamDataId);
                                });
                            }
                        }

                        if (userRole >= TeamRole.Moderator && teamMemberData.Role < TeamRole.Moderator)
                        {
                            if (GUILayout.Button("Moderator", GUILayout.ExpandWidth(true)))
                            {
                                _padlockSet.AddPadlock("PromoteModerator");

                                TeamManagerRequestHandler.PromoteMember(teamMemberData.Id, TeamRole.Moderator, (responseCode) =>
                                {
                                    if (responseCode == HttpResponseSuccess)
                                    {
                                        Debug.LogFormat("[DebugUtils] Promoted Member {0} to {1}. Response Code: {2}", teamMemberData.Id, "Moderator", responseCode);
                                        RefreshTeamData(teamDataId);
                                    }
                                    else
                                    {
                                        Debug.LogErrorFormat("[DebugUtils] Could not promote Member {0} to {1}. Response Code: {2}", teamMemberData.Id, "Moderator", responseCode);
                                    }
                                    _padlockSet.RemovePadlock("PromoteModerator");
                                    RefreshTeamData(teamDataId);
                                });
                            }
                        }
                    }
                }
            }                               
        }

        private void DrawDemoteButtons(string teamDataId, int userRole, TeamMemberData teamMemberData)
        {
            // Cannot demote people if they have the same or a higher role than you and if they are members
            if (userRole > teamMemberData.Role && teamMemberData.Role > TeamRole.Member)
            {
                using (new GUILayout.HorizontalScope())
                {
                    string labelText = string.Concat("Demote to: ");
                    EditorGUILayout.LabelField(labelText, EditorStyles.boldLabel, GUILayout.Width(100));

                    using (new EditorGUI.DisabledGroupScope(_padlockSet.HasPadlocks()))
                    {
                        if (GUILayout.Button("Member", GUILayout.ExpandWidth(true)))
                        {
                            _padlockSet.AddPadlock("DemoteModerator");

                            TeamManagerRequestHandler.DemoteMember(teamMemberData.Id, TeamRole.Member, (responseCode) =>
                            {
                                if (responseCode == HttpResponseSuccess)
                                {
                                    Debug.LogFormat("[DebugUtils] Demoted Member {0} to {1}. Response Code: {2}", teamMemberData.Id, "Moderator", responseCode);
                                    RefreshTeamData(teamDataId);
                                }
                                else
                                {
                                    Debug.LogErrorFormat("[DebugUtils] Could not demote Member {0} to {1}. Response Code: {2}", teamMemberData.Id, "Moderator", responseCode);
                                }
                                _padlockSet.RemovePadlock("DemoteModerator");
                                RefreshTeamData(teamDataId);
                            });
                        }
                    }
                }
            }          
        }
    }
}
