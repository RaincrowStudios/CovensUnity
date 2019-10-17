using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButtonListener : MonoBehaviour
{
    public static event System.Action onPressBackBtn;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("escape pressed");
            onPressBackBtn?.Invoke();
        }
    }
}
