using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public class LevelData : ScriptableObject
    {
        public List<ObjectConfig> decorationConfigs = new();
        public List<ObjectConfig> obstacleConfigs = new();
        public List<ObjectConfig> platformConfigs = new();
    }

    [Serializable]
    public class ObjectConfig
    {
        public string hashKey;
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
    }
}