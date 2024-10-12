using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SilevelGames
{
    [CreateAssetMenu(fileName = "PrefabMap", menuName = "ScriptableObjects/PrefabMap", order = 1)]
    public class PrefabMap : ScriptableObject
    {
        [SerializeField] private List<MappedObject> objects;

        private bool _initialized = false;
        private Dictionary<ObjectType, Dictionary<string, GameObject>> _map;

        public string[] GetKeyArray(ObjectType type) => _map[type].Keys.ToArray();
        public GameObject GetObject(ObjectType type, string hashKey) => _map[type][hashKey];

        public void Initialize()
        {
            if (Application.isPlaying)
            {
                if (_initialized) return;
                _initialized = true;
            }

            _map = new Dictionary<ObjectType, Dictionary<string, GameObject>>();
            foreach (int i in Enum.GetValues(typeof(ObjectType)))
            {
                _map.Add((ObjectType)i, new Dictionary<string, GameObject>());
            }

            for (int i = 0; i < objects.Count; i++)
            {
                if (!_map[objects[i].objectType].ContainsKey(objects[i].gameObject.tag))
                    _map[objects[i].objectType].Add(objects[i].gameObject.tag, objects[i].gameObject);
                else 
                    _map[objects[i].objectType][objects[i].gameObject.tag] = objects[i].gameObject;
            }
        }
    }

    [Serializable]
    public class MappedObject
    {
        public ObjectType objectType;
        public GameObject gameObject;
    }
}