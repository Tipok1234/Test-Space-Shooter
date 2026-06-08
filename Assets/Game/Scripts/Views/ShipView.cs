using UnityEngine;
using System;

namespace Views
{
    public class ShipView : MonoBehaviour
    {
        public Transform BulletSpawnPoint => bulletSpawnPoint;
        
        [SerializeField] private Transform bulletSpawnPoint;
        [SerializeField] private Vector3 spawnPosition;
        
        public event Action<AsteroidView> HitAsteroid;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<AsteroidView>(out var asteroidView))
            {
                HitAsteroid?.Invoke(asteroidView);
            }
        }
        
        public void Activate()
        {
            transform.position = spawnPosition;
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
