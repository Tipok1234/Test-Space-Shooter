using UnityEngine;
using Interfaces;
using Managers;
using Data;
using System.Collections.Generic;
using System.Linq;
using WorldViews;
using System;
using Enums;

namespace Controllers
{
    public class AsteroidController : ITickable, IDisposable
    {
        public event Action DestroyedAsteroid;
        public event Action AllAsteroidsDestroyed;
        
        public int TotalAsteroids => _levelVariables.AsteroidCount;
        
        private readonly AsteroidPool _asteroidPool;
        private readonly BulletController _bulletController;
        private LevelVariables _levelVariables;

        private readonly Dictionary<AsteroidView, AsteroidType> _activeAsteroids =
            new Dictionary<AsteroidView, AsteroidType>();

        private float _minX;
        private float _maxX;
        private float _spawnY;
        private float _despawnY;

        private float _spawnTimer;
        
        private const float SpawnInterval = 2f;
        
        private int _asteroidsSpawned;
        
        private bool isActive;
        private bool isWin;

        public AsteroidController(AsteroidPool asteroidPool, BulletController bulletController)
        {
            _asteroidPool = asteroidPool;
            _bulletController = bulletController;

            CalculateBounds();
            _bulletController.OnBulletHitAsteroid += OnBulletHitAsteroid;
        }

        public void Activate(LevelVariables levelVariables)
        {
            _levelVariables = levelVariables;
            isActive = true;
            ResetComponents();
        }

        public void Deactivate()
        {
            isActive = false;

            foreach (var asteroid in _activeAsteroids)
            {
                _asteroidPool.Return(asteroid.Key, asteroid.Value);
            }

            _activeAsteroids.Clear();
        }

        public void Tick()
        {
            if (!isActive)
                return;

            MoveAsteroids();
            HandleSpawn();
            CheckWin();
        }
        
        public void DestroyAsteroid(AsteroidView asteroidView)
        {
            if (!_activeAsteroids.TryGetValue(asteroidView, out var asteroid)) 
                return;

            _asteroidPool.Return(asteroidView, asteroid);
            _activeAsteroids.Remove(asteroidView);
        }
        
        public void Reset()
        {
            Deactivate();
            ResetComponents();
        }

        private void ResetComponents()
        {
            isWin = false;
            _asteroidsSpawned = 0;
            _spawnTimer = 0f;
        }

        private void OnBulletHitAsteroid(AsteroidView asteroid)
        {
            DestroyAsteroid(asteroid);

            DestroyedAsteroid?.Invoke();
        }

        private void CheckWin()
        {
            if (isWin)
                return;
    
            if (_asteroidsSpawned >= _levelVariables.AsteroidCount && _activeAsteroids.Count == 0)
            {
                isWin = true;
                AllAsteroidsDestroyed?.Invoke();
            }
        }

        private void HandleSpawn()
        {
            if (_asteroidsSpawned >= _levelVariables.AsteroidCount) 
                return;

            _spawnTimer += Time.deltaTime;

            if (_spawnTimer >= SpawnInterval)
            {
                _spawnTimer = 0f;
                SpawnAsteroid();
            }
        }

        private void SpawnAsteroid()
        {
            float x = UnityEngine.Random.Range(_minX, _maxX);
            var position = new Vector3(x, _spawnY, 0);

            var asteroid = _asteroidPool.Get(_levelVariables.AsteroidType, position);
            _activeAsteroids.Add(asteroid, _levelVariables.AsteroidType);
            _asteroidsSpawned++;
        }

        private void MoveAsteroids()
        {
            foreach (var asteroid in _activeAsteroids.Keys.ToList())
            {
                asteroid.transform.position += Vector3.down * _levelVariables.AsteroidSpeed * Time.deltaTime;

                if (asteroid.transform.position.y < _despawnY)
                {
                    _asteroidPool.Return(asteroid, _activeAsteroids[asteroid]);
                    _activeAsteroids.Remove(asteroid);
                }
            }
        }

        private void CalculateBounds()
        {
            var cam = Camera.main;
            _minX = cam.ViewportToWorldPoint(new Vector2(0, 0)).x + 0.5f;
            _maxX = cam.ViewportToWorldPoint(new Vector2(1, 0)).x - 0.5f;
            _spawnY = cam.ViewportToWorldPoint(new Vector2(0, 1)).y + 1f;
            _despawnY = cam.ViewportToWorldPoint(new Vector2(0, 0)).y - 1f;
        }

        public void Dispose()
        {
            _bulletController.OnBulletHitAsteroid -= OnBulletHitAsteroid;
        }
    }
}