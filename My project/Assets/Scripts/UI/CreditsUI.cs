using UnityEngine;
using UnityEngine.UI;

namespace TurtlePath.UI
{
    public class CreditsUI : MonoBehaviour
    {
        public Button backButton;

        private UIManager uiManager;

        public void Initialize(UIManager uiManager)
        {
            this.uiManager = uiManager;
            backButton.onClick.AddListener(OnBackPressed);
        }

        private void OnBackPressed()
        {
            uiManager.ShowPanel("MainMenu");
            MainMenuUI mainMenu = uiManager.mainMenuPanel.GetComponent<MainMenuUI>();
            if (mainMenu != null) mainMenu.Refresh();
        }
    }
}
