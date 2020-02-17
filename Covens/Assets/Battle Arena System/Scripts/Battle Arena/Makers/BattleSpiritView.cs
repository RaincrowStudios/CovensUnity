using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.View
{
    public class BattleSpiritView : AbstractCharacterView
    {
        private ISpiritModel _SpiritModel;
        private float m_EnergyFill = 0;
        private int m_EnergyRingTweenId;

        public void Init(ICharacterModel characterModel)
        {
            _SpiritModel = ((ISpiritModel)characterModel);
            SetupCharacter();
            UpdateEnergy(0);
        }

        private void SetupCharacter()
        {
            m_AvatarRender.material.color = new Color(1, 1, 1, 0);
            //m_EnergyRing.color = spiritToken.IsBossSummon ? new Color(1, 0, 0.2f) : Color.white;

            DownloadedAssets.GetSprite(_SpiritModel.Texture, (sprite) =>
            {
                if (m_AvatarRender != null && sprite != null)
                {
                    float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
                    float spriteWidth = sprite.rect.width / sprite.pixelsPerUnit;
                    m_AvatarRender.transform.localScale = new Vector3(spriteWidth * 5.5f, spriteHeight * 5.5f, 0);
                    //m_AvatarRender.transform.localPosition = new Vector3(0, spriteHeight * 0.4f * m_AvatarRender.transform.localScale.x, 0);
                    m_AvatarRender.material.SetTexture("_MainTex", sprite.texture);
                    LeanTween.color(
                        m_AvatarRender.gameObject,
                        Color.white, 1f
                    ).setEaseOutCubic();
                }
            });
        }

        public override void UpdateEnergy(float time = 1f)
        {
            LeanTween.cancel(m_EnergyRingTweenId);

            m_EnergyFill = (float)_SpiritModel.Energy;
            m_EnergyFill /= _SpiritModel.BaseEnergy;

            if (time == 0)
                m_EnergyRing.color = new Color(1, 1, 1, m_EnergyFill);
            else
                m_EnergyRingTweenId = LeanTween.alpha(m_EnergyRing.gameObject, m_EnergyFill, time).uniqueId;
        }

        public override void FaceCamera(Quaternion cameraRotation, Vector3 cameraForward)
        {
            Vector3 worldPosition = transform.position + cameraRotation * Vector3.forward;
            m_AvatarRender.transform.LookAt(worldPosition, cameraForward);
        }
    }
}
