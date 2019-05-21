using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlayerPortrait : MonoBehaviour {

    private static LoadPlayerPortrait m_Instance;

    [SerializeField] private UnityEngine.UI.Image m_Image;

    private void Awake()
    {
        m_Instance = this;

        if (m_Image == null)
            m_Image = GetComponent<UnityEngine.UI.Image>();
    }

    private void OnEnable()
    {
        ReloadPortrait();
    }

    private IEnumerator WaitForPlayerdata()
    {
        while (PlayerDataManager.playerData == null || PlayerDataManager.playerData.equipped == null || PlayerDataManager.playerData.equipped.Count == 0)
        {
            yield return 60;
        }

        AvatarSpriteUtil.Instance.GenerateWardrobePortrait(spr =>
        {
            m_Image.overrideSprite = spr;
            LeanTween.value(0, 1, 1f)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                m_Image.color = new Color(1, 1, 1, t);
            });
        });
    }

    public static void ReloadPortrait()
    {
        m_Instance.m_Image.color = new Color(0, 0, 0, 0);
        m_Instance.StartCoroutine(m_Instance.WaitForPlayerdata());
    }
}
