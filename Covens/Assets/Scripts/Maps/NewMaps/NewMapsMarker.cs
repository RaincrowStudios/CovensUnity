using Mapbox.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Raincrow.Maps
{
    public class NewMapsMarker : MonoBehaviour, IMarker
    {
        private object m_CustomData;
        public object customData
        {
            get { return m_CustomData; }
            set { m_CustomData = value; }
        }

        Vector2 m_Position;
        public Vector2 position
        {
            get { return m_Position; }
            set
            {
                m_Position = value;
                transform.position = MapController.Instance.CoordsToWorldPosition(value.x, value.y);
            }
        }

        public System.Action<IMarker> m_OnClick;
        public System.Action<IMarker> OnClick
        {
            get { return m_OnClick; }
            set
            {
                m_OnClick = value;
            }
        }

        public float scale
        {
            get { return transform.localScale.x; }
            set { transform.localScale = new Vector3(value, value, value); }
        }

        public GameObject gameObject { get { return base.gameObject; } }

        public bool inMapView { get; set; }
        //{
        //    get
        //    {
        //        //replace for faster method?
        //        Vector2 view = MapController.Instance.camera.WorldToViewportPoint(transform.position);
        //        return view.x >= 0 && view.x <= 1 && view.y >= 0 && view.y <= 1;
        //    }
        //}

        public void SetPosition(double lng, double lat)
        {
            m_Position = new Vector2((float)lng, (float)lat);
            this.transform.position = MapController.Instance.CoordsToWorldPosition(lng, lat);
        }

        private void OnMouseUpAsButton()
        {
            //dont trigger if clicking over an UI element
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            if (m_Interactable)
                m_OnClick?.Invoke(this);
        }


        /******* NEW MARKER METHODS **********/

        private Token m_Data;
        MarkerSpawner.MarkerType m_Type;

        private Transform m_IconGroup;
        private Transform m_AvatarGroup;
        private Transform m_Character;
        private TextMeshPro m_DisplayName;
        private TextMeshPro m_Stats;

        private int m_TweenId;
        private Transform m_CameraCenter;

        public bool IsShowingIcon { get; private set; }
        public bool IsShowingAvatar { get; private set; }

        private const string m_ShadowColor = "#C100C8";
        private const string m_GreyColor = "#00AFE4";
        private const string m_WhiteColor = "#F48D00";

        private void Awake()
        {
            enabled = false;
        }

        public void Setup(Token data)
        {
            m_Data = data;
            m_Type = data.Type;
            m_CameraCenter = MapController.Instance.m_StreetMap.cameraCenter;

            if (data.Type == MarkerSpawner.MarkerType.witch || data.Type == MarkerSpawner.MarkerType.spirit)
            {
                m_AvatarGroup = transform.GetChild(0);
                m_Character = m_AvatarGroup.GetChild(0);
                m_IconGroup = transform.GetChild(1);

                m_AvatarGroup.localScale = Vector3.zero;
                m_IconGroup.localScale = Vector3.zero;

                m_AvatarGroup.gameObject.SetActive(false);
                m_IconGroup.gameObject.SetActive(false);

                IsShowingAvatar = false;
                IsShowingIcon = false;

                if (data.Type == MarkerSpawner.MarkerType.witch)
                {
                    m_DisplayName = m_AvatarGroup.GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
                    m_DisplayName.text = data.displayName;
                    m_DisplayName.alpha = defaultTextAlpha;

                    m_Stats = m_AvatarGroup.GetChild(0).GetChild(2).GetComponent<TextMeshPro>();
                    m_Stats.alpha = defaultTextAlpha;
                    SetStats(data.level, data.energy);

                    //setup the school particle fx
                    Transform iconSchools = transform.GetChild(1).GetChild(0).GetChild(0);
                    Transform avatarSchools = transform.GetChild(0).GetChild(1);

                    for (int i = 0; i < 3; i++)
                    {
                        iconSchools.GetChild(i).gameObject.SetActive(false);
                        avatarSchools.GetChild(i).gameObject.SetActive(false);
                    }

                    if (data.degree < 0)
                    {
                        avatarSchools.GetChild(0).gameObject.SetActive(true);
                        iconSchools.GetChild(0).gameObject.SetActive(true);
                    }
                    else if (data.degree == 0)
                    {
                        avatarSchools.GetChild(1).gameObject.SetActive(true);
                        iconSchools.GetChild(1).gameObject.SetActive(true);
                    }
                    else
                    {
                        avatarSchools.GetChild(2).gameObject.SetActive(true);
                        iconSchools.GetChild(2).gameObject.SetActive(true);
                    }
                }
                else if (data.Type == MarkerSpawner.MarkerType.spirit)
                {
                    m_Stats = m_AvatarGroup.GetChild(0).GetChild(1).GetComponent<TextMeshPro>();
                    SetStats(0, data.energy);
                }

                enabled = true;
            }
            else
            {
                IsShowingIcon = true;
                IsShowingAvatar = true;
            }
        }

        public void EnablePortaitIcon()
        {
            if (IsShowingIcon)
                return;

            m_Interactable = false;

            IsShowingIcon = true;
            IsShowingAvatar = false;

            LeanTween.cancel(m_TweenId);

            m_TweenId = LeanTween.value(m_IconGroup.localScale.x, 1, 0.5f)
                .setEaseOutCubic()
                .setOnStart(() =>
                {
                    m_IconGroup.gameObject.SetActive(true);
                })
                .setOnUpdate((float t) =>
                {
                    m_IconGroup.localScale = new Vector3(t, t, t);
                    m_AvatarGroup.localScale = new Vector3(1 - t, 1 - t, 1 - t);
                })
                .setOnComplete(() =>
                {
                    m_AvatarGroup.gameObject.SetActive(false);
                })
                .uniqueId;
        }

        public void EnableAvatar()
        {
            if (IsShowingAvatar)
                return;

            //     Debug.LogError("ENabling avatar");

            m_Interactable = true;

            IsShowingAvatar = true;
            IsShowingIcon = false;

            LeanTween.cancel(m_TweenId);

            m_TweenId = LeanTween.value(m_AvatarGroup.localScale.x, 1, 0.5f)
                .setEaseOutCubic()
                .setOnStart(() =>
                {
                    m_AvatarGroup.gameObject.SetActive(true);
                })
                .setOnUpdate((float t) =>
                {
                    m_AvatarGroup.localScale = new Vector3(t, t, t);
                    m_IconGroup.localScale = new Vector3(1 - t, 1 - t, 1 - t);
                })
                .setOnComplete(() =>
                {
                    m_IconGroup.gameObject.SetActive(false);
                })
                .uniqueId;
        }

        public void SetStats(int level, int energy)
        {
            if (m_Stats == null)
                return;

            if (m_Type == MarkerSpawner.MarkerType.witch)
            {
                string color = "";
                if (m_Data.degree < 0) color = m_ShadowColor;
                else if (m_Data.degree == 0) color = m_GreyColor;
                else color = m_WhiteColor;

                m_Stats.text =
                    $"Energy: <color={color}><b>{energy}</b></color>\n" +
                    $"lvl: <color={color}><b>{level}</b></color>";
            }
            else if (m_Type == MarkerSpawner.MarkerType.spirit)
            {
                m_Stats.text = $"Energy: <color=#4C80FD><b>{energy}</b></color>\n";
            }
        }

        public void SetupAvatar(bool male, List<EquippedApparel> equips)
        {
            if (m_AvatarGroup == null)
            {
                m_AvatarGroup = transform.GetChild(0);
                m_Character = m_AvatarGroup.GetChild(0);
            }

            //shadow scale
            //m_AvatarGroup.GetChild(2).localScale = male ? new Vector3(8, 8, 8) : new Vector3(6, 6, 6);

            //generate sprites for avatar and icon
            AvatarSpriteUtil.Instance.GenerateFullbodySprite(male, equips, spr =>
            {
                SpriteRenderer renderer = m_Character.GetChild(0).GetComponent<SpriteRenderer>();
                renderer.transform.localPosition = Vector3.zero;
                //renderer.transform.localScale = new Vector3(8, 8, 8);
                renderer.sprite = spr;
            });
        }

        public void SetupAvatarAndPortrait(bool male, List<EquippedApparel> equips)
        {
            if (m_AvatarGroup == null)
            {
                m_AvatarGroup = transform.GetChild(0);
                m_Character = m_AvatarGroup.GetChild(0);
                m_IconGroup = transform.GetChild(1);
            }

            //shadow scale
            //  m_AvatarGroup.GetChild(2).localScale = male ? new Vector3(8, 8, 8) : new Vector3(6, 6, 6);

            //generate the sprites
            AvatarSpriteUtil.Instance.GeneratePortraitAndFullbody(male, equips,
                portrait =>
                {
                    //portrait
                    SpriteRenderer renderer = m_IconGroup.GetChild(0).GetComponent<SpriteRenderer>();
                    //renderer.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
                    renderer.sprite = portrait;
                },
                avatar =>
                {
                    SpriteRenderer renderer = m_Character.GetChild(0).GetComponent<SpriteRenderer>();
                    renderer.transform.localPosition = Vector3.zero;
                    //renderer.transform.localScale = new Vector3(8, 8, 8);
                    renderer.sprite = avatar;
                });
        }

        //private float m_Distance;
        //private void Update()
        //{
        //    m_Distance = Vector2.Distance(
        //        new Vector2(m_CameraCenter.position.x, m_CameraCenter.position.z), new Vector2(transform.position.x, transform.position.z));

        //    if (m_Distance < 55)
        //        EnableAvatar();
        //    else
        //        EnablePortaitIcon();
        //}

        private bool m_Interactable = true;
        public bool interactable { get { return m_Interactable; } set { m_Interactable = value; } }

        public Transform characterTransform { get { return transform.GetChild(0).GetChild(0); } }

        public static float defaultTextAlpha = 0.35f;
        public static float highlightTextAlpha = 1f;

        public void SetTextAlpha(float a)
        {
            if (m_Stats == null)
                return;

            LeanTween.value(m_DisplayName.alpha, a, 0.3f)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    m_DisplayName.alpha = t;
                    m_Stats.alpha = t;
                });
        }
    }
}