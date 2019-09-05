using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTFSpawn : MonoBehaviour
{
    [SerializeField] private GameObject m_FancyWhiteFlame;
    [SerializeField] private GameObject m_SavWeaken;

    private Dictionary<string, GameObject> m_AvailableFX;

    private void Awake()
    {
        m_FancyWhiteFlame.gameObject.SetActive(false);
        m_SavWeaken.gameObject.SetActive(false);

        m_AvailableFX = new Dictionary<string, GameObject>()
        {
            { "fancy_whiteFlame", m_FancyWhiteFlame },
            { "savWeaken", m_SavWeaken },
        };
    }

    public void Spawn(string id, float longitude, float latitude)
    {
        if (m_AvailableFX.ContainsKey(id) == false)
        {
            Debug.LogError("[FTFSpawn] " + id + " not found");
            return;
        }
        Debug.LogError(id + " " + longitude + " " + latitude);
        GameObject go = m_AvailableFX[id];
        Instantiate(go, MapsAPI.Instance.GetWorldPosition(longitude, latitude), Quaternion.identity).gameObject.SetActive(true);
    }
}
