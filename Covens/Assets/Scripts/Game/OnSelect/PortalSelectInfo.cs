using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PortalSelectInfo : MonoBehaviour
{
	public Text creater;
	public Text timer;
	public Text energy;
	// Use this for initialization
	public void Setup(MarkerDataDetail md)
	{
		creater.text = md.owner;
		timer.text = "Summon in " + Utilities.EpocToDateTime (md.summonOn);
		energy.text = md.energy.ToString();
	}
}

