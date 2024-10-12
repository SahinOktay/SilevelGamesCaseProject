using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public class Platform : SceneObject
    {
        [SerializeField] private MeshRenderer mainRenderer;

        public override Vector3 MaxBound => mainRenderer.bounds.max;
        public float Length
        { 
            get
            {
                float localMinZ = transform.parent.InverseTransformPoint(Vector3.forward * mainRenderer.bounds.min.z).z;

                return mainRenderer.bounds.size.z - (
                    localMinZ < 0 ? Mathf.Abs(localMinZ) : 0
                );
            }
        }
        public float Extent => mainRenderer.bounds.extents.x;
    }
}