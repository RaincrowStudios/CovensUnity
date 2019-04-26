using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Raincrow.Maps
{
    public class MuskMarker : MonoBehaviour, IMarker
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
            set { SetPosition(value.x, value.y); }
        }

        public System.Action<IMarker> m_OnClick;
        public System.Action<IMarker> OnClick
        {
            get { return m_OnClick; }
            set { m_OnClick = value; }
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
            this.transform.position = MapsAPI.Instance.GetWorldPosition(lng, lat);
        }

        public new GameObject gameObject
        {
            get { return base.gameObject; }
        }


        /******* NEW MARKER METHODS **********/

        protected bool m_Interactable = true;
        protected Token m_Data;
        MarkerSpawner.MarkerType m_Type;

        public bool interactable { get { return m_Interactable; } set { m_Interactable = value; } }
        public bool IsShowingIcon { get; protected set; }
        public bool IsShowingAvatar { get; protected set; }
        public virtual Transform characterTransform { get { return base.transform; } }
        public MarkerSpawner.MarkerType type { get { return m_Type; } }
        public Token token { get { return m_Data; } }

        protected const string m_ShadowColor = "#C100C8";
        protected const string m_GreyColor = "#00AFE4";
        protected const string m_WhiteColor = "#F48D00";

        public const float defaultTextAlpha = 0.35f;
        public const float highlightTextAlpha = 1f;

        public float alpha { get; protected set; }
        public float textAlpha { get; protected set; }
        public float multipliedAlpha { get; protected set; }

        protected SpriteRenderer[] m_Renderers;
        protected TextMeshPro[] m_TextMeshes;

        private List<Transform> m_ParentedObjects = new List<Transform>();

        private void Awake()
        {
            enabled = false;
            alpha = 1;
            multipliedAlpha = 1;
            textAlpha = 1;
            m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
        }

        public virtual void Setup(Token data)
        {
            m_Data = data;
            m_Type = data.Type;
        }

        public virtual void EnablePortait() { }

        public virtual void EnableAvatar() { }

        public virtual void SetStats(int level) { }

        public virtual void UpdateEnergy(int energy, int baseEnergy) { }


        public virtual void SetTextAlpha(float a)
        {
            textAlpha = a;
        }

        public virtual void SetCharacterAlpha(float a)
        {
            alpha = a;
        }

        public void MultiplyAlpha(float a)
        {
            multipliedAlpha = a;
            float targetAlpha = multipliedAlpha * alpha;

            Color aux;
            for (int i = 0; i < m_Renderers.Length; i++)
            {
                try
                {
                    aux = m_Renderers[i].color;
                    aux.a = targetAlpha;
                    m_Renderers[i].color = aux;
                }
                catch (System.Exception)
                {

                    return;
                }

            }

            for (int i = 0; i < m_TextMeshes.Length; i++)
                m_TextMeshes[i].alpha = textAlpha * targetAlpha;
        }

        public void AddChild(Transform t)
        {
            m_ParentedObjects.Add(t);
            t.SetParent(characterTransform);

            m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
        }

        public void AddCharacterChild(Transform t)
        {
            m_ParentedObjects.Add(t);
            t.SetParent(transform);

            m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
        }

        private void OnDestroy()
        {
            foreach (var t in m_ParentedObjects)
            {
                if (t.parent == characterTransform || t.parent == transform)
                    t.SetParent(null);
            }

            m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
        }
    }
}