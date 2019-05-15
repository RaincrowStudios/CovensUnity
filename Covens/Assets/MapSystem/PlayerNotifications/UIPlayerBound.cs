using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPlayerBound : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Subtitle;

    public static void Show(string caster)
    {
        UIPlayerBound instance = Instantiate(Resources.Load<UIPlayerBound>("UIPlayerBound"));
        instance.m_Title.text = "Bind";
        instance.m_Subtitle.text = "You have been bound by " + caster;
        instance.gameObject.SetActive(true);
        Destroy(instance.gameObject, 3.5f);
    }
}
