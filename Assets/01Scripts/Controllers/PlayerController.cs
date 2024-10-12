using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public class PlayerController : MonoBehaviour, ISilevelObject, IMovingObject, IDamagable
    {
        [SerializeField] private CapsuleCollider mCollider;
        [SerializeField] private float forwardSpeed = 10f;
        [SerializeField] private InputManager inputManager;

        private float _minBound, _maxBound, _posRatio = .5f;

        public event DamageEvent GotDamaged;
        public event MovementEvent Moved;

        public float PlatformExtent 
        { 
            set 
            { 
                _minBound = -Mathf.Abs(value) + mCollider.radius;
                _maxBound = Mathf.Abs(value) - mCollider.radius;
            } 
        }

        public void Initialize()
        {

        }

        public void Reset()
        {
            
        }

        public void OnGameStart()
        {
            inputManager.Swerve += OnSwerve;
        }

        public void OnGameStop()
        {
            inputManager.Swerve -= OnSwerve;
        }

        private void OnSwerve(float swerveRatio)
        {
            Vector3 currentPos = transform.position;
            float oldRatio = _posRatio;
            _posRatio = Mathf.Clamp01(_posRatio + swerveRatio);

            float forwardDistance = forwardSpeed * Time.deltaTime;

            currentPos += Vector3.forward * forwardDistance;

            transform.position = new Vector3(
                Mathf.Lerp(_minBound, _maxBound, Mathf.Lerp(oldRatio, _posRatio, .25f)), 
                currentPos.y, 
                currentPos.z
            );
            Moved?.Invoke(transform.position);
        }

        public void GetDamaged()
        {
            GotDamaged?.Invoke(this);
        }
    }
}
