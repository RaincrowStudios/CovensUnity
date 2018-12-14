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

    public Transform container;

    public List<TeamInviteRequest> lastRequests = null; //cached requests used after accepting/declining a invitation-request
    public List<TeamInvites> lastInvites = null;

    void Awake()
    {
        Instance = this;
    }

    void clearContainer()
    {
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
            var tData = Utilities.InstantiateObject(memberPrefab, container);
            tData.GetComponent<TeamItemData>().Setup(members[i]);
            tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
        }

    }

    public void CreateInvites(TeamInvites[] invites)
    {
        lastInvites = new List<TeamInvites>(invites);
        clearContainer();
        if (invites.Length > 0)
        {
            for (int i = 0; i < invites.Length; i++)
            {
                var tData = Utilities.InstantiateObject(requestPrefab, container);
                tData.GetComponent<TeamItemData>().Setup(invites[i]);
                tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
            }
        }
        else
        {
            var tData = Utilities.InstantiateObject(emptyPrefab, container);
        }
    }

    public void CreateRequests(TeamInviteRequest[] requests)
    {
        lastRequests = new List<TeamInviteRequest>(requests);
        clearContainer();
        if (requests.Length > 0)
        {
            for (int i = 0; i < requests.Length; i++)
            {
                var tData = Utilities.InstantiateObject(requestPrefab, container);
                tData.GetComponent<TeamItemData>().Setup(requests[i]);
                tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
            }
        }
        else
        {
            var tData = Utilities.InstantiateObject(emptyPrefab, container);
        }
    }

    public void CreateAllied(TeamInvites[] invites)
    {
        clearContainer();
        if (invites.Length > 0)
        {
            for (int i = 0; i < invites.Length; i++)
            {
                // var tData = Utilities.InstantiateObject(memberPrefab, container);
                // tData.GetComponent<TeamItemData>().Setup(invites[i]);
                // tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
            }
        }
        else
        {
            var tData = Utilities.InstantiateObject(emptyPrefab, container);
        }
    }

    public void CreateCovenAllied(TeamInvites[] invites)
    {
        clearContainer();
        if (invites.Length > 0)
        {
            for (int i = 0; i < invites.Length; i++)
            {
                // var tData = Utilities.InstantiateObject(memberPrefab, container);
                // tData.GetComponent<TeamItemData>().Setup(invites[i]);
                // tData.transform.GetChild(0).gameObject.SetActive(i % 2 == 0);
            }
        }
        else
        {
            var tData = Utilities.InstantiateObject(emptyPrefab, container);
        }
    }
}