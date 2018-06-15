using UnityEngine;
using System.Collections;

public class WardobeButton : MonoBehaviour
{
	public EnumWardrobeCategory type;
	CanvasGroup CG;
	void Start()
	{
		CG = GetComponentInChildren<CanvasGroup> ();
	}
	public void onClick()
	{
		WardrobeUIManager.Instance.filter (type, true);
		CG.alpha = 1;
	}



}

