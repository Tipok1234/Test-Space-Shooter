using UnityEngine;

namespace Models
{ 
    public class ShipModel
    {
        public int MaxLives { get; } = 3;
        public int CurrentLives { get; private set; }

        public ShipModel()
        {
            CurrentLives = MaxLives;
        }

        public void TakeDamage()
        {
            CurrentLives--;
        }
        
        public void ResetShipModel()
        {
            CurrentLives = MaxLives;
        }
        public bool IsDead => CurrentLives <= 0;
    }
}
