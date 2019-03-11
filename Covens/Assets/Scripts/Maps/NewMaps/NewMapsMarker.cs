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
            //todo: dont click if holding for too long

            //dont trigger if clicking over an UI element
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
                return;

            if (m_Interactable)
                m_OnClick?.Invoke(this);
        }


        /******* NEW MARKER METHODS **********/

        protected bool m_Interactable = true;
        public bool interactable { get { return m_Interactable; } set { m_Interactable = value; } }

        protected Token m_Data;
        MarkerSpawner.MarkerType m_Type;
        
        public bool IsShowingIcon { get; protected set; }
        public bool IsShowingAvatar { get; protected set; }

        public virtual Transform characterTransform { get { return transform; } }

        protected const string m_ShadowColor = "#C100C8";
        protected const string m_GreyColor = "#00AFE4";
        protected const string m_WhiteColor = "#F48D00";

        public const float defaultTextAlpha = 0.35f;
        public const float highlightTextAlpha = 1f;

        private void Awake()
        {
            enabled = false;
            IsShowingIcon = true;
            IsShowingAvatar = true;
        }

        public virtual void Setup(Token data)
        {
            m_Data = data;
            m_Type = data.Type;
        }

        public virtual void EnablePortait() { }

        public virtual void EnableAvatar() { }

        public virtual void SetStats(int level, int energy) { }

        public virtual void SetupAvatar(bool male, List<EquippedApparel> equips) { }

        public virtual void SetupAvatarAndPortrait(bool male, List<EquippedApparel> equips) { }
                
        public virtual void SetTextAlpha(float a) { }

        public virtual void SetAlpha(float a) { }
    }
}