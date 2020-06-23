using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static Raincrow.GameEventResponses.LevelUpHandler;
using Newtonsoft.Json;

public class UIDailyRewardDouble : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _closeButton;
    [SerializeField] private LayoutGroup _rewardContainer;
    [SerializeField] private UIDailyRewardDoubleReward _rewardPrefab;
    [Header("Double")]
    [SerializeField] private Image _iconDrach;
    [SerializeField] private TextMeshProUGUI _textPrice;
    [SerializeField] private Button _buttonBuyDouble;

    [Header("Sprites")]
    [SerializeField] private Sprite _spriteGolden;
    [SerializeField] private Sprite _spriteSilver;
    [SerializeField] private Sprite _spriteXP;
    [SerializeField] private Sprite _spriteEnergy;
    [SerializeField] private Sprite _spriteIngredient;
    [SerializeField] private Sprite _spriteElixir;

    private Color _noColor = new Vector4(0, 0, 0, 0);
    private List<UIDailyRewardDoubleReward> rewardItems;
    private DailyQuestRewards dailyQuestRewards;
    private void Awake()
    {
        _buttonBuyDouble.onClick.AddListener(OnClickDoubleReward);
        _closeButton.onClick.AddListener(Close);
        _canvasGroup.alpha = 0;
        _closeButton.interactable = false;
        _rewardPrefab.gameObject.SetActive(false);
    }

    public void Show(DailyQuestRewards data)
    {
        SoundManagerOneShot.Instance.PlayReward();
        PlayerDataManager.playerData.quest.completed = true;

        bool isGold = data.gold > 0;
        _textPrice.text = isGold ? data.gold.ToString() : data.silver.ToString();
        _iconDrach.sprite = isGold ? _spriteGolden : _spriteSilver;

        SpawnRewards(data.rewards);
        dailyQuestRewards = data;

        LeanTween.value(0, 0, 0).setDelay(1f).setOnComplete(() => _closeButton.interactable = true);

        LeanTween.alphaCanvas(_canvasGroup, 1, 0.7f)
            .setEaseOutCubic();

        BackButtonListener.AddCloseAction(Close);
    }

    private void SpawnRewards(DailyRewards[] data)
    {
        List<UIDailyRewardDoubleReward> rewardItems = new List<UIDailyRewardDoubleReward>();

        foreach (DailyRewards reward in data)
        {
            switch (reward.type)
            {
                case DailyRewards.DailyRewardsType.gold:

                    UIDailyRewardDoubleReward goldReward = Instantiate(_rewardPrefab, _rewardContainer.transform);

                    string textGold = "+{0} " + LocalizeLookUp.GetText("store_gold");
                    goldReward.Setup(textGold, reward.amount, _noColor, _spriteGolden);

                    rewardItems.Add(goldReward);
                    break;
                case DailyRewards.DailyRewardsType.silver:
                    UIDailyRewardDoubleReward silverReward = Instantiate(_rewardPrefab, _rewardContainer.transform);

                    string textSilver = "+{0} " + LocalizeLookUp.GetText("store_silver");
                    silverReward.Setup(textSilver, reward.amount, _noColor, _spriteSilver);

                    rewardItems.Add(silverReward);
                    break;
                case DailyRewards.DailyRewardsType.energy:
                    UIDailyRewardDoubleReward energyReward = Instantiate(_rewardPrefab, _rewardContainer.transform);

                    string textEnergy = "+{0} " + LocalizeLookUp.GetText("cast_energy");
                    energyReward.Setup(textEnergy, reward.amount, Utilities.Blue, _spriteEnergy);

                    rewardItems.Add(energyReward);
                    break;
                case DailyRewards.DailyRewardsType.xp:
                    UIDailyRewardDoubleReward xpReward = Instantiate(_rewardPrefab, _rewardContainer.transform);

                    string textXP = "+{0} XP";
                    xpReward.Setup(textXP, reward.amount, _noColor, _spriteXP);

                    rewardItems.Add(xpReward);
                    break;
                case DailyRewards.DailyRewardsType.ingredients:
                    UIDailyRewardDoubleReward ingredientReward = Instantiate(_rewardPrefab, _rewardContainer.transform);

                    string textIngredient = "+{0} " + LocalizeLookUp.GetText("store_ingredients");
                    ingredientReward.Setup(textIngredient, reward.amount, _noColor, _spriteIngredient);

                    rewardItems.Add(ingredientReward);
                    break;
                case DailyRewards.DailyRewardsType.consumables:

                    UIDailyRewardDoubleReward ingredientElixir = Instantiate(_rewardPrefab, _rewardContainer.transform);

                    DownloadedAssets.GetSprite(
                        reward.item,
                        spr =>
                        {
                            ingredientElixir.UpdateIcon(spr);
                        },
                        true
                    );

                    string textElixir = "+{0} " + LocalizeLookUp.GetStoreTitle(reward.item);
                    ingredientElixir.Setup(textElixir, reward.amount, Utilities.GetElixirColor(reward.item), _spriteIngredient);

                    rewardItems.Add(ingredientElixir);

                    break;
                case DailyRewards.DailyRewardsType.effect:
                    MarkerSpawner.ApplyStatusEffect(PlayerDataManager.playerData.instance, PlayerDataManager.playerData.instance, reward.effect);
                    break;

            }
        }

        this.rewardItems = rewardItems;
    }

    private void OnClickDoubleReward()
    {
        bool isGold = dailyQuestRewards.gold > 0;
        int price = isGold ? dailyQuestRewards.gold : dailyQuestRewards.silver;

        if ((isGold && PlayerDataManager.playerData.gold < price) || (!isGold && PlayerDataManager.playerData.silver < price))
        {
            UIGlobalPopup.ShowError(null, isGold ? LocalizeLookUp.GetText("store_not_enough_gold") : LocalizeLookUp.GetText("store_not_enough_silver"));
            return;
        }

        string text = LocalizeLookUp.GetText("popup_double_quest_confirm_double");
        string finalText = string.Format(text, price, LocalizeLookUp.GetText(isGold ? "store_gold" : "store_silver"));
        UIGlobalPopup.ShowPopUp(RequestDuplicate, null, finalText);

    }

    private void RequestDuplicate()
    {
        LoadingOverlay.Show();
        APIManager.Instance.Get("dailies/doublereward",
          (string result, int response) =>
          {
              LoadingOverlay.Hide();

              if (response == 200)
              {
                  DuplicatedRewards();
                  _buttonBuyDouble.gameObject.SetActive(false);
              }
              else
              {
                  APIManager.ParseError(result);
              }
          });
    }

    private void DuplicatedRewards()
    {

        if(dailyQuestRewards.gold > 0)
        {
            PlayerDataManager.playerData.gold -= dailyQuestRewards.gold;
        }
        else
        {
            PlayerDataManager.playerData.silver -= dailyQuestRewards.silver;
        }

        if (PlayerDataManager.Instance != null)
        {
            PlayerManagerUI.Instance.UpdateDrachs();
        }

        foreach (UIDailyRewardDoubleReward reward in rewardItems)
        {
            reward.DoubleItem();
        }
    }

    private void Close()
    {
        BackButtonListener.RemoveCloseAction();

        _closeButton.interactable = false;

        LeanTween.alphaCanvas(_canvasGroup, 0, 0.4f)
            .setEaseOutCubic()
            .setOnComplete(() =>
            {
                gameObject.SetActive(false);
                Destroy(this.gameObject);
            });
    }
}
