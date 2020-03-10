using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Raincrow.BattleArena.Model;

namespace Raincrow.BattleArena.Views
{
    public class RewardsBatttleView : MonoBehaviour, IRewardsBatttleView
    {
        [SerializeField] private ObjectPool _objectPool;

        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _textTitle;
        [SerializeField] private TextMeshProUGUI _textSubtitle;
        [SerializeField] private CanvasGroup _groupPopup;

        [Header("Icons")]
        [SerializeField] private Sprite _iconGoldCoin;
        [SerializeField] private Sprite _iconSilverCoin;
        [SerializeField] private Sprite _iconXP;
        [SerializeField] private Sprite _iconCrafting;
        [SerializeField] private Sprite _iconGem;
        [SerializeField] private Sprite _iconHerb;

        [Header("Reward")]
        [SerializeField] private CanvasGroup _contentRewardsBasic;
        [SerializeField] private CanvasGroup _contentRewardsItems;
        [SerializeField] private WonRewardView _wonRewardPrefab;

        [Header("Animation")]
        [SerializeField] float _timeOpen;
        [SerializeField] float _intervalBetweenReward = 2f;
        [SerializeField] float _intervalBetweenScreens = 2f;
        [SerializeField] float _timeChangeScreen = 0.25f;

        IBattleRewardModel _battleReward;
        Coroutine _screenReciveRewards;
        public IRewardsBatttleView Show(string title, string subtitle, IBattleRewardModel battleReward)
        {
            _textTitle.text = title;
            _textSubtitle.text = subtitle;

            _battleReward = battleReward;

            gameObject.SetActive(true);
            LeanTween.alphaCanvas(_groupPopup, 1, _timeOpen).setOnComplete(() =>
            {
                _screenReciveRewards = StartCoroutine(ReciveRewards());
            });

            return this;
        }

        public Coroutine WaitEndScreen()
        {
            return _screenReciveRewards;
        }

        private IEnumerator ReciveRewards()
        {
            WonRewardView wonRewardGold = _objectPool.Spawn(_wonRewardPrefab, _contentRewardsBasic.transform, false);
            WonRewardView wonRewardSilver = _objectPool.Spawn(_wonRewardPrefab, _contentRewardsBasic.transform, false);
            WonRewardView wonRewardXP = _objectPool.Spawn(_wonRewardPrefab, _contentRewardsBasic.transform, false);

            wonRewardGold.InitVariables(string.Format("+{0} Gold Drach(s)", _battleReward.GoldCurrency), _iconGoldCoin).Show();
            yield return new WaitForSeconds(_intervalBetweenReward);
            wonRewardSilver.InitVariables(string.Format("+{0} Silver Drach(s)", _battleReward.SilverCurrency), _iconSilverCoin).Show();
            yield return new WaitForSeconds(_intervalBetweenReward);
            wonRewardXP.InitVariables(string.Format("+{0} XP", _battleReward.Experience), _iconXP).Show();

            yield return new WaitForSeconds(_intervalBetweenScreens);

            if(_battleReward.Tools.Length > 0 || _battleReward.Gems.Length > 0 || _battleReward.Herbs.Length > 0)
            {
                yield return new WaitForSeconds(LeanTween.alphaCanvas(_contentRewardsBasic, 0, _timeChangeScreen).time);

                List<WonRewardView> wonRewards = new List<WonRewardView>();
                foreach (InventoryItemModel tool in _battleReward.Tools)
                {
                    WonRewardView reward = _objectPool.Spawn(_wonRewardPrefab, _contentRewardsItems.transform, false);
                    reward.InitVariables(string.Format("{0} {1}x", tool.Name, tool.Count), _iconCrafting);
                    wonRewards.Add(reward);
                }

                foreach (InventoryItemModel gem in _battleReward.Gems)
                {
                    WonRewardView reward = _objectPool.Spawn(_wonRewardPrefab, _contentRewardsItems.transform, false);
                    reward.InitVariables(string.Format("{0} {1}x", gem.Name, gem.Count), _iconGem);
                    wonRewards.Add(reward);
                }

                foreach (InventoryItemModel herb in _battleReward.Herbs)
                {
                    WonRewardView reward = _objectPool.Spawn(_wonRewardPrefab, _contentRewardsItems.transform, false);
                    reward.InitVariables(string.Format("{0} {1}x", herb.Name, herb.Count), _iconHerb);
                    wonRewards.Add(reward);
                }

                foreach (WonRewardView reward in wonRewards)
                {
                    reward.Show();
                    yield return new WaitForSeconds(_intervalBetweenReward);
                }
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }

    public interface IRewardsBatttleView
    {
        IRewardsBatttleView Show(string title, string subtitle, IBattleRewardModel battleReward);
        void Hide();
        Coroutine WaitEndScreen();
    }
}