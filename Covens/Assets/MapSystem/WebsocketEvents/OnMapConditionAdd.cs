using UnityEngine;
using System.Collections;

public static class OnMapConditionAdd
{
    public static event System.Action<Conditions> OnConditionAdded;
    public static event System.Action<Conditions> OnPlayerConditionAdded;

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
            OnPlayerConditionAdded?.Invoke(data.condition);
        }
        else
        {
            OnConditionAdded?.Invoke(data.condition);
        }
    }
}
