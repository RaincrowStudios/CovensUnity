using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Model;
using System.Collections;
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

        public IEnumerator AddDamage(int damage)
        {
            Model.Energy -= damage;
            Model.Energy = Mathf.Max(Model.Energy, 0);
            UpdateView(Model.BaseEnergy, Model.Energy);
            yield return null;
        }        

        public void UpdateView(int baseEnergy, int energy)
        {
            float energyNormalized = Mathf.InverseLerp(0f, baseEnergy, energy);
            _damageRingMat.SetFloat(AlphaCutoffPropertyId, Mathf.Max(energyNormalized, MinAlphaCutoff));

            Debug.LogFormat("Update Energy {0} {1}", baseEnergy, energy);
        }

        public IEnumerator Move(float time, Vector3 targetPosition, Easings.Functions function)
        {
            Vector3 position = transform.position;
            for (float elapsedTime = 0; elapsedTime < time; elapsedTime += Time.deltaTime)
            {
                float t = Easings.Interpolate(elapsedTime, function);
                transform.position = Vector3.Lerp(position, targetPosition, t);
                yield return null;
            }
        }

        public IEnumerator Summon(float time, Easings.Functions function)
        {
            for (float elapsedTime = 0; elapsedTime < time; elapsedTime += Time.deltaTime)
            {
                float t = Easings.Interpolate(elapsedTime, function);
                transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }
        }
    }
}
