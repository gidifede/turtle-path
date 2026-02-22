using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TurtlePath.Save;

namespace TurtlePath.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI turtleCountText;
        public Button playButton;
        public Button creditsButton;

        private UIManager uiManager;
        private System.Action onPlayClicked;

        public void Initialize(UIManager uiManager, System.Action onPlayClicked)
        {
            this.uiManager = uiManager;
            this.onPlayClicked = onPlayClicked;

            playButton.onClick.AddListener(OnPlayPressed);
            creditsButton.onClick.AddListener(OnCreditsPressed);
        }

        private void OnPlayPressed()
        {
            onPlayClicked?.Invoke();
        }

        private void OnCreditsPressed()
        {
            uiManager.ShowPanel("Credits");
        }

        public void Refresh()
        {
            int turtles = SaveManager.GetTotalBabyTurtles();
            turtleCountText.text = $"Tartarughe salvate: {turtles}";
        }
    }
}
