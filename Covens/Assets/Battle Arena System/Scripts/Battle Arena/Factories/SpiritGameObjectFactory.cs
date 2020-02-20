using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.View;
using Raincrow.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class SpiritGameObjectFactory : AbstractCharacterGameObjectFactory<ISpiritModel, ISpiritViewModel>
    {
        // serialized variables
        [SerializeField] private BattleSpiritView _battleSpiritViewPrefab;
        [SerializeField] private ServiceLocator _serviceLocator;
        
        [Header("Alignment")]
        [SerializeField] private Material _wildAlignmentMaterial;

        // private variables        
        private ISpiritAvatarFactory _spiritAvatarFactory;

        protected virtual void OnEnable()
        {
            if (_spiritAvatarFactory == null)
            {
                _spiritAvatarFactory = _serviceLocator.GetSpiritAvatarFactory();
            }
        }

        public override IEnumerator<AbstractCharacterView<ISpiritModel, ISpiritViewModel>> Create(Transform cellTransform, ISpiritModel model)
        {
            // Create character
            AbstractCharacterView<ISpiritModel, ISpiritViewModel> characterView = Instantiate(_battleSpiritViewPrefab, cellTransform);
            yield return null;

            // wait for coroutine
            Coroutine<Texture> request = this.StartCoroutine<Texture>(GetWitchAvatar(model));
            while (request.keepWaiting)
            {
                yield return null;
            }

            ISpiritViewModel spiritViewModel = new SpiritViewModel()
            {
                Texture = request.ReturnValue,
                AlignmentMaterial = _wildAlignmentMaterial
            };
            characterView.Init(model, spiritViewModel, _serviceLocator.GetBattleCamera());
            yield return characterView;
        }

        private IEnumerator<Texture> GetWitchAvatar(ISpiritModel witchModel)
        {
            IEnumerator<TextureRequest> enumerator = _spiritAvatarFactory.CreateSpiritAvatar(witchModel);
            Coroutine<TextureRequest> coroutine = this.StartCoroutine<TextureRequest>(enumerator);
            while (coroutine.keepWaiting)
            {
                yield return null;
            }

            Texture avatarTexture = coroutine.ReturnValue.Texture;
            yield return avatarTexture;
        }
    }
}
