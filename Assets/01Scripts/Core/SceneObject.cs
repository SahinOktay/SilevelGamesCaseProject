using SilevelGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public class SceneObject : MonoBehaviour, ISilevelObject, IPoolable
    {
        [SerializeField] private ObjectType objectType;

        public ObjectType ObjectType => objectType;
        public virtual Vector3 MaxBound => transform.position;

        public virtual void Initialize()
        {
            
        }

        public virtual void Reset()
        {
            
        }

        public virtual void OnGameStart()
        {
            
        }

        public virtual void OnGameStop()
        {

        }
    }
}