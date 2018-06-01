using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WardrobeItemData : MonoBehaviour
{
	public WardrobeData data; 
	public Text title;
	public Image icon;
	// Use this for initialization
	void Start ()
	{
		title.text = data.type.ToString (); 
	}

}

