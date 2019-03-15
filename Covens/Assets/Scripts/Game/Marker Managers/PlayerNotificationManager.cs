using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class PlayerNotificationManager : MonoBehaviour
{
    private static PlayerNotificationManager m_Instance;
    public static PlayerNotificationManager Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<PlayerNotificationManager>("UINotifications"));
            return m_Instance;
        }
    }

    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;

    [SerializeField] private PlayerNotificationItem m_NotificationItemPrefab;
    [SerializeField] private int m_MaxAmount = 3;
    [SerializeField] private LayoutGroup m_LayoutGroup;

    public Sprite spirit;
	public Sprite spellBookIcon;

    private SimplePool<PlayerNotificationItem> m_ItemPool;
    private List<string> m_MessageQueue = new List<string>();
    private List<Sprite> m_IconQueue = new List<Sprite>();
    private int m_Showing = 0;

	void Awake()
	{
		m_Instance = this;
        m_ItemPool = new SimplePool<PlayerNotificationItem>(m_NotificationItemPrefab, 0);
        m_Showing = 0;
        m_Canvas.enabled = false;
        m_InputRaycaster.enabled = false;
        m_LayoutGroup.enabled = false;
    }
	
	public void ShowNotification(string message, Sprite icon = null)
	{
        m_MessageQueue.Add(message);
        m_IconQueue.Add(icon);

        if (m_Showing == 0)
            StartCoroutine(ShowNotificationsCoroutine());
	}

    private void ShowNotification(string message, WitchMarker marker)
    {
        ShowNotification(message, marker.portrait);
    }

    private IEnumerator ShowNotificationsCoroutine()
    {
        string text;
        Sprite icon;

        m_Canvas.enabled = true;
        m_InputRaycaster.enabled = true;
        m_LayoutGroup.enabled = true;

        while (m_MessageQueue.Count > 0 || m_Showing > 0)
        {
            for (int i = m_Showing; i < m_MaxAmount && m_MessageQueue.Count > 0; i++)
            {
                text = m_MessageQueue[0];
                icon = m_IconQueue[0];

                m_MessageQueue.RemoveAt(0);
                m_IconQueue.RemoveAt(0);

                PlayerNotificationItem notification = m_ItemPool.Spawn(m_LayoutGroup.transform);
                notification.Show(text, icon, () =>
                {
                    m_Showing -= 1;
                    m_ItemPool.Despawn(notification);
                    if (m_Showing == 0)
                    {
                        m_Canvas.enabled = false;
                        m_InputRaycaster.enabled = false;
                        m_LayoutGroup.enabled = false;
                    }
                });

                m_Showing += 1;
            }

            yield return 0;
        }
    }
}

