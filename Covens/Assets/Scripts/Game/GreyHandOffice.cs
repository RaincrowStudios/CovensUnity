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

    private Dictionary<string, InventoryItems> inventoryDict;
    private List<string> inventoryIds = new List<string>();
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
			LeanTween.alphaCanvas(SavCG, 0f, 0.4f);
			LeanTween.alphaCanvas(TextContainer, 0f, 0.4f);
			LeanTween.alphaCanvas(NotToday.GetComponent<CanvasGroup>(), 0f, 0.4f);
			LeanTween.alphaCanvas(BGCG, 0.01f, 0.5f).setOnComplete(() => {
				LeanTween.alphaCanvas(BGCG, 0f, 0.2f).setOnComplete(() => {
				Destroy(gameObject);
				});
			});
        });
		LeanTween.alphaCanvas (TextContainer, 0f, 1f).setEase (LeanTweenType.easeOutCubic).setOnComplete (() => {
			NotToday.SetActive (true);
			LeanTween.alphaCanvas(WarningBG, 1f, 1.5f).setEase(LeanTweenType.easeOutCubic);
			LeanTween.alphaCanvas(WarningTextCont, 1f, 2f).setEase(LeanTweenType.easeOutCubic);
		});
	}

    public void TextSetup(string officeName)
    {

        inventoryDict = PlayerDataManager.playerData.ingredients.toolsDict;

        foreach (var item in inventoryDict)
        {
            if (item.Value.forbidden)
            {
                var tool = item.Value;
                inventoryIds.Add(item.Key);
                forbidTool += tool.count;
                if (tool.rarity == 1)
                {
                    forbidToolValue += (5 * tool.count);
                }
                else if (tool.rarity == 2)
                {
                    forbidToolValue += (15 * tool.count);
                }
                else if (tool.rarity == 3)
                {
                    forbidToolValue += (50 * tool.count);
                }
                else
                {
                    forbidToolValue += (125 * tool.count);
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
            foreach (string id in inventoryIds)
            {
                Debug.Log("Removing tool: " + id);
                PlayerDataManager.playerData.ingredients.toolsDict.Remove(id);
            }
            PlayerDataManager.playerData.silver += forbidToolValue;
            PlayerManagerUI.Instance.UpdateDrachs();
        }
        else
        {
            Debug.LogError(result);
        }
    }

}
