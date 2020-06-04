using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElixirHolderUI : MonoBehaviour
{
    [SerializeField] private ElixirUI []_elixirsUI;
    [SerializeField] private Color _opaqueColor;
    [SerializeField] private Button _buttonOpen;
    [SerializeField] private ElixirManagementUI _elixirManagementUI;

    private void Start()
    {
        UpdateElixirsStatus();
        _buttonOpen.onClick.AddListener(ElixirManagemenOpen);
    }

    private void ElixirManagemenOpen()
    {
        Raincrow.SceneManager.LoadSceneAsync(Raincrow.SceneManager.Scene.STORE, UnityEngine.SceneManagement.LoadSceneMode.Additive,null,null);
        _elixirManagementUI.Show();
    }

    public void UpdateElixirsStatus()
    {
        foreach(ElixirUI elixirUI in _elixirsUI)
        {
            bool have = PlayerDataManager.playerData.HaveEffect(elixirUI.GetId());
            elixirUI.ChangeStatus(have, have ? Color.white : _opaqueColor, 0.3f);
        }
    }
}
