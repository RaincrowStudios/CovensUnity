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

    private List<CollectableItem> inventoryItems = new List<CollectableItem>();
    private int forbidTool = 0;
    private int forbidToolValue = 0;

    // Use this for initialization
    void Start () {
        
        rewardContinue.onClick.AddListener(() => {
			LeanTween.alphaCanvas(SavCG, 0f, 0.4f);
			LeanTween.alphaCanvas(TextContainer, 0f, 0.4f);
			LeanTween.alphaCanvas(BGCG, 0.01f, 0.5f).setOnComplete(() => {
				LeanTween.alphaCanvas(BGCG, 0f, 0.2f).setOnComplete(() => {
					Destroy(gameObject);
				});
			});
        });
        InitAnims ();
    }

	void InitAnims() {
        close.SetActive(false);
        NotToday.SetActive (false);
		accept.SetActive (false);
		WarningBG.alpha = 0;
		SavScale = Vector3.one * 0.3f;
		BGCG.alpha = 0;
		WarningTextCont.alpha = 1;
        WarningTextCont.transform.GetChild(0).GetComponent<CanvasGroup>().alpha = 0;
        WarningTextCont.transform.GetChild(1).GetComponent<CanvasGroup>().alpha = 0;
        SavCG.alpha = 0;
		ToolsCG.alpha = 0;
		DrachsCG.alpha = 0;
		TextContainer.alpha = 0;
		LeanTween.alphaCanvas (BGCG, 1f, 0.5f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (TextContainer, 1f, 2f).setEase (LeanTweenType.easeOutCubic);
        Anim();
        Anim2();
    }
	// Update is called once per frame
	void Anim () {
		LeanTween.scale (Sav, Vector3.one*0.7f, 7f).setEase (LeanTweenType.easeOutCubic);
		LeanTween.alphaCanvas (SavCG, 1f, 1f).setEase (LeanTweenType.easeOutCubic).setOnComplete (() => {
			//Anim2();
		});
		//callOnCompletes(Anim2());
		//Invoke ("Anim2", 1f);
	}
	void Anim2 () {
        LeanTween.alphaCanvas(ToolsCG, 1f, 1f).setEase(LeanTweenType.easeOutCubic);
        LeanTween.alphaCanvas(DrachsCG, 1f, 1f).setEase(LeanTweenType.easeOutCubic);
        //	.setEase (LeanTweenType.easeOutCubic);

    }
	public void Warning () {
        close.SetActive(true);
        close.GetComponent<Button>().onClick.AddListener(() => {
			LeanTween.alphaCanvas(SavCG, 0f, 0.4f);
			LeanTween.alphaCanvas(TextContainer, 0f, 0.4f);
			LeanTween.alphaCanvas(NotToday.GetComponent<CanvasGroup>(), 0f, 0.4f);
			LeanTween.alphaCanvas(BGCG, 0.01f, 0.5f).setOnComplete(() => {
				LeanTween.alphaCanvas(BGCG, 0f, 0.2f).setOnComplete(() => {
				Destroy(gameObject);
				});
			});
        });
		LeanTween.alphaCanvas (TextContainer, 0f, .5f).setEase (LeanTweenType.easeOutCubic).setOnComplete (() => {
			NotToday.SetActive (true);
			LeanTween.alphaCanvas(WarningBG, 1f, 1f).setEase(LeanTweenType.easeOutCubic);
            LeanTween.alphaCanvas(WarningTextCont.transform.GetChild(0).GetComponent<CanvasGroup>(), 1f, 1f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
            {
            LeanTween.alphaCanvas(WarningTextCont.transform.GetChild(1).GetComponent<CanvasGroup>(), 1f, 0.5f).setEase(LeanTweenType.easeInCubic);
            });
		});
	}

    public void TextSetup(string officeName)
    {
        foreach (var item in PlayerDataManager.playerData.ingredients.toolsDict)
        {
            IngredientData data = DownloadedAssets.GetCollectable(item.Value.collectible);
            if (data.forbidden)
            {
                inventoryItems.Add(item.Value);
                forbidTool += item.Value.count;
                if (data.rarity == 1)
                {
                    forbidToolValue += (5 * item.Value.count);
                }
                else if (data.rarity == 2)
                {
                    forbidToolValue += (15 * item.Value.count);
                }
                else if (data.rarity == 3)
                {
                    forbidToolValue += (50 * item.Value.count);
                }
                else
                {
                    forbidToolValue += (125 * item.Value.count);
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
                APIManager.Instance.GetData("vendor/give", TurnInCallback);
            });
        }

        greyHandOffice.text = officeName;
        toolNum.text = forbidTool.ToString();
        drachNum.text = forbidToolValue.ToString();
		rewardText.text = forbidToolValue.ToString ();

    }

    void TurnInCallback(string result, int response)
    {
        if (response == 200)
        {
            Debug.Log("turn in was a success");
            accept.SetActive(true);
            PlayerDataManager.playerData.ingredients.RemoveIngredients(inventoryItems);
            PlayerDataManager.playerData.silver += forbidToolValue;
            PlayerManagerUI.Instance.UpdateDrachs();
        }
        else
        {
            Debug.LogError(result);
        }
    }

}
