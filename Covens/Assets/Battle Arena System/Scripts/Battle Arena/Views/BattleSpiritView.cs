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
        private Material _alignmentRingMat;

        // Static readonlies
        private static readonly int MainTexPropertyId = Shader.PropertyToID("_MainTex");
        private static readonly int MainColorPropertyId = Shader.PropertyToID("_Color");

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

            if (_alignmentRingMat == null)
            {
                _alignmentRingMat = new Material(_alignmentRingRenderer.sharedMaterial);
                _alignmentRingRenderer.material = _alignmentRingMat;
            }
        }

        public override void Init(ISpiritModel characterModel, ISpiritViewModel characterViewModel, Camera battleCamera)
        {
            base.Init(characterModel, characterViewModel, battleCamera);
            _avatarMat.SetTexture(MainTexPropertyId, characterViewModel.Texture);
        }

        public override void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward)
        {
            Vector3 worldPosition = transform.position + cameraRotation * Vector3.forward;
            _avatarRoot.transform.LookAt(worldPosition, cameraForward);
        }

        //[SerializeField] protected SpriteRenderer m_EnergyRing;
        //private float m_EnergyFill = 0;
        //private int m_EnergyRingTweenId;

        //public void Init(ICharacterModel characterModel)
        //{
        //    _SpiritModel = ((ISpiritModel)characterModel);
        //    SetupCharacter();
        //}

        //private void SetupCharacter()
        //{
        //    m_AvatarRenderer.material.color = new Color(1, 1, 1, 0);
        //    //m_EnergyRing.color = spiritToken.IsBossSummon ? new Color(1, 0, 0.2f) : Color.white;

        //    DownloadedAssets.GetSprite(_SpiritModel.Texture, (sprite) =>
        //    {
        //        if (m_AvatarRenderer != null && sprite != null)
        //        {
        //            float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
        //            float spriteWidth = sprite.rect.width / sprite.pixelsPerUnit;
        //            m_AvatarRenderer.transform.localScale = new Vector3(spriteWidth * 5.5f, spriteHeight * 5.5f, 0);
        //            //m_AvatarRender.transform.localPosition = new Vector3(0, spriteHeight * 0.4f * m_AvatarRender.transform.localScale.x, 0);
        //            m_AvatarRenderer.material.SetTexture("_MainTex", sprite.texture);
        //            LeanTween.color(
        //                m_AvatarRenderer.gameObject,
        //                Color.white, 1f
        //            ).setEaseOutCubic();
        //        }
        //    });
        //}

        //public override void UpdateEnergy(float time = 1f)
        //{
        //    LeanTween.cancel(m_EnergyRingTweenId);

        //    m_EnergyFill = _SpiritModel.Energy;
        //    m_EnergyFill /= _SpiritModel.BaseEnergy;

        //    if (Mathf.Approximately(time, 0f))
        //    {
        //        m_EnergyRing.color = new Color(1, 1, 1, m_EnergyFill);
        //    }
        //    else
        //    {
        //        m_EnergyRingTweenId = LeanTween.alpha(m_EnergyRing.gameObject, m_EnergyFill, time).uniqueId;
        //    }
        //} 
    }
}
