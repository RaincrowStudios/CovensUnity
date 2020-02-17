using UnityEngine;

namespace Raincrow.BattleArena.View
{
    public class BattleWitchView : AbstractCharacterView
    {
        // Serialized variables
        [SerializeField] private Transform _avatarRoot;
        [SerializeField] private Renderer _renderer;

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