using UnityEngine;

namespace Raincrow.BattleArena.Model
{
    [System.Serializable]
    public class CharacterGameObjectModel
    {
        // Serializable variables
        [SerializeField] private GameObject _characterPrefab; // Character Prefab

        public GameObject CharacterPrefab { get => _characterPrefab; set => _characterPrefab = value; }
    }

    public class CharacterModel : ICharacterModel
    {
        // Properties

        /// <summary>
        /// Player or Spirit ID
        /// </summary>
        public int Id { set; get; }

        /// <summary>
        /// Position X on Grid
        /// </summary>
        public int Line { set; get; }

        /// <summary>
        /// Position Y on Grid
        /// </summary>
        public int Column { set; get; }

        public CharacterModel(CharacterBuilder builder)
        {

        }
    }    

    public class CharacterBuilder
    {

    }
}