using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class LoadPlayerPortrait : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image m_Image;

    private static LoadPlayerPortrait m_Instance;

    private Sprite m_LastGeneratedSprite = null;

    private void Awake()
    {
        m_Instance = this;

        if (m_Image == null)
            m_Image = GetComponent<UnityEngine.UI.Image>();

        m_Instance.m_Image.color = new Color(0, 0, 0, 0);
    }

    private void OnEnable()
    {
        ReloadPortrait();
    }

    private IEnumerator WaitForPlayerdata()
    {
        while (PlayerDataManager.playerData == null || PlayerDataManager.playerData.equipped == null || PlayerDataManager.playerData.equipped.Count == 0)
            yield return null;

        while (AvatarSpriteUtil.Instance == null)
            yield return null;

        //hide current portrait
        LeanTween.value(m_Image.color.a, 0, m_Image.color.a/2f)
                .setOnUpdate((float t) => m_Image.color = new Color(1, 1, 1, t));

        yield return new WaitForSeconds(0.5f);

        AvatarSpriteUtil.Instance.GenerateWardrobePortrait(spr =>
        {
            //remove old sprite from memory
            if (m_LastGeneratedSprite != null)
            {
                Destroy(m_LastGeneratedSprite.texture);
                Destroy(m_LastGeneratedSprite);
            }
            
            m_LastGeneratedSprite = m_Image.overrideSprite = spr;

            //show the portrait
            LeanTween.value(0, 1, 1f)
                .setEaseOutCubic()
                .setOnUpdate((float t) => m_Image.color = new Color(1, 1, 1, t));
        });
    }

    public static void ReloadPortrait()
    {
        m_Instance.StartCoroutine(m_Instance.WaitForPlayerdata());
    }
}
