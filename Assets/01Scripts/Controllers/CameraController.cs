using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public class CameraController : MonoBehaviour, ISilevelObject
    {
        [SerializeField] private Camera mainCam;
        [SerializeField] private Transform movementTarget;

        private IMovingObject _target;
        private Vector3 _offset;

        public Camera MainCam => mainCam;

        public void Initialize()
        {
            _target = movementTarget.GetComponent<IMovingObject>();
            _offset = transform.position - movementTarget.transform.position;
        }

        public void Reset()
        {
            transform.position = movementTarget.position + _offset;
        }

        public void OnGameStart()
        {
            _target.Moved += OnTargetMoved;
        }

        public void OnGameStop()
        {
            _target.Moved -= OnTargetMoved;
        }

        private void OnTargetMoved(Vector3 newPos)
        {
            transform.position = movementTarget.position + _offset;
        }
    }
}
