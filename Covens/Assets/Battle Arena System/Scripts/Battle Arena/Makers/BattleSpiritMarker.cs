using Raincrow.BattleArena.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Raincrow.BattleArena.Marker
{
    public class BattleSpiritMarker : AbstractCharacterMaker
    {
        private ISpiritModel _SpiritModel;

        public override Renderer AvatarRenderer { get; set; }

        public void Init(ICharacterModel characterModel)
        {
            _SpiritModel = ((ISpiritModel)characterModel);
            SetupCharacter();
        }

        private void SetupCharacter()
        {
            AvatarRenderer.material.color = new Color(1, 1, 1, 0);
            //m_EnergyRing.color = spiritToken.IsBossSummon ? new Color(1, 0, 0.2f) : Color.white;

            DownloadedAssets.GetSprite(_SpiritModel.Texture, (sprite) =>
            {
                if (AvatarRenderer != null && sprite != null)
                {
                    float spriteHeight = sprite.rect.height / sprite.pixelsPerUnit;
                    AvatarRenderer.transform.localPosition = new Vector3(0, spriteHeight * 0.4f * AvatarRenderer.transform.localScale.x, 0);
                    AvatarRenderer.material.SetTexture("_MainTex", sprite.texture);
                    LeanTween.color(
                        AvatarRenderer.gameObject,
                        Color.white, 1f
                    ).setEaseOutCubic();
                }
            });
        }
    }
}
