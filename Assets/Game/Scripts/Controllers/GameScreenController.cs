using Core;
using Managers;
using Screens;
using Models;
using UnityEngine;
using WorldViews;
using Configs;
using System;
using Data;
using Enums;

namespace Controllers
{
    public class GameScreenController : IDisposable
    {
        private readonly UIManager _uiManager;
        private readonly LevelModel _levelModel;
        private readonly ShipModel _shipModel;
        private readonly PrefabConfig _prefabConfig;
        private readonly Ticker _ticker;
        private readonly GameManager _gameManager;

        private ShipView _shipView;
        private ShipController _shipController;
        private BulletController _bulletController;
        private AsteroidController _asteroidController;
        private LevelVariables _currentLevelVariables;
        
        private int _currentLevelId;
        private int _currentScore;

        private bool isGameOver;
        
        public GameScreenController(UIManager uiManager, ShipModel shipModel, PrefabConfig prefabConfig, Ticker ticker, LevelModel levelModel, GameManager gameManager, BulletController bulletController, AsteroidController asteroidController)
        {
            _uiManager = uiManager;
            _shipModel = shipModel;
            _prefabConfig = prefabConfig;
            _bulletController = bulletController;
            _asteroidController = asteroidController;
            _ticker = ticker;
            _levelModel = levelModel;
            _gameManager = gameManager;
            _gameManager.GameStateChanged += OnGameStateChanged;
        }

        private void Show(int levelId, LevelVariables levelVariables)
        {
            isGameOver = false;
            _currentLevelId = levelId;
            _currentLevelVariables = levelVariables;
            _currentScore = 0;

            var gameScreen = _uiManager.GetScreen<GameScreen>();
            
            gameScreen.OpenScreen();
            gameScreen.UpdateScore(0);
            gameScreen.UpdateHealth(_shipModel.MaxLives);

            if (_shipView == null)
            {
                Setup(levelVariables);
            }
            else
            {
                ResetControllers(levelVariables);
            }
        }
        
        private void OnGameStateChanged(GameStateType gameState)
        {
            switch (gameState)
            {
                case GameStateType.Map:
                    break;
                case GameStateType.LevelSelect:
                    break;
                case GameStateType.Game:
                    var levelVariables = _levelModel.GetVariables(_levelModel.CurrentLevel);
                    Show(_levelModel.CurrentLevel,levelVariables);
                    break;
                case GameStateType.Win:
                    break;
                case GameStateType.Lose:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
            }
        }

        private void Setup(LevelVariables levelVariables)
        {
            SpawnShip();
            RegisterBullets();
            SpawnAsteroids(levelVariables);
            InitShipController();
        }

        private void ResetControllers(LevelVariables levelVariables)
        {
            _shipView.ResetShip();
            _shipController.ResetShipController();
            _ticker.Register(_shipController);
            _bulletController.ResetBullets();
            _asteroidController.ResetAsteroids();
            _asteroidController.Activate(_bulletController,levelVariables);
        }

        private void SpawnShip()
        {
            if (_shipView == null)
            {
                _shipView = UnityEngine.Object.Instantiate(_prefabConfig.ShipPrefab, Vector2.zero, Quaternion.identity);
            }

            _shipView.ResetShip();
        }

        private void RegisterBullets()
        {
            _ticker.Register(_bulletController);
        }

        private void SpawnAsteroids(LevelVariables levelVariables)
        {
            _asteroidController.Activate(_bulletController,levelVariables);
            _asteroidController.OnScoreChanged += OnScoreChanged;
            _asteroidController.OnAllAsteroidsDestroyed += OnWin;
            _ticker.Register(_asteroidController);
        }
        
        private void OnScoreChanged(int score)
        {
            _currentScore = score;
            _uiManager.GetScreen<GameScreen>().UpdateScore(score);
        }

        private void OnWin()
        {
            if (isGameOver)
                return;
    
            _shipController.Deactivate();
            var winScreen = _uiManager.GetScreen<WinScreen>();
            
            winScreen.Init(OnMenu, OnNextLevel);
            winScreen.UpdateView(_currentScore / 10, _asteroidController.TotalAsteroids);
            winScreen.OpenScreen();
            
            _levelModel.CompleteLevel(_currentLevelId);
            _levelModel.RegenerateSeed(_currentLevelId); 
        }
        
        private void OnMenu()
        {
            _gameManager.SetState(GameStateType.Map);
            Cleanup();
            _shipModel.ResetShipModel();
            _uiManager.GetScreen<WinScreen>().CloseScreen();
            _uiManager.GetScreen<GameScreen>().CloseScreen();
        }

        private void OnNextLevel()
        {
            _gameManager.SetState(GameStateType.Game);
            Cleanup();
            _shipModel.ResetShipModel();
            _uiManager.GetScreen<WinScreen>().CloseScreen();
            
            var levelsData = _levelModel.GetLevelsData();
    
            int nextLevelId = (_currentLevelId + 1) % levelsData.Count;
    
            _levelModel.GenerateSeed(nextLevelId);
            var nextVariables = _levelModel.GetVariables(nextLevelId);
    
            Show(nextLevelId, nextVariables);
        }
        
        private void OnAsteroidHit(AsteroidView asteroidView)
        {
            _asteroidController.DestroyAsteroid(asteroidView);
        }
        
        private void OnHealthChanged(int health)
        {
            _uiManager.GetScreen<GameScreen>().UpdateHealth(health);
        }

        private void OnLose()
        {
            _gameManager.SetState(GameStateType.Lose);
            
            isGameOver = true;
            _shipController.Deactivate();
            
            var loseScreen = _uiManager.GetScreen<LoseScreen>();
            loseScreen.Init(OnRestart);
            loseScreen.OpenScreen();
            
            _ticker.Register(loseScreen);
        }
        
        private void OnRestart()
        {
            _gameManager.SetState(GameStateType.Game);
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
            _shipController.OnDeactivate += OnShipDeactivated;
            _ticker.Register(_shipController);
        }
        
        private void OnShipDeactivated()
        {
            _ticker.Unregister(_shipController);
        }
        
        private void Cleanup()
        {
            var loseScreen = _uiManager.GetScreen<LoseScreen>();
            _ticker.Unregister(loseScreen);
            loseScreen.CloseScreen();
            _asteroidController.Deactivate();
        }

        public void Dispose()
        {
            _gameManager.GameStateChanged -= OnGameStateChanged;
            _asteroidController.OnScoreChanged -= OnScoreChanged;
            _asteroidController.OnAllAsteroidsDestroyed -= OnWin;
            _shipController.OnHealthChanged -= OnHealthChanged;
            _shipController.OnAsteroidHit -= OnAsteroidHit;
            _shipController.OnDeath -= OnLose;
            _shipController.OnDeactivate -= OnShipDeactivated;
        }
    }
}
