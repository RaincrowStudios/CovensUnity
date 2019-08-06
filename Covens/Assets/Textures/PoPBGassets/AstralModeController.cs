using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AstralModeController : MonoBehaviour
{
    public SpriteRenderer WitchSprite;
    public Button AstralModeBtn;
    public GameObject AstralVFX;
    public GameObject AstralVFX_Instance;
    public Image CooldownFill;
    public Image DurationFill;
    public bool isActive;
    public bool isCooldown;
    public bool isReady = true;
    public float Duration = 5f;
    public float Cooldown = 10f;


    // Start is called before the first frame update
    void Start()
    {
        AstralModeBtn.onClick.AddListener(() => { ActivateAstralMode(); });
        SetReady();
    }

    // Update is called once per frame
    void ActivateAstralMode()
    {
        if (isActive == true)
        {
            Debug.Log("Canceling Astral");
            LeanTween.cancelAll();
            LeanTween.value(DurationFill.fillAmount, 0f, 0.5f).setOnUpdate((float i) =>
            {
                DurationFill.fillAmount = i;
            })
            .setOnComplete(() => { DeactivateAstralMode(); });
            //DeactivateAstralMode();


        }
        else if (isCooldown == false && isReady == true)
        {
            Debug.Log("Astral Mode On");
            VFXon();
            isActive = true;
            isReady = false;
            LeanTween.value(1f, 0f, Duration).setOnUpdate((float i) =>
              {
                  DurationFill.fillAmount = i;
              })
            .setOnComplete(() => { DeactivateAstralMode(); });
        }
        else if (isCooldown == true)
        {
            Debug.Log("On Cooldown");
            return;
        }
        else
        {
            Debug.Log("Somethings wrong!");
        }
    }
    void DeactivateAstralMode()
    {
        Debug.Log("Deactivating Astral");
        VFXoff();
        CooldownFill.transform.parent.GetComponent<CanvasGroup>().alpha = 1f;
        isActive = false;
        isCooldown = true;
        CooldownFill.fillAmount = 0;
        LeanTween.value(0f, 1f, Cooldown).setOnUpdate((float i) =>
              {
                  CooldownFill.fillAmount = i;
              })
            .setOnComplete(() => { isCooldown = false; SetReady(); });
    }
    void SetReady()
    {
        Debug.Log("Astral is Ready");

        LeanTween.value(DurationFill.fillAmount, 1f, 0.3f).setOnUpdate((float i) =>
              {
                  DurationFill.fillAmount = i;
              }).setOnComplete(() =>
              {
                  isReady = true;
              });
        LeanTween.alphaCanvas(CooldownFill.transform.parent.GetComponent<CanvasGroup>(), 0f, 0.5f).setOnComplete(() => { CooldownFill.fillAmount = 0; });
    }
    void VFXon()
    {
        AstralVFX_Instance = Utilities.InstantiateObject(AstralVFX, WitchSprite.transform, 2.5f);
    }
    void VFXoff()
    {
        AstralVFX_Instance.GetComponent<ParticleSystem>().Stop();
        AstralVFX_Instance.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        LeanTween.value(0f, 1f, 4f).setOnComplete(() =>
        {
            Destroy(AstralVFX_Instance);
        });
    }
}
