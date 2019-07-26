using Newtonsoft.Json;
using Raincrow.Team;
using System.Collections.Generic;
using UnityEngine;

public static class TeamManager
{    
    public static event System.Action<string, string> OnJoinCoven;
    public static event System.Action OnLeaveCoven;
    public static event System.Action OnKicked;
    public static event System.Action<string, string> OnCovenCreated;
    public static event System.Action OnCovenDisbanded;

    public static string MyCovenId { get { return PlayerDataManager.playerData.covenInfo.coven; } }
    
    public static CovenInfo MyCovenInfo { get { return PlayerDataManager.playerData.covenInfo; } }

    public static CovenRole MyRole
    {
        get
        {
            if (string.IsNullOrEmpty(MyCovenId))
                return CovenRole.NONE;
            else
                return (CovenRole)PlayerDataManager.playerData.covenInfo.role;
        }
    }

    public static TeamData MyCovenData { get; set; }
    
    public static void GetCoven(string coven, bool isName, System.Action<TeamData, string> callback)
    {
        Debug.Log("retriving coven " + coven);

        string endpoint = "coven/" + coven + "?name=" + isName.ToString().ToLowerInvariant();
        APIManager.Instance.Get(endpoint, (response, result) =>
        {
            if (result == 200)
            {
                //return the TeamData
                TeamData teamData = JsonUtility.FromJson<TeamData>(response);

                //cache the players coven data
                if (string.IsNullOrEmpty(MyCovenId) == false && teamData.Id == MyCovenId)
                    MyCovenData = teamData;

                callback?.Invoke(teamData, null);
            }
            else
            {
                //return the error
                callback?.Invoke(null, APIManager.ParseError(response));
            }
        });
    }

    public static void CreateCoven(string covenName, System.Action<TeamData, string> callback)
    {
        Debug.Log("creating the coven \"" + covenName + "\"");

        string data = $"{{\"name\":\"{covenName}\"}}";
        
        APIManager.Instance.Post("coven/", data, (response, responseCode) =>
        {
            if (responseCode == 200)
            {
                TeamData covenData = JsonUtility.FromJson<TeamData>(response);
                //MyCovenData = covenData; //dont cache this data, its missing some attributes
                PlayerDataManager.playerData.covenInfo = new CovenInfo
                {
                    coven = covenData.Id,
                    role =  (int)covenData.Members[0].Role,
                    joinedOn = covenData.CreatedOn
                };

                //OnJoinCoven?.Invoke(covenData.Id)
                callback(covenData, null);
            }
            else
            {
                callback(null, APIManager.ParseError(response));
            }
        });
    }

    public static void DisbandCoven(System.Action<int, string> callback)
    {
        Debug.Log("disbanding " + PlayerDataManager.playerData.name + "'s coven");

        APIManager.Instance.Delete(
            "coven/disband",
            "{}",
            (response, result) =>
            {
                if (result == 200)
                {
                    MyCovenData = null;
                    PlayerDataManager.playerData.covenInfo = new CovenInfo
                    {
                        coven = null,
                        joinedOn = 0,
                        role = 0,
                    };
                }
                callback(result, response);
            });
    }

    public static void SendRequest(string coven, bool isName, System.Action<CovenRequest, string> callback)
    {
        Debug.Log("sending request to join the coven " + coven);

        APIManager.Instance.Put("coven/sendRequest/" + coven + "?name=" + isName.ToString().ToLowerInvariant(), "{}", (response, result) =>
        {
            if (result == 200)
            {
                CovenRequest covenRequest = JsonConvert.DeserializeObject<CovenRequest>(response);
                PlayerDataManager.playerData.covenRequests.Add(covenRequest);
                callback?.Invoke(covenRequest, null);
            }
            else
            {
                callback?.Invoke(null, APIManager.ParseError(response));
            }
        });
    }

    public static void AcceptRequest(string playerId, System.Action<TeamMemberData, string> callback)
    {
        Debug.Log("accepting " + playerId + " into the coven");

        APIManager.Instance.Put("coven/acceptRequest/" + playerId, "{}", (response, result) =>
        {
            if (result == 200)
            {
                TeamMemberData member = JsonUtility.FromJson<TeamMemberData>(response);

                //updat the coven's members
                MyCovenData.Members.Add(member);

                //update the requests
                MyCovenData.PendingRequests.RemoveAll(req => req.Character == playerId);

                callback?.Invoke(member, null);
            }
            else
            {
                callback?.Invoke(null, APIManager.ParseError(response));
            }
        });
    }

    public static void RejectRequest(string playerId, System.Action<string> callback)
    {
        Debug.Log("reject request from " + playerId);

        APIManager.Instance.Put("coven/rejectRequest/" + playerId, "{}", (response, result) =>
        {
            if (result == 200)
            {
                MyCovenData.PendingRequests.RemoveAll(req => req.Name == playerId);
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(APIManager.ParseError(response));
            }
        });
    }
    
    public static void SendInvite(string player, bool isName, System.Action<PendingInvite, string> callback)
    {
        Debug.Log("sending coven invitation to " + player);

        APIManager.Instance.Put("coven/invite/" + player + "?name=" + isName.ToString().ToLowerInvariant(), "{}", (response, result) =>
        {
            if (result == 200)
            {
                PendingInvite pendingInvite = JsonUtility.FromJson<PendingInvite>(response);
                MyCovenData.PendingInvites.Add(pendingInvite);
                callback?.Invoke(pendingInvite, null);
            }
            else
            {
                callback?.Invoke(null, APIManager.ParseError(response));
            }
        });
    }

    public static void CancelInvite(string playerId, System.Action<string> callback)
    {
        Debug.Log("cancel invite to " + playerId);

        APIManager.Instance.Put("coven/cancelInvite/" + playerId, "{}", (response, result) =>
        {
            if (result == 200)
            {
                MyCovenData.PendingInvites.RemoveAll(inv => inv.Character == playerId);
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(APIManager.ParseError(response));
            }
        });
    }

    public static void AcceptInvite(string covenId, System.Action<CovenInfo?, string> callback)
    {
        Debug.Log("accepting invite to join " + covenId);

        APIManager.Instance.Put("coven/acceptInvite/" + covenId, "{}", (response, result) =>
        {
            if (result == 200)
            {
                CovenInfo coven = JsonConvert.DeserializeObject<CovenInfo>(response);

                //update the player's coven
                PlayerDataManager.playerData.covenInfo = coven;

                //remove from invites
                PlayerDataManager.playerData.covenInvites.RemoveAll((invite) => invite.coven == covenId);

                callback?.Invoke(coven, null);
            }
            else
            {
                callback?.Invoke(null, APIManager.ParseError(response));
            }
        });
    }

    public static void DeclineInvite(string covenId, System.Action<string> callback)
    {
        Debug.Log("decline invite from coven " + covenId);

        APIManager.Instance.Put("coven/declineInvite/" + covenId, "{}", (response, result) =>
        {
            if (result == 200)
            {
                //remove from invites
                PlayerDataManager.playerData.covenInvites.RemoveAll(invite => invite.coven == covenId);
                callback(null);
            }
            else
            {
                callback(APIManager.ParseError(response));
            }
        });
    }
    
    public static void KickMember(TeamMemberData member, System.Action<string> callback)
    {
        Debug.Log("kicking " + member.Id + " from oven");
        
        APIManager.Instance.Put("coven/kick/" + member.Id, "{}", (response, result) =>
        {
            if (result == 200)
            {
                if (MyCovenData != null)
                    MyCovenData.Members.Remove(member);
                callback?.Invoke(null);
            }
            else
            {
                callback(APIManager.ParseError(response));
            }
        });        
    }

    public static void PromoteMember(TeamMemberData member, System.Action<string> callback)
    {
        Debug.Log("promoting " + member.Id + " to " + ((int)member.Role + 1));
        
        APIManager.Instance.Patch("coven/promote/" + member.Id, "{}", (response, responseCode) =>
        {
            if (responseCode == 200)
            {
                member.Role = member.Role + 1;
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(APIManager.ParseError(response));
            }
        });
    }

    public static void DemoteMember(TeamMemberData member, System.Action<string> callback)
    {
        Debug.Log("demoting " + member.Id + " to " + ((int)member.Role - 1));
        
        APIManager.Instance.Patch("coven/demote/" + member.Id, "{}", (response, responseCode) =>
        {
            if (responseCode == 200)
            {
                member.Role = member.Role - 1;
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(APIManager.ParseError(response));
            }
        });
    }

    public static void ChangeMemberTitle(TeamMemberData member, string title, System.Action<string> callback)
    {
        Debug.Log("giving the title \"" + title  + "\" to " + member.Id);

        callback("NOT IMPLEMENTED");
    }

    public static void LeaveCoven(System.Action<string> callback)
    {
        APIManager.Instance.Put("coven/leave", "{}", (response, result) =>
        {
            if (result == 200)
            {
                MyCovenData = null;
                PlayerDataManager.playerData.covenInfo = new CovenInfo
                {
                    coven = null,
                    joinedOn = 0,
                    role = 0
                };
                callback?.Invoke(null);
            }
            else
            {
                callback(APIManager.ParseError(response));
            }
        });
    }
}