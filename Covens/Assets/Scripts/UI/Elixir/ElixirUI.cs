using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ElixirUI : MonoBehaviour
{
    [SerializeField] private string _elixirId;
    [SerializeField] private Image _elixirIcon;
    [SerializeField] private Image _elixirGlow;

    public void ChangeStatus(bool active, Color iconColor, float timeAnimation)
    {
        Vector4 targetAlpha = _elixirGlow.color;
        targetAlpha.w = active ? 1 : 0;

        _elixirIcon.color = iconColor;
        _elixirGlow.color = targetAlpha;
    }

    public string GetId()
    {
        return _elixirId;
    }
}
