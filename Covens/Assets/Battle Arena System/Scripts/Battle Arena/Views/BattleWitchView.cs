using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.View
{
    public class BattleWitchView : AbstractCharacterView<IWitchModel>
    {
        // Serialized variables
        [SerializeField] private Transform _avatarRoot;
        [SerializeField] private Renderer _renderer;

        [Header("Nameplate")]
        [SerializeField] private GameObject _immunityIcon;
        [SerializeField] private GameObject _deathIcon;
        [SerializeField] private TMPro.TextMeshProUGUI _playerLevel;
        [SerializeField] private TMPro.TextMeshProUGUI _playerName;

        // private variables
        private Material _rendererMaterial;

        // Static readonlies
        private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");

        protected virtual void OnEnable()
        {
            if (_rendererMaterial == null)
            {
                _rendererMaterial = new Material(_renderer.sharedMaterial);
                _renderer.material = _rendererMaterial;
            }
        }

        public override void Init(IWitchModel characterModel)
        {
            base.Init(characterModel);

            _immunityIcon.SetActive(false);
            _deathIcon.SetActive(false);

            _playerLevel.gameObject.SetActive(true);
            _playerLevel.text = characterModel.Level.ToString();
            _playerName.gameObject.SetActive(true);
            _playerName.text = characterModel.Name;
        }

        public override void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward)
        {
            Vector3 worldPosition = transform.position + cameraRotation * Vector3.forward;
            _avatarRoot.transform.LookAt(worldPosition, cameraForward);
        }

        public override void ChangeCharacterTexture(Texture texture)
        {
            _rendererMaterial.SetTexture(MainTexPropertyId, texture);
        }
    }
}