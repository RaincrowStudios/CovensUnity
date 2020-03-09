using UnityEngine;
using TMPro;

namespace Raincrow.BattleArena.Views
{
    public class DebriefView : MonoBehaviour, IDebriefView
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _textRankPosition;
        [SerializeField] private TextMeshProUGUI _textDamageDealt;
        [SerializeField] private TextMeshProUGUI _textFavoriteSpell;
        [SerializeField] private TextMeshProUGUI _textDegree;
        [SerializeField] private TextMeshProUGUI _textSummonedSpirit;
        [SerializeField] private TextMeshProUGUI _textDefeated;

        [Header("Animation")]
        [SerializeField] private CanvasGroup _groupText;
        [SerializeField] private float _animationTime;

        private int _animationID;
        public void Show(int position, int damageDealt, string favoriteSpell, string degree, string summonedSpirit, string defeated)
        {
            _textRankPosition.text = string.Format("{0}st Place", position.ToString());
            _textDamageDealt.text = string.Format("Damage Dealt: {0}", damageDealt.ToString());
            _textFavoriteSpell.text = string.Format("Favorite Spell: {0}", favoriteSpell);
            _textDegree.text = string.Format("Degree: {0}", degree);
            _textSummonedSpirit.text = string.Format("Summoned Spirit: {0}", summonedSpirit);
            _textDefeated.text = string.Format("Defeated: {0}", defeated);

            _textFavoriteSpell.gameObject.SetActive(true);
            _textDegree.gameObject.SetActive(true);
            _textSummonedSpirit.gameObject.SetActive(true);
            _textDefeated.gameObject.SetActive(true);

            gameObject.SetActive(true);

            LeanTween.cancel(_animationID);
            _animationID = LeanTween.alphaCanvas(_groupText, 1f, _animationTime).id;
        }

        public void Show(int position, int damageDealt)
        {
            _textRankPosition.text = string.Format("{0}st Place", position.ToString());
            _textDamageDealt.text = string.Format("Damage Dealt: {0}", damageDealt.ToString());

            _textFavoriteSpell.gameObject.SetActive(false);
            _textDegree.gameObject.SetActive(false);
            _textSummonedSpirit.gameObject.SetActive(false);
            _textDefeated.gameObject.SetActive(false);

            gameObject.SetActive(true);

            LeanTween.cancel(_animationID);
            _animationID = LeanTween.alphaCanvas(_groupText, 1f, _animationTime).id;
        }

        public void Hide()
        {
            LeanTween.cancel(_animationID);
            _animationID = LeanTween.alphaCanvas(_groupText, 1f, _animationTime).setOnComplete(()=> gameObject.SetActive(false)).id;
        }
    }

    public interface IDebriefView
    {
        void Show(int position, int damageDealt, string favoriteSpell, string degree, string summonedSpirit, string defeated);
        void Show(int position, int damageDealt);
        void Hide();
    }

    public struct DebriefAnimationModel
    {
        public float CameraDistance;
        public float CameraHeight;
        public float CameraSpeed;
        public float TimeAnimation;
    }
}