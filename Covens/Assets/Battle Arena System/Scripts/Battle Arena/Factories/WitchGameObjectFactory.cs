using Raincrow.BattleArena.View;
using Raincrow.BattleArena.Model;
using System.Collections.Generic;
using UnityEngine;
using Raincrow.Services;

namespace Raincrow.BattleArena.Factory
{
    public class WitchGameObjectFactory : AbstractCharacterGameObjectFactory
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

        public override IEnumerator<AbstractCharacterView> Create(Transform cellTransform, ICharacterModel character)
        {
            IWitchModel witchModel = character as IWitchModel;

            // Create character
            AbstractCharacterView characterMarker = Instantiate(_battleWitchViewPrefab, cellTransform);
            yield return null;

            // wait for coroutine
            Coroutine<Texture> tex = this.StartCoroutine<Texture>(GetWitchAvatar(witchModel));            
            while (tex.keepWaiting)
            {
                yield return null;
            }

            characterMarker.ChangeCharacterTexture(tex.ReturnValue);

            yield return characterMarker;
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

    public struct AvatarRequest
    {
        public Texture Avatar { get; set; }
        public bool IsDone { get; set; }
    }
}