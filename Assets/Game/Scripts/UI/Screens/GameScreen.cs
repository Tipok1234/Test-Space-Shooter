using TMPro;
using UnityEngine;

namespace Screens
{
    public class GameScreen : BaseScreen
    {
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text healthText;

        public override void OpenScreen()
        {
            UpdateScore(0);
            base.OpenScreen();
        }

        public void UpdateScore(int score)
        {
            scoreText.text = $"Score: {score}";
        }
        
        public void UpdateHealth(int health)
        {
            healthText.text = $"Health: {health}";
        }
    }
}
