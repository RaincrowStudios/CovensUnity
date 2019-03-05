using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadPlayerPortrait : MonoBehaviour {

    [SerializeField] private UnityEngine.UI.Image m_Image;

    private void Awake()
    {
        if (m_Image == null)
            m_Image = GetComponent<UnityEngine.UI.Image>();

        m_Image.color = new Color(0, 0, 0, 0);

        StartCoroutine(WaitForPlayerdata());
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
}
