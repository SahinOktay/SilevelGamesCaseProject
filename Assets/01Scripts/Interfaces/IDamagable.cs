using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SilevelGames
{
    public delegate void DamageEvent(IDamagable damagable);
    public interface IDamagable
    {
        public event DamageEvent GotDamaged;
        public void GetDamaged();
    }
}