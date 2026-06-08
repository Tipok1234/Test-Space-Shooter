using UnityEngine;
using System;

namespace WorldViews
{
    public class BulletView : MonoBehaviour
    {
        public event Action<BulletView, AsteroidView> HitAsteroid;

        public void Launch(Vector3 position)
        {
            transform.position = position;
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<AsteroidView>(out var asteroidView))
            {
                HitAsteroid?.Invoke(this, asteroidView);
            }
        }
    }
}
