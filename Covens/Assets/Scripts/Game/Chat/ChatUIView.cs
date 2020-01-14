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
        [SerializeField] private UIChatPlayerLocation _chatLocationPrefab;
        [SerializeField] private UIChatHelp _chatHelpPlayerPrefab;
        [SerializeField] private UIChatHelp _chatHelpCrowPrefab;
        [SerializeField] private UIChatPlayerImage _chatImagePrefab;
        [SerializeField] private UIChatNpc _chatNpcPrefab;


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
            if (eCategory == ChatCategory.SUPPORT)
            {
                pMessages.Insert(0, new ChatMessage
                {
                    data = new ChatMessageData
                    {
                        message = LocalizeLookUp.GetText("help_call_crows_confirm")
                    },
                    timestamp = (long)Utilities.GetUnixTimestamp(System.DateTime.UtcNow)
                });
            }

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

                    case MessageType.BOSS:
                    case MessageType.NPC:
                        cellView = scroller.GetCellView(_chatNpcPrefab) as UIChatItem;
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
                UIChatMessage prefab = null;

                if (m_pCategory == ChatCategory.SUPPORT)
                {
                    prefab = _chatHelpPlayerPrefab as UIChatMessage;
                }
                else
                {
                    switch (pItem.type)
                    {
                        case MessageType.IMAGE:
                            prefab = _chatImagePrefab as UIChatMessage;
                            break;
                        case MessageType.LOCATION:
                            prefab = _chatLocationPrefab as UIChatMessage;
                            break;
                        case MessageType.BOSS:
                        case MessageType.NPC:
                            prefab = _chatNpcPrefab as UIChatMessage;
                            break;
                        case MessageType.TEXT:
                            prefab = _chatMessagePrefab as UIChatMessage;
                            break;
                    }
                }

                if (prefab != null)
                {
                    pItem.height = Mathf.Max(prefab.GetHeight(pItem), 150);
                }
                else
                {
                    pItem.height = 150;
                }
            }
            return pItem.height;
        }
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return m_pChatMessages.Count;
        }
    }


}