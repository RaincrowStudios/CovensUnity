using UnityEngine;
using UnityEngine.UI;
public class SummonSuccess : MonoBehaviour
{
    public Image summonSuccessSpirit;
    public Text headingText;
    public Text bodyText;
    public Button close;
    public SpiritMarker spirit;

    void Start()
    {
        close.onClick.AddListener(OnClickClose);
    }

    private void OnClickClose()
    {
        //focus the camera on the marker
        if (spirit != null)
            MapCameraUtils.FocusOnPosition(spirit.transform.position, false, 2);

        //destroy the UI
        Destroy(gameObject);
    }
}