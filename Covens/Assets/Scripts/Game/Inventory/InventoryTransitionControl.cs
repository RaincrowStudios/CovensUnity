using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTransitionControl : MonoBehaviour
{

    public static InventoryTransitionControl Instance { get; set; }

    public Animator anim;
    public GameObject InventoryObject;
    public UnityEngine.UI.Button closeButton;

    public UnityEngine.UI.Button apothecaryButton;
    public GameObject apothecaryEmptyObject;
    public Transform apothecaryVisibleRef;
    public Transform apothecaryHiddenRef;
	public GameObject InitClickBlocker;

    [SerializeField] private CanvasGroup backgroundOverlay;

    private int apothecaryTweenId;
    private int overlayTweenId;

    void Awake()
    {
        Instance = this;
		Instantiate (InitClickBlocker, transform);
        apothecaryButton.onClick.AddListener(OnClickApothecary);
        apothecaryButton.transform.position = apothecaryHiddenRef.position;
        apothecaryButton.gameObject.SetActive(false);
    }

    void Start()
    {
        OnAnimateIn();
    }

    void OnAnimateIn()
    {
        FadeBackground(1f);
        UIStateManager.Instance.CallWindowChanged(false);
        SoundManagerOneShot.Instance.MenuSound();
        InventoryObject.SetActive(true);
        anim.SetBool("animate", true);

        bool apothecaryAvailable = PlayerDataManager.playerData.inventory.consumables.Count > 0;
        apothecaryButton.interactable = apothecaryAvailable;
        apothecaryEmptyObject.SetActive(apothecaryAvailable == false);
        closeButton.gameObject.SetActive(true);

        Invoke("ShowApothecaryButton", 0.4f);
    }


    public void OnAnimateOut()
    {
        FadeBackground(0);
        UIStateManager.Instance.CallWindowChanged(true);
        SoundManagerOneShot.Instance.MenuSound();
        anim.SetBool("animate", false);
        Invoke("disable", .8f);
        HideApothecaryButton();
        closeButton.gameObject.SetActive(true);
    }

    void disable()
    {
        Destroy(gameObject);
    }

    public void OpenApothecary()
    {
        FadeBackground(0f);
        anim.SetBool("openapothecary", true);
        HideApothecaryButton();
        closeButton.gameObject.SetActive(false);
    }

    public void CloseApothecary()
    {
        anim.SetBool("openapothecary", false);
        OnAnimateOut();
    }

    public void ReturnFromApothecary()
    {
        FadeBackground(1, 1.5f);
        anim.SetBool("openapothecary", false);
        Invoke("ShowApothecaryButton", 0.5f);
        closeButton.gameObject.SetActive(true);
    }



    public void ShowApothecaryButton()
    {
        LeanTween.cancel(apothecaryTweenId);
        apothecaryButton.gameObject.SetActive(true);
        apothecaryTweenId = LeanTween.move(apothecaryButton.gameObject, apothecaryVisibleRef, 0.5f)
            .setEaseOutCubic()
            .uniqueId;
    }

    public void HideApothecaryButton()
    {
        LeanTween.cancel(apothecaryTweenId);
        apothecaryTweenId = LeanTween.move(apothecaryButton.gameObject, apothecaryHiddenRef, 0.25f)
            .setOnComplete(() => apothecaryButton.gameObject.SetActive(false))
            .setEaseOutCubic()
            .uniqueId;
    }

    private void OnClickApothecary()
    {
        InventoryTransitionControl.Instance.OpenApothecary();
        UIApothecary.Instance.Show();
    }

    private void FadeBackground(float alpha, float duration = 0.5f)
    {
        LeanTween.cancel(overlayTweenId);
        overlayTweenId = LeanTween.value(backgroundOverlay.alpha, alpha, duration)
            .setEaseOutCubic()
            .setOnUpdate((float t) =>
            {
                try
                {
                    backgroundOverlay.alpha = t;

                }
                catch
                {

                    return;
                }
            })
            .uniqueId;
    }
}
