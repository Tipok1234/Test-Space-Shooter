using Core;
using Managers;
using Screens;
using Models;
using Views;
using System;
using Data;
using Enums;
using UnityEngine;

namespace Controllers
{
    public class GameController : IDisposable
    {
        private readonly UIManager _uiManager;
        private readonly LevelModel _levelModel;
        private readonly Ticker _ticker;
        private readonly GameManager _gameManager;

        private readonly ShipController _shipController;
        private readonly BulletController _bulletController;
        private readonly AsteroidController _asteroidController;
        
        private const int ScoreForHitAsteroid = 10;
        
        private int _currentLevelId;
        private int _currentScore;

        private bool _isGameOver;
        private bool _isInitialized = false;
        
        public GameController(UIManager uiManager, Ticker ticker, LevelModel levelModel, GameManager gameManager, BulletController bulletController, AsteroidController asteroidController,ShipController shipController)
        {
            _shipController = shipController;
            _shipController.HealthChanged += OnHealthChanged;
            _shipController.AsteroidHit += OnAsteroidHit;
            _shipController.Death += OnLoseGame;
            
            _uiManager = uiManager;
            _bulletController = bulletController;
            _asteroidController = asteroidController;
            _ticker = ticker;
            _levelModel = levelModel;
            _gameManager = gameManager;
            _gameManager.GameStateChanged += OnGameStateChanged;
        }

        private void Show(int levelId, LevelVariables levelVariables)
        {
            _isGameOver = false;
            _currentLevelId = levelId;
            _currentScore = 0;

            var gameScreen = _uiManager.GetScreen<GameScreen>();
            gameScreen.OpenScreen();
            
            if (_isInitialized)
            {
                ResetControllers();
                SetupShip();
            }
            else
            {
                Setup();
            }
            
            _asteroidController.Activate(levelVariables);
            
            gameScreen.UpdateHealth(_shipController.MaxHealth);
        }
        
        private void OnGameStateChanged(GameStateType gameState)
        {
            switch (gameState)
            {
                case GameStateType.Game:
                    var levelVariables = _levelModel.GetVariables(_levelModel.CurrentLevel);
                    Show(_levelModel.CurrentLevel,levelVariables);
                    break;
            }
        }

        private void Setup()
        {
            _shipController.SpawnShip();
            RegistersControllers();
            SubscribeAsteroids();
            _isInitialized = true;
        }

        private void SetupShip()
        {
            _ticker.Register(_shipController);
            _shipController.SetEnabled(true);
        }

        private void ResetControllers()
        {
            _shipController.Reset();
            _bulletController.Reset();
            _asteroidController.Reset();
        }

        private void RegistersControllers()
        {
            _ticker.Register(_shipController);
            _ticker.Register(_bulletController);
            _ticker.Register(_asteroidController);
        }
        
        private void SubscribeAsteroids()
        {
            _asteroidController.DestroyedAsteroid += OnAsteroidDestroyed;
            _asteroidController.AllAsteroidsDestroyed += OnWinGame;
        }
        
        private void OnAsteroidDestroyed()
        {
            _currentScore += ScoreForHitAsteroid; 
            _uiManager.GetScreen<GameScreen>().UpdateScore(_currentScore);
        }

        private void OnWinGame()
        {
            if (_isGameOver)
                return;
    
            _shipController.Deactivate();
            var winScreen = _uiManager.GetScreen<WinScreen>();
            
            winScreen.Init(OnMenu, OnNextLevel);
            winScreen.UpdateView(_currentScore / ScoreForHitAsteroid, _asteroidController.TotalAsteroids);
            winScreen.OpenScreen();
            
            _levelModel.CompleteLevel(_currentLevelId);
            _levelModel.ResetSeed(_currentLevelId); 
            _ticker.Unregister(_shipController);
        }
        
        private void OnMenu()
        {
            _gameManager.SetState(GameStateType.Map);
            _asteroidController.Deactivate();
            _uiManager.GetScreen<WinScreen>().CloseScreen();
            _uiManager.GetScreen<GameScreen>().CloseScreen();
        }

        private void OnNextLevel()
        {
            _gameManager.SetState(GameStateType.Win);
            _asteroidController.Deactivate();
            _uiManager.GetScreen<WinScreen>().CloseScreen();
            
            var levelsData = _levelModel.GetLevelsData();
    
            int nextLevelId = (_currentLevelId + 1) % levelsData.Count;
    
            _levelModel.GenerateSeed(nextLevelId);
            _levelModel.SetCurrentLevel(nextLevelId);
            
            _gameManager.SetState(GameStateType.Game);
        }
        
        private void OnAsteroidHit(AsteroidView asteroidView)
        {
            _asteroidController.DestroyAsteroid(asteroidView);
        }
        
        private void OnHealthChanged(int health)
        {
            _uiManager.GetScreen<GameScreen>().UpdateHealth(health);
        }

        private void OnLoseGame()
        {
            _gameManager.SetState(GameStateType.Lose);
            
            _isGameOver = true;
            _shipController.Deactivate();
            
            var loseScreen = _uiManager.GetScreen<LoseScreen>();
            loseScreen.Init(OnRestartGame);
            loseScreen.OpenScreen();
            
            _ticker.Register(loseScreen);
            _ticker.Unregister(_shipController);
        }
        
        private void OnRestartGame()
        {
            _asteroidController.Deactivate();
            _gameManager.SetState(GameStateType.Game);
            ResetLoseScreen();
        }
        
        private void ResetLoseScreen()
        {
            var loseScreen = _uiManager.GetScreen<LoseScreen>();
            _ticker.Unregister(loseScreen);
            loseScreen.CloseScreen();
        }

        public void Dispose()
        {
            _gameManager.GameStateChanged -= OnGameStateChanged;
            _asteroidController.DestroyedAsteroid -= OnAsteroidDestroyed;
            _asteroidController.AllAsteroidsDestroyed -= OnWinGame;
            _shipController.HealthChanged -= OnHealthChanged;
            _shipController.AsteroidHit -= OnAsteroidHit;
            _shipController.Death -= OnLoseGame;
        }
    }
}
