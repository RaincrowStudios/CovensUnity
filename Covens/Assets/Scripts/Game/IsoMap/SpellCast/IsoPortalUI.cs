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

	void Awake()
	{
		instance = this;
	}

	public void EnablePortalCasting()
	{
		Show (container); 
	}

	public void DisablePortalCasting()
	{
		Hide (container);
		MapSelection.Instance.GoBack ();
	}

	public void CastPortal()
	{
		print ("en" + traces * 5);
		SpellCastAPI.PortalCast (traces * 5);
	}

	public void PortalFX(int newEnergy)
	{
		if (newEnergy < MarkerSpawner.SelectedMarker.energy) {
			portalAttackText.text = (MarkerSpawner.SelectedMarker.energy - newEnergy).ToString ();
			portalAttack.SetActive (true);
		} else {
			portalEmpText.text = (MarkerSpawner.SelectedMarker.energy - newEnergy).ToString ();
			portalEmp.SetActive (true);
		}
	}

}

