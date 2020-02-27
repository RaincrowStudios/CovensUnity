using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.View;
using Raincrow.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.UI
{
    public class CharacterOrderUI : MonoBehaviour, ICharacterOrderView
    {
        [SerializeField] private ServiceLocator _serviceLocator;

        [Header("Prefabs UI")]
        [SerializeField] private CharacterPositionView _characterUI;
        [SerializeField] private Image _actionUI;

        [Header("Points UI")]
        [SerializeField] private Sprite m_EmptyPoint;
        [SerializeField] private Sprite m_FilledPoint;

        [Header("Roots")]
        [SerializeField] private Transform m_CharactersRoot;
        [SerializeField] private Transform m_ActionsRoot;

        // private variables     
        private ISpiritPortraitFactory spiritPortraitFactory;
        private IWitchPortraitFactory witchPortraitFactory;
        private ObjectPool objectPool;

        private List<CharacterPositionView> characterPositions = new List<CharacterPositionView>();
        private List<Image> actionsPoints = new List<Image>();

        private Dictionary<string, IWitchModel> _dicWitches = new Dictionary<string, IWitchModel>(); // List with all witches
        private Dictionary<string, ISpiritModel> _dicSpirits = new Dictionary<string, ISpiritModel>(); // List with all spirits

        public IEnumerator Show(string[] planningOrder, int maxActionsAllowed, IList<IWitchModel> witchModels, IList<ISpiritModel> spiritModels)
        {
            gameObject.SetActive(true);

            if (spiritPortraitFactory == null)
            {
                spiritPortraitFactory = _serviceLocator.GetSpiritPortraitFactory();
            }

            if (witchPortraitFactory == null)
            {
                witchPortraitFactory = _serviceLocator.GetWitchPortraitFactory();
            }

            if (objectPool == null)
            {
                objectPool = _serviceLocator.GetObjectPool();
            }

            _dicWitches.Clear();
            _dicSpirits.Clear();

            foreach (IWitchModel witch in witchModels)
            {
                _dicWitches.Add(witch.Id, witch);
            }

            foreach (ISpiritModel spirit in spiritModels)
            {
                _dicSpirits.Add(spirit.Id, spirit);
            }

            yield return StartCoroutine(CreateOrder(planningOrder));
            yield return StartCoroutine(CreateActionsPoints(maxActionsAllowed));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private IEnumerator CreateOrder(string[] characters)
        {
            foreach (string characterID in characters)
            {
                Coroutine<Sprite> coroutine = null;
                Color alignmentColor = Color.clear;

                if (_dicWitches.TryGetValue(characterID, out IWitchModel witch))
                {
                    alignmentColor = witch.GetAlignmentColor();
                    coroutine = this.StartCoroutine<Sprite>(GetWitchPortraitAvatar(witch));
                    while (coroutine.keepWaiting)
                    {
                        yield return null;
                    }

                }
                else if (_dicSpirits.TryGetValue(characterID, out ISpiritModel spirit))
                {
                    alignmentColor = spirit.GetAlignmentColor();
                    coroutine = this.StartCoroutine<Sprite>(GetSpiritPortraitAvatar(spirit));
                    while (coroutine.keepWaiting)
                    {
                        yield return null;
                    }
                }

                if (coroutine != null && coroutine.ReturnValue != null)
                {
                    CharacterPositionView characterPosition = objectPool.Spawn(_characterUI, m_CharactersRoot, false);
                    characterPosition.Init(coroutine.ReturnValue, alignmentColor);
                    characterPositions.Add(characterPosition);
                }
            }

            yield return null;
        }

        private IEnumerator<Sprite> GetWitchPortraitAvatar(IWitchModel witchModel)
        {
            IEnumerator<SpriteRequest> enumerator = witchPortraitFactory.CreateIWitchPortrait(witchModel);
            Coroutine<SpriteRequest> coroutine = this.StartCoroutine<SpriteRequest>(enumerator);
            while (coroutine.keepWaiting)
            {
                yield return null;
            }

            Sprite avatarSprite = coroutine.ReturnValue.Sprite;
            yield return avatarSprite;
        }

        private IEnumerator<Sprite> GetSpiritPortraitAvatar(ISpiritModel spiritModel)
        {
            IEnumerator<SpriteRequest> enumerator = spiritPortraitFactory.CreateSpiritPortrait(spiritModel);
            Coroutine<SpriteRequest> coroutine = this.StartCoroutine<SpriteRequest>(enumerator);
            while (coroutine.keepWaiting)
            {
                yield return null;
            }

            Sprite avatarSprite = coroutine.ReturnValue.Sprite;
            yield return avatarSprite;
        }

        private IEnumerator CreateActionsPoints(int numActions)
        {
            for (int i = 0; i < numActions; i++)
            {
                Image point = objectPool.Spawn(_actionUI, m_ActionsRoot, false);
                yield return point;
                actionsPoints.Add(point);
            }

            UpdateActionsPoints(2);
        }

        private void UpdateActionsPoints(int actionsUsed)
        {
            foreach (Image point in actionsPoints)
            {
                point.sprite = actionsPoints.IndexOf(point) < actionsUsed ? m_FilledPoint : m_EmptyPoint;
            }
        }        

        //public Coroutine<T> Invoke<T>(IEnumerator<T> routine)
        //{
        //    return this.StartCoroutine<T>(routine);
        //}

        //public void StopInvoke<T>(IEnumerator<T> routine)
        //{
        //    StopCoroutine(routine);
        //}
    }

    public interface ICharacterOrderView
    {
        IEnumerator Show(string[] planningOrder, int maxActionsAllowed, IList<IWitchModel> witchModels, IList<ISpiritModel> spiritModels);
        void Hide();
    }
}