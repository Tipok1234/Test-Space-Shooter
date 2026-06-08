using UnityEngine;
using UnityEngine.UI;
using System;
using Data;
using Enums;
using TMPro;

namespace UI.Views
{
    public class LevelView : MonoBehaviour
    {
        private Action<int> ClickCallback;
    
        [Header("Components")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text statusText;

        public int LevelId { get; private set; }

        public void Init(int levelId, Action<int> onClickCallback)
        {
            LevelId = levelId;
            ClickCallback = onClickCallback;
            button.onClick.AddListener(OnClick);
        }

        public void UpdateView(LevelState state)
        {
            switch (state.Status)
            {
                case LevelStatusType.Locked:
                    statusText.text = "Locked";
                    button.interactable = false;
                    break;
                case LevelStatusType.Unlocked:
                    statusText.text = "Unlocked";
                    button.interactable = true;
                    break;
                case LevelStatusType.Completed:
                    statusText.text = "Completed";
                    button.interactable = true;
                    break;
            }
        }

        private void OnClick()
        {
            ClickCallback?.Invoke(LevelId);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClick);
        }
    }
}
