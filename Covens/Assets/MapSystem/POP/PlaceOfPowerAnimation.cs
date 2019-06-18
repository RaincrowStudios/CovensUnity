using Raincrow.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceOfPowerAnimation : MonoBehaviour
{
    [SerializeField] private Transform m_GlyphTransform;
    [SerializeField] private SpriteRenderer[] m_GlyphSprites;
    [SerializeField] private SpriteRenderer m_SpiritGroundShadow;

    [SerializeField] private GameObject entryVFX;
    [SerializeField] private GameObject closingVFX;

    private Vector2 min;
    private Vector2 max;
    private Vector3 rot;
    [Header("Spirit")]

    private Material vignetterMat;
    private int m_PoPTweenId;
    private Coroutine m_AnimateSpiritCoroutine = null;
    private IMaps map;

    private void Awake()
    {
        map = MapsAPI.Instance;
        vignetterMat = RadialBlur.Instance.materialInstance;
        Color aux;

        foreach (SpriteRenderer _spr in m_GlyphSprites)
        {
            aux = _spr.color;
            aux.a = 0;
            _spr.color = aux;
        }

        m_GlyphTransform.localScale = Vector3.zero;

        //LeanTween.alpha(m_SpiritBackShadow.gameObject, 0f, 0f);
        LeanTween.alpha(m_SpiritGroundShadow.gameObject, 0f, 0f);
    }

    [ContextMenu("Show")]
    public void Show()
    {
        //moving the notification Popups to the bottom
        var p = PlayerNotificationManager.Instance.transform.GetChild(0);
        var o = p.GetComponent<RectTransform>();
        min = o.offsetMin;
        max = o.offsetMax;

        o.offsetMin = new Vector2(0f, -915f);
        o.offsetMax = new Vector2(0f, 1005f)*-1f;
        //
        //map.allowControl = false;
        SoundManagerOneShot.Instance.PlayEnYaSa();


        MapCameraUtils.SetZoom(.983f, 2f, false);
        MapCameraUtils.SetRotation(180, 2, false, null);


        //MapCameraUtils.SetZoom(1.1f, 2, false);//(.983f, 2, false)
        //MapCameraUtils.SetRotation(30, 2, false, null);
        //MapCameraUtils.SetCameraRotation(new Vector3(-7f,0f,0f), 2f, null);
    

        //LeanTween.value(0f,1f,2.1f).setOnComplete(() => {
          //  MapCameraUtils.SetZoom(.983f, 3f, false);
            //MapCameraUtils.SetCameraRotation(new Vector3(0f,0f,0f), 2f, null);
            //MapCameraUtils.SetRotation(60, 3f, false, null);
            //});

        MapsAPI.Instance.ScaleBuildings(0);

        LeanTween.cancel(m_PoPTweenId);

        float v2;
        Color aux;

        m_PoPTweenId = LeanTween.value(0f, 1f, 2f).setEaseOutCubic()
            .setOnStart(() =>
            {
                gameObject.SetActive(true);
            })
            .setOnUpdate((float v) =>
            {
                v2 = v * v;

                foreach (SpriteRenderer _spr in m_GlyphSprites)
                {
                    aux = _spr.color;
                    aux.a = v;
                    _spr.color = aux;
                }
                m_GlyphTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, v);
            })
            .uniqueId;
    }

    [ContextMenu("Hide")]
    public void Hide()
    {
        closingVFX.gameObject.SetActive(true);

        SoundManagerOneShot.Instance.SetBGTrack(PlayerDataManager.soundTrack);
        SoundManagerOneShot.Instance.PlayAHSAWhisper();
        float v2;
        Color aux;

        MapCameraUtils.SetZoom(1f, 2, false);
        MapCameraUtils.SetRotation(25, 2, false, null);

        LeanTween.cancel(m_PoPTweenId);

        m_PoPTweenId = LeanTween.value(1f, 0f, 1f).setEaseInCubic()
            .setOnUpdate((float v) =>
            {
                v2 = v * v;

                foreach (SpriteRenderer _spr in m_GlyphSprites)
                {
                    aux = _spr.color;
                    aux.a = v;
                    _spr.color = aux;
                }
                m_GlyphTransform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, v);
            })
            .setOnComplete(() =>
            {
                MapsAPI.Instance.ScaleBuildings(1);
                gameObject.SetActive(false);
            })
            .uniqueId;
            
        var p = PlayerNotificationManager.Instance.transform.GetChild(0);
        var o = p.GetComponent<RectTransform>();
        o.offsetMin = min;
        o.offsetMax = max;
    }

    public void AnimateWitchEntry(PlaceOfPowerPosition position)
    {
        Utilities.InstantiateObject(entryVFX, position.transform, 0.6f).SetActive(true);
    }

    public void AnimateSpirit(IMarker spirit)
    {
        //LeanTween.alpha(m_SpiritBackShadow.gameObject, 1f, 1f);
        LeanTween.alpha(m_SpiritGroundShadow.gameObject, 1f, 0.66f);

        if (m_AnimateSpiritCoroutine != null)
            StopCoroutine(m_AnimateSpiritCoroutine);
        m_AnimateSpiritCoroutine = StartCoroutine(AnimateSpiritCoroutine(spirit));
    }

    private IEnumerator AnimateSpiritCoroutine(IMarker spirit)
    {
        if (spirit != null)
        {
            while (spirit.isNull == false)
            {
                //m_SpiritBackShadow.transform.position = spirit.characterTransform.position + spirit.characterTransform.forward * 2;
                //m_SpiritBackShadow.transform.rotation = spirit.characterTransform.rotation;
                yield return 0;
            }
        }
    }
}
