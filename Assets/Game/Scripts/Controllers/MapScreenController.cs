using Models;
using Screens;
using System;
using Enums;
using Managers;

namespace Controllers
{
    public class MapScreenController : IDisposable
    {
        private readonly UIManager _uiManager;
        private readonly LevelModel _levelModel;
        private readonly GameManager _gameManager;
        
        public MapScreenController(UIManager uiManager, LevelModel levelModel, GameManager gameManager)
        {
            _uiManager = uiManager;
            _levelModel = levelModel;
            _gameManager = gameManager;
            _gameManager.GameStateChanged += OnGameStateChanged;
        }

        public void Init()
        {
            _uiManager.GetScreen<MapScreen>().Init(_levelModel.GetLevelsData(), OnLevelClick);
        }

        public void Show()
        {
            var mapScreen = _uiManager.GetScreen<MapScreen>();
            mapScreen.UpdateView(_levelModel.GetLevelStates());
            mapScreen.OpenScreen();
        }

        private void OnLevelClick(int levelId)
        {
            _levelModel.SetCurrentLevel(levelId);
            _gameManager.SetState(GameStateType.LevelSelect);
        }
        
        private void OnGameStateChanged(GameStateType gameState)
        {
            switch (gameState)
            {
                case GameStateType.Map:
                    Show();
                    break;
            }
        }

        public void Dispose()
        {
            _gameManager.GameStateChanged -= OnGameStateChanged;
        }
    }
}
