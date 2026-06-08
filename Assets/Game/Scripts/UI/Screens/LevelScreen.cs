using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Configs;
using Data;

namespace Screens
{
    public class LevelScreen : BaseScreen
    {
        private Action PlayAction;
        private Action CloseAction;
        
        [Header("Components")]
        [SerializeField] private TMP_Text levelNameText;
        [SerializeField] private TMP_Text asteroidCountText;
        [SerializeField] private TMP_Text asteroidSpeedText;
        [SerializeField] private TMP_Text asteroidTypeText;
        
        [SerializeField] private Button playButton;
        [SerializeField] private Button closeButton;

        public void Init(Action onPlay, Action onClose)
        {
            PlayAction = onPlay;
            CloseAction = onClose;
            
            playButton.onClick.AddListener(OnPlay);
            closeButton.onClick.AddListener(OnClose);
        }

        public void UpdateView(LevelConfig levelConfig, LevelVariables levelVariables)
        {
            levelNameText.text = $"Level {levelConfig.LevelId + 1}";

            SetVariablesText(
                levelVariables?.AsteroidCount.ToString(),
                levelVariables?.AsteroidSpeed.ToString("F1"),
                levelVariables?.AsteroidType.ToString()
            );
        }

        private void SetVariablesText(string count, string speed, string type)
        {
            asteroidCountText.text = $"Asteroid Count: {count ?? "???"}";
            asteroidSpeedText.text = $"Asteroid Speed: {speed ?? "???"}";
            asteroidTypeText.text = $"Asteroid Type: {type ?? "???"}";
        }

        private void OnPlay()
        {
            PlayAction?.Invoke();
        }

        private void OnClose()
        {
            CloseAction?.Invoke();
        }

        private void OnDestroy()
        {
            playButton.onClick.RemoveListener(OnPlay);
            closeButton.onClick.RemoveListener(OnClose);
        }
    }
}