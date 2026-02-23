using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TurtlePath.UI
{
    public class ResultScreenUI : MonoBehaviour
    {
        public TextMeshProUGUI starsText;
        public TextMeshProUGUI shellsText;
        public TextMeshProUGUI babiesText;
        public Button nextButton;
        public Button replayButton;
        public Button menuButton;

        private System.Action onNext;
        private System.Action onReplay;
        private System.Action onMenu;

        public void Initialize(System.Action onNext, System.Action onReplay, System.Action onMenu)
        {
            this.onNext = onNext;
            this.onReplay = onReplay;
            this.onMenu = onMenu;

            nextButton.onClick.AddListener(() => this.onNext?.Invoke());
            replayButton.onClick.AddListener(() => this.onReplay?.Invoke());
            menuButton.onClick.AddListener(() => this.onMenu?.Invoke());
        }

        public void Show(int stars, int shells, int totalShells, int babies, int totalBabies, bool hasNext)
        {
            string starDisplay = "";
            for (int i = 0; i < stars; i++) starDisplay += "<color=#FFD700>*</color>";
            for (int i = stars; i < 3; i++) starDisplay += "<color=#999999>*</color>";

            starsText.text = starDisplay;
            shellsText.text = $"Conchiglie: {shells}/{totalShells}";
            babiesText.text = $"Baby turtle: {babies}/{totalBabies}";

            nextButton.gameObject.SetActive(hasNext);
        }
    }
}
