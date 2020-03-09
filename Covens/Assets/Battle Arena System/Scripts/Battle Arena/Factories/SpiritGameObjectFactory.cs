using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Model;
using Raincrow.BattleArena.Views;
using Raincrow.Services;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Factory
{
    public class SpiritGameObjectFactory : AbstractCharacterGameObjectFactory<ISpiritModel, ISpiritUIModel>
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

        protected virtual void OnDisable()
        {
            if (_objectPool != null)
            {
                _objectPool.RecycleAll(_battleSpiritViewPrefab);
            }
        }

        public override IEnumerator<ICharacterController<ISpiritModel, ISpiritUIModel>> Create(Transform cellTransform, ISpiritModel model)
        {
            // Create character            
            ICharacterController<ISpiritModel, ISpiritUIModel> characterView = _objectPool.Spawn(_battleSpiritViewPrefab, cellTransform);
            yield return null;

            // wait for coroutine
            Coroutine<Texture> tex = this.StartCoroutine<Texture>(GetSpiritAvatar(model));
            while (tex.keepWaiting)
            {
                yield return null;
            }

            ISpiritUIModel spiritViewModel = new SpiritUIModel(characterView.Transform)
            {
                Texture = tex.ReturnValue,
                AlignmentMaterial = _wildAlignmentMaterial
            };
            characterView.Init(model, spiritViewModel);
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
