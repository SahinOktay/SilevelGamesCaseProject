using SilevelGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public class PoolManager
    {
        private Dictionary<ObjectType, Dictionary<string, Pool<SceneObject>>> pools = new();

        public PoolManager() 
        {
            PrefabMap prefabMap = Resources.Load<PrefabMap>(Constants.Paths.PREFAB_MAP);
            prefabMap.Initialize();

            string[] keyArray;
            ObjectType currentObjectType;
            foreach (int i in Enum.GetValues(typeof(ObjectType)))
            {
                currentObjectType = (ObjectType)i;
                pools.Add(currentObjectType, new());
                keyArray = prefabMap.GetKeyArray(currentObjectType);

                for (int j = 0; j < keyArray.Length; j++)
                {
                    pools[currentObjectType].Add(
                        keyArray[j],
                        new Pool<SceneObject>(
                            new PrefabFactory<SceneObject>(prefabMap.GetObject(currentObjectType, keyArray[j])),
                            Constants.Numbers.INITIAL_POOL_SIZE
                        )
                    );
                }
            }
        }

        public SceneObject GetObject(ObjectType type, string hashKey) => pools[type][hashKey].GetItem();
        public void Recycle(SceneObject sceneObject)
        {
            pools[sceneObject.ObjectType][sceneObject.tag].Recycle(sceneObject);
        }
    }
}