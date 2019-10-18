using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerBound : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Subtitle;
    [SerializeField] private Image m_Glyph;

    public static void Show(string caster, double expire)
    {
        UIPlayerBound instance = Instantiate(Resources.Load<UIPlayerBound>("UIPlayerBound"));

        instance.m_Title.text = LocalizeLookUp.GetSpellName("spell_bind");
        instance.m_Subtitle.text = LocalizeLookUp.GetText("spell_bound_witch")
            .Replace("{{Caster}}", caster)
            .Replace("{{Time}}", Utilities.GetSummonTime(expire));

        DownloadedAssets.GetSprite("spell_bind", spr =>
        {
            instance.m_Glyph.overrideSprite = spr;
        });

        instance.gameObject.SetActive(true);
        Destroy(instance.gameObject, 3.5f);
    }
}
