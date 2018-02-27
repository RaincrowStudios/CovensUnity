using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RemoveIngredients : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject item;
	#region IBeginDragHandler implementation
	
	public void OnBeginDrag (PointerEventData eventData)
	{
		item = gameObject;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		Vector3 pos = Input.mousePosition;
		item.transform.position = Camera.main.ScreenToWorldPoint(pos);
	}

	#endregion

	#region IEndDragHandler implementation
	public void OnEndDrag (PointerEventData eventData)
	{
		
	}
	#endregion

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
