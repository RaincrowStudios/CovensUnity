using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.BattleArena.Views
{
    public class StatusEffectView : MonoBehaviour, IStatusEffectView
    {
        [SerializeField] private Image _spellImage;
        [SerializeField] private Image _fillImage;

        public Image SpellImage => _spellImage;
        public float DurationNormalized { get => _fillImage.fillAmount; set => _fillImage.fillAmount = value; }
    }

    public interface IStatusEffectView
    {
        Image SpellImage { get; }
        float DurationNormalized { get; set; }
    }
}