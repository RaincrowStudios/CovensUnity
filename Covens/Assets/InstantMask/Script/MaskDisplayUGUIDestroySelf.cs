using UnityEngine;
using System.Collections;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

[ExecuteInEditMode()]
public class MaskDisplayUGUIDestroySelf : MonoBehaviour
{
	GameObject ParentGO;

	void Update ()
	{
		if (ParentGO == null)
			ParentGO = transform.parent.gameObject;

		if (ParentGO != null) {
			if (ParentGO.GetComponent<InstantTextureMask> ()) {

			} else {
				#if (UNITY_EDITOR)
				if (!EditorApplication.isPlaying && !EditorApplication.isPaused)
					DestroyImmediate (gameObject);
				else
					#endif
					Destroy (gameObject);
			}
		}
	}

}
