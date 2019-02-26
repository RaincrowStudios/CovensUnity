using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWaitingCastResult : MonoBehaviour
{
    [SerializeField] private Canvas m_Canvas;
    [SerializeField] private GraphicRaycaster m_InputRaycaster;
    [SerializeField] private RectTransform m_MainPanel;
    [SerializeField] private CanvasGroup m_CanvasGroup;


    [Header("casting")]
    [SerializeField] private Image m_SpellGlyph;
    [SerializeField] private Image m_ShadowCrest;
    [SerializeField] private Image m_GreyCrest;
    [SerializeField] private Image m_WhiteCrest;

    [SerializeField] private GameObject m_ShadowFX;
    [SerializeField] private GameObject m_GreyFX;
    [SerializeField] private GameObject m_WhiteFX;

    private static UIWaitingCastResult m_Instance;
    public static UIWaitingCastResult Instance
    {
        get
        {
            if (m_Instance == null)
                m_Instance = Instantiate(Resources.Load<UIWaitingCastResult>("UIWaitingCastResult"));
            return m_Instance;
        }
    }

    private float m_ShowTime;

    private void Awake()
    {
        m_Instance = this;

        EnableCanvas(false);

        m_ShadowCrest.gameObject.SetActive(false);
        m_GreyCrest.gameObject.SetActive(false);
        m_WhiteCrest.gameObject.SetActive(false);

        m_ShadowFX.SetActive(false);
        m_GreyFX.SetActive(false);
        m_WhiteFX.SetActive(false);

        m_CanvasGroup.alpha = 0;
        m_MainPanel.anchoredPosition = new Vector2(m_MainPanel.sizeDelta.x, 0);
    }

    private int m_TweenId;

    public void Show(IMarker target, SpellData spell)
    {
        LeanTween.cancel(m_TweenId);

        //setup
        Image crest;
        GameObject fx;

        if (spell.school < 0)
        {
            crest = m_ShadowCrest;
            fx = m_ShadowFX;
        }
        else if (spell.school > 0)
        {
            crest = m_WhiteCrest;
            fx = m_WhiteFX;
        }
        else
        {
            crest = m_GreyCrest;
            fx = m_GreyFX;
        }

        crest.gameObject.SetActive(true);
        Color aux = new Color(1, 1, 1, 0);
        m_SpellGlyph.color = aux;

        //load the glyph icon
        string baseSpell = string.IsNullOrEmpty(spell.baseSpell) ? spell.id : spell.baseSpell;
        DownloadedAssets.GetSprite(baseSpell, 
            (spr) => 
            {
                m_SpellGlyph.sprite = spr;

                //show the spell glyph
                LeanTween.value(0, 1, 1f)
                    .setEaseOutCubic()
                    .setOnUpdate((float t) =>
                    {
                        aux.a = t;
                        m_SpellGlyph.color = aux;
                    })
                    .setDelay(0.25f);
            });

        //activate the fx after few moments
        LeanTween.value(0, 1, 0.65f)
            .setOnComplete(() =>
            {
                fx.gameObject.SetActive(true);
            });

        EnableCanvas(true);
        m_TweenId = LeanTween.value(0, 1, 0.5f)
           .setOnUpdate((float t) =>
           {
               m_MainPanel.anchoredPosition = new Vector2((1 - t) * m_MainPanel.sizeDelta.x, 0);
               m_CanvasGroup.alpha = t;
           })
           .setEaseOutCubic()
           .uniqueId;

        m_ShowTime = Time.time;
    }

    public void Close(System.Action onFinish = null)
    {
        float timeSinceOpen = Time.time - m_ShowTime;
        float minTime = 3f;
        float delay;

        if (timeSinceOpen < minTime)
            delay = minTime - timeSinceOpen;
        else
            delay = 0;

        m_InputRaycaster.enabled = false;
        m_TweenId = LeanTween.value(0, 1, 0.5f)
            .setOnStart(() => { onFinish?.Invoke(); })
           .setOnUpdate((float t) =>
           {
               m_MainPanel.anchoredPosition = new Vector2(t * m_MainPanel.sizeDelta.x, 0);
               m_CanvasGroup.alpha = 1 - t;
           })
           .setOnComplete(() =>
           {
               EnableCanvas(false);

               m_ShadowCrest.gameObject.SetActive(false);
               m_GreyCrest.gameObject.SetActive(false);
               m_WhiteCrest.gameObject.SetActive(false);

               m_ShadowFX.SetActive(false);
               m_GreyFX.SetActive(false);
               m_WhiteFX.SetActive(false);
           })
           .setEaseOutCubic()
           .setDelay(delay)
           .uniqueId;
    }

    public void EnableCanvas(bool enable)
    {
        m_Canvas.enabled = enable;
        m_InputRaycaster.enabled = enable;
    }
}
