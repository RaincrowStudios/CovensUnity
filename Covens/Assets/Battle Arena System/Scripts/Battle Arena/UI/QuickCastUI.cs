using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.View;
using Raincrow.BattleArena.Model;
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
        [SerializeField] private GameObject m_SpellMenu;


        private CellView selectedView;
        private GameObject currentMenu;
        private bool open;

        private const float TimeToToggle = 0.05f;

        public void OnClickMove()
        {
            if (m_BattleController.TurnController.RemainingActions <= 0)
            {
                return;
            }

            BattleSlot slot = new BattleSlot() { Col = selectedView.CellModel.Y, Row = selectedView.CellModel.X };
            m_BattleController.TurnController.AddAction(new MoveActionModel() { Position = slot });

            m_TextAmountActions.text = m_BattleController.TurnController.RemainingActions.ToString();
        }

        public void OnClickSummon()
        {
            if (m_BattleController.TurnController.RemainingActions <= 0)
            {
                return;
            }

            UIMainScreens.PushEventAnalyticUI(UIMainScreens.Arena, UIMainScreens.SummonArena);
            UISummoning.Open(AddActionSummon);
        }

        private void AddActionSummon(string spiritID)
        {
            BattleSlot slot = new BattleSlot() { Col = selectedView.CellModel.Y, Row = selectedView.CellModel.X };
            m_BattleController.TurnController.AddAction(new SummonActionModel() { Position = slot, SpiritId = spiritID });

            m_TextAmountActions.text = m_BattleController.TurnController.RemainingActions.ToString();
        }

        public void OnClickCell(CellView cell)
        {
            selectedView = cell;

            if (cell.IsEmpty)
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
}
