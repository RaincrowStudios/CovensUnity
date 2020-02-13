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
        public string Id { set; get; }

        /// <summary>
        /// Position X on Grid
        /// </summary>
        public int Line { set; get; }

        /// <summary>
        /// Position Y on Grid
        /// </summary>
        public int Column { set; get; }

        public int BaseEnergy => throw new System.NotImplementedException();
        public int Energy { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int Power { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public int Resilience { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public CharacterType Type => throw new System.NotImplementedException();

        public CharacterModel(CharacterBuilder builder)
        {

        }
    }

    public class ItemModel : IItemModel
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public int Count { set; get; }
    }

    public class CharacterBuilder
    {

    }

}