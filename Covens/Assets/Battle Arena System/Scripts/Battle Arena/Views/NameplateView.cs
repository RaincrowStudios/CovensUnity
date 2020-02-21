using Raincrow.BattleArena.Model;
using UnityEngine;

namespace Raincrow.BattleArena.View
{
    public class NameplateView : MonoBehaviour
    {
        [SerializeField] private GameObject _immunityIcon;
        [SerializeField] private GameObject _deathIcon;
        [SerializeField] private TMPro.TextMeshProUGUI _playerLevel;
        [SerializeField] private TMPro.TextMeshProUGUI _playerName;

        private Transform _witchTarget;
        private Camera _battleCamera;

        public void Init(IWitchModel witchModel, Transform witchTransform, Transform parent, Camera battleCamera)
        {
            _immunityIcon.SetActive(false);
            _deathIcon.SetActive(false);

            _playerLevel.gameObject.SetActive(true);
            _playerLevel.text = witchModel.Level.ToString();
            _playerName.gameObject.SetActive(true);
            _playerName.text = witchModel.Name;

            _witchTarget = witchTransform;
            _battleCamera = battleCamera;

            transform.SetParent(parent, false);
            UpdateView();
        }

        protected virtual void Update()
        {
            UpdateView();
        }

        public void UpdateView()
        {            
            transform.position = WorldToUISpace(transform.parent, _witchTarget.position, _battleCamera);
        }

        public Vector3 WorldToUISpace(Transform uiParent, Vector3 worldPos, Camera battleCamera)
        {
            //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
            Vector3 screenPos = battleCamera.WorldToScreenPoint(worldPos);

            //Convert the screenpoint to ui rectangle local point
            RectTransformUtility.ScreenPointToLocalPointInRectangle(uiParent as RectTransform, screenPos, battleCamera, out Vector2 movePos);

            //Convert the local point to world point
            return uiParent.TransformPoint(movePos);
        }
    }
}