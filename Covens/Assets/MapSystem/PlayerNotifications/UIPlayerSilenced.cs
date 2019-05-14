using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIPlayerSilenced : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Subtitle;

    public static void Show(string caster)
    {
        UIPlayerSilenced instance = Instantiate(Resources.Load<UIPlayerSilenced>("UIPlayerSilenced"));
        instance.m_Title.text = "Silenced";
        instance.m_Subtitle.text = "You have been silenced by " + caster;
        instance.gameObject.SetActive(true);
        Destroy(instance.gameObject, 5f);
    }
}
