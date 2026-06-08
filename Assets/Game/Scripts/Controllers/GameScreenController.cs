using Core;
using Managers;
using Screens;
using Models;
using WorldViews;
using System;
using Data;
using Enums;
using UnityEngine;

namespace Controllers
{
    public class GameScreenController : IDisposable
    {
        private readonly UIManager _uiManager;
        private readonly LevelModel _levelModel;
        private readonly Ticker _ticker;
        private readonly GameManager _gameManager;

        private ShipController _shipController;
        private BulletController _bulletController;
        private AsteroidController _asteroidController;
        
        private const int ScoreForHitAsteroid = 10;
        
        private int _currentLevelId;
        private int _currentScore;

        private bool isGameOver;
        private bool isInitialized = false;
        
        public GameScreenController(UIManager uiManager, Ticker ticker, LevelModel levelModel, GameManager gameManager, BulletController bulletController, AsteroidController asteroidController,ShipController shipController)
        {
            _shipController = shipController;
            _shipController.OnHealthChanged += OnHealthChanged;
            _shipController.OnAsteroidHit += OnAsteroidHit;
            _shipController.OnDeath += OnLose;
            
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
            isGameOver = false;
            _currentLevelId = levelId;
            _currentScore = 0;

            var gameScreen = _uiManager.GetScreen<GameScreen>();
            gameScreen.OpenScreen();
            
            if (isInitialized)
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
            isInitialized = true;
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
            _asteroidController.AllAsteroidsDestroyed += Win;
        }
        
        private void OnAsteroidDestroyed()
        {
            _currentScore += ScoreForHitAsteroid; 
            _uiManager.GetScreen<GameScreen>().UpdateScore(_currentScore);
        }

        private void Win()
        {
            if (isGameOver)
                return;
    
            _shipController.Deactivate();
            var winScreen = _uiManager.GetScreen<WinScreen>();
            
            winScreen.Init(OnMenu, OnNextLevel);
            winScreen.UpdateView(_currentScore / ScoreForHitAsteroid, _asteroidController.TotalAsteroids);
            winScreen.OpenScreen();
            
            _levelModel.CompleteLevel(_currentLevelId);
            _levelModel.RegenerateSeed(_currentLevelId); 
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
            _gameManager.SetState(GameStateType.Game);
            _asteroidController.Deactivate();
            _uiManager.GetScreen<WinScreen>().CloseScreen();
            
            var levelsData = _levelModel.GetLevelsData();
    
            int nextLevelId = (_currentLevelId + 1) % levelsData.Count;
    
            _levelModel.GenerateSeed(nextLevelId);
            var nextVariables = _levelModel.GetVariables(nextLevelId);
    
            _levelModel.SetCurrentLevel(nextLevelId);
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
            _ticker.Unregister(_shipController);
        }
        
        private void OnRestart()
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
            _asteroidController.AllAsteroidsDestroyed -= Win;
            _shipController.OnHealthChanged -= OnHealthChanged;
            _shipController.OnAsteroidHit -= OnAsteroidHit;
            _shipController.OnDeath -= OnLose;
        }
    }
}
