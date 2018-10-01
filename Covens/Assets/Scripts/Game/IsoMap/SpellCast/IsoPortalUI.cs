using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class IsoPortalUI : UIAnimationManager
{
	public static IsoPortalUI instance{ get; set;}
	public static int traces = 0;
	public GameObject container;
	public GameObject portalAttack;
	public Text portalAttackText;
	public GameObject portalEmp;
	public Text portalEmpText;
	public GameObject portalSummon;
	public GameObject portalDead;
	public static bool isPortal;
	public GameObject portalDestroyed;
	void Awake()
	{
		instance = this;
	}

	public void EnablePortalCasting()
	{
		isPortal = true;
		Show (container); 
	}

	public void DisablePortalCasting()
	{
		Hide (portalDead,false);
		Hide (container);
		if (MapSelection.currentView == CurrentView.IsoView) {
			MapSelection.Instance.GoBack ();
		}
	}

	public void CastPortal()
	{
		SpellCastAPI.PortalCast (traces * 5);
	}

	public void PortalFX(int newEnergy)
	{
		if (newEnergy < MarkerSpawner.SelectedMarker.energy) {
			portalAttackText.text = ( newEnergy - MarkerSpawner.SelectedMarker.energy ).ToString ();
			portalAttack.SetActive (true);
		} else {
			portalEmpText.text = ( newEnergy - MarkerSpawner.SelectedMarker.energy ).ToString ();
			portalEmp.SetActive (true);
		}
	}

	public void Destroyed()
	{
		Invoke ("DestroyHelper", 1.5f);
	}

	void DestroyHelper()
	{
		StartCoroutine (ScaleDownPortal ());
		Invoke ("DisablePortalCasting", 4.5f);
		Show (portalDead,false);
		portalDestroyed.SetActive (true);
	}

	public void Summoned()
	{
		StartCoroutine (ScaleDownPortal ());
		Invoke ("DisablePortalCasting", 2.9f);
		portalSummon.SetActive (true);
	}

	IEnumerator ScaleDownPortal()
	{
		if (MapSelection.currentView == CurrentView.IsoView) {
			float t = 0;
			if (MapSelection.selectedItemTransform == null)
				yield break;
			while (t <= 1) {
				t += Time.deltaTime * 2;
				MapSelection.selectedItemTransform.localScale = Vector3.Lerp (Vector3.zero, Vector3.one * 41, Mathf.SmoothStep (1, 0, t));
				MapSelection.selectedItemTransform.localEulerAngles = new Vector3 (0, Mathf.SmoothStep (243f, 0, t), 0);
				yield return 0;
			}
			MapSelection.IsSelf = true;
		}
	}
}

