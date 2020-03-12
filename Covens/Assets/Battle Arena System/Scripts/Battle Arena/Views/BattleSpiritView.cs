using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class BattleSpiritView : MonoBehaviour, ICharacterController<ISpiritModel, ISpiritUIModel>
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
        private static readonly float MinAlphaCutoff = 0.0001f;

        // Properties
        public ISpiritModel Model { get; private set; }
        public ISpiritUIModel UIModel { get; private set; }
        public Transform Transform => transform;
        public GameObject GameObject => gameObject;

        ICharacterModel ICharacterController.Model => Model;
        ICharacterUIModel ICharacterController.UIModel => UIModel;

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

        public void Init(ISpiritModel characterModel, ISpiritUIModel characterViewModel)
        {            
            Model = characterModel;
            UIModel = characterViewModel;

            // Set avatar texture
            _avatarMat.SetTexture(MainTexPropertyId, characterViewModel.Texture);

            // Set alignment color
            _alignmentRingRenderer.sharedMaterial = characterViewModel.AlignmentMaterial;

            UpdateView(Model.BaseEnergy, Model.Energy);
        }

        public void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward)
        {
            Vector3 worldPosition = transform.position + cameraRotation * Vector3.forward;
            _avatarRoot.transform.LookAt(worldPosition, cameraForward);
        }

        public void AddDamage(int damage)
        {
            Model.Energy += damage;
            Model.Energy = Mathf.Max(Model.Energy, 0);
            UpdateView(Model.BaseEnergy, Model.Energy);
        }

        public void UpdateEnergy(int energy)
        {
            Model.Energy = energy;
            UpdateView(Model.BaseEnergy, Model.Energy);
        }

        public void UpdateView(int baseEnergy, int energy)
        {
            float energyNormalized = Mathf.InverseLerp(0f, baseEnergy, energy);
            _damageRingMat.SetFloat(AlphaCutoffPropertyId, Mathf.Max(energyNormalized, MinAlphaCutoff));

            Debug.LogFormat("Update Energy {0} {1}", baseEnergy, energy);
        }        
    }
}
