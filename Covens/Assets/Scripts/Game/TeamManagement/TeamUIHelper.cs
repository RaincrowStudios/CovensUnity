using System.Collections.Generic;
using UnityEngine;

public class TeamUIHelper : MonoBehaviour
{
    public static TeamUIHelper Instance { get; set; }

    public GameObject memberPrefab;
    public GameObject requestInvitePrefab;
    public GameObject allyPrefab;
    public GameObject unallyPrefab;
    public GameObject emptyPrefab;
    public GameObject requestPrefab;
    public GameObject locationPrefab;

    public Transform container;
    public Dictionary<string, TeamItemData> uiItems;

    void Awake()
    {
        Instance = this;
    }

    public void clearContainer()
    {
        uiItems = new Dictionary<string, TeamItemData>();
        foreach (Transform t in container)
        {
            Destroy(t.gameObject);
        }
    }

    public void CreateMembers(List<TeamMember> members)
    {
        clearContainer();
        for (int i = 0; i < members.Count; i++)
        {
            var tData = Utilities.InstantiateObject(memberPrefab, container).GetComponent<TeamItemData>();
            tData.Setup(members[i]);
            tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
            uiItems.Add(members[i].displayName, tData);
        }
    }

    public void CreateInvites(TeamInvites[] invites)
    {
        clearContainer();
        if (invites.Length > 0)
        {
            for (int i = 0; i < invites.Length; i++)
            {
                var tData = Utilities.InstantiateObject(requestPrefab, container).GetComponent<TeamItemData>();
                tData.Setup(invites[i]);
                tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);

                if (string.IsNullOrEmpty(invites[i].covenName))
                    uiItems.Add(invites[i].displayName, tData);
                else
                    uiItems.Add(invites[i].covenName, tData);
            }
        }
        else
        {
            var tData = Utilities.InstantiateObject(emptyPrefab, container);
        }
    }

    public void CreateRequests(TeamInviteRequest[] requests)
    {
        clearContainer();
        if (requests.Length > 0)
        {
            for (int i = 0; i < requests.Length; i++)
            {
                var tData = Utilities.InstantiateObject(requestPrefab, container).GetComponent<TeamItemData>();
                tData.Setup(requests[i]);
                tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
                uiItems.Add(requests[i].displayName, tData);
            }
        }
        else
        {
            var tData = Utilities.InstantiateObject(emptyPrefab, container);
        }
    }

    public void CreateAllies(TeamAlly[] invites)
    {
        clearContainer();
        if (invites.Length > 0)
        {
            for (int i = 0; i < invites.Length; i++)
            {
                var tData = Utilities.InstantiateObject(allyPrefab, container).GetComponent<TeamItemData>();
                tData.SetupAlly(invites[i]);
                tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
                uiItems.Add(invites[i].covenName, tData);
            }
        }
        else
        {
            var tData = Utilities.InstantiateObject(emptyPrefab, container);
        }
    }

    public void CreateAllied(TeamAlly[] invites)
    {
        clearContainer();
        if (invites.Length > 0)
        {
            for (int i = 0; i < invites.Length; i++)
            {
                var tData = Utilities.InstantiateObject(allyPrefab, container).GetComponent<TeamItemData>();
                tData.SetupAllied(invites[i]);
                tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
                uiItems.Add(invites[i].covenName, tData);
            }
        }
        else
        {
            var tData = Utilities.InstantiateObject(emptyPrefab, container);
        }
    }

    public void CreateLocations(TeamLocation[] locations)
    {
        clearContainer();
        if (locations.Length > 0)
        {
            for (int i = 0; i < locations.Length; i++)
            {
                var tData = Utilities.InstantiateObject(locationPrefab, container).GetComponent<TeamItemData>();
                tData.Setup(locations[i]);
                tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
                uiItems.Add(locations[i].displayName, tData);
            }
        }
        else
        {
            var tData = Utilities.InstantiateObject(emptyPrefab, container);
        }
    }
}