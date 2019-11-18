using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Raincrow.Maps
{
    public class MuskMarker : MonoBehaviour, IMarker
    {
        private static int HIDDEN_STATE_ID = Animator.StringToHash("hidden");

        [Header("Base Marker")]
        [SerializeField] protected SpriteRenderer m_AvatarRenderer;
        [SerializeField] protected Animator m_Animator;
        //[SerializeField] protected SpriteRenderer[] m_Shadows;

        protected bool m_Interactable = true;

        public GameObject GameObject { get { return gameObject; } }
        public virtual Transform AvatarTransform { get { return transform; } }

        public MarkerManager.MarkerType Type { get; private set; }
        public bool IsShowingIcon { get; set; }
        public bool IsShowingAvatar { get; set; }

        public Vector2 Coords { get; set; }
        public System.Action<IMarker> OnClick { get; set; }

        public float AvatarAlpha { get; protected set; }
        public float TextAlpha { get; protected set; }
        public float Alpha { get; protected set; }

        public Token Token { get; protected set; }

        public bool IsPlayer => Token.instance == PlayerDataManager.playerData.instance; //(Object)PlayerManager.marker == this;

        public virtual string Name => "";


        //protected List<SpriteRenderer> m_Renderers;
        //protected List<SpriteRenderer> m_CharacterRenderers;
        //protected TextMeshPro[] m_TextMeshes;
        //protected ParticleSystem[] m_Particles;

        protected float m_CharacterAlphaMul = 1f;
        protected int m_MoveTweenId;
        protected int m_AlphaTweenId;
        protected int m_CharacterAlphaTweenId;

        protected Color m_SchoolColor = Color.white;

        protected List<Transform> m_SpawnedItems = new List<Transform>();

        public float scale
        {
            get { return transform.localScale.x; }
            set { transform.localScale = new Vector3(value, value, value); }
        }

        public bool inMapView { get; set; }

        //public bool isNull { get { return this == null || this.GameObject == null; } }

        public virtual void OnDespawn()
        {
            LeanTween.cancel(m_AlphaTweenId);
            LeanTween.cancel(m_MoveTweenId);
            LeanTween.cancel(m_CharacterAlphaTweenId);

            OnClick = null;
            //Token = null;

            for (int i = 0; i < m_SpawnedItems.Count; i++)
            {
                if (m_SpawnedItems[i] != null)
                    Destroy(m_SpawnedItems[i].gameObject);
            }
            m_SpawnedItems.Clear();
        }

        public bool Interactable
        {
            get { return m_Interactable; }
            set
            {
                if (GameObject != null)
                {
                    Collider[] colliders = GameObject.GetComponentsInChildren<Collider>(true);
                    foreach (Collider _col in colliders)
                        _col.enabled = value;
                }
                m_Interactable = value;
            }
        }

        private void Awake()
        {
            //enabled = false;
            AvatarAlpha = 1;
            Alpha = 1;
            TextAlpha = 1;

            //m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
            //m_Particles = GetComponentsInChildren<ParticleSystem>(true);

            //if (m_AvatarRenderer != null)
            //    m_CharacterRenderers = new List<SpriteRenderer> { m_AvatarRenderer };
            //else
            //    m_CharacterRenderers = new List<SpriteRenderer>();

            //if (m_Shadows == null)
            //    m_Shadows = new SpriteRenderer[0];

            //UpdateRenderers();
        }

        public virtual void Setup(Token data)
        {
            Token = data;
            Type = data.Type;
        }

        public virtual void ScaleNamePlate(bool scaleUp, float time = 1)
        {
        }

        public virtual void EnablePortait() { }

        public virtual void EnableAvatar() { }

        public virtual void SetStats() { }

        public virtual void UpdateEnergy()
        {
            
        }

        public virtual void UpdateNameplate(float preferredWidth)
        {
            
        }

        public virtual void SetHidden(bool hidden)
        {
            m_Animator.SetBool(HIDDEN_STATE_ID, hidden);
        }

        //public void SetTextAlpha(float a)
        //{
        //    if (this == null)
        //        return;

        //    TextAlpha = a;

        //    for (int i = 0; i < m_TextMeshes.Length; i++)
        //        m_TextMeshes[i].alpha = TextAlpha * Alpha;
        //}

        //public void SetCharacterAlpha(float a, float time = 0, System.Action onComplete = null)
        //{
        //    if (this == null)
        //        return;

        //    LeanTween.cancel(m_CharacterAlphaTweenId);

        //    Color aux;

        //    if (time == 0)
        //    {
        //        AvatarAlpha = a;

        //        for (int i = 0; i < m_CharacterRenderers.Count; i++)
        //        {
        //            aux = m_CharacterRenderers[i].color;
        //            aux.a = Alpha * AvatarAlpha * m_CharacterAlphaMul;
        //            m_CharacterRenderers[i].color = aux;
        //        }
        //        onComplete?.Invoke();
        //    }
        //    else
        //    {
        //        m_CharacterAlphaTweenId = LeanTween.value(AvatarAlpha, a, time)
        //              .setEaseOutCubic()
        //              .setOnUpdate((float t) =>
        //              {
        //                  AvatarAlpha = t;

        //                  for (int i = 0; i < m_CharacterRenderers.Count; i++)
        //                  {
        //                      aux = m_CharacterRenderers[i].color;
        //                      aux.a = Alpha * AvatarAlpha * m_CharacterAlphaMul;
        //                      m_CharacterRenderers[i].color = aux;
        //                  }
        //              })
        //              .setOnComplete(onComplete)
        //              .uniqueId;
        //    }
        //}
        
        //public virtual void UpdateRenderers()
        //{
        //    m_TextMeshes = GetComponentsInChildren<TextMeshPro>(true);
        //    m_Renderers = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>(true));

        //    foreach (SpriteRenderer sr in m_CharacterRenderers)
        //        m_Renderers.Remove(sr);
        //    foreach (SpriteRenderer sr in m_Shadows)
        //        m_Renderers.Remove(sr);
        //}

        public void InitializePositionPOP()
        {
            transform.localPosition = Vector3.zero;
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
                .setOnComplete(() =>
                {
                    transform.position = worldPos;
                    onComplete?.Invoke();
                })
                .uniqueId;
        }

        public virtual void OnApplyStatusEffect(StatusEffect effect)
        {

        }

        public virtual void OnExpireStatusEffect(StatusEffect effect)
        {

        }

        public void SetAlpha(float vlaue, float time)
        {

        }

        public virtual void OnEnterMapView()
        {
            ////marker.SetAlpha(0);
            //SetAlpha(Alpha);
            //SetAlpha(1, 1f);
            //gameObject.SetActive(true);
            inMapView = true;
        }

        public virtual void OnLeaveMapView()
        {
            //SetAlpha(0, 1f, () => gameObject.SetActive(false));
            inMapView = false;
            IsShowingAvatar = false;
            IsShowingIcon = false;
        }


#if UNITY_EDITOR
        [ContextMenu("Print token")]
        private void PrintToken()
        {
            Debug.Log(Newtonsoft.Json.JsonConvert.SerializeObject(Token, Newtonsoft.Json.Formatting.Indented));
        }

        [ContextMenu("Focus on")]
        private void FocusOnMarker()
        {
            MapCameraUtils.FocusOnPosition(transform.position, false);
        }
#endif
    }
}