using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
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
        private ObjectPool _objectPool;

        protected virtual void OnEnable()
        {
            if (_serviceLocator == null)
            {
                _serviceLocator = FindObjectOfType<ServiceLocator>();
            }

            if (_spiritAvatarFactory == null)
            {
                _spiritAvatarFactory = _serviceLocator.GetSpiritAvatarFactory();
            }

            if (_objectPool == null)
            {
                _objectPool = _serviceLocator.GetObjectPool();
            }
        }

        public override IEnumerator<AbstractCharacterView<ISpiritModel, ISpiritViewModel>> Create(Transform cellTransform, ISpiritModel model)
        {
            // Create character            
            AbstractCharacterView<ISpiritModel, ISpiritViewModel> characterView = _objectPool.Spawn(_battleSpiritViewPrefab, cellTransform);
            yield return null;

            // wait for coroutine
            Coroutine<Texture> tex = this.StartCoroutine<Texture>(GetSpiritAvatar(model));
            while (tex.keepWaiting)
            {
                yield return null;
            }

            ISpiritViewModel spiritViewModel = new SpiritViewModel()
            {
                Texture = tex.ReturnValue,
                AlignmentMaterial = _wildAlignmentMaterial
            };
            characterView.Init(model, spiritViewModel, _serviceLocator.GetBattleCamera());
            yield return characterView;
        }

        private IEnumerator<Texture> GetSpiritAvatar(ISpiritModel witchModel)
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
