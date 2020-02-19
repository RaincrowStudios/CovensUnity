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
            IEnumerator<AvatarRequest> enumerator = _witchAvatarFactory.CreateWitchAvatar(witchModel);
            Coroutine<AvatarRequest> coroutine = this.StartCoroutine<AvatarRequest>(enumerator);
            while (coroutine.keepWaiting)
            {
                yield return null;
            }

            Texture avatarTexture = coroutine.ReturnValue.Avatar;
            yield return avatarTexture;
        }
    }

    public interface IWitchAvatarFactory
    {
        IEnumerator<AvatarRequest> CreateWitchAvatar(IWitchModel witchModel);
    }

    public interface ISpiritAvatarFactory
    {
        IEnumerator<AvatarRequest> CreateSpiritAvatar(ISpiritModel spiritModel);
    }

    public struct AvatarRequest
    {
        public Texture Avatar { get; set; }
        public bool IsDone { get; set; }
    }
}