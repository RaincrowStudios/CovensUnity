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
        [SerializeField] private NameplateView _nameplateViewPrefab;
        [SerializeField] private ServiceLocator _serviceLocator;
        [SerializeField] private Transform _nameplatesParent;

        [Header("Alignment")]
        [SerializeField] private Material _alignmentWhiteMaterial;
        [SerializeField] private Material _alignmentShadowMaterial;
        [SerializeField] private Material _alignmentGreyMaterial;

        // private variables                
        private IWitchAvatarFactory _witchAvatarFactory;
        private ObjectPool _objectPool;
        private Camera _battleCamera;

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

            if (_battleCamera == null)
            {
                _battleCamera = _serviceLocator.GetBattleCamera();
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

            // Nameplate
            NameplateView nameplateView = Instantiate(_nameplateViewPrefab);
            nameplateView.Init(model, battleWitchView.NameplateTarget, _nameplatesParent, _serviceLocator.GetBattleCamera());
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