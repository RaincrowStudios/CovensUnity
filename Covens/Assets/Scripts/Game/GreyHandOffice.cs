using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class GreyHandOffice : MonoBehaviour {

	public GameObject NotToday;
	
	public TextMeshProUGUI greyHandOffice;
	public TextMeshProUGUI toolNum;
	public TextMeshProUGUI drachNum;

	public CanvasGroup SavCG;
	public GameObject Sav;

	public CanvasGroup BGCG;

	public CanvasGroup ToolsCG;

	public CanvasGroup DrachsCG;

	public CanvasGroup TextContainer;

	public CanvasGroup WarningTextCont;
	public CanvasGroup WarningBG;

	public GameObject accept;

	public Vector3 SavScale;

    public GameObject close;

    public Button rewardContinue;
    public Text rewardText;


	// Use this for initialization
	void Start () {
        rewardContinue.onClick.AddListener(() => {
            Destroy(gameObject);
        });
        InitAnims ();
    }

	void InitAnims() {
		NotToday.SetActive (false);
		accept.SetActive (false);
		WarningBG.alpha = 0;
		SavScale = Vector3.one * 0.3f;
		BGCG.alpha = 0;
		WarningTextCont.alpha = 0;
		SavCG.alpha = 0;
		ToolsCG.alpha = 0;
		DrachsCG.alpha = 0;
		TextContainer.alpha = 0;
		LeanTween.alphaCanvas (BGCG, 1f, 0.5f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (TextContainer, 1f, 2f).setEase (LeanTweenType.easeOutCubic);
		Invoke ("Anim", 1f);
	}
	// Update is called once per frame
	void Anim () {
		LeanTween.scale (Sav, Vector3.one*0.7f, 7f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (SavCG, 1f, 1.5f).setEase (LeanTweenType.easeOutCubic).setOnComplete (() => {
			Anim2();
		});
		//callOnCompletes(Anim2());
		//Invoke ("Anim2", 1f);
	}
	void Anim2 () {
		LeanTween.alphaCanvas (ToolsCG, 1f, 1f).setEase(LeanTweenType.easeOutCubic).setOnComplete  (() => {
			LeanTween.alphaCanvas (DrachsCG, 1f, 1f).setEase (LeanTweenType.easeOutCubic);
		});
		//	.setEase (LeanTweenType.easeOutCubic);

	}
	public void Warning () {
        close.SetActive(true);
        close.GetComponent<Button>().onClick.AddListener(() => {
            Destroy(gameObject);
        });
		LeanTween.alphaCanvas (TextContainer, 0f, 1f).setEase (LeanTweenType.easeOutCubic).setOnComplete (() => {
			NotToday.SetActive (true);
			LeanTween.alphaCanvas(WarningBG, 1f, 1.5f).setEase(LeanTweenType.easeOutCubic);
			LeanTween.alphaCanvas(WarningTextCont, 1f, 2f).setEase(LeanTweenType.easeOutCubic);
		});
	}

    public void TextSetup(string officeName)
    {
        int forbidTool = 0;
        int forbidToolValue = 0;
        List<InventoryItems> pIng = PlayerDataManager.playerData.ingredients.tools;
        for (int i = 0; i < pIng.Count; i++)
        {
            if (pIng[i].forbidden)
            {
                forbidTool += pIng[i].count;
                if (pIng[i].rarity == 1)
                {
                    forbidToolValue += (5 * pIng[i].count);
                }
                else if (pIng[i].rarity == 2)
                {
                    forbidToolValue += (15 * pIng[i].count);
                }
                else if (pIng[i].rarity == 3)
                {
                    forbidToolValue += (50 * pIng[i].count);
                }
                else
                {
                    forbidToolValue += (125 * pIng[i].count);
                }
            }
        }
        if (forbidTool == 0)
        {
            transform.GetChild(2).GetChild(4).GetComponent<TextMeshProUGUI>().color = Color.red;
            transform.GetChild(2).GetChild(4).GetComponent<Button>().interactable = false;
        }
        else
        {
            transform.GetChild(2).GetChild(4).GetComponent<Button>().onClick.AddListener(() => {
                for (int i = 0; i < pIng.Count; i++)
                {
                    if (pIng[i].forbidden)
                    {
                        Debug.Log(PlayerDataManager.playerData.ingredients.tools.Remove(pIng[i]));
                    }
                }
                PlayerDataManager.playerData.silver += forbidToolValue;
                PlayerManagerUI.Instance.UpdateDrachs();
            });
        }

        Debug.Log(forbidTool);
        greyHandOffice.text = officeName;
        toolNum.text = forbidTool.ToString();
        drachNum.text = forbidToolValue.ToString();
        rewardText.text = forbidToolValue.ToString() + " " + DownloadedAssets.localizedText[LocalizationManager.store_silver_drachs_upper];
    }
}
