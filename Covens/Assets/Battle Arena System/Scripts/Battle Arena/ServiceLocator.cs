using Raincrow.BattleArena.Controller;
using Raincrow.BattleArena.Factory;
using Raincrow.Loading.View;
using UnityEngine;

namespace Raincrow.Services
{
    public class ServiceLocator : MonoBehaviour
    {
        #region Avatar Sprite Util 

        [Header("Service Prefabs")]
        [SerializeField] private AvatarSpriteUtil _avatarSpriteUtilPrefab; // Avatar Sprite Util Prefab
        [SerializeField] private LoadingView _loadingViewPrefab;
        [SerializeField] private BattleController _battleControllerPrefab;
        [SerializeField] private Camera _battleCameraPrefab;

        [Header("Service Instances")]
        [SerializeField] private AvatarSpriteUtil _avatarSpriteUtilInstance; // Avatar Sprite Util Instance
        [SerializeField] private LoadingView _loadingViewInstance; // Loading View Instance
        [SerializeField] private BattleController _battleControllerInstance;
        [SerializeField] private Camera _battleCameraInstance;

        [Header("UI")]
        [SerializeField] private Canvas _mainCanvas;

        public ILoadingView GetLoadingView()
        {
            if (_loadingViewInstance == null)
            {
                _loadingViewInstance = GetInstance(_loadingViewPrefab, _mainCanvas.transform);
            }
            return _loadingViewInstance;
        }

        public IWitchAvatarFactory GetWitchAvatarFactory()
        {
            if (_avatarSpriteUtilInstance == null)
            {
                _avatarSpriteUtilInstance = GetInstance(_avatarSpriteUtilPrefab);
            }
            return _avatarSpriteUtilInstance;
        }

        public ISpiritAvatarFactory GetSpiritAvatarFactory()
        {
            if (_avatarSpriteUtilInstance == null)
            {
                _avatarSpriteUtilInstance = GetInstance(_avatarSpriteUtilPrefab);
            }
            return _avatarSpriteUtilInstance;
        }

        public ISpiritPortraitFactory GetSpiritPortraitFactory()
        {
            if (_avatarSpriteUtilInstance == null)
            {
                _avatarSpriteUtilInstance = GetInstance(_avatarSpriteUtilPrefab);
            }
            return _avatarSpriteUtilInstance;
        }

        public Camera GetBattleCamera()
        {
            if (_battleCameraInstance == null)
            {
                _battleCameraInstance = GetInstance(_battleCameraPrefab);
            }
            return _battleCameraInstance;
        }

        public BattleController GetBattleController()
        {
            if (_battleControllerInstance == null)
            {
                _battleControllerInstance = GetInstance(_battleControllerPrefab);
            }
            return _battleControllerInstance;
        }

        private T GetInstance<T>(T prefab, Transform parent = null) where T : Object
        {
            T target = FindObjectOfType<T>();
            if (target != null)
            {
                return target;
            }

            return Instantiate(prefab, parent);
        }

        #endregion
    }
}
