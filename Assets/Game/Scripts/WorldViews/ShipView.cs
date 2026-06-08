using UnityEngine;
using System;

namespace WorldViews
{
    public class ShipView : MonoBehaviour
    {
        public Transform BulletSpawnPoint => bulletSpawnPoint;
        
        [SerializeField] private Transform bulletSpawnPoint;
        public Vector3 SpawnPosition => new Vector3(0, -4, 0);
        
        public event Action<AsteroidView> OnHitAsteroid;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<AsteroidView>(out var asteroidView))
            {
                OnHitAsteroid?.Invoke(asteroidView);
            }
        }
        
        public void ResetShip()
        {
            transform.position = SpawnPosition;
            gameObject.SetActive(true);
        }

        public void SetPosition(Vector3 position)
        {
            transform.position = position;
        }
        
        public void Deactivate()
        {
            gameObject.SetActive(false);
        }
    }
}
