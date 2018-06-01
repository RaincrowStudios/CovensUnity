using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerManagerUI : MonoBehaviour
{
	public static PlayerManagerUI Instance { get; set;}

	[Header("Flight")]
	public GameObject FlightObject;

	[Header("PlayerInfo UI")]
	public Text Level;
	public Text Energy;
	public GameObject EnergyWhite;
	public GameObject EnergyShadow;
	public GameObject EnergyGrey;
	public GameObject spiritForm;
	public GameObject physicalForm;
	public GameObject flyFX;
	public Text EnergyIso;
	 FlightVisualManager FVM;
	void Awake ()
	{
		Instance = this;
		FVM = GetComponent<FlightVisualManager> ();
	}

	// ___________________________________________ Main Player UI ________________________________________________________________________________________________

	public void SetupUI()
	{
		EnergyGrey.SetActive (false);
		EnergyShadow.SetActive (false);
		EnergyWhite.SetActive (false);
		Level.text =  PlayerDataManager.playerData.level.ToString();
        if(EnergyIso)
		EnergyIso.text = PlayerDataManager.playerData.energy.ToString ();
        if(Energy)
		Energy.text = PlayerDataManager.playerData.energy.ToString();
		if ( PlayerDataManager.playerData.degree < 0) {
			EnergyShadow .SetActive (true);
		} else if ( PlayerDataManager.playerData.degree > 0) {
			EnergyWhite.SetActive (true);
		} else {
			EnergyGrey.SetActive (true);
		}
	}

	public void UpdateEnergy()
	{
		Energy.text = PlayerDataManager.playerData.energy.ToString();
	}

	// ___________________________________________ FLIGHT UI _____________________________________________________________________________________________________

	public void Flight()
	{
		physicalForm.SetActive (false);
		FlightObject.SetActive (true);
		spiritForm.SetActive (true);
		flyFX.SetActive (true);
		FVM.FadeOut ();
	}

	public void Hunt()
	{
		flyFX.SetActive (false);
		FlightObject.SetActive (false);
		FVM.FadeIn ();

	}

	public void home()
	{
		spiritForm.SetActive (false);
		physicalForm.SetActive (true);
	}
		

}

