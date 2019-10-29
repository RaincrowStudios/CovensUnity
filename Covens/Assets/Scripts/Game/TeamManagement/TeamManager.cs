using Newtonsoft.Json;
using Raincrow.Team;
using System.Collections.Generic;
using UnityEngine;

public static class TeamManager
{    
    public static System.Action<string, string> OnJoinCoven;
    public static System.Action OnLeaveCoven;

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

                if (MyCovenData == null)
                {
                    //cache the players coven data
                    if (string.IsNullOrEmpty(MyCovenId) == false && teamData.Id == MyCovenId)
                        MyCovenData = teamData;
                }

                callback?.Invoke(teamData, null);
            }
            else
            {
                //return the error
                callback?.Invoke(null, response);
            }
        });
    }

    public static void CreateCoven(string covenName, System.Action<TeamData, string> callback)
    {
        Debug.Log("creating the coven \"" + covenName + "\"");

        string data = $"{{\"name\":\"{covenName}\"}}";
        
        APIManager.Instance.Post("coven", data, (response, responseCode) =>
        {
            if (responseCode == 200)
            {
                TeamData covenData = JsonUtility.FromJson<TeamData>(response);

                MyCovenData = covenData;
                PlayerDataManager.playerData.covenInfo = new CovenInfo
                {
                    coven = covenData.Id,
                    name = covenData.Name,
                    role =  (int)covenData.Members[0].Role,
                    joinedOn = covenData.CreatedOn
                };

                OnJoinCoven?.Invoke(covenData.Id, covenData.Name);
                callback(covenData, null);
            }
            else
            {
                callback(null, response);
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
                    PlayerDataManager.playerData.covenInfo = new CovenInfo();
                }
                OnLeaveCoven?.Invoke();
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
                callback?.Invoke(null, response);
            }
        });
    }

    public static void AcceptRequest(string playerId, System.Action<string> callback)
    {
        Debug.Log("accepting " + playerId + " into the coven");

        APIManager.Instance.Put("coven/acceptRequest/" + playerId, "{}", (response, result) =>
        {
            if (result == 200)
            {
                TeamMemberData newMember = JsonUtility.FromJson<TeamMemberData>(response);

                if (MyCovenData != null)
                {
                    //updat the coven's members
                    MyCovenData.Members.RemoveAll(member => member.Id == newMember.Id);
                    MyCovenData.Members.Add(newMember);
                    //update the requests
                    MyCovenData.PendingRequests.RemoveAll(req => req.Character == playerId);
                }

                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(response);
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
                if (MyCovenData != null)
                    MyCovenData.PendingRequests.RemoveAll(req => req.Character == playerId);
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(response);
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
                if (MyCovenData != null)
                    MyCovenData.PendingInvites.Add(pendingInvite);
                callback?.Invoke(pendingInvite, null);
            }
            else
            {
                callback?.Invoke(null, response);
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
                if (MyCovenData != null)
                    MyCovenData.PendingInvites.RemoveAll(inv => inv.Character == playerId);
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(response);
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

                OnJoinCoven?.Invoke(coven.coven, coven.name);
                callback?.Invoke(coven, null);
            }
            else
            {
                callback?.Invoke(null, response);
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
                callback(response);
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
                callback(response);
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
                callback?.Invoke(response);
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
                callback?.Invoke(response);
            }
        });
    }

    public static void ChangeMemberTitle(TeamMemberData member, string title, System.Action<string> callback)
    {
        Debug.Log("changing " + member.Name + "'s title to \"" + title + "\"");

        if (title == member.Title)
        {
            callback?.Invoke(null);
            return;
        }

        string data = $"{{\"targetId\":\"{member.Id}\",\"title\":\"{title}\"}}";
        APIManager.Instance.Patch("coven/title", data, (response, result) =>
        {
            if (result == 200)
            {
                member.Title = title;
                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(response);
            }
        });
    }

    public static void LeaveCoven(System.Action<string> callback)
    {
        APIManager.Instance.Put("coven/leave", "{}", (response, result) =>
        {
            if (result == 200)
            {
                MyCovenData = null;
                PlayerDataManager.playerData.covenInfo = new CovenInfo();
                OnLeaveCoven?.Invoke();
                callback?.Invoke(null);
            }
            else
            {
                callback(response);
            }
        });
    }

    public static void ChangeMotto(string motto, System.Action<string> callback)
    {
        Debug.Log("changing motto to \"" + motto + "\"");
        string data = $"{{\"motto\":\"{motto}\"}}";
        APIManager.Instance.Patch("coven/motto", data, (response, result) =>
        {
            if (result == 200)
            {
                if (MyCovenData != null)
                    MyCovenData.Motto = motto;

                callback?.Invoke(null);
            }
            else
            {
                callback?.Invoke(response);
            }
        });
    }
}