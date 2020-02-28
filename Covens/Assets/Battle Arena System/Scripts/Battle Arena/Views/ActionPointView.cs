using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class ActionPointView : MonoBehaviour
    {
        [SerializeField] private Sprite _emptyPoint;
        [SerializeField] private Sprite _filledPoint;
        [SerializeField] private Image _image;

        public void SetState(bool available)
        {
            if (available)
            {
                _image.overrideSprite = _emptyPoint;
            }
            else
            {
                _image.overrideSprite = _filledPoint;
            }
        }
    }
}