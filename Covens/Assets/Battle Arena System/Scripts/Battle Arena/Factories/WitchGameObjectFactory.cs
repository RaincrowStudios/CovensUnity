using Raincrow.BattleArena.View;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Services;

namespace Raincrow.BattleArena.Factory
{
    public class WitchGameObjectFactory : AbstractCharacterGameObjectFactory<IWitchModel>
    {
        // serialized variables
        [SerializeField] private BattleWitchView _battleWitchViewPrefab;
        [SerializeField] private ServiceLocator _serviceLocator;

        // private variables        
        private IWitchAvatarFactory _witchAvatarFactory;

        protected virtual void OnEnable()
        {
            if (_witchAvatarFactory == null)
            {
                _witchAvatarFactory = _serviceLocator.GetWitchAvatarFactory();
            }
        }

        public override IEnumerator<AbstractCharacterView<IWitchModel>> Create(Transform cellTransform, IWitchModel model)
        {
            // Create character
            AbstractCharacterView<IWitchModel> characterView = Instantiate(_battleWitchViewPrefab, cellTransform);
            yield return null;

            // wait for coroutine
            Coroutine<Texture> request = this.StartCoroutine<Texture>(GetWitchAvatar(model));            
            while (request.keepWaiting)
            {
                yield return null;
            }

            characterView.ChangeCharacterTexture(request.ReturnValue);

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