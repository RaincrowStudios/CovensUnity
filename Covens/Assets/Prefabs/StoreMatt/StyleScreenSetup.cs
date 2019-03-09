using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class StyleScreenSetup : MonoBehaviour {

	public ApparelData[] styles;
	public List<GameObject> elipses = new List<GameObject>();
	public Image stylePic;
	public Sprite ownedSpr;
	public TextMeshProUGUI title;
	public TextMeshProUGUI desc;
	public TextMeshProUGUI silver;
	public TextMeshProUGUI gold;
	public GameObject purchaseSuccessScreen;

	public Transform elipseHolder;
	public GameObject elipsePrefab;

	private int styleIndex;
	private bool isSilver;

	public float moveStyle;

	public SwipeDetector sd;

	// Use this for initialization
	void Start () {
		//Add listeners for buying the styles
		sd.SwipeRight = SwipedRight;
		sd.SwipeLeft = SwipedLeft;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetupStyleScreen(List<ApparelData> data)
	{
		styles = data.ToArray();

//		transform.GetChild(9).GetComponent<Button>().onClick.AddListener(() => {
//			//add logic to allow for buying/setting up purchase screen
//			print("trying to buy style");
//			NewStoreUIManager.Instance.InitiateCosmeticPurchase(styles[styleIndex]);
//		});


		for (int i = 0; i < data.Count; i++) {
			elipses.Add(Instantiate (elipsePrefab, elipseHolder));
			if (i != 0) {
				elipses [i].GetComponent<Image>().color = new Color (.35f, .35f, .35f, 1);
			}
		}
		//ChangeStylePreview (styleIndex);
		LeanTween.moveLocalX (stylePic.gameObject, moveStyle, .5f).setEase (LeanTweenType.easeInOutQuad);
		DownloadedAssets.GetSprite (styles [styleIndex].iconId, stylePic, false);

		//		var s =  styles [styleIndex].id.Replace("_S_","_M_");
		//		print (s);
		title.text = DownloadedAssets.storeDict [styles[styleIndex].id].title;
		if (styles [styleIndex].owned) {
			transform.GetChild (9).GetComponent<Image> ().sprite = ownedSpr;
		} else {
			transform.GetChild(9).GetComponent<Button>().onClick.AddListener(() => {
				//add logic to allow for buying/setting up purchase screen
				print("trying to buy style");
				NewStoreUIManager.Instance.InitiateCosmeticPurchase(styles[styleIndex]);
				this.gameObject.SetActive(false);
			});
		}
		//below is intended to be a description
		//desc.text = DownloadedAssets.storeDict [styles [styleIndex].id];
		silver.text = styles [styleIndex].silver.ToString ();
		gold.text = styles [styleIndex].gold.ToString ();
		elipses [styleIndex].GetComponent<Image>().color = Color.white;
	}

	private void SwipedLeft()
	{
		elipses[styleIndex].GetComponent<Image>().color = new Color (.35f, .35f, .35f, 1);
		ChangeStylePreview (1);
	}

	private void SwipedRight()
	{
		elipses[styleIndex].GetComponent<Image>().color = new Color (.35f, .35f, .35f, 1);
		ChangeStylePreview ((-1));
	}

	public void ChangeStylePreview(int changingIndex)
	{
		styleIndex += changingIndex;

		if (styleIndex < 0) {
			styleIndex = 0;
			elipses [styleIndex].GetComponent<Image>().color = Color.white;
		} else if (styleIndex > (styles.Length - 1)) {
			styleIndex = styles.Length - 1;
			elipses [styleIndex].GetComponent<Image>().color = Color.white;
		} else {
			elipses [styleIndex].GetComponent<Image>().color = Color.white;

			LeanTween.moveLocalX (stylePic.gameObject, (moveStyle - 700), .5f)
				.setEase (LeanTweenType.easeInOutQuad)
				.setOnComplete(() => {
					DownloadedAssets.GetSprite (styles [styleIndex].iconId, stylePic, false);

					//		var s =  styles [styleIndex].id.Replace("_S_","_M_");
					//		print (s);
					title.text = DownloadedAssets.storeDict [styles[styleIndex].id].title;
					if (styles [styleIndex].owned) {
						transform.GetChild (9).GetComponent<Image> ().sprite = ownedSpr;
						transform.GetChild(9).GetComponent<Button>().enabled = false;
					} else {
						transform.GetChild(9).GetComponent<Button>().enabled = true;
					}
					//below is intended to be a description
					//desc.text = DownloadedAssets.storeDict [styles [styleIndex].id];
					silver.text = styles [styleIndex].silver.ToString ();
					gold.text = styles [styleIndex].gold.ToString ();
					LeanTween.moveLocalX (stylePic.gameObject, moveStyle, .5f).setEase (LeanTweenType.easeInOutQuad);
				});
		}
	}
}
