using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace SilevelGames
{
    public class LevelController : ISilevelObject
    {
        private Camera _mainCam;
        private float _levelEndZPoint,
            _levelStartOffset, 
            _levelLength = 0, 
            _platformExtent = Mathf.Infinity;
        private GameObject _levelParent;
        private List<SceneObject> decorations = new();
        private Queue<Obstacle> obstacles = new();
        private Queue<Platform> platforms = new();
        private int _level;
        private PlayerController _playerController;
        private PoolManager _poolManager;

        public Action LevelComplete;

        public float LevelStartOffset => _levelStartOffset;
        public float LevelEndZPoint => _levelEndZPoint;
        public float LevelLength => _levelLength;
        public float PlatformExtent => _platformExtent;
        public int Level => _level;

        public void Initialize(
            int level, 
            PoolManager poolManager, 
            float levelStartOffset, 
            PlayerController player,
            Camera mainCam
        )
        {
            _mainCam = mainCam;
            _level = level;
            _poolManager = poolManager;
            _levelStartOffset = levelStartOffset;
            _playerController = player;

            Initialize();
        }

        public void Initialize()
        {
            string levelName = "Level" + _level;
            LevelData levelData = Resources.Load<LevelData>(
                Constants.Paths.LEVELS_FOLDER + levelName
            );

            _levelParent = new GameObject(levelName);
            _levelParent.transform.position = Vector3.forward * _levelStartOffset;

            Obstacle newObstacle;
            for (int i = 0; i < levelData.obstacleConfigs.Count; i++)
            {
                newObstacle = _poolManager.GetObject(
                    ObjectType.Obstacle,
                    levelData.obstacleConfigs[i].hashKey
                ) as Obstacle;

                obstacles.Enqueue(newObstacle);

                newObstacle.transform.SetParent(_levelParent.transform);
                newObstacle.transform.localPosition = levelData.obstacleConfigs[i].position;
                newObstacle.transform.localScale = levelData.obstacleConfigs[i].scale;
                newObstacle.transform.localEulerAngles = levelData.obstacleConfigs[i].eulerAngles;
                newObstacle.gameObject.SetActive(true);
                newObstacle.Initialize();
            }

            for (int i = 0; i < levelData.decorationConfigs.Count; i++)
            {
                decorations.Add(
                    _poolManager.GetObject(
                        ObjectType.Decoration,
                        levelData.decorationConfigs[i].hashKey
                    )
                );

                decorations[^1].transform.SetParent(_levelParent.transform);
                decorations[^1].transform.localPosition = levelData.decorationConfigs[i].position;
                decorations[^1].transform.localScale = levelData.decorationConfigs[i].scale;
                decorations[^1].transform.localEulerAngles = levelData.decorationConfigs[i].eulerAngles;
                decorations[^1].gameObject.SetActive(true);
                decorations[^1].Initialize();
            }

            Platform newPlatform;
            for (int i = 0; i < levelData.platformConfigs.Count; i++)
            {
                newPlatform = _poolManager.GetObject(
                        ObjectType.Platform,
                        levelData.platformConfigs[i].hashKey
                    ) as Platform;

                platforms.Enqueue(newPlatform);

                newPlatform.transform.SetParent(_levelParent.transform);
                newPlatform.transform.localPosition = levelData.platformConfigs[i].position;
                newPlatform.transform.localScale = levelData.platformConfigs[i].scale;
                newPlatform.transform.localEulerAngles = levelData.platformConfigs[i].eulerAngles;
                newPlatform.gameObject.SetActive(true);
                newPlatform.Initialize();

                _levelLength += newPlatform.Length;
                if (newPlatform.Extent < _platformExtent)
                    _platformExtent = newPlatform.Extent;
            }

            _levelEndZPoint = _levelStartOffset + _levelLength;
        }

        public IEnumerator SelfDestroy()
        {
            yield return new WaitForSeconds(1f);
            RecycleAll();
            _playerController.Moved -= OnPlayerMove;
        }

        private void RecycleAll()
        {
            while (platforms.Count > 0)
                _poolManager.Recycle(platforms.Dequeue());

            while (obstacles.Count > 0)
                _poolManager.Recycle(obstacles.Dequeue());

            for (int i = 0; i < decorations.Count; i++)
                _poolManager.Recycle(decorations[i]);
        }

        public void Reset()
        {
            RecycleAll();

            _levelLength = 0;

            platforms.Clear();
            obstacles.Clear();
            decorations.Clear();
            Initialize();
        }

        public void OnGameStart()
        {
            foreach (Platform platform in platforms)
                platform.OnGameStart();

            foreach (Obstacle obstacle in obstacles)
                obstacle.OnGameStart();

            for (int i = 0; i < decorations.Count; i++)
                decorations[i].OnGameStart();

            _playerController.Moved += OnPlayerMove;
        }

        public void OnGameStop()
        {
            foreach (Platform platform in platforms)
                platform.OnGameStop();

            foreach (Obstacle obstacle in obstacles)
                obstacle.OnGameStart();

            for (int i = 0; i < decorations.Count; i++)
                decorations[i].OnGameStop();

            _playerController.Moved -= OnPlayerMove;
        }

        private void OnPlayerMove(Vector3 newPos)
        {
            if (newPos.z > _levelEndZPoint)
            {
                LevelComplete?.Invoke();
                return;
            }

            if (obstacles.Count > 0)
                if (_mainCam.WorldToScreenPoint(obstacles.Peek().MaxBound).y < 0)
                {
                    _poolManager.Recycle(obstacles.Dequeue());
                }

            if (platforms.Count > 0)
                if (_mainCam.WorldToScreenPoint(platforms.Peek().MaxBound).y < 0)
                {
                    _poolManager.Recycle(platforms.Dequeue());
                }
        }
    }
}
