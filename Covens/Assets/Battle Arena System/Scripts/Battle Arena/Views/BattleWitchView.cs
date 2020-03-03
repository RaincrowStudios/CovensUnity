using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class BattleWitchView : MonoBehaviour, ICharacterView<IWitchModel, IWitchUIModel>
    {
        // Serialized variables        
        [Header("Avatar")]
        [SerializeField] private Transform _avatarRoot;
        [SerializeField] private Renderer _avatarRenderer;        

        [Header("Health")]
        [SerializeField] private Renderer _damageRingRenderer;
        [SerializeField] private Renderer _alignmentRingRenderer;

        [Header("Nameplate")]
        [SerializeField] private GameObject _immunityIcon;
        [SerializeField] private GameObject _deathIcon;
        [SerializeField] private TMPro.TextMeshPro _playerLevel;
        [SerializeField] private TMPro.TextMeshPro _playerName;

        // private variables
        private Material _avatarMat;
        //private Material _damageRingMat;
        private MaterialPropertyBlock props;

        // Static readonlies
        private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");        
        private static readonly int AlphaCutoffPropertyId = Shader.PropertyToID("_Cutoff");

        // Properties
        public IWitchModel Model { get; private set; }
        public IWitchUIModel UIModel { get; private set; }

        protected virtual void OnEnable()
        {
            if (_avatarMat == null)
            {
                _avatarMat = new Material(_avatarRenderer.sharedMaterial);
                _avatarRenderer.material = _avatarMat;
            }

            if (props == null)
            {
                props = new MaterialPropertyBlock();
            }
            //if (_damageRingMat == null)
            //{
            //    _damageRingMat = new Material(_damageRingRenderer.sharedMaterial);
            //    _damageRingRenderer.material = _damageRingMat;
            //}
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

        public void Init(IWitchModel characterModel, IWitchUIModel characterViewModel)
        {
            Model = characterModel;
            UIModel = characterViewModel;

            // Set avatar texture
            _avatarMat.SetTexture(MainTexPropertyId, characterViewModel.Texture);

            // Set alignment color
            _alignmentRingRenderer.sharedMaterial = characterViewModel.AlignmentMaterial;

            _immunityIcon.SetActive(false);
            _deathIcon.SetActive(false);

            _playerLevel.gameObject.SetActive(true);
            _playerLevel.text = characterModel.Level.ToString();
            _playerName.gameObject.SetActive(true);
            _playerName.text = characterModel.Name;
        }

        public void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward)
        {
            Vector3 worldPosition = transform.position + cameraRotation * Vector3.forward;
            _avatarRoot.transform.LookAt(worldPosition, cameraForward);
        }

        public void UpdateView()
        {
            int baseEnergy = Model.BaseEnergy;
            int energy = Model.Energy;
            float energyNormalized = Mathf.InverseLerp(Mathf.Epsilon, baseEnergy, energy);

            //_damageRingMat.SetFloat(AlphaCutoffPropertyId, 1f - energyNormalized);
            props.SetFloat(AlphaCutoffPropertyId, 1f - energyNormalized);
            _damageRingRenderer.SetPropertyBlock(props);
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}