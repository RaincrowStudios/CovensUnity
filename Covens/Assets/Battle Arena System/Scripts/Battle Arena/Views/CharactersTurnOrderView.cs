using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Model;
using Raincrow.Services;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class CharactersTurnOrderView : MonoBehaviour, ICharactersTurnOrderView
    {
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private CharacterTurnOrderView _characterTurnOrderViewPrefab;
        [SerializeField] private ActionPointView _actionPointViewPrefab;
        [SerializeField] private Transform _characterTurnOrderViewRoot;
        [SerializeField] private Transform _actionsRoot;

        // private variables     
        private ISpiritPortraitFactory _spiritPortraitFactory;
        private IWitchPortraitFactory _witchPortraitFactory;
        private ObjectPool _objectPool;
        private List<CharacterTurnOrderView> _characterPositions = new List<CharacterTurnOrderView>();
        private List<ActionPointView> _actionsPoints = new List<ActionPointView>();
        private Dictionary<string, IWitchModel> _dictWitches = new Dictionary<string, IWitchModel>(); // List with all witches
        private Dictionary<string, ISpiritModel> _dictSpirits = new Dictionary<string, ISpiritModel>(); // List with all spirits

        protected virtual void OnValidate()
        {
            if (_serviceLocator == null)
            {
                _serviceLocator = FindObjectOfType<ServiceLocator>();
            }

            // Could not lazily initialize Service Locator
            if (_serviceLocator == null)
            {
                Debug.LogError("Could not find Service Locator!");
            }
        }

        public IEnumerator Show(string[] planningOrder, int maxActionsAllowed, IList<IWitchModel> witchModels, IList<ISpiritModel> spiritModels)
        {
            gameObject.SetActive(true);

            // Lazy initialization
            if (_spiritPortraitFactory == null)
            {
                _spiritPortraitFactory = _serviceLocator.GetSpiritPortraitFactory();
            }

            if (_witchPortraitFactory == null)
            {
                _witchPortraitFactory = _serviceLocator.GetWitchPortraitFactory();
            }

            if (_objectPool == null)
            {
                _objectPool = _serviceLocator.GetObjectPool();
            }

            // Clear all arrays
            _dictWitches.Clear();
            _dictSpirits.Clear();
            _characterPositions.Clear();
            _actionsPoints.Clear();

            // Clean up object pool
            _objectPool.RecycleAll(_actionPointViewPrefab, false);
            _objectPool.RecycleAll(_characterTurnOrderViewPrefab, false);

            foreach (IWitchModel witch in witchModels)
            {
                _dictWitches.Add(witch.Id, witch);
            }

            foreach (ISpiritModel spirit in spiritModels)
            {
                _dictSpirits.Add(spirit.Id, spirit);
            }            

            yield return StartCoroutine(CreateCharactersTurnOrder(planningOrder));
            yield return StartCoroutine(CreateActionsPoints(maxActionsAllowed));
        }

        public void Hide()
        {
            gameObject.SetActive(false);

            // Clear all arrays
            _dictWitches.Clear();
            _dictSpirits.Clear();
            _characterPositions.Clear();
            _actionsPoints.Clear();

            // Clean up object pool
            _objectPool.RecycleAll(_actionPointViewPrefab, false);
            _objectPool.RecycleAll(_characterTurnOrderViewPrefab, false);
        }

        private IEnumerator CreateCharactersTurnOrder(string[] characters)
        {
            foreach (string characterID in characters)
            {
                Coroutine<Sprite> coroutine = null;
                Color alignmentColor = Color.clear;

                if (_dictWitches.TryGetValue(characterID, out IWitchModel witch))
                {
                    alignmentColor = witch.GetAlignmentColor();
                    coroutine = this.StartCoroutine<Sprite>(GetWitchPortraitAvatar(witch));
                    while (coroutine.keepWaiting)
                    {
                        yield return null;
                    }

                }
                else if (_dictSpirits.TryGetValue(characterID, out ISpiritModel spirit))
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
                    CharacterTurnOrderView characterTurnOrderView = _objectPool.Spawn(_characterTurnOrderViewPrefab, _characterTurnOrderViewRoot, false);
                    characterTurnOrderView.Init(coroutine.ReturnValue, alignmentColor);
                    _characterPositions.Add(characterTurnOrderView);
                }

                yield return new WaitForSeconds(0.1f);
            }
        }

        private IEnumerator<Sprite> GetWitchPortraitAvatar(IWitchModel witchModel)
        {
            IEnumerator<SpriteRequest> enumerator = _witchPortraitFactory.CreateIWitchPortrait(witchModel);
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
            IEnumerator<SpriteRequest> enumerator = _spiritPortraitFactory.CreateSpiritPortrait(spiritModel);
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
                ActionPointView actionPointView = _objectPool.Spawn(_actionPointViewPrefab, _actionsRoot, false);
                _actionsPoints.Add(actionPointView);
                yield return new WaitForSeconds(0.1f);
            }

            UpdateActionsPoints(0);
        }

        public void UpdateActionsPoints(int numActionsUsed)
        {            
            // Set all actions that are not available anymore
            for (int i = 0; i < numActionsUsed; i++)
            {
                ActionPointView actionPointView = _actionsPoints[i];
                actionPointView.SetState(false);
            }

            // Set actions that are still available
            for (int i = numActionsUsed; i < _actionsPoints.Count; i++)
            {
                ActionPointView actionPointView = _actionsPoints[i];
                actionPointView.SetState(true);
            }
        }
    }

    public interface ICharactersTurnOrderView
    {
        IEnumerator Show(string[] planningOrder, int maxActionsAllowed, IList<IWitchModel> witchModels, IList<ISpiritModel> spiritModels);

        void UpdateActionsPoints(int numActionsUsed);

        void Hide();
    }
}