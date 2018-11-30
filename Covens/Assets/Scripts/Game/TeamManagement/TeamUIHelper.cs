using System.Collections.Generic;
using UnityEngine;

public class TeamUIHelper : MonoBehaviour
{
    public static TeamUIHelper Instance { get; set; }

    public GameObject memberPrefab;
    public GameObject requestPrefab;
    public GameObject cancelPrefab;
    public GameObject allyPrefab;
    public GameObject unallyPrefab;

    public Transform container;
    void Awake()
    {
        Instance = this;
    }

    public void CreateMembers(List<TeamMembers> members)
    {
        foreach (var item in members)
        {
            var tData = Utilities.InstantiateObject(memberPrefab, container);
        }
    }
}