using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayerBanished : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Subtitle;

    public static void Show(string caster)
    {
        UIPlayerBanished instance = Instantiate(Resources.Load<UIPlayerBanished>("UIPlayerBanished"));
        instance.m_Title.text = "Banish";
        instance.m_Subtitle.text = "You have been banished by " + caster;
        instance.gameObject.SetActive(true);
        Destroy(instance.gameObject, 5f);
    }
}
