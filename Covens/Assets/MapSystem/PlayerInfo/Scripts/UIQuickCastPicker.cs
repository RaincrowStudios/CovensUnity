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
    private CanvasGroup UIQCPCanvasGroup;
    
    private string m_SelectedSpell = null;
    private List<CanvasGroup> m_SpawnedItems = new List<CanvasGroup>();
    private System.Action<string> m_OnSelectSpell;
    private System.Action m_OnClose;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_CloseButton.onClick.AddListener(Hide);
        m_ItemPrefab.gameObject.SetActive(false);
        UIQCPCanvasGroup = this.GetComponent<CanvasGroup>();
        UIQCPCanvasGroup.alpha = 0f;
    }

    public void Show(string selected, System.Action<string> onSelect, System.Action onClose)
    {
        m_OnSelectSpell = onSelect;
        m_OnClose = onClose;

        if (IsOpen)
            return;

        IsOpen = true;
        StopAllCoroutines();
        StartCoroutine(SpawnSpellsCoroutine());

        m_Canvas.enabled = true;
        LeanTween.alphaCanvas(UIQCPCanvasGroup, 1f, 0.6f).setEaseOutCubic().setOnComplete(() =>
        {
            m_InputRaycaster.enabled = true;
        });
    }

    private void Hide()
    {
        IsOpen = false;
        m_OnClose?.Invoke();
        m_OnClose = null;
        StopAllCoroutines();

        LeanTween.alphaCanvas(UIQCPCanvasGroup, 0f, 0.4f).setEaseInCubic().setOnComplete(() =>
        {
            m_Canvas.enabled = false;
        });
        m_InputRaycaster.enabled = false;
    }

    private void OnClickSpell(string spell)
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
        
        foreach (CanvasGroup t in m_SpawnedItems)
            t.gameObject.SetActive(false);

        foreach (SpellData spell in PlayerDataManager.playerData.UnlockedSpells)
        {
            SetupSpell(spell);
            yield return 0;// new WaitForSeconds(0.1f);
        }
    }

    private void SetupSpell(SpellData spell)
    {
        Transform item = null;
        for (int i = 0; i < m_SpawnedItems.Count; i++)
        {
            if (m_SpawnedItems[i].gameObject.activeSelf == false)
            {
                item = m_SpawnedItems[i].transform;
                break;
            }
        }

        if (item == null)
        {
            item = Instantiate(m_ItemPrefab);
            m_SpawnedItems.Add(item.GetComponent<CanvasGroup>());
        }

        if (spell.school < 0)
            item.SetParent(m_ShadowContainer);
        else if (spell.school > 0)
            item.SetParent(m_WhiteContainer);
        else
            item.SetParent(m_GreyContainer);

        item.localScale = Vector3.one;

        CanvasGroup cg = item.GetComponent<CanvasGroup>();
        Button button = item.GetComponent<Button>();
        TextMeshProUGUI title = item.GetComponentInChildren<TextMeshProUGUI>();
        Image icon = item.GetChild(0).GetChild(0).GetComponent<Image>();

        button.onClick.AddListener(() => OnClickSpell(spell.id));
        title.text = spell.Name;
        cg.alpha = 0;
        DownloadedAssets.GetSprite(spell.id, spr =>
        {
            icon.overrideSprite = spr;
            LeanTween.alphaCanvas(cg, 1f, 1f);
        });
        item.gameObject.SetActive(true);
    }
}
