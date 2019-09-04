using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UILevelUp : MonoBehaviour
{
    [SerializeField] private CanvasGroup m_CanvasGroup;
    [SerializeField] private TextMeshProUGUI m_LevelText;
    [SerializeField] private Button m_CloseButton;
    [Space(10)]
    [SerializeField] private LayoutGroup m_RewardContainer;
    [SerializeField] private UILevelUpReward m_RewardPrefab;


    private static UILevelUp m_Instance;
    public static UILevelUp Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UILevelUp>("UILevelUp"));
            return m_Instance;
        }
    }

    private void Awake()
    {
        m_CloseButton.onClick.AddListener(Close);
        m_CanvasGroup.alpha = 0;
        m_CloseButton.interactable = false;
        m_RewardPrefab.gameObject.SetActive(false);
    }

    public void Show()
    {
        m_LevelText.text = LocalizeLookUp.GetText("leveled_number") + ": " + PlayerDataManager.playerData.level;

        StartCoroutine(SpawnRewards());

        SoundManagerOneShot.Instance.PlayLevel();
        
        m_CloseButton.interactable = false;
        LeanTween.value(0, 0, 0).setDelay(0.25f).setOnStart(() => { m_CloseButton.interactable = true; });

        LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.7f)
            .setEaseOutCubic();
    }

    private IEnumerator SpawnRewards()
    {
        List<SpellData> unlockedSpells = new List<SpellData>();
        List<UILevelUpReward> rewardItems = new List<UILevelUpReward>();
        foreach (var spell in PlayerDataManager.playerData.UnlockedSpells)
        {
            if (spell.level == PlayerDataManager.playerData.level)
            {
                unlockedSpells.Add(spell);
                rewardItems.Add(Instantiate(m_RewardPrefab, m_RewardContainer.transform));
                rewardItems[rewardItems.Count - 1].gameObject.SetActive(true);
            }
        }

        yield return new WaitForSeconds(0.5f);

        int left = unlockedSpells.Count / 2-1;
        int right = left + 1;

        //center first
        if (unlockedSpells.Count % 2 == 1)
        {
            rewardItems[right].SetupSpell(unlockedSpells[right].id);
            right++;
            yield return new WaitForSeconds(0.45f);
        }

        //setup from center to borders
        while (left >= 0 || right < unlockedSpells.Count)
        {
            if (left >= 0)
            {
                rewardItems[left].SetupSpell(unlockedSpells[left].id);
                left--;
            }
            if (right < unlockedSpells.Count)
            {
                rewardItems[right].SetupSpell(unlockedSpells[right].id);
                right++;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Close()
    {
        m_CloseButton.interactable = false;

        LeanTween.alphaCanvas(m_CanvasGroup, 0, 0.4f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                gameObject.SetActive(false);
                Destroy(this.gameObject);
            });
    }
}
