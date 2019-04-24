using UnityEngine;

public class PlayerLandFX : MonoBehaviour
{
    public GameObject LandingFX;
    public GameObject Shadow;
    public GameObject Character;
    public GameObject SelectionRing;
    public float animTime = 1;
    public LeanTweenType easeType = LeanTweenType.easeInOutQuad;

    void OnEnable()
    {
        LandingFX.SetActive(true);
        Shadow.transform.localScale = Vector3.zero;
        SelectionRing.transform.localScale = Vector3.zero;
        Character.transform.localScale = Vector3.zero;
        LeanTween.scale(Character, Vector3.one * .7f, animTime).setEase(easeType);
        LeanTween.scale(Shadow, Vector3.one * 15, animTime).setEase(easeType);
        LeanTween.scale(SelectionRing, Vector3.one * 0.06536969f, animTime).setEase(easeType);
    }

}