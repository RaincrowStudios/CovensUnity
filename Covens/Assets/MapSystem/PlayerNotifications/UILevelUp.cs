using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Raincrow.GameEventResponses.LevelUpHandler;

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

    public void Show(LevelUpEventData data)
    {
        m_LevelText.text = LocalizeLookUp.GetText("leveled_number") + ": " + PlayerDataManager.playerData.level;

        StartCoroutine(SpawnRewards(data));

        SoundManagerOneShot.Instance.PlayLevel();
        SoundManagerOneShot.Instance.IngredientAdded();

        m_CloseButton.interactable = false;
        LeanTween.value(0, 0, 0).setDelay(0.25f).setOnStart(() => { m_CloseButton.interactable = true; });

        LeanTween.alphaCanvas(m_CanvasGroup, 1, 0.7f)
            .setEaseOutCubic();
    }

    private IEnumerator SpawnRewards(LevelUpEventData data)
    {
        List<UILevelUpReward> rewardItems = new List<UILevelUpReward>();

        UILevelUpReward silverReward = Instantiate(m_RewardPrefab, m_RewardContainer.transform);
        silverReward.SetupSilver(data.silver);
        silverReward.gameObject.SetActive(true);
        rewardItems.Add(silverReward);

        foreach (var spell in PlayerDataManager.playerData.UnlockedSpells)
        {
            if (spell.level == PlayerDataManager.playerData.level)
            {
                UILevelUpReward item = Instantiate(m_RewardPrefab, m_RewardContainer.transform);
                item.SetupSpell(spell.id);
                item.gameObject.SetActive(true);
                rewardItems.Add(item);
            }
        }

        yield return new WaitForSeconds(0.5f);

        int left = rewardItems.Count / 2 - 1;
        int right = left + 1;

        //center first
        if (rewardItems.Count % 2 == 1)
        {
            rewardItems[right].Show();
            right++;
            yield return new WaitForSeconds(0.45f);
        }

        //setup from center to borders
        while (left >= 0 || right < rewardItems.Count)
        {
            if (left >= 0)
            {
                rewardItems[left].Show();
                left--;
            }
            if (right < rewardItems.Count)
            {
                rewardItems[right].Show();
                right++;
            }
            yield return new WaitForSeconds(0.5f);
        }

        silverReward.SetupSilver(data.silver);
    }

    private void Close()
    {
        m_Instance = null;
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
