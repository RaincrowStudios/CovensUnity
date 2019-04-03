using UnityEngine;
using TMPro;

public class textSizeTest : MonoBehaviour
{
    public TextMeshPro text;

    void Update()
    {
        Debug.Log(text.GetPreferredValues().x);
    }
}