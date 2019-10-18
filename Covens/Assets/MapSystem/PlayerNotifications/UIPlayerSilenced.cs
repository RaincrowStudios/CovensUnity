using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlayerSilenced : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private TextMeshProUGUI m_Subtitle;
    [SerializeField] private Image m_Glyph;

    public static void Show(string caster, double expire)
    {
        UIPlayerSilenced instance = Instantiate(Resources.Load<UIPlayerSilenced>("UIPlayerSilenced"));
        instance.m_Title.text = LocalizeLookUp.GetSpellName("spell_silence");
        instance.m_Subtitle.text = LocalizeLookUp.GetText("spell_silenced_witch")
            .Replace("{{Caster}}", caster)
            .Replace("{{Time}}", Utilities.GetSummonTime(expire));

        DownloadedAssets.GetSprite("spell_silence", spr =>
        {
            instance.m_Glyph.overrideSprite = spr;
        });

        instance.gameObject.SetActive(true);
        Destroy(instance.gameObject, 5f);
    }
}
