using UnityEngine;
using Interfaces;
using Managers;
using Datas;
using System.Collections.Generic;
using System.Linq;
using WorldViews;
using Enums;
using System;

namespace Controllers
{
    public class AsteroidController : ITickable
    {
        public event Action<int> OnScoreChanged;
        public event Action OnAllAsteroidsDestroyed;
        
        public int TotalAsteroids => _levelVariables.AsteroidCount;
        
        private readonly AsteroidPool _asteroidPool;
        private LevelVariables _levelVariables;

        private readonly Dictionary<AsteroidView, AsteroidTypeEnum> _activeAsteroids =
            new Dictionary<AsteroidView, AsteroidTypeEnum>();

        private float _minX;
        private float _maxX;
        private float _spawnY;
        private float _despawnY;

        private float _spawnTimer;
        
        private const float SpawnInterval = 2f;
        private const int AsteroidScoreCollect = 10;
        
        private int _asteroidsSpawned;
        private int _score;
        
        private bool isActive;
        private bool isWin;

        public AsteroidController(AsteroidPool asteroidPool, LevelVariables levelVariables)
        {
            _asteroidPool = asteroidPool;
            _levelVariables = levelVariables;

            CalculateBounds();
        }

        public void Activate(BulletController bulletController)
        {
            isActive = true;
            isWin = false;
            _asteroidsSpawned = 0;
            _score = 0;
            _spawnTimer = 0f;

            bulletController.OnBulletHitAsteroid += OnBulletHitAsteroid;
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
            if (!_activeAsteroids.ContainsKey(asteroidView)) return;

            _asteroidPool.Return(asteroidView, _activeAsteroids[asteroidView]);
            _activeAsteroids.Remove(asteroidView);
        }
        
        public void ResetAsteroids(LevelVariables levelVariables)
        {
            Deactivate();
            _levelVariables = levelVariables; 
            isWin = false;
            _asteroidsSpawned = 0;
            _score = 0;
            _spawnTimer = 0f;
        }

        private void OnBulletHitAsteroid(BulletView bullet, AsteroidView asteroid)
        {
            if (!_activeAsteroids.ContainsKey(asteroid)) return;

            _asteroidPool.Return(asteroid, _activeAsteroids[asteroid]);
            _activeAsteroids.Remove(asteroid);
            _score += AsteroidScoreCollect;

            OnScoreChanged?.Invoke(_score);
        }

        private void CheckWin()
        {
            if (isWin)
                return;
    
            if (_asteroidsSpawned >= _levelVariables.AsteroidCount && _activeAsteroids.Count == 0)
            {
                isWin = true;
                OnAllAsteroidsDestroyed?.Invoke();
            }
        }

        private void HandleSpawn()
        {
            if (_asteroidsSpawned >= _levelVariables.AsteroidCount) return;

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
            _minX = cam.ViewportToWorldPoint(new Vector2(0, 0)).x;
            _maxX = cam.ViewportToWorldPoint(new Vector2(1, 0)).x;
            _spawnY = cam.ViewportToWorldPoint(new Vector2(0, 1)).y + 1f;
            _despawnY = cam.ViewportToWorldPoint(new Vector2(0, 0)).y - 1f;
        }
    }
}