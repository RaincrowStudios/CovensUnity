using UnityEngine;
using System.Collections;

public static class OnCharacterLocationBoot
{
    public static void HandleEvent(WSData data)
    {
        PlayerManager.Instance.StartCoroutine(BootCharacterLocation(data));
    }

    private static IEnumerator BootCharacterLocation(WSData data, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        var lm = LocationUIManager.Instance;
        if (LocationUIManager.isLocation)
        {
            if (SummoningManager.isOpen)
            {
                SummoningController.Instance.Close();
            }
            lm.Escape();
        }
        yield return null;
    }
}
