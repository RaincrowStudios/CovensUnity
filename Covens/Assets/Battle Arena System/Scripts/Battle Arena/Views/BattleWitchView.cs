using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Model;
using System.Collections;
using UnityEngine;

namespace Raincrow.BattleArena.Views
{
    public class BattleWitchView : MonoBehaviour, ICharacterController<IWitchModel, IWitchUIModel>
    {
        // Serialized variables        
        [Header("Avatar")]
        [SerializeField] private Transform _avatarRoot;
        [SerializeField] private Renderer _avatarRenderer;

        [Header("Health")]
        [SerializeField] private GameObject _healthView;
        [SerializeField] private SpriteRenderer _energyRing;

        [Header("Player Ring")]
        [SerializeField] private GameObject _selectionRing;
        [SerializeField] private GameObject _shadowRing;
        [SerializeField] private GameObject _greyRing;
        [SerializeField] private GameObject _whiteRing;


        [Header("Nameplate")]
        [SerializeField] private GameObject _nameplate;
        [SerializeField] private GameObject _immunityIcon;
        [SerializeField] private GameObject _deathIcon;
        [SerializeField] private TMPro.TextMeshPro _playerLevel;
        [SerializeField] private TMPro.TextMeshPro _playerName;

        // private variables
        private Material _avatarMat;
        //private Material _damageRingMat;
        private MaterialPropertyBlock props;

        private int _energyRingTweenId;

        // Static readonlies
        private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");
        private static readonly int AlphaCutoffPropertyId = Shader.PropertyToID("_Cutoff");
        private static readonly float MinAlphaCutoff = 0.0001f;

        // Properties
        public IWitchModel Model { get; private set; }
        public IWitchUIModel UIModel { get; private set; }
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

            if (props == null)
            {
                props = new MaterialPropertyBlock();
            }
        }

        public void Init(IWitchModel characterModel, IWitchUIModel characterViewModel)
        {
            Model = characterModel;
            UIModel = characterViewModel;

            // Set avatar texture
            _avatarMat.SetTexture(MainTexPropertyId, characterViewModel.Texture);

            // Set alignment color
            _energyRing.color = characterModel.GetAlignmentColor();

            _immunityIcon.SetActive(false);
            _deathIcon.SetActive(false);

            if (characterModel.Id.Equals(PlayerDataManager.playerData.instance))
            {
                _nameplate.SetActive(false);
                _healthView.SetActive(false);
                _selectionRing.SetActive(true);
                ActiveRing(characterModel.Degree);
            }
            else
            {
                _nameplate.SetActive(true);
                _healthView.SetActive(true);
                _selectionRing.SetActive(false);

                _playerLevel.gameObject.SetActive(true);
                _playerLevel.text = characterModel.Level.ToString();
                _playerName.gameObject.SetActive(true);
                _playerName.text = characterModel.Name;
            }

            UpdateView(Model.BaseEnergy, Model.Energy);
        }

        private void ActiveRing(int degree)
        {
            if (degree > 0)
            {
                _shadowRing.SetActive(false);
                _greyRing.SetActive(false);
                _whiteRing.SetActive(true);
            }
            else if (degree < 0)
            {
                _shadowRing.SetActive(true);
                _greyRing.SetActive(false);
                _whiteRing.SetActive(false);
            }
            else
            {
                _shadowRing.SetActive(false);
                _greyRing.SetActive(true);
                _whiteRing.SetActive(false);
            }
        }

        public void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward)
        {
            Vector3 worldPosition = transform.position + cameraRotation * Vector3.forward;
            _avatarRoot.transform.LookAt(worldPosition, cameraForward);
        }

        public void UpdateView(int baseEnergy, int energy)
        {
            LeanTween.cancel(_energyRingTweenId);

            float energyFill = ((float)energy) / baseEnergy;

            _energyRingTweenId = LeanTween.alpha(_energyRing.gameObject, energyFill, 0.3f).uniqueId;

            //float energyNormalized = Mathf.InverseLerp(0f, baseEnergy, energy);            
            //props.SetFloat(AlphaCutoffPropertyId, Mathf.Max(energyNormalized, MinAlphaCutoff));
            //_damageRingRenderer.SetPropertyBlock(props);

            bool isDead = energy <= 0;
            _deathIcon.SetActive(isDead);
            //Debug.LogFormat("Update Energy {0} {1}", baseEnergy, energy);                        
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
    }
}