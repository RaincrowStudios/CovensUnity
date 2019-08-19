using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Raincrow;

public class UIQuickCastPicker : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private Button m_CloseButton;

    [SerializeField] private Transform m_ItemPrefab;
    [SerializeField] private RectTransform m_WhiteContainer;
    [SerializeField] private RectTransform m_GreyContainer;
    [SerializeField] private RectTransform m_ShadowContainer;

    private static UIQuickCastPicker m_Instance;

    private bool m_SpellsSpawned = false;
    private System.Action<SpellData> m_OnSelectSpell;

    private static void Open(System.Action<SpellData> onClick)
    {
        if (m_Instance != null)
        {
            m_Instance.Show(onClick);
        }
        else
        {
            LoadingOverlay.Show();
            SceneManager.LoadSceneAsync(SceneManager.Scene.QUICKCAST_PICKER, 
                UnityEngine.SceneManagement.LoadSceneMode.Additive,
                null,
                () =>
                {
                    m_Instance.Show(onClick);
                    LoadingOverlay.Hide();
                });
        }
    }

    private void Awake()
    {
        m_Instance = this;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;

        DownloadedAssets.OnWillUnloadAssets += DownloadedAssets_OnWillUnloadAssets;
    }

    private void DownloadedAssets_OnWillUnloadAssets()
    {
        DownloadedAssets.OnWillUnloadAssets -= DownloadedAssets_OnWillUnloadAssets;        
        SceneManager.UnloadScene(SceneManager.Scene.QUICKCAST_PICKER, null, null);
    }

    private void Show(System.Action<SpellData> onClick)
    {
        if (!m_SpellsSpawned)
        {
            StartCoroutine(SpawnSpellsCoroutine());
        }

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
    }

    private void Hide()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
    }

    private void OnClickSpell(SpellData spell)
    {
        Hide();
        m_OnSelectSpell?.Invoke(spell);
        m_OnSelectSpell = null;
    }

    private IEnumerator SpawnSpellsCoroutine()
    {
        if (PlayerDataManager.playerData == null)
        {
            Debug.LogError("player data not initialized");
            yield break;
        }

        m_SpellsSpawned = true;

        foreach (SpellData spell in PlayerDataManager.playerData.Spells)
        {
            SetupSpell(spell);
            yield return 0;// new WaitForSeconds(0.1f);
        }
    }

    private void SetupSpell(SpellData spell)
    {
        Transform item = Instantiate(m_ItemPrefab);

        if (spell.school < 0)
            item.SetParent(m_ShadowContainer);
        else if (spell.school > 0)
            item.SetParent(m_WhiteContainer);
        else
            item.SetParent(m_GreyContainer);

        item.localScale = Vector3.one;

        Button button = item.GetComponent<Button>();
        TextMeshProUGUI title = item.GetComponentInChildren<TextMeshProUGUI>();
        Image icon = item.GetChild(1).GetComponent<Image>();

        button.onClick.AddListener(() => OnClickSpell(spell));
        title.text = spell.Name;
        DownloadedAssets.GetSprite(spell.id, icon);
    }

    [ContextMenu("Open")]
    private void DebugOpen()
    {
        Open(null);
    }

    [ContextMenu("Close")]
    private void DebugCLose()
    {
        Hide();
    }    
}
