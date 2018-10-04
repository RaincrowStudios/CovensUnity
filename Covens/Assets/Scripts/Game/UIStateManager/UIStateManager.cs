using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIStateManager : MonoBehaviour
{
	public static UIStateManager Instance{get;set;}
	public delegate void WindowChanged(bool isMainWindow);
	public static event WindowChanged windowChanged;

	public void CallWindowChanged(bool isMain)
	{
		if(windowChanged!=null)
			windowChanged (isMain );
	}

	public CanvasGroup[] DisableButtons;

	void Start ()
	{
		windowChanged += SetMainUI;
	}

	void Awake()
	{
		Instance = this;
	}

	void SetMainUI (bool isMain){
		StartCoroutine (SetMainUIHelper (isMain));
	}

	IEnumerator SetMainUIHelper (bool isMainUI)
	{
		yield return new WaitForSeconds(.5f);
		foreach (var item in DisableButtons) {
			item.interactable = isMainUI;
		}
		if (isMainUI) {
			try {
				if (MapSelection.marker != null) {
					OnlineMapsControlBase3D.instance.RemoveMarker3D (MapSelection.marker);
				}
			} catch { }

			try {
				if (SpellBookScrollController.Instance.magicTrace != null) {
					Destroy(SpellBookScrollController.Instance.magicTrace);
				}
			} catch { }

			try {
				if (SpellBookScrollController.Instance.magicTrace != null) {
					Destroy(SpellBookScrollController.Instance.magicTrace);
				}
			} catch { }
		}


	}
	
}

