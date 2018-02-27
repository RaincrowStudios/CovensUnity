using UnityEngine;
using System.Collections;

public class GemItemManager : MonoBehaviour
{

	public void swapScrollStart()
	{
		GetComponentInParent<GemScroll> ().canScroll = true;
	}
}

