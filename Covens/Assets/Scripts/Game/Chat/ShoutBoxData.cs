using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShoutBoxData : MonoBehaviour
{
	public Text Title;
	public Text Content;
	// Use this for initialization
	void Start ()
	{
		Destroy (gameObject, 5);
	}
	
	public void Setup(string title,string content)
	{
		Title.text = title;
		Content.text = content;
	}
}

