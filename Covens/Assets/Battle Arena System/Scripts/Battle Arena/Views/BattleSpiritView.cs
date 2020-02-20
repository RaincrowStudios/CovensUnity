using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.View
{
    public class BattleSpiritView : AbstractCharacterView<ISpiritModel, ISpiritViewModel>
    {        
        // Serialized variables
        [SerializeField] private Transform _avatarRoot;
        [SerializeField] protected Renderer _avatarRenderer;

        [Header("Health")]
        [SerializeField] private Renderer _damageRingRenderer;
        [SerializeField] private Renderer _alignmentRingRenderer;

        // private variables
        private Material _avatarMat;
        private Material _damageRingMat;

        // Static readonlies
        private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");
        private static readonly int ColorPropertyId = Shader.PropertyToID("_Color");
        private static readonly int AlphaCutoffPropertyId = Shader.PropertyToID("_Cutoff");

        protected virtual void OnEnable()
        {
            if (_avatarMat == null)
            {
                _avatarMat = new Material(_avatarRenderer.sharedMaterial);
                _avatarRenderer.material = _avatarMat;
            }

            if (_damageRingMat == null)
            {
                _damageRingMat = new Material(_damageRingRenderer.sharedMaterial);
                _damageRingRenderer.material = _damageRingMat;
            }
        }

        protected virtual void Update()
        {
            if (Model != null)
            {
                float lerpTime = 5f;
                float f = Mathf.InverseLerp(0, lerpTime, Time.time % lerpTime);
                Model.Energy = Mathf.FloorToInt(Mathf.Lerp(0, Model.BaseEnergy, f));
                UpdateView();
            }
        }

        public override void Init(ISpiritModel characterModel, ISpiritViewModel characterViewModel, Camera battleCamera)
        {
            base.Init(characterModel, characterViewModel, battleCamera);

            // Set avatar texture
            _avatarMat.SetTexture(MainTexPropertyId, characterViewModel.Texture);

            // Set alignment color
            _alignmentRingRenderer.sharedMaterial = characterViewModel.AlignmentMaterial;
        }

        public override void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward)
        {
            Vector3 worldPosition = transform.position + cameraRotation * Vector3.forward;
            _avatarRoot.transform.LookAt(worldPosition, cameraForward);
        }

        public override void UpdateView()
        {
            int baseEnergy = Model.BaseEnergy;
            int energy = Model.Energy;
            float energyNormalized = Mathf.InverseLerp(Mathf.Epsilon, baseEnergy, energy);
            _damageRingMat.SetFloat(AlphaCutoffPropertyId, 1f - energyNormalized);
        }
    }
}
