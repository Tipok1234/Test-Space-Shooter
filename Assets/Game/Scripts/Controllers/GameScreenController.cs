using Core;
using Managers;
using Screens;
using Models;
using UnityEngine;
using WorldViews;
using Datas;

namespace Controllers
{
    public class GameScreenController
    {
        private readonly GameScreen _gameScreen;
        private readonly LoseScreen _loseScreen;
        private readonly ShipModel _shipModel;
        private readonly PrefabConfig _prefabConfig;
        private readonly Ticker _ticker;

        private ShipView _shipView;
        private ShipController _shipController;
        private BulletController _bulletController;
        private AsteroidController _asteroidController;
        private LevelVariables _currentLevelVariables;
        private readonly Transform _asteroidsParent;

        private int _currentLevelId;

        public GameScreenController(GameScreen gameScreen, LoseScreen loseScreen, ShipModel shipModel, PrefabConfig prefabConfig, Ticker ticker, Transform asteroidsParent)
        {
            _gameScreen = gameScreen;
            _loseScreen = loseScreen;
            _shipModel = shipModel;
            _prefabConfig = prefabConfig;
            _ticker = ticker;
            _asteroidsParent = asteroidsParent;
        }

        public void Show(int levelId, LevelVariables levelVariables)
        {
            _currentLevelId = levelId;
            _currentLevelVariables = levelVariables;
            
            _gameScreen.OpenScreen();
            _gameScreen.UpdateScore(0);
            _gameScreen.UpdateHealth(_shipModel.MaxLives);
            SpawnShip();
            SpawnBullets();
            SpawnAsteroids(levelVariables);
            InitShipController();
        }

        public void Hide()
        {
            _gameScreen.CloseScreen();
        }

        private void SpawnShip()
        {
            if (_shipView == null)
            {
                _shipView = Object.Instantiate(_prefabConfig.ShipPrefab, _shipModel.SpawnPosition, Quaternion.identity);
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
            _gameScreen.UpdateScore(score);
        }

        private void OnWin()
        {
            _asteroidController.Deactivate();
            
            Debug.LogError("WinGame");
            //_gameScreen.ShowWin();
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
        
        private void OnAsteroidHit(AsteroidView asteroidView)
        {
            _asteroidController.DestroyAsteroid(asteroidView);
        }
        
        private void OnHealthChanged(int health)
        {
            _gameScreen.UpdateHealth(health);
        }

        private void OnLose()
        {
            _shipController.Deactivate();
            _loseScreen.Init(OnRestart);
            _loseScreen.OpenScreen();
            _ticker.Register(_loseScreen);
        }
        
        private void Cleanup()
        {
            _ticker.Unregister(_bulletController);
            _ticker.Unregister(_asteroidController);
            _ticker.Unregister(_loseScreen);

            _asteroidController.Deactivate();
            _loseScreen.CloseScreen();
        }

        private void OnRestart()
        {
            Cleanup();
            _shipModel.ResetShipModel();
            Show(_currentLevelId, _currentLevelVariables);
        }
    }
}
