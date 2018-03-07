using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))] 
public class FadeInOut : MonoBehaviour {


	public float frequency = 1f;
	CanvasGroup cg;
	// Use this for initialization
	void Start () {
		cg = GetComponent<CanvasGroup> ();
	}

	// Update is called once per frame
	void Update () {
		cg.alpha = Mathf.Abs( Mathf.Sin (Time.time * frequency)) ;
	}
}
