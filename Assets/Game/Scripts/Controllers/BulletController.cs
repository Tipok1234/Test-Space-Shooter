using UnityEngine;
using Interfaces;
using Managers;
using WorldViews;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Controllers
{
    public class BulletController : ITickable
    {
        private readonly BulletPool _bulletPool;
        private readonly Dictionary<BulletView, bool> _activeBullets = new Dictionary<BulletView, bool>();

        public event Action<AsteroidView> OnBulletHitAsteroid;

        private const float BulletSpeed = 10f;
        
        private float _maxY;

        public BulletController(BulletPool bulletPool)
        {
            _bulletPool = bulletPool;
            _maxY = Camera.main.ViewportToWorldPoint(new Vector2(0, 1)).y;
        }

        public void Tick()
        {
            MoveBullets();
        }

        public void Shoot(Vector3 spawnPosition)
        {
            var bullet = _bulletPool.Get(spawnPosition);
            bullet.OnHitAsteroid += OnHitAsteroid;
            _activeBullets.Add(bullet, true);
        }
        
        public void Reset()
        {
            foreach (var bullet in _activeBullets.Keys.ToList())
            {
                ReturnBullet(bullet);
            }
            _activeBullets.Clear();
        }

        private void MoveBullets()
        {
            foreach (var bullet in _activeBullets.Keys.ToList())
            {
                bullet.transform.position += Vector3.up * BulletSpeed * Time.deltaTime;

                if (bullet.transform.position.y > _maxY)
                {
                    ReturnBullet(bullet);
                }
            }
        }

        private void OnHitAsteroid(BulletView bullet, AsteroidView asteroid)
        {
            OnBulletHitAsteroid?.Invoke(asteroid);
            ReturnBullet(bullet);
        }

        private void ReturnBullet(BulletView bullet)
        {
            bullet.OnHitAsteroid -= OnHitAsteroid;
            _activeBullets.Remove(bullet);
            _bulletPool.Return(bullet);
        }
    }
}