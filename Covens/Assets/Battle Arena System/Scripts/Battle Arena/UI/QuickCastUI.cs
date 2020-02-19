using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.View;
using TMPro;

namespace Raincrow.BattleArena.UI
{
    public class QuickCastUI : MonoBehaviour
    {
        [SerializeField] private BattleController m_BattleController;

        [Header("UI")]
        [SerializeField] private Image m_ImageIcon;
        [SerializeField] private Sprite m_IconClose;
        [SerializeField] private Sprite m_IconOpen;
        [SerializeField] private TextMeshProUGUI m_TextAmountActions;

        [Header("Menus")]
        [SerializeField] private GameObject m_ActionsMenu;


        private GameObject currentMenu;
        private bool open;

        private const float TimeToToggle = 0.05f;

        public void OnClickCell(CellView cell)
        {
            if (cell.IsEmpty)
            {
                ChangeMenu(m_ActionsMenu);
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
            
            if (currentMenu == null)
            {
                LeanTween.scaleY(menu, 1.0f, TimeToToggle);
            } else
            {
                LeanTween.scaleY(currentMenu, 0.0f, TimeToToggle).setOnComplete(() => {
                    currentMenu.SetActive(false);
                    LeanTween.scaleY(menu, 1.0f, TimeToToggle).setDelay(0.05f);
                });
            }

            currentMenu = menu;
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
}
