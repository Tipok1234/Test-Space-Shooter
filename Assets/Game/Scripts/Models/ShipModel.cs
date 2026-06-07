using UnityEngine;

namespace Models
{ 
    public class ShipModel
    {
        public Vector3 SpawnPosition => new Vector3(0, -4, 0);
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
