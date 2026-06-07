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
        private Action _onNext;

        public override void CloseScreen()
        {
            Debug.Log("WinScreen CloseScreen called");
            base.CloseScreen();
        }

        public override void OpenScreen()
        {
            Debug.Log("WinScreen OPENSCREEN called");
            base.OpenScreen();
        }

        public void Init(Action onMenu, Action onNext)
        {
            _onMenu = onMenu;
            _onNext = onNext;

            menuButton.onClick.RemoveAllListeners();
            nextButton.onClick.RemoveAllListeners();
            
            menuButton.onClick.AddListener(OnMenu);
            nextButton.onClick.AddListener(OnNext);
        }

        public void UpdateView(int collected, int total)
        {
            collectedText.text = $"Collected: {collected} / {total}";
        }

        private void OnMenu()
        {
            _onMenu?.Invoke();
        }

        private void OnNext()
        {
            _onNext?.Invoke();
        }

        private void OnDestroy()
        {
            menuButton.onClick.RemoveListener(OnMenu);
            nextButton.onClick.RemoveListener(OnNext);
        }
    }
}
