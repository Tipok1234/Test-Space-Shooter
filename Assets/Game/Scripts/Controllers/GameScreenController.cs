using Core;
using Managers;
using Screens;
using Models;
using UnityEngine;
using WorldViews;
using Datas;
using System;

namespace Controllers
{
    public class GameScreenController
    {
        private readonly GameScreens _screens;
        private readonly GameScreenCallbacks _callbacks;
        private readonly ShipModel _shipModel;
        private readonly PrefabConfig _prefabConfig;
        private readonly Ticker _ticker;
        private readonly Transform _asteroidsParent;

        private ShipView _shipView;
        private ShipController _shipController;
        private BulletController _bulletController;
        private AsteroidController _asteroidController;
        private LevelVariables _currentLevelVariables;
        
        private int _currentLevelId;
        private int _currentScore;

        private bool isGameOver;
        
        public GameScreenController(GameScreens screens, ShipModel shipModel, PrefabConfig prefabConfig, Ticker ticker, Transform asteroidsParent, GameScreenCallbacks callbacks)
        {
            _screens = screens;
            _shipModel = shipModel;
            _prefabConfig = prefabConfig;
            _ticker = ticker;
            _asteroidsParent = asteroidsParent;
            _callbacks = callbacks;
        }

        public void Show(int levelId, LevelVariables levelVariables)
        {
            isGameOver = false;
            _currentLevelId = levelId;
            _currentLevelVariables = levelVariables;
            _currentScore = 0;

            _screens.GameScreen.OpenScreen();
            _screens.GameScreen.UpdateScore(0);
            _screens.GameScreen.UpdateHealth(_shipModel.MaxLives);

            if (_shipView == null)
            {
                Setup(levelVariables);
            }
            else
            {
                ResetControllers(levelVariables);
            }
        }

        private void Setup(LevelVariables levelVariables)
        {
            SpawnShip();
            SpawnBullets();
            SpawnAsteroids(levelVariables);
            InitShipController();
        }

        private void ResetControllers(LevelVariables levelVariables)
        {
            _shipView.ResetShip(_shipModel.SpawnPosition);
            _shipController.ResetShipController();
            _ticker.Register(_shipController);
            _bulletController.ResetBullets();
            _asteroidController.ResetAsteroids(levelVariables);
            _asteroidController.Activate(_bulletController);
        }

        private void SpawnShip()
        {
            if (_shipView == null)
            {
                _shipView = UnityEngine.Object.Instantiate(_prefabConfig.ShipPrefab, _shipModel.SpawnPosition, Quaternion.identity);
            }
            else
            {
                _shipView.ResetShip(_shipModel.SpawnPosition);
            }
        }

        private void SpawnBullets()
        {
            var bulletPool = new BulletPool(_prefabConfig.BulletPrefab, _shipView.BulletSpawnPoint);
            _bulletController = new BulletController(bulletPool);
            _ticker.Register(_bulletController);
        }

        private void SpawnAsteroids(LevelVariables levelVariables)
        {
            var asteroidPool = new AsteroidPool(
                _prefabConfig.SmallAsteroidPrefab,
                _prefabConfig.MediumAsteroidPrefab,
                _prefabConfig.LargeAsteroidPrefab,
                _asteroidsParent
            );
            _asteroidController = new AsteroidController(asteroidPool, levelVariables);
            _asteroidController.Activate(_bulletController);
            _asteroidController.OnScoreChanged += OnScoreChanged;
            _asteroidController.OnAllAsteroidsDestroyed += OnWin;
            _ticker.Register(_asteroidController);
        }
        
        private void OnScoreChanged(int score)
        {
            _currentScore = score;
            _screens.GameScreen.UpdateScore(score);
        }

        private void OnWin()
        {
            if (isGameOver)
                return;
    
            _shipController.Deactivate();
            _screens.WinScreen.Init(OnMenu, OnNextLevel);
            _screens.WinScreen.UpdateView(_currentScore / 10, _asteroidController.TotalAsteroids);
            _screens.WinScreen.OpenScreen();
            _callbacks.OnWin?.Invoke(_currentLevelId);
        }
        
        private void OnMenu()
        {
            Cleanup();
            _shipModel.ResetShipModel();
            _screens.WinScreen.CloseScreen();
            _screens.GameScreen.CloseScreen();
            _callbacks.OnMenu?.Invoke();
        }

        private void OnNextLevel()
        {
            Cleanup();
            _shipModel.ResetShipModel();
            _screens.WinScreen.CloseScreen();
            _callbacks.OnNext?.Invoke(_currentLevelId);
        }
        
        private void OnAsteroidHit(AsteroidView asteroidView)
        {
            _asteroidController.DestroyAsteroid(asteroidView);
        }
        
        private void OnHealthChanged(int health)
        {
            _screens.GameScreen.UpdateHealth(health);
        }

        private void OnLose()
        {
            isGameOver = true;
            _shipController.Deactivate();
            _screens.LoseScreen.Init(OnRestart);
            _screens.LoseScreen.OpenScreen();
            _ticker.Register(_screens.LoseScreen);
        }
        
        private void OnRestart()
        {
            Cleanup();
            _shipModel.ResetShipModel();
            Show(_currentLevelId, _currentLevelVariables);
        }

        private void InitShipController()
        {
            _shipController = new ShipController(_shipModel, _shipView, _bulletController);
            _shipController.OnHealthChanged += OnHealthChanged;
            _shipController.OnAsteroidHit += OnAsteroidHit;
            _shipController.OnDeath += OnLose;
            _shipController.OnDeactivate += () => _ticker.Unregister(_shipController);
            _ticker.Register(_shipController);
        }
        
        private void Cleanup()
        {
            _ticker.Unregister(_screens.LoseScreen);
            _screens.LoseScreen.CloseScreen();
            _asteroidController.Deactivate();
        }
    }
}
