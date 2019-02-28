using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISignatureUnlocked : MonoBehaviour
{
    private static UISignatureUnlocked m_Instance;
    public static UISignatureUnlocked Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UISignatureUnlocked>("UISignatureUnlocked"));
            return m_Instance;
        }
    }

    [SerializeField] private Text m_SpellName;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void Show(WSData data)
    {
        SoundManagerOneShot.Instance.PlayCrit();
        gameObject.SetActive(true);
    }
}
