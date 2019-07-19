using UnityEngine;

namespace Raincrow.Team
{
    public class TeamManagerRequestHandler
    {
        private const int HttpRequestCodeSuccess = 200;

        public static void GetCoven(string covenId, System.Action<TeamData, int> requestResponse)
        {
            string endpoint = string.Concat("coven/", covenId);
            APIManager.Instance.Get(endpoint, (covenData, responseCode) =>
            {
                if (responseCode == HttpRequestCodeSuccess)
                {
                    TeamData teamData = JsonUtility.FromJson<TeamData>(covenData);
                    requestResponse?.Invoke(teamData, responseCode);
                }
                else
                {
                    requestResponse?.Invoke(null, responseCode);
                }
            });
        }

        [System.Serializable]
        private class CreateCovenRequest
        {
            public string name;
        }

        public static void CreateCoven(string covenName, System.Action<TeamData, int> requestResponse)
        {
            CreateCovenRequest request = new CreateCovenRequest
            {
                name = covenName
            };

            string data = JsonUtility.ToJson(request);

            APIManager.Instance.Post("coven/", data, (response, responseCode) =>
            {
                if (responseCode == HttpRequestCodeSuccess)
                {
                    TeamData teamData = JsonUtility.FromJson<TeamData>(response);
                    requestResponse?.Invoke(teamData, responseCode);
                }
                else
                {
                    requestResponse?.Invoke(null, responseCode);
                }
            });
        }

        [System.Serializable]
        private class PromoteMemberRequest
        {
            public string promotedId;
            public int role;
        }

        public static void PromoteMember(string teamMemberId, int memberRole, System.Action<int> requestResponse)
        {
            PromoteMemberRequest request = new PromoteMemberRequest
            {
                promotedId = teamMemberId,
                role = memberRole
            };

            string data = JsonUtility.ToJson(request);
            APIManager.Instance.Patch("coven/promote", data, (response, responseCode) =>
            {
                requestResponse?.Invoke(responseCode);
            });
        }

        [System.Serializable]
        private class DemoteMemberRequest
        {
            public string demotedId;
            public int role;
        }

        public static void DemoteMember(string teamMemberId, int memberRole, System.Action<int> requestResponse)
        {
            DemoteMemberRequest request = new DemoteMemberRequest
            {
                demotedId = teamMemberId,
                role = memberRole
            };

            string data = JsonUtility.ToJson(request);
            APIManager.Instance.Patch("coven/demote", data, (response, responseCode) =>
            {
                requestResponse?.Invoke(responseCode);
            });
        }
    }
}
