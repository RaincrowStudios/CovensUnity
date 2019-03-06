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

        m_Image.color = new Color(0, 0, 0, 0);

        ReloadPortrait();
    }

    private IEnumerator WaitForPlayerdata()
    {
        while (PlayerDataManager.playerData == null)
        {
            yield return 60;
        }

        AvatarSpriteUtil.Instance.GenerateWardrobePortrait(spr =>
        {
            m_Image.sprite = spr;
            m_Image.color = new Color(1, 1, 1, 1);
        });
    }

    public static void ReloadPortrait()
    {
        m_Instance.StartCoroutine(m_Instance.WaitForPlayerdata());
    }
}
