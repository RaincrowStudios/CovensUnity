using Raincrow.BattleArena.Factory;
using UnityEngine;

namespace Raincrow.Services
{
    public class ServiceLocator : MonoBehaviour
    {
        #region Avatar Sprite Util 

        [Header("Services")]
        // Avatar Sprite Util Prefab
        [SerializeField] private AvatarSpriteUtil _avatarSpriteUtilPrefab;

        // Avatar Sprite Util Instance
        private AvatarSpriteUtil _avatarSpriteUtilInstance;

        public IWitchAvatarFactory GetWitchAvatarFactory()
        {
            if (_avatarSpriteUtilInstance == null)
            {
                _avatarSpriteUtilInstance = GetInstance(_avatarSpriteUtilPrefab);
            }
            return _avatarSpriteUtilInstance;
        }

        private T GetInstance<T>(T prefab) where T : Object
        {
            T target = FindObjectOfType<T>();
            if (target != null)
            {
                return target;
            }

            return Instantiate(prefab);
        }

        #endregion
    }
}
