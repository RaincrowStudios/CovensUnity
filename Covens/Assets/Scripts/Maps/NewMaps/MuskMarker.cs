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

        public Vector2 coords { get; set; }
        
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

        public new GameObject gameObject
        {
            get { return base.gameObject; }
        }


        /******* NEW MARKER METHODS **********/

        protected bool m_Interactable = true;
        protected Token m_Data;
        MarkerSpawner.MarkerType m_Type;

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

        public float characterAlpha { get; protected set; }
        public float textAlpha { get; protected set; }
        public float alpha { get; protected set; }

        protected SpriteRenderer[] m_Renderers;
        [SerializeField] protected SpriteRenderer m_AvatarRenderer;
        protected TextMeshPro[] m_TextMeshes;

        private List<System.Action> m_ParentedObjects = new List<System.Action>();

        private int m_MoveTweenId;
        private int m_AlphaTweenId;

        public bool interactable
        {
            get { return m_Interactable; }
            set
            {
                Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
                foreach (Collider _col in colliders)
                    _col.enabled = value;
                m_Interactable = value;
            }
        }

        private void Awake()
        {
            enabled = false;
            characterAlpha = 1;
            alpha = 1;
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
            characterAlpha = a;
        }

        public virtual void SetAlpha(float a, float time = 0, System.Action onComplete = null)
        {
            LeanTween.cancel(m_AlphaTweenId);

            if (time == 0)
            {
                alpha = a;

                Color aux;
                for (int i = 0; i < m_Renderers.Length; i++)
                {
                    aux = m_Renderers[i].color;
                    aux.a = alpha;
                    m_Renderers[i].color = aux;
                }

                aux = m_AvatarRenderer.color;
                aux.a = alpha * characterAlpha;
                m_AvatarRenderer.color = aux;

                for (int i = 0; i < m_TextMeshes.Length; i++)
                    m_TextMeshes[i].alpha = textAlpha * alpha;

                onComplete?.Invoke();
            }
            else
            {
                m_AlphaTweenId = LeanTween.value(alpha, a, time)
                    .setEaseOutCubic()
                    .setOnUpdate((float t) =>
                    {
                        alpha = t;

                        Color aux;
                        for (int i = 0; i < m_Renderers.Length; i++)
                        {
                            aux = m_Renderers[i].color;
                            aux.a = alpha;
                            m_Renderers[i].color = aux;
                        }

                        for (int i = 0; i < m_TextMeshes.Length; i++)
                            m_TextMeshes[i].alpha = textAlpha * alpha;
                    })
                    .setOnComplete(onComplete)
                    .uniqueId;
            }
        }

        public void AddChild(Transform t, System.Action onDestroy)
        {
            m_ParentedObjects.Add(onDestroy);

            t.SetParent(transform);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;

            m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
        }

        public void AddCharacterChild(Transform t, System.Action onDestroy)
        {
            m_ParentedObjects.Add(onDestroy);

            t.SetParent(characterTransform);
            t.localPosition = Vector3.zero;
            t.localScale = Vector3.one;
            t.localRotation = Quaternion.identity;

            m_Renderers = GetComponentsInChildren<SpriteRenderer>(true);
            m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
        }

        public void DespawnParented()
        {
            foreach (var t in m_ParentedObjects)
            {
                t?.Invoke();
            }

            m_ParentedObjects.Clear();
        }

        public void SetWorldPosition(Vector3 worldPos, float time = 0, System.Action onComplete = null)
        {
            LeanTween.cancel(m_MoveTweenId);

            if (time == 0)
            {
                transform.position = worldPos;
                MarkerSpawner.Instance.UpdateMarker(this);
                onComplete?.Invoke();
                return;
            }

            Vector3 startPos = transform.position;

            m_MoveTweenId = LeanTween.value(0, 1, time)
                .setEaseOutCubic()
                .setOnUpdate((float t) =>
                {
                    transform.position = Vector3.Lerp(startPos, worldPos, t);
                    MarkerSpawner.Instance.UpdateMarker(this);
                })
                .setOnComplete(onComplete)
                .uniqueId;
        }

        private void OnDestroy()
        {
            LeanTween.cancel(m_MoveTweenId);
        }
    }
}