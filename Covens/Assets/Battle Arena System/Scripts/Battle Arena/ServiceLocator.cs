using Raincrow.BattleArena.Controllers;
using Raincrow.BattleArena.Factory;
using Raincrow.BattleArena.Views;
using Raincrow.Loading.View;
using Raincrow.Utils;
using UnityEngine;

namespace Raincrow.Services
{
    public class ServiceLocator : MonoBehaviour, ISummoningView
    {        
        [Header("Service Prefabs")]
        [SerializeField] private AvatarSpriteUtil _avatarSpriteUtilPrefab; // Avatar Sprite Util Prefab
        [SerializeField] private LoadingView _loadingViewPrefab;
        [SerializeField] private BattleController _battleControllerPrefab;
        [SerializeField] private Camera _battleCameraPrefab;
        [SerializeField] private ObjectPool _objectPoolPrefab;
        [SerializeField] private CharactersTurnOrderView _charactersTurnOrderViewPrefab;
        [SerializeField] private QuickCastView _quickCastViewPrefab;
        [SerializeField] private CountdownView _countdownViewPrefab;
        [SerializeField] private EnergyView _energyViewPrefab;
        [SerializeField] private BarEventLogView _barEventLogViewPrefab;
        [SerializeField] private PlayerBadgeView _playerBadgeViewPrefab;
        [SerializeField] private CameraTargetController _cameraTargetControllerPrefab;
        [SerializeField] private CelebratoryView _celebratoryViewPrefab;
        [SerializeField] private DebriefView _debriefViewPrefab;
        [SerializeField] private SmoothCameraFollow _smoothCameraFollowPrefab;
        [SerializeField] private RewardsBatttleView _rewardsBatttleViewPrefab;

        [Header("Service Instances")]
        [SerializeField] private AvatarSpriteUtil _avatarSpriteUtilInstance; // Avatar Sprite Util Instance
        [SerializeField] private LoadingView _loadingViewInstance; // Loading View Instance
        [SerializeField] private BattleController _battleControllerInstance;
        [SerializeField] private Camera _battleCameraInstance;
        [SerializeField] private ObjectPool _objectPoolInstance;
        [SerializeField] private CharactersTurnOrderView _charactersTurnOrderViewInstance;
        [SerializeField] private QuickCastView _quickCastViewInstance;
        [SerializeField] private CountdownView _countdownViewInstace;
        [SerializeField] private EnergyView _energyViewInstance;
        [SerializeField] private BarEventLogView _barEventLogViewInstace;
        [SerializeField] private PlayerBadgeView _playerBadgeViewInstance;
        [SerializeField] private CameraTargetController _cameraTargetControllerInstance;
        [SerializeField] private CelebratoryView _celebratoryViewInstance;
        [SerializeField] private DebriefView _debriefViewInstance;
        [SerializeField] private SmoothCameraFollow _smoothCameraFollowInstance;
        [SerializeField] private RewardsBatttleView _rewardsBatttleViewInstance;

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

        public IWitchPortraitFactory GetWitchPortraitFactory()
        {
            if (_avatarSpriteUtilInstance == null)
            {
                _avatarSpriteUtilInstance = GetInstance(_avatarSpriteUtilPrefab);
            }
            return _avatarSpriteUtilInstance;
        }

        public ICharactersTurnOrderView GetCharactersTurnOrderView()
        {
            if (_charactersTurnOrderViewInstance == null)
            {
                _charactersTurnOrderViewInstance = GetInstance(_charactersTurnOrderViewPrefab, _mainCanvas.transform);
            }
            return _charactersTurnOrderViewInstance;
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

        public ObjectPool GetObjectPool()
        {
            if (_objectPoolInstance == null)
            {
                _objectPoolInstance = GetInstance(_objectPoolPrefab);
            }
            return _objectPoolInstance;
        }

        public IQuickCastView GetQuickCastView()
        {
            if (_quickCastViewInstance == null)
            {
                _quickCastViewInstance = GetInstance(_quickCastViewPrefab);
            }
            return _quickCastViewInstance;
        }

        public ICountdownView GetCountdownView()
        {
            if (_countdownViewInstace == null)
            {
                _countdownViewInstace = GetInstance(_countdownViewPrefab);
            }
            return _countdownViewInstace;
        }

        public IBarEventLogView GetBarEventLogView()
        {
            if (_barEventLogViewInstace == null)
            {
                _barEventLogViewInstace = GetInstance(_barEventLogViewPrefab);
            }
            return _barEventLogViewInstace;
        }

        public Canvas GetMainCanvas()
        {
            return _mainCanvas;
        }

        public IEnergyView GetEnergyView()
        {
            if (_energyViewInstance == null)
            {
                _energyViewInstance = GetInstance(_energyViewPrefab);
            }
            return _energyViewInstance;
        }

        public IPlayerBadgeView GetPlayerBadgeView()
        {
            if (_playerBadgeViewInstance == null)
            {
                _playerBadgeViewInstance = GetInstance(_playerBadgeViewPrefab);
            }
            return _playerBadgeViewInstance;
        }

        public ICameraTargetController GetCameraTargetController()
        {
            if (_cameraTargetControllerInstance == null)
            {
                _cameraTargetControllerInstance = GetInstance(_cameraTargetControllerPrefab);
            }
            return _cameraTargetControllerInstance;
        }

        public ICelebratoryView GetCelebratoryView()
        {
            if (_celebratoryViewInstance == null)
            {
                _celebratoryViewInstance = GetInstance(_celebratoryViewPrefab);
            }
            return _celebratoryViewInstance;
        }

        public IDebriefView GetDebriefView()
        {
            if (_debriefViewInstance == null)
            {
                _debriefViewInstance = GetInstance(_debriefViewPrefab);
            }
            return _debriefViewInstance;
        }

        public ISmoothCameraFollow GetSmoothCameraFollow()
        {
            if (_smoothCameraFollowInstance == null)
            {
                _smoothCameraFollowInstance = GetInstance(_smoothCameraFollowPrefab);
            }
            return _smoothCameraFollowInstance;
        }  
        
        public IRewardsBatttleView GetRewardsBatttleView()
        {
            if (_rewardsBatttleViewInstance == null)
            {
                _rewardsBatttleViewInstance = GetInstance(_rewardsBatttleViewPrefab);
            }
            return _rewardsBatttleViewInstance;
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


        public void Open(System.Action<string> onSummon)
        {
            BattleArena.Views.UISummoning.Open(onSummon);
        }
    }
}
