using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    [System.Serializable]
    public class CharacterUIModel
    {
        // Serializable variables
        [SerializeField] private GameObject _characterPrefab; // Character Prefab

        public GameObject CharacterPrefab { get => _characterPrefab; set => _characterPrefab = value; }
    }
}