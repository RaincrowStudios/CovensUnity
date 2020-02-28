using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class CharacterTurnOrderView : MonoBehaviour
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
