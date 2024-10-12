using SilevelGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private UIController uIController;
    [SerializeField] private PlayerController playerController;

    private bool _allLevelsComplete;
    private float _currentLevelStart = 0;
    private int _currentLevel = 0, _levelCount, _beatenLevelCount;
    private LevelController _currentLevelController, _nextLevelController;
    private PoolManager _poolManager;

    private void Awake()
    {
        Time.maximumDeltaTime = 1f / 30f;
        _allLevelsComplete = PlayerPrefs.GetInt(Constants.Prefs.ALL_LEVELS_COMPLETE, 0) == 1;
        _currentLevel = PlayerPrefs.GetInt(Constants.Prefs.CURRENT_LEVEL, 0);
        _beatenLevelCount = PlayerPrefs.GetInt(Constants.Prefs.BEATEN_LEVEL_COUNT, 0);

        _levelCount = Resources.Load<LevelCount>(Constants.Paths.LEVEL_COUNT).levelCount;
        _poolManager = new PoolManager();

        _currentLevelController = new LevelController();
        _currentLevelController.Initialize(
            _currentLevel, 
            _poolManager, 
            _currentLevelStart, 
            playerController,
            cameraController.MainCam
        );
        _currentLevelController.LevelComplete += OnLevelComplete;

        playerController.PlatformExtent = _currentLevelController.PlatformExtent;

        _currentLevelStart += _currentLevelController.LevelLength;

        _nextLevelController = new LevelController();
        _nextLevelController.Initialize(
            NextLevel(), 
            _poolManager, 
            _currentLevelStart, 
            playerController,
            cameraController.MainCam
        );

        inputManager.Tap += OnFirstTap;

        uIController.Initialize();
        playerController.Initialize();
        cameraController.Initialize();

        uIController.SetLevel(_beatenLevelCount);
    }

    private int NextLevel()
    {
        if (_allLevelsComplete)
        {
            int nextLevel = Random.Range(0, _levelCount);

            if (_levelCount > 1)
                while (nextLevel == _currentLevel)
                    nextLevel = Random.Range(0, _levelCount);

            return nextLevel;
        }

        return _currentLevel + 1;
    }

    private void OnFirstTap()
    {
        inputManager.Tap -= OnFirstTap;

        uIController.OnGameStart();
        playerController.OnGameStart();
        cameraController.OnGameStart();
        _currentLevelController.OnGameStart();

        playerController.GotDamaged += OnPlayerTakeDamage;
    }

    private void OnLevelComplete()
    {
        _currentLevelController.LevelComplete -= OnLevelComplete;
        StartCoroutine(_currentLevelController.SelfDestroy());

        _currentLevel = _nextLevelController.Level;
        PlayerPrefs.SetInt(Constants.Prefs.CURRENT_LEVEL, _currentLevel);

        _beatenLevelCount++;
        PlayerPrefs.SetInt(Constants.Prefs.BEATEN_LEVEL_COUNT, _beatenLevelCount);

        if (!_allLevelsComplete) 
        {
            _allLevelsComplete = _currentLevel == _levelCount - 1;
            if (_allLevelsComplete)
                PlayerPrefs.SetInt(Constants.Prefs.ALL_LEVELS_COMPLETE, 1);
        }

        PlayerPrefs.Save();

        uIController.SetLevel(_beatenLevelCount);


        _currentLevelController = _nextLevelController;
        _currentLevelController.LevelComplete += OnLevelComplete;
        playerController.PlatformExtent = _currentLevelController.PlatformExtent;
        _currentLevelController.OnGameStart();

        _currentLevelStart = _currentLevelController.LevelEndZPoint;

        _nextLevelController = new LevelController();
        _nextLevelController.Initialize(
            NextLevel(), 
            _poolManager, 
            _currentLevelStart, 
            playerController,
            cameraController.MainCam
        );
    }

    private void OnPlayerTakeDamage(IDamagable damagable)
    {
        playerController.GotDamaged -= OnPlayerTakeDamage;
        uIController.RetryButtonTap += OnRetryTap;

        _currentLevelController.OnGameStop();
        uIController.OnGameStop();
        playerController.OnGameStop();
        cameraController.OnGameStop();
    }

    private void OnRetryTap()
    {
        playerController.transform.position = Vector3.forward * _currentLevelController.LevelStartOffset;
        uIController.RetryButtonTap -= OnRetryTap;

        uIController.Reset();
        playerController.Reset();
        _currentLevelController.Reset();
        cameraController.Reset();
        inputManager.Tap += OnFirstTap;
    }
}
