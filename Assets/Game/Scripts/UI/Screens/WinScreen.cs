using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace Screens
{
    public class WinScreen : BaseScreen
    {
        [SerializeField] private TMP_Text collectedText;
        [SerializeField] private Button menuButton;
        [SerializeField] private Button nextButton;

        private Action _onMenu;
        private Action _onNextLevel;

        public void Init(Action onMenu, Action onNextLevel)
        {
            _onMenu = onMenu;
            _onNextLevel = onNextLevel;

            menuButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
            
            menuButton.onClick.AddListener(OnMenu);
            nextButton.onClick.AddListener(OnNextLevel);
        }

        public void UpdateView(int collected, int total)
        {
            collectedText.text = $"Collected: {collected} / {total}";
        }

        private void OnMenu()
        {
            _onMenu?.Invoke();
        }

        private void OnNextLevel()
        {
            _onNextLevel?.Invoke();
        }

        private void OnDestroy()
        {
            menuButton.onClick.RemoveListener(OnMenu);
            nextButton.onClick.RemoveListener(OnNextLevel);
        }
    }
}
