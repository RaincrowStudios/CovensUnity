using EnhancedScrollerDemos.CellEvents;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Raincrow.Chat.UI
{
    public class ChatUIView : MonoBehaviour, IEnhancedScrollerDelegate
    {
        public EnhancedScroller scroller;

        [Header("Chat item Prefabs")]
        [SerializeField] private UIChatMessage _chatMessagePrefab;
        [SerializeField] private UIChatLocation _chatLocationPrefab;
        [SerializeField] private UIChatHelp _chatHelpPlayerPrefab;
        [SerializeField] private UIChatHelp _chatHelpCrowPrefab;
        [SerializeField] private UIChatImage _chatImagePrefab;


        private List<ChatMessage> m_pChatMessages = new List<ChatMessage>();
        private ChatCategory m_pCategory = ChatCategory.WORLD;
        private UnityAction<bool> m_onRequestChatLoading;
        private UnityAction m_onRequestChatClose;

        private void Awake()
        {
            scroller.Delegate = this;
        }

        public void SetupData(List<ChatMessage> pMessages, ChatCategory eCategory, UnityAction<bool> onRequestChatLoading = null, UnityAction onRequestChatClose = null)
        {
            m_pChatMessages = pMessages;
            m_pCategory = eCategory;
            m_onRequestChatLoading = onRequestChatLoading;
            m_onRequestChatClose = onRequestChatClose;
            scroller.ReloadData(1f);
        }
        public void Refresh()
        {
            scroller.ReloadData(1f);
        }


        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            UIChatItem cellView = null;
            int iItemIdx = dataIndex;
            ChatMessage pItem = m_pChatMessages[iItemIdx];
            if (m_pCategory == ChatCategory.SUPPORT)
            {
                if (pItem.player.name == ChatManager.Player.name)
                {
                    cellView = scroller.GetCellView(_chatHelpPlayerPrefab) as UIChatItem;
                }
                else
                {
                    cellView = scroller.GetCellView(_chatHelpCrowPrefab) as UIChatItem;
                }
            }
            else
            {
                switch (pItem.type)
                {
                    case MessageType.IMAGE:
                        cellView = scroller.GetCellView(_chatImagePrefab) as UIChatItem;
                        break;

                    case MessageType.LOCATION:
                        cellView = scroller.GetCellView(_chatLocationPrefab) as UIChatItem;
                        break;

                    case MessageType.TEXT:
                        cellView = scroller.GetCellView(_chatMessagePrefab) as UIChatItem;
                        break;
                    default:
                        cellView = scroller.GetCellView(_chatMessagePrefab) as UIChatItem;
                        break;
                }
            }

            cellView.SetupMessage(pItem, m_onRequestChatLoading, m_onRequestChatClose);
            cellView.transform.localScale = Vector3.one;
            return cellView;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return GetTemplateHeight(m_pChatMessages[dataIndex]);
        }
        float GetTemplateHeight(ChatMessage pItem)
        {
            if (pItem.height == 0)
            {
                float fHeight = 0;
                if (m_pCategory == ChatCategory.SUPPORT)
                {
                    if (pItem.player.name == ChatManager.Player.name)
                        fHeight = ((RectTransform)_chatHelpPlayerPrefab.transform).sizeDelta.y;
                    else
                        fHeight = ((RectTransform)_chatHelpCrowPrefab.transform).sizeDelta.y;
                }
                switch (pItem.type)
                {
                    case MessageType.IMAGE:
                        fHeight = ((RectTransform)_chatImagePrefab.transform).sizeDelta.y;
                        break;
                    case MessageType.LOCATION:
                        fHeight = ((RectTransform)_chatLocationPrefab.transform).sizeDelta.y;
                        break;
                }
                if (fHeight == 0)
                {
                    fHeight = ((RectTransform)_chatMessagePrefab.transform).sizeDelta.y;
                }
                UIChatMessage pPrefab = _chatMessagePrefab as UIChatMessage;
                float fHeightText = pPrefab.GetHeight(pItem);
                pItem.height = Mathf.Max(150f, fHeight, fHeightText);
            }
            return pItem.height;
        }
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return m_pChatMessages.Count;
        }
    }


}