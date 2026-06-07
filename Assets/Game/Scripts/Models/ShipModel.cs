using UnityEngine;

namespace Models
{ 
    public class ShipModel
    {
        public Vector3 SpawnPosition => new Vector3(0, -4, 0);

        private const int MaxLives = 3;
        public int CurrentLives { get; private set; }

        public ShipModel()
        {
            CurrentLives = MaxLives;
        }

        public void TakeDamage()
        {
            CurrentLives--;
        }

        public bool IsDead => CurrentLives <= 0;
    }
}
