using Models;
using Screens;
using System;
using Enums;
using Managers;

namespace Controllers
{
    public class LevelScreenController : IDisposable
    {
        private readonly LevelScreen _levelScreen;
        private readonly LevelModel _levelModel;
        private readonly UIManager _uiManager;
        private readonly GameManager _gameManager;
        
        private readonly Action<int> _onPlay;

        private int _currentLevelId;
                
        public LevelScreenController(LevelModel levelModel, UIManager uiManager,GameManager gameManager)
        {
            _uiManager = uiManager;
            _levelScreen = _uiManager.GetScreen<LevelScreen>();
            _levelModel = levelModel;
            _gameManager = gameManager;

            _gameManager.GameStateChanged += OnGameStateChanged;
        }

        public void Init()
        {
            _levelScreen.Init(OnPlay, OnClose);
        }

        private void Show()
        {
            _currentLevelId = _levelModel.CurrentLevel;

            var levelData = _levelModel.GetData(_currentLevelId);
            var levelVariables = _levelModel.GetVariables(_currentLevelId);

            _levelScreen.UpdateView(levelData, levelVariables);
            _levelScreen.OpenScreen();
        }

        private void OnPlay()
        {
            if (!_levelModel.GetState(_currentLevelId).IsSeedGenerated)
            {
                _levelModel.GenerateSeed(_currentLevelId);
            }

            _levelModel.SetCurrentLevel(_currentLevelId);
            _uiManager.CloseScreen<MapScreen>();
            _levelScreen.CloseScreen();
            
            _gameManager.SetState(GameStateType.Game);
        }

        private void OnGameStateChanged(GameStateType gameState)
        {
            switch (gameState)
            {
                case GameStateType.Map:
                    break;
                case GameStateType.LevelSelect:
                    Show();
                    break;
                case GameStateType.Game:
                    break;
                case GameStateType.Win:
                    break;
                case GameStateType.Lose:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
            }
        }

        private void OnClose()
        {
            _levelScreen.CloseScreen();
            _gameManager.SetState(GameStateType.Map);
        }

        public void Dispose()
        {
            _gameManager.GameStateChanged -= OnGameStateChanged;
        }
    }
}
