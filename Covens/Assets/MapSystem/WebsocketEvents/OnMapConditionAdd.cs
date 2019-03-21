using UnityEngine;
using System.Collections;

public static class OnMapConditionAdd
{
    public static event System.Action<Conditions> OnConditionAdded;

    public static void HandleEvent(WSData data)
    {
        if (data.condition.bearer == PlayerDataManager.playerData.instance)
        {
            ConditionsManager.WSAddCondition(data.condition);

            if (data.condition.status == "silenced")
            {
                BanishManager.Instance.Silenced(data);
            }
            if (data.condition.status == "bound")
            {
                BanishManager.Instance.Bind(data);
            }
        }

        OnConditionAdded?.Invoke(data.condition);
    }
}
