using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.View
{
    public class CharacterPositionView : MonoBehaviour
    {
        [SerializeField] private Image m_Avatar;
        [SerializeField] private Image m_Alignment;

        private readonly Color NoAlignmentColor = new Vector4(0, 1, 1, 1);
        private readonly Color ShadowAlignmentColor = new Vector4(0.698f, 0, 1, 1);
        private readonly Color WhiteAlignmentColor = new Vector4(0.97f, 0.67f, 0.18f, 1.0f);
        private readonly Color SpiritColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        public void Init(Sprite witchPortrait, int degree, bool spirit = false)
        {
            m_Avatar.sprite = witchPortrait;

            if (spirit) {
                m_Alignment.color = SpiritColor;
            }
            else
            {
                if (degree == 0)
                {
                    m_Alignment.color = NoAlignmentColor;
                }
                else if (degree > 0)
                {
                    m_Alignment.color = WhiteAlignmentColor;
                }
                else
                {
                    m_Alignment.color = ShadowAlignmentColor;
                }
            }
        }
    }
}
