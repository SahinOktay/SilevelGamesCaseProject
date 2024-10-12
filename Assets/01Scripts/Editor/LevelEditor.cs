using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Linq;

namespace SilevelGames
{
    public class LevelEditor : EditorWindow
    {
        private const string LEVEL_EDIT_PARENT = "LevelEditParent";
        private GameObject _currentLevelParent;
        private int _levelBeingEdited = 0, _changeOrderLevel;
        private LevelCount _levelCount;
        private LevelData _currentLevelData;
        private PrefabMap _prefabMap;

        [MenuItem("Window/Level Editor")]
        public static void OpenWindow()
        {
            LevelEditor levelEditor = GetWindow<LevelEditor>();
            levelEditor.titleContent = new GUIContent("Level Editor");
            levelEditor.Show();
        }

        private void OnDestroy()
        {
            DestroyLevelParent();
        }

        private void CreateGUI()
        {
            string levelCountPath = Constants.Paths.RESOURCES + Constants.Paths.LEVEL_COUNT + ".asset";

            int levelCount = AssetDatabase.FindAssets(
                "",
                new[] { Constants.Paths.RESOURCES + Constants.Paths.LEVELS_FOLDER }
            ).Length;

            _levelCount = AssetDatabase.LoadAssetAtPath<LevelCount>(levelCountPath);

            if (_levelCount == null)
            {
                _levelCount = ScriptableObject.CreateInstance<LevelCount>();
                _levelCount.levelCount = levelCount;

                AssetDatabase.CreateAsset(_levelCount, levelCountPath);
                AssetDatabase.SaveAssets();
            } else if (_levelCount.levelCount != levelCount)
            {
                _levelCount.levelCount = levelCount;
                EditorUtility.SetDirty(_levelCount);
                AssetDatabase.SaveAssets();
            }

            _prefabMap = Resources.Load<PrefabMap>(Constants.Paths.PREFAB_MAP);
            _prefabMap.Initialize();

            if (_levelCount.levelCount > 0) SwitchLevel();

            _changeOrderLevel = _levelBeingEdited;
        }

        private void OnGUI()
        {
            int[] levelIndexes = Enumerable.Range(0, _levelCount.levelCount).ToArray();
            
            EditorGUI.BeginChangeCheck();

            _levelBeingEdited = EditorGUILayout.IntPopup(
                "Level to edit",
                _levelBeingEdited, 
                levelIndexes.Select(item => item.ToString()).ToArray(), 
                levelIndexes
            );

            if (EditorGUI.EndChangeCheck())
                SwitchLevel();

            if (_currentLevelData != null)
            {
                _changeOrderLevel = EditorGUILayout.IntPopup(
                    "Change Level Order",
                    _changeOrderLevel, 
                    levelIndexes.Select(item => item.ToString()).ToArray(), 
                    levelIndexes
                );

                GUI.backgroundColor = Color.blue * 2;
                if (_changeOrderLevel != _levelBeingEdited)
                    if (GUILayout.Button("Change Order"))
                        ChangeLevelOrder();

                GUI.backgroundColor = Color.white;

                if (GUILayout.Button("Save Level"))
                    SaveLevel();

                if (GUILayout.Button("Delete Level"))
                    DeleteLevel();
            }

            if (GUILayout.Button("Add New Level"))
                AddNewLevel();
        }

        private void AddNewLevel()
        {
            LevelData newLevelData = ScriptableObject.CreateInstance<LevelData>();
            AssetDatabase.CreateAsset(
                newLevelData, 
                Constants.Paths.RESOURCES +
                Constants.Paths.LEVELS_FOLDER + 
                "Level" + 
                _levelCount.levelCount +
                ".asset"
            );

            AssetDatabase.SaveAssets();

            _levelBeingEdited = _levelCount.levelCount++;
            EditorUtility.SetDirty(_levelCount);
            AssetDatabase.SaveAssets();
            SwitchLevel();
        }

        private void CreateLevelParent()
        {
            _currentLevelParent = new GameObject(LEVEL_EDIT_PARENT);
            _currentLevelParent.transform.position = Vector3.zero;
        }

        private void DestroyLevelParent()
        {
            if (_currentLevelParent != null) DestroyImmediate(_currentLevelParent);
        }

        private void LoadCurrentLevel()
        {
            _changeOrderLevel = _levelBeingEdited;

            DestroyLevelParent();
            CreateLevelParent();

            _currentLevelData = AssetDatabase.LoadAssetAtPath<LevelData>(
                Constants.Paths.RESOURCES +
                Constants.Paths.LEVELS_FOLDER +
                "Level" +
                _levelBeingEdited +
                ".asset"
            );

            GameObject spawnedObject;
            for (int i = 0; i < _currentLevelData.obstacleConfigs.Count; i++)
            {
                spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(
                    _prefabMap.GetObject(
                        ObjectType.Obstacle,
                        _currentLevelData.obstacleConfigs[i].hashKey
                    )
                );

                spawnedObject.transform.SetParent(_currentLevelParent.transform);
                spawnedObject.transform.position = _currentLevelData.obstacleConfigs[i].position;
                spawnedObject.transform.localScale = _currentLevelData.obstacleConfigs[i].scale;
                spawnedObject.transform.eulerAngles = _currentLevelData.obstacleConfigs[i].eulerAngles;
            }

            for (int i = 0; i < _currentLevelData.decorationConfigs.Count; i++)
            {
                spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(
                    _prefabMap.GetObject(
                        ObjectType.Decoration,
                        _currentLevelData.decorationConfigs[i].hashKey
                    )
                );

                spawnedObject.transform.SetParent(_currentLevelParent.transform);
                spawnedObject.transform.position = _currentLevelData.decorationConfigs[i].position;
                spawnedObject.transform.localScale = _currentLevelData.decorationConfigs[i].scale;
                spawnedObject.transform.eulerAngles = _currentLevelData.decorationConfigs[i].eulerAngles;
            }

            for (int i = 0; i < _currentLevelData.platformConfigs.Count; i++)
            {
                spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(
                    _prefabMap.GetObject(
                        ObjectType.Platform,
                        _currentLevelData.platformConfigs[i].hashKey
                    )
                );

                spawnedObject.transform.SetParent(_currentLevelParent.transform);
                spawnedObject.transform.position = _currentLevelData.platformConfigs[i].position;
                spawnedObject.transform.localScale = _currentLevelData.platformConfigs[i].scale;
                spawnedObject.transform.eulerAngles = _currentLevelData.platformConfigs[i].eulerAngles;
            }
        }

        private void SwitchLevel()
        {
            GameObject[] rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();

            for (int i = 0; i < rootObjects.Length; i++)
            {
                if (rootObjects[i].name != "Core")
                    DestroyImmediate(rootObjects[i]);
            }

            LoadCurrentLevel();
        }

        private void ChangeLevelOrder()
        {
            AssetDatabase.RenameAsset(
                Constants.Paths.RESOURCES +
                Constants.Paths.LEVELS_FOLDER +
                "Level" +
                _levelBeingEdited +
                ".asset",
                "ToBeChanged"
            );

            if (_changeOrderLevel < _levelBeingEdited)
            {
                for (int i = _levelBeingEdited - 1; i >= _changeOrderLevel; i--)
                {
                    AssetDatabase.RenameAsset(
                        Constants.Paths.RESOURCES +
                        Constants.Paths.LEVELS_FOLDER +
                        "Level" +
                        i +
                        ".asset",
                        "Level" + (i + 1)
                    );
                }
            } else
            {
                for (int i = _levelBeingEdited + 1; i <= _changeOrderLevel; i++)
                {
                    AssetDatabase.RenameAsset(
                        Constants.Paths.RESOURCES +
                        Constants.Paths.LEVELS_FOLDER +
                        "Level" +
                        i +
                        ".asset",
                        "Level" + (i - 1)
                    );
                }
            }
            AssetDatabase.RenameAsset(
                Constants.Paths.RESOURCES +
                Constants.Paths.LEVELS_FOLDER +
                "ToBeChanged.asset",
                "Level" + _changeOrderLevel
            );

            _levelBeingEdited = _changeOrderLevel;
            LoadCurrentLevel();
        }

        private void DeleteLevel()
        {
            AssetDatabase.DeleteAsset(
                Constants.Paths.RESOURCES +
                Constants.Paths.LEVELS_FOLDER +
                "Level" +
                _levelBeingEdited +
                ".asset"
            );

            for (int i = _levelBeingEdited + 1; i < _levelCount.levelCount; i++)
            {
                AssetDatabase.RenameAsset(
                    Constants.Paths.RESOURCES +
                    Constants.Paths.LEVELS_FOLDER +
                    "Level" +
                    i +
                    ".asset",
                    "Level" + (i - 1)
                );
            }

            _levelCount.levelCount--;

            if (_levelCount.levelCount > 0)
            {
                _levelBeingEdited = Mathf.Clamp(_levelBeingEdited, 0, _levelCount.levelCount - 1);
                LoadCurrentLevel();
            }

            EditorUtility.SetDirty(_levelCount);
            AssetDatabase.SaveAssets();
        }

        private void SaveLevel()
        {
            if (_currentLevelParent == null) CreateLevelParent();

            GameObject[] rootObjects = EditorSceneManager.GetActiveScene().GetRootGameObjects();

            for (int i = 0; i < rootObjects.Length; i++)
            {
                if (rootObjects[i].name != "Core")
                    rootObjects[i].transform.SetParent(_currentLevelParent.transform);
            }

            SceneObject[] sceneObjects = FindObjectsOfType<SceneObject>();
            sceneObjects = sceneObjects.OrderBy(x => x.MaxBound.z).ToArray();

            _currentLevelData.obstacleConfigs.Clear();
            _currentLevelData.decorationConfigs.Clear();
            _currentLevelData.platformConfigs.Clear();

            for (int i = 0; i < sceneObjects.Length; i++)
            {
                switch (sceneObjects[i].ObjectType)
                {
                    case ObjectType.Obstacle:
                        _currentLevelData.obstacleConfigs.Add(new ObjectConfig());

                        _currentLevelData.obstacleConfigs[^1].hashKey = sceneObjects[i].gameObject.tag;
                        _currentLevelData.obstacleConfigs[^1].scale = sceneObjects[i].transform.localScale;
                        _currentLevelData.obstacleConfigs[^1].eulerAngles = sceneObjects[i].transform.eulerAngles;
                        _currentLevelData.obstacleConfigs[^1].position = sceneObjects[i].transform.position;
                        break;

                    case ObjectType.Platform:
                        _currentLevelData.platformConfigs.Add(new ObjectConfig());

                        _currentLevelData.platformConfigs[^1].hashKey = sceneObjects[i].gameObject.tag;
                        _currentLevelData.platformConfigs[^1].scale = sceneObjects[i].transform.localScale;
                        _currentLevelData.platformConfigs[^1].eulerAngles = sceneObjects[i].transform.eulerAngles;
                        _currentLevelData.platformConfigs[^1].position = sceneObjects[i].transform.position;
                        break;

                    case ObjectType.Decoration:
                        _currentLevelData.decorationConfigs.Add(new ObjectConfig());

                        _currentLevelData.decorationConfigs[^1].hashKey = sceneObjects[i].gameObject.tag;
                        _currentLevelData.decorationConfigs[^1].scale = sceneObjects[i].transform.localScale;
                        _currentLevelData.decorationConfigs[^1].eulerAngles = sceneObjects[i].transform.eulerAngles;
                        _currentLevelData.decorationConfigs[^1].position = sceneObjects[i].transform.position;
                        break;

                    default: break;
                }
            }

            EditorUtility.SetDirty(_currentLevelData);
            AssetDatabase.SaveAssets();
        }
    }
}
