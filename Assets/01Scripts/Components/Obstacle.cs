using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public class Obstacle : SceneObject
    {
        [SerializeField] public Collider mCollider;

        public override Vector3 MaxBound => mCollider.bounds.max;

        private void OnTriggerEnter(Collider other)
        {
            other.gameObject.GetComponentInParent<IDamagable>()?.GetDamaged();
        }
    }
}