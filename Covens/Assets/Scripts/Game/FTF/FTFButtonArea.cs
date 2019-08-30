using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Raincrow.FTF
{
    public class FTFButtonArea : FTFRectBase
    {
        [Header("FTFButtonArea")]
        [SerializeField] private Button m_Button;

        public event System.Action OnClick;

        protected override void Awake()
        {
            base.Awake();
            m_Button.onClick.AddListener(OnClickButton);
        }

        private void OnClickButton()
        {
            OnClick?.Invoke();
        }
    }
}