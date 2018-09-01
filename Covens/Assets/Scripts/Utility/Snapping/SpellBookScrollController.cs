using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
public class SpellBookScrollController : UIAnimationManager
{
	public static SpellBookScrollController Instance{ get; set;}
	
	public GameObject BookOfShadowObject;

	public RectTransform container;
	public Transform NavContainer; 
	public RectTransform playerInfo;
	public RectTransform spellBase;
	public SwipeDetector SD;
	public GameObject circleNav;
	List<RectTransform> items = new List<RectTransform>();
	public List<CanvasGroup> CG = new List<CanvasGroup> ();
	public Dictionary<string,GameObject> glyphAnims = new Dictionary<string, GameObject>();
	public float swipeSpeed = 2;
	public float offset = 1200;
	public int current;

	public Text title;
	public Text displayPicMsg;


	public GameObject whiteWitchPic;
	public GameObject greyWitchPic;
	public GameObject shadowWitchPic;

	public GameObject shadowCrest;
	public GameObject greyCrest;
	public GameObject whiteCrest;

	public Text witchPath;
	public Text coven;
	public Text domRank;
	public Text worldRank;
	public Text favoriteSpell;
	public Text nemesis;
	public Text benefactor;

	public Transform glyphs;
	public GameObject shadowGlyph;
	public GameObject greyGlyph;
	public GameObject whiteGlyph;

	public GameObject loadingIcon;

	bool isFirstTime = true;

	public CastingSound CS;
	public Camera cam;
	public GameObject magic;
	public GameObject magicTrace;
	public float distancefromcamera = 5;

	BookOfShadowData bsData;
	GameObject curGlyphActive = null;

	void Awake()
	{
		Instance = this;
	}

	public void Open()
	{
		loadingIcon.SetActive (true);
		APIManager.Instance.PostData ("character/display", "null", Callback, true);
	}
	
	public void Callback( string result,int response)
	{
		loadingIcon.SetActive (false);
		if (response == 200) {
			bsData = JsonConvert.DeserializeObject<BookOfShadowData> (result); 
			Show (BookOfShadowObject, true);
			Init ();
		} else {
			Debug.LogError (result + " " + response);
		}
	}

	public void Close()
	{
		Hide (BookOfShadowObject, true);
		CS.enabled = false;
		if(magicTrace!=null)
			Destroy (magicTrace);
	}
	
	public void Init()
	{
		var pData = PlayerDataManager.playerData;
		if (isFirstTime) {
			SD.SwipeRight = SwipeRight;
			SD.SwipeLeft = SwipeLeft;
			items.Add (playerInfo);   
			for (int i = 1; i < pData.spells.Count; i++) {
				var rt = InstantiateObject (i * offset, spellBase.gameObject);
				rt.GetComponent<SpellbookSpelldata> ().Setup (pData.spells [i]);
				rt.name = pData.spells [i].id;
				items.Add (rt);
			}

			foreach (var item in items) {
				CG.Add (Utilities.InstantiateObject (circleNav, NavContainer).GetComponent<CanvasGroup> ());
			}

			CG [0].alpha = 1;
			foreach (Transform item in glyphs) {
				glyphAnims.Add (item.name, item.gameObject);
			}
//			nemesis.text = "Favorite spell: <color=#000000>" + pData.ToString() + "</color>";
		} else {
			current = 0;
			StartCoroutine (SwipeHandler ());
			if (curGlyphActive != null)
				curGlyphActive.SetActive (false);
		}

		SetupUI ();
	}

	void SetupUI()
	{
		title.text = "Book of Shadows";
		var pData = PlayerDataManager.playerData;
		whiteWitchPic.SetActive (false);
		shadowWitchPic.SetActive (false);
		greyWitchPic.SetActive (false);
		if (PlayerDataManager.playerData.degree > 0) {
			whiteWitchPic.SetActive (true);
			displayPicMsg.text ="You are on the path of Light";
		} else if (PlayerDataManager.playerData.degree < 0) {
			shadowWitchPic.SetActive (true);
			displayPicMsg.text ="You are on the path of Shadow";
		} else {
			greyWitchPic.SetActive (true);
			displayPicMsg.text ="You are on the path of Gray";
		}
		SetCrest (PlayerDataManager.playerData.degree);
		current = 0;
		isFirstTime = false;

		witchPath.text = Utilities.witchTypeControlSmallCaps (pData.degree); 
		if (pData.coven != "") {
			if (pData.covenTitle != "") {
				coven.text = pData.covenTitle + " of <color=#000000>" + pData.coven + "</color>";
			} else {
				coven.text ="Member of <color=#000000>" + pData.coven + "</color>";
			}
		} else {
			coven.text = "No Coven";
		}
		worldRank.text = "Rank " + bsData.worldRank.ToString() + " in the <color=#000000>World</color>";
		domRank.text = "Rank " + bsData.worldRank.ToString() + " in the dominion of <color=#000000>"+ pData.dominion+"</color>";
		favoriteSpell.text = "Favorite spell: <color=#000000>" + bsData.favoriteSpell.ToString() + "</color>";
	}

	public void SetCrest(int degree)
	{
		whiteCrest.SetActive (false);
		shadowCrest.SetActive (false);
		greyCrest.SetActive (false);
		if (degree > 0) {
			whiteCrest.SetActive (true);
		} else if (degree < 0) {
			shadowCrest.SetActive (true);

		} else {
			greyCrest.SetActive (true);
		}
	}

	RectTransform InstantiateObject(float _offset,GameObject prefab)    
	{
		GameObject g = Instantiate(prefab, container.transform);
		g.transform.SetParent(container.transform); 
		g.transform.localPosition = Vector3.zero;
		g.transform.localEulerAngles = Vector3.zero;
		g.transform.transform.localScale = Vector3.one;

		var rt = g.GetComponent<RectTransform> ();
		rt.anchoredPosition = new Vector2 (_offset, 0);

		return  rt; 
	}

	public void SwipeRight()
	{
		current--;
		this.StopAllCoroutines ();
		StartCoroutine (SwipeHandler ());
	}

	public void SwipeLeft()
	{
		current++;
		this.StopAllCoroutines ();
		StartCoroutine (SwipeHandler ());
	}

	IEnumerator SwipeHandler()
	{
		if (current < 0) {
			current = 0;
			yield break;
		}
		if (current > items.Count-1) {
			current = items.Count-1;
			yield break;
		}
		for (int i = 0; i < CG.Count; i++) {
			if (i == current) {
				CG [i].alpha = 1;
			} else {
				CG [i].alpha = .25f;
			}
		}
		float t = 0;
		Vector2 cur = container.anchoredPosition;

		if (current == 0) {
			title.text = "Book of Shadows";
			shadowGlyph.SetActive (false);
			whiteGlyph.SetActive (false);
			greyGlyph.SetActive (false);
			curGlyphActive.SetActive (false);
			SetupUI ();
		} else {
			title.text = DownloadedAssets.spellDictData [items [current].name].spellName;
			var spData =  items [current].GetComponent<SpellbookSpelldata> ().data;
			SetCrest (spData.school);
			whiteWitchPic.SetActive (false);
			shadowWitchPic.SetActive (false);
			greyWitchPic.SetActive (false);
			shadowGlyph.SetActive (false);
			whiteGlyph.SetActive (false);
			greyGlyph.SetActive (false);

			if (spData.school > 0) {
				whiteGlyph.SetActive (true);
				displayPicMsg.text = DownloadedAssets.spellDictData [spData.id].spellName + " aligns you to White School";
			} else if (spData.school < 0) {
				shadowGlyph.SetActive (true);
				displayPicMsg.text = DownloadedAssets.spellDictData [spData.id].spellName + " aligns you to Shadow School";
			} else {
				greyGlyph.SetActive (true);
				displayPicMsg.text = DownloadedAssets.spellDictData [spData.id].spellName + " aligns you to Gray School";
			}

			if (curGlyphActive == null) {
				glyphAnims [spData.id].SetActive (true);
				curGlyphActive = glyphAnims [spData.id];
			} else {
				curGlyphActive.SetActive (false);
				glyphAnims [spData.id].SetActive (true);
				curGlyphActive = glyphAnims [spData.id];
			}
		}

		while (true) {
			t += Time.deltaTime * swipeSpeed;
			container.anchoredPosition = Vector2.Lerp (cur, new Vector2 (-offset*current, 0), Mathf.SmoothStep(0,1,t)); 
			yield return 0;  
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			CS.enabled = true;
			var targetPos = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distancefromcamera);
			targetPos = cam.ScreenToWorldPoint (targetPos);
			magicTrace = Instantiate (magic, targetPos, Quaternion.identity);
		} 
		if (Input.GetMouseButtonUp (0)) {
			CS.enabled = false;
		}
	}
}


public class BookOfShadowData
{
	public int worldRank{ get; set;}
	public int dominionRank{ get; set;}
//mastery
	public string favoriteSpell{get;set;}

}
