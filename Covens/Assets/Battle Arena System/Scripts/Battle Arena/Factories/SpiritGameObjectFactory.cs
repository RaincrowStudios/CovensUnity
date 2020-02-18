using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.View;
using Raincrow.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class SpiritGameObjectFactory : AbstractCharacterGameObjectFactory
    {
        // serialized variables
        [SerializeField] private BattleSpiritView _battleSpiritViewPrefab;
        [SerializeField] private ServiceLocator _serviceLocator;

        // private variables        
        private ISpiritAvatarFactory _spiritAvatarFactory;

        protected virtual void OnEnable()
        {
            if (_spiritAvatarFactory == null)
            {
                _spiritAvatarFactory = _serviceLocator.GetSpiritAvatarFactory();
            }
        }

        public override IEnumerator<AbstractCharacterView> Create(Transform cellTransform, ICharacterModel character)
        {
            ISpiritModel spiritModel = character as ISpiritModel;

            // Create character
            AbstractCharacterView characterView = Instantiate(_battleSpiritViewPrefab, cellTransform);
            yield return null;

            // wait for coroutine
            Coroutine<Texture> tex = this.StartCoroutine<Texture>(GetWitchAvatar(spiritModel));
            while (tex.keepWaiting)
            {
                yield return null;
            }

            characterView.ChangeCharacterTexture(tex.ReturnValue);
            yield return characterView;
        }

        private IEnumerator<Texture> GetWitchAvatar(ISpiritModel witchModel)
        {
            IEnumerator<AvatarRequest> enumerator = _spiritAvatarFactory.CreateSpiritAvatar(witchModel);
            Coroutine<AvatarRequest> coroutine = this.StartCoroutine<AvatarRequest>(enumerator);
            while (coroutine.keepWaiting)
            {
                yield return null;
            }

            Texture avatarTexture = coroutine.ReturnValue.Avatar;
            yield return avatarTexture;
        }
    }
}
