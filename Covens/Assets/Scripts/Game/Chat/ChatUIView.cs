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
            //Debug.Log($"SetupData: {eCategory} mess[{pMessages.Count}]");
            m_pChatMessages = pMessages;
            m_pCategory = eCategory;
            m_onRequestChatLoading = onRequestChatLoading;
            m_onRequestChatClose = onRequestChatClose;
            scroller.ReloadData(1f);
        }
        public void AddMessage(ChatMessage pMessage)
        {
            m_pChatMessages.Add(pMessage);
            scroller.ReloadData();
        }
        public void Refresh()
        {
            scroller.ReloadData(1f);
        }


        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            UIChatItem cellView = null;
            int iItemIdx = dataIndex;// m_pChatMessages.Count - 1 - dataIndex;
            var pItem = m_pChatMessages[iItemIdx];
            //Debug.Log($"iItemIdx: [{iItemIdx}]: {pItem.data.message}");
            
            //var pItem = m_pChatMessages[dataIndex];
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
                }
            }

            cellView.SetupMessage(pItem, m_onRequestChatLoading, m_onRequestChatClose);
            cellView.transform.localScale = Vector3.one;
            return cellView;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            var pItem = m_pChatMessages[dataIndex];
            if (m_pCategory == ChatCategory.SUPPORT)
            {
                if (pItem.player.name == ChatManager.Player.name)
                    return _chatHelpPlayerPrefab.Height;
                else
                    return _chatHelpCrowPrefab.Height;
            }
            switch (pItem.type)
            {
                case MessageType.IMAGE:
                    return _chatImagePrefab.Height;
                case MessageType.LOCATION:
                    return _chatLocationPrefab.Height;
            }
            return _chatMessagePrefab.Height;
        }

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return m_pChatMessages.Count;
        }
    }


}