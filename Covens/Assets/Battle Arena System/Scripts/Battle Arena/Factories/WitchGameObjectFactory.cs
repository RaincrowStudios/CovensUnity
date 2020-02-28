using Raincrow.BattleArena.Views;
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
        [SerializeField] private Material _alignmentWhiteMaterial;
        [SerializeField] private Material _alignmentShadowMaterial;
        [SerializeField] private Material _alignmentGreyMaterial;

        // private variables                
        private IWitchAvatarFactory _witchAvatarFactory;
        private ObjectPool _objectPool;

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

        protected virtual void OnEnable()
        {
            if (_witchAvatarFactory == null)
            {
                _witchAvatarFactory = _serviceLocator.GetWitchAvatarFactory();
            }

            if (_objectPool == null)
            {
                _objectPool = _serviceLocator.GetObjectPool();
            }
        }

        public override IEnumerator<AbstractCharacterView<IWitchModel, IWitchViewModel>> Create(Transform cellTransform, IWitchModel model)
        {
            // Create character            
            BattleWitchView battleWitchView = _objectPool.Spawn(_battleWitchViewPrefab, cellTransform);
            yield return null;

            // wait for coroutine
            Coroutine<Texture> request = this.StartCoroutine<Texture>(GetWitchAvatar(model));
            while (request.keepWaiting)
            {
                yield return null;
            }

            Material alignmentMaterial = _alignmentGreyMaterial;
            if (model.Degree > 0)
            {
                alignmentMaterial = _alignmentWhiteMaterial;
            }
            else if (model.Degree < 0)
            {
                alignmentMaterial = _alignmentShadowMaterial;
            }

            IWitchViewModel witchViewModel = new WitchViewModel()
            {
                Texture = request.ReturnValue,
                AlignmentMaterial = alignmentMaterial
            };
            battleWitchView.Init(model, witchViewModel, _serviceLocator.GetBattleCamera());
            yield return battleWitchView;
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
        IEnumerator<SpriteRequest> CreateSpiritPortrait(ISpiritModel spiritModel);
    }

    public interface IWitchPortraitFactory
    {
        IEnumerator<SpriteRequest> CreateIWitchPortrait(IWitchModel witchModel);
    }

    public struct TextureRequest
    {
        public Texture Texture { get; set; }
        public bool IsDone { get; set; }
    }

    public struct SpriteRequest
    {
        public Sprite Sprite { get; set; }
        public bool IsDone { get; set; }
    }
}