using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public static class Constants
    {
        public static class Numbers
        {
            public const int INITIAL_POOL_SIZE = 5;
        }

        public static class Paths
        {
            public const string LEVELS_FOLDER = "Levels/";
            public const string LEVEL_COUNT = "LevelCount";
            public const string PREFAB_MAP = "PrefabMap";
            public const string RESOURCES = "Assets/Resources/";
        }

        public static class Prefs
        {
            public const string ALL_LEVELS_COMPLETE = "AllLevelsComplete";
            public const string BEATEN_LEVEL_COUNT = "BeatenLevelCount";
            public const string CURRENT_LEVEL = "CurrentLevel";
        }
    }
}
