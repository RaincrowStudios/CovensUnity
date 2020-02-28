using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Views;
using Raincrow.BattleArena.Model;
using TMPro;
using UnityEngine.Events;
using System;

namespace Raincrow.BattleArena.UI
{
    public class QuickCastUI : MonoBehaviour, QuickCastView
    {
        public enum QuickCastMenus
        {
            Spell,
            Action
        }

        [SerializeField] private BattleController m_BattleController;

        [Header("UI")]
        [SerializeField] private Image m_ImageIcon;
        [SerializeField] private Sprite m_IconClose;
        [SerializeField] private Sprite m_IconOpen;
        [SerializeField] private TextMeshProUGUI m_TextAmountActions;

        [Header("Actions Buttons")]
        [SerializeField] private Button m_ButtonFly;
        [SerializeField] private Button m_ButtonSummon;

        [Header("Menus")]
        [SerializeField] private GameObject m_ActionsMenu;
        [SerializeField] private GameObject m_SpellMenu;

        private GameObject currentMenu;
        private bool open;

        private Action<string> onSummon;

        private const float TimeToToggle = 0.05f;


        public void Init(UnityAction onClickFly, UnityAction onClickSummom)
        {
            m_ButtonFly.onClick.AddListener(onClickFly);
            m_ButtonSummon.onClick.AddListener(onClickSummom);
        }
              
        public void OnClickCell(QuickCastMenus selectedMenu)
        {
            if (selectedMenu == QuickCastMenus.Action)
            {
                ChangeMenu(m_ActionsMenu);
            }
            else
            {
                ChangeMenu(m_SpellMenu);
            }
        }

        private void ChangeMenu(GameObject menu)
        {
            if(menu == currentMenu)
            {
                if (!open)
                {
                    OnClickToggle();
                }
                return;
            }

            menu.SetActive(true);

            if (currentMenu == null || !open)
            {
                if(currentMenu != null)
                {
                    currentMenu.SetActive(false);
                }

                LeanTween.scaleY(menu, 1.0f, TimeToToggle);
                currentMenu = menu;
            }
            else if (open)
            {
                LeanTween.scaleY(currentMenu, 0.0f, TimeToToggle).setOnComplete(() => {
                    currentMenu.SetActive(false);
                    LeanTween.scaleY(menu, 1.0f, TimeToToggle);
                    currentMenu = menu;
                });
            }
           

            open = true;
            m_ImageIcon.sprite = m_IconOpen;
        }

        public void OnClickToggle(float time = 0.05f)
        {
            if(currentMenu == null)
            {
                return;
            }

            LeanTween.scaleY(currentMenu, open ? 0 : 1, time);
            open = !open;
            m_ImageIcon.sprite = open ? m_IconOpen : m_IconClose;
        }
    }

    public interface QuickCastView
    {
        void Init(UnityAction onClickFly, UnityAction onClickSummom);

        void OnClickCell(QuickCastUI.QuickCastMenus selectedMenu);
    }
}
