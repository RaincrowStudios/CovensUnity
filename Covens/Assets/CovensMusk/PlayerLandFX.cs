using UnityEngine;

public class PlayerLandFX : MonoBehaviour
{
    public ParticleSystem LandingFX;
    public ParticleSystem.MainModule m_ParticleModule;

    public GameObject Shadow;
    public GameObject Character;
    public GameObject SelectionRing;
    public float animTime = 1;
    public LeanTweenType easeType = LeanTweenType.easeInOutQuad;

    private int m_TweenId;

    private void Awake()
    {
        PlayerManager.onStartFlight += OnStartFlight;
        PlayerManager.onFinishFlight += LandingAnim;

        m_ParticleModule = LandingFX.main;
        m_ParticleModule.playOnAwake = false;
        LandingFX.gameObject.SetActive(true);

        Shadow.transform.localScale = Vector3.zero;
        SelectionRing.transform.localScale = Vector3.zero;
        Character.transform.localScale = Vector3.zero;
    }

    private void Start()
    {
        LandingAnim();
    }

    private void OnStartFlight()
    {
        LeanTween.cancel(m_TweenId);
        LandingFX.Stop(true);

        Shadow.transform.localScale = Vector3.zero;
        SelectionRing.transform.localScale = Vector3.zero;
        Character.transform.localScale = Vector3.zero;
    }

    void LandingAnim()
    {
        LeanTween.cancel(m_TweenId);
        LeanTween.value(0, 1, animTime).setEase(easeType).setOnUpdate((float t) =>
        {
            Character.transform.localScale = Vector3.one * t;
            Shadow.transform.localScale = Character.transform.localScale * 15;
            SelectionRing.transform.localScale = Vector3.one * t * 0.06536969f;
        });

        LandingFX.Play(true);
    }
}