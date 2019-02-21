using UnityEngine;
using UnityEngine.UI;
public class SummonSuccess : MonoBehaviour
{
    public Image summonSuccessSpirit;
    public Text headingText;
    public Text bodyText;
    public Button close;
    void Start()
    {
        close.onClick.AddListener(() => Destroy(gameObject));
    }

}