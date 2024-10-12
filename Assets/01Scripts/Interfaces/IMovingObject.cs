using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public delegate void MovementEvent(Vector3 newPos);
    public interface IMovingObject
    {
        public event MovementEvent Moved;
    }
}