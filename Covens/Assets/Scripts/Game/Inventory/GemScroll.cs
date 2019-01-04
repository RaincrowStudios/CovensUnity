using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class GemScroll : MonoBehaviour
{
	public bool canScroll = false;
	public float speed= 1;
	public float MaxSpeed= 18;

	public float inertia = 3;
	public bool CanRotate;

	float rotateSpeed;
	int direction = 0;
	public Sprite gemIcon;
	public Transform[] AllGems;
	public Button[] bloodAgate;
	public Button[] malachite;
	public Button[] demonEye;
	public Button[] motherTear;
	public Button[] brimstone;

	public Text[] bloodAgateCount;
	public Text[] malachiteCount;
	public Text[] demonEyeCount;
	public Text[] motherTearCount;
	public Text[] brimstoneCount;

	public Button[] AllGemsImage; 
	public GameObject[] AllCounts;
    // Use this for initialization

    void SetupItems()
	{
		foreach (var item in AllGemsImage) {
			item.interactable = false;
			item.transform.GetChild(1).GetComponent<Image>().raycastTarget = true;
		}

		foreach (var item in AllCounts) {
			item.SetActive (false);
		}

		foreach (var item in PlayerDataManager.playerData.ingredients.gemsDict) {
			if (item.Key == "coll_bloodAgate") {
				foreach (var g in bloodAgate) {
					g.interactable = true;
					g.transform.GetChild(1).GetComponent<Image>().raycastTarget = true;
				}
				foreach (var t in bloodAgateCount) {
					t.text = item.Value.count.ToString();
					t.transform.parent.gameObject.SetActive (true);
				}
			} else if (item.Key == "coll_brimstone") {
				foreach (var g in brimstone) {
					g.interactable = true;
					g.transform.GetChild(1).GetComponent<Image>().raycastTarget = true;
				
				}
				foreach (var t in brimstoneCount) {
					t.text = item.Value.count.ToString();
					t.transform.parent.gameObject.SetActive (true);
				}
			} else if (item.Key == "coll_motherTear") {
				foreach (var g in motherTear) {
					g.interactable = true;
					g.transform.GetChild(1).GetComponent<Image>().raycastTarget = true;

				}
				foreach (var t in motherTearCount) {
					t.text = item.Value.count.ToString();
					t.transform.parent.gameObject.SetActive (true);
				}
			} else if (item.Key == "coll_malachite") {
				foreach (var g in malachite) {
					g.interactable = true;
					g.transform.GetChild(1).GetComponent<Image>().raycastTarget = true;
				}
				foreach (var t in malachiteCount) {
					t.text = item.Value.count.ToString();
					t.transform.parent.gameObject.SetActive (true);
				}
			} else {
				foreach (var g in demonEye) {
					g.interactable = true;
					g.transform.GetChild(1).GetComponent<Image>().raycastTarget = true;

				}
				foreach (var t in demonEyeCount) {
					t.text = item.Value.count.ToString();
					t.transform.parent.gameObject.SetActive (true);
				}
			}
		}
	}

	public void OnClick(string id)
	{
		InventoryInfo.Instance.Show (id, gemIcon);
	}

	void OnEnable ()
	{
		transform.localEulerAngles = Vector3.zero;
		SetupItems ();
		fixRotationStart ();
	}

	void fixRotationStart()
	{
		foreach (var item in AllGems) {
			item.transform.localEulerAngles = new Vector3 (0, 0, -item.parent.localEulerAngles.z);
		}
	}

	void fixRotation()
	{
		foreach (var item in AllGems) {
			item.Rotate (0, 0, -rotateSpeed*direction );
		}
	}

	public void StartRotation()
	{
		StopAllCoroutines ();
		CanRotate = true;
		StartCoroutine (RotateGemWheel ());
	}

	public void StopRotation()
	{
		CanRotate = false;
		StartCoroutine (RotateGemWheel());
	}

	void Update ()
	{
		if (CanRotate) {
			rotateSpeed = Input.GetAxis ("Mouse Y") * speed;
			if (rotateSpeed > 0)
				direction = 1;
			else
				direction = -1;

			rotateSpeed = Mathf.Clamp(  Mathf.Abs (rotateSpeed), 0, MaxSpeed);
			transform.Rotate (0, 0, rotateSpeed*direction );
			fixRotation ();
		}
	}

	IEnumerator RotateGemWheel()
	{
		while (rotateSpeed>0) {
			rotateSpeed -= Time.deltaTime*inertia;
			transform.Rotate (0, 0, rotateSpeed*direction );
			fixRotation ();
			yield return null;
		} 
	}
}

