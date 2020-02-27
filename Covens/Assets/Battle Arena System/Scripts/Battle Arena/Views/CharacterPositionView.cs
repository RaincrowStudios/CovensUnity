using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.View
{
    public class CharacterPositionView : MonoBehaviour
    {
        [SerializeField] private Image _avatar;
        [SerializeField] private Image _alignment;

        public void Init(Sprite characterPortrait, Color aligmentColor)
        {
            _avatar.sprite = characterPortrait;
            _alignment.color = aligmentColor;
        }
    }
}
