using Enums;
using Models;
using Screens;
using UnityEngine;
using System;

namespace Controllers
{
    public class LevelScreenController 
    {
        private readonly LevelScreen _levelScreen;
        private readonly LevelModel _levelModel;
        private readonly Action<int> _onPlay; 

        private int _currentLevelId;

        public LevelScreenController(LevelScreen levelScreen, LevelModel levelModel, Action<int> onPlay)
        {
            _levelScreen = levelScreen;
            _levelModel = levelModel;
            _onPlay = onPlay;
        }

        public void Init()
        {
            _levelScreen.Init(OnPlay, OnClose);
        }

        public void Show(int levelId)
        {
            _currentLevelId = levelId;

            var levelData = _levelModel.GetData(levelId);
            var levelVariables = _levelModel.GetVariables(levelId);

            _levelScreen.UpdateView(levelData, levelVariables);
            _levelScreen.OpenScreen();
        }

        public void Hide()
        {
            _levelScreen.CloseScreen();
        }

        private void OnPlay()
        {
            if (!_levelModel.GetState(_currentLevelId).IsSeedGenerated)
            {
                _levelModel.GenerateSeed(_currentLevelId);
            }

            _onPlay?.Invoke(_currentLevelId);
        }   

        private void OnClose()
        {
            Hide();
        }
    }
}
