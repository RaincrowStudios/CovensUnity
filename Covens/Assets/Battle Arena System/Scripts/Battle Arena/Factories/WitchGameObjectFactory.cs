using Raincrow.BattleArena.View;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Services;

namespace Raincrow.BattleArena.Factory
{
    public class WitchGameObjectFactory : AbstractCharacterGameObjectFactory<IWitchModel, IWitchViewModel>
    {
        // serialized variables
        [SerializeField] private BattleWitchView _battleWitchViewPrefab;
        [SerializeField] private ServiceLocator _serviceLocator;

        [Header("Alignment")]
        [SerializeField] private Color _alignmentWhiteColor;
        [SerializeField] private Color _alignmentShadowColor;
        [SerializeField] private Color _alignmentGreyColor;

        // private variables        
        private IWitchAvatarFactory _witchAvatarFactory;

        protected virtual void OnEnable()
        {
            if (_witchAvatarFactory == null)
            {
                _witchAvatarFactory = _serviceLocator.GetWitchAvatarFactory();
            }
        }

        public override IEnumerator<AbstractCharacterView<IWitchModel, IWitchViewModel>> Create(Transform cellTransform, IWitchModel model)
        {
            // Create character
            AbstractCharacterView<IWitchModel, IWitchViewModel> characterView = Instantiate(_battleWitchViewPrefab, cellTransform);
            yield return null;

            // wait for coroutine
            Coroutine<Texture> request = this.StartCoroutine<Texture>(GetWitchAvatar(model));            
            while (request.keepWaiting)
            {
                yield return null;
            }

            Color alignmentColor = _alignmentGreyColor;
            if (model.Degree > 0)
            {
                alignmentColor = _alignmentWhiteColor;
            }
            else if (model.Degree < 0)
            {
                alignmentColor = _alignmentShadowColor;
            }

            IWitchViewModel viewModel = new WitchViewModel()
            {
                Texture = request.ReturnValue,
                AlignmentColor = alignmentColor
            };
            characterView.Init(model, viewModel, _serviceLocator.GetBattleCamera());

            yield return characterView;
        }

        private IEnumerator<Texture> GetWitchAvatar(IWitchModel witchModel)
        {
            IEnumerator<TextureRequest> enumerator = _witchAvatarFactory.CreateWitchAvatar(witchModel);
            Coroutine<TextureRequest> coroutine = this.StartCoroutine<TextureRequest>(enumerator);
            while (coroutine.keepWaiting)
            {
                yield return null;
            }

            Texture avatarTexture = coroutine.ReturnValue.Texture;
            yield return avatarTexture;
        }
    }

    public interface IWitchAvatarFactory
    {
        IEnumerator<TextureRequest> CreateWitchAvatar(IWitchModel witchModel);
    }

    public interface ISpiritAvatarFactory
    {
        IEnumerator<TextureRequest> CreateSpiritAvatar(ISpiritModel spiritModel);
    }

    public interface ISpiritPortraitFactory
    {
        IEnumerator<TextureRequest> CreateSpiritPortrait(ISpiritModel spiritModel);
    }

    public struct TextureRequest
    {
        public Texture Texture { get; set; }
        public bool IsDone { get; set; }
    }
}