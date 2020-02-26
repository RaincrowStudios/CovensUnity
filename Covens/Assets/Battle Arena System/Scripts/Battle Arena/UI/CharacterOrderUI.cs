using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Events;
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
    public class CharacterOrderUI : MonoBehaviour
    {
        [Header("Prefabs UI")]
        [SerializeField] private CharacterPositionView m_CharacterUI;
        [SerializeField] private GameObject m_ActionUI;

        [Header("Points UI")]
        [SerializeField] private Sprite m_EmptyPoint;
        [SerializeField] private Sprite m_FilledPoint;


        [Header("Roots")]
        [SerializeField] private Transform m_CharactersRoot;
        [SerializeField] private Transform m_ActionsRoot;

        // private variables     
        private ISpiritPortraitFactory spiritPortraitFactory;
        private IWitchPortraitFactory witchPortraitFactory;

        private ServiceLocator serviceLocator;
        private BattleController battleController;
        private List<CharacterPositionView> characterPositions = new List<CharacterPositionView>();
        private List<Image> actionsPoints = new List<Image>();


        private void Awake()
        {
            serviceLocator = FindObjectOfType<ServiceLocator>();
            battleController = FindObjectOfType<BattleController>();

            spiritPortraitFactory = serviceLocator.GetSpiritPortraitFactory();
            witchPortraitFactory = serviceLocator.GetWitchPortraitFactory();
        }

        public void Init(PlanningPhaseReadyEventArgs args)
        {
            StartCoroutine(CreateOrder(args.PlanningOrder));
            StartCoroutine(CreateActionsPoints(args.MaxActionsAllowed));
        }

        private IEnumerator CreateOrder(string[] characters)
        {
            foreach (string character in characters)
            {
                ICharacterModel characterModel = GetCharacterModel(character);
                int degree = 0;
                if (characterModel != null)
                {
                    Coroutine<Sprite> coroutine;
                    if (characterModel.ObjectType == ObjectType.Witch)
                    {
                        degree = ((IWitchModel)characterModel).Degree;
                        coroutine = this.StartCoroutine<Sprite>(GetWitchPortraitAvatar(characterModel as IWitchModel));
                        while (coroutine.keepWaiting)
                        {
                            yield return null;
                        }
                    }
                    else
                    {
                        coroutine = this.StartCoroutine<Sprite>(GetSpiritPortraitAvatar(characterModel as ISpiritModel));
                        while (coroutine.keepWaiting)
                        {
                            yield return null;
                        }
                    }

                    CharacterPositionView characterPosition = Instantiate(m_CharacterUI, m_CharactersRoot);
                    characterPosition.Init(coroutine.ReturnValue, degree, characterModel.ObjectType == ObjectType.Spirit);
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

        private ICharacterModel GetCharacterModel(string id)
        {
            foreach (AbstractCharacterView<IWitchModel, IWitchViewModel> witch in battleController.Witches)
            {
                if (witch.Model.Id.Equals(id))
                {
                    return witch.Model;
                }
            }

            foreach (AbstractCharacterView<ISpiritModel, ISpiritViewModel> spirit in battleController.Spirits)
            {
                if (spirit.Model.Id.Equals(id))
                {
                    return spirit.Model;
                }
            }

            return null;
        }

        private IEnumerator CreateActionsPoints(int length)
        {
            for(int x = 0; x < length; x++)
            {
                Image point = Instantiate(m_ActionUI, m_ActionsRoot).GetComponent<Image>();
                yield return point;
                actionsPoints.Add(point);
            }

            UpdateActionsPoints(0);
        }

        private void UpdateActionsPoints(int actionsUsed)
        {
            foreach(Image point in actionsPoints)
            {
                point.sprite = actionsPoints.IndexOf(point) < actionsUsed ? m_FilledPoint : m_EmptyPoint;
            }
        }
    }
}