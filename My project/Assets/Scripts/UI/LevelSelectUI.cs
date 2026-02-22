using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TurtlePath.Save;
using TurtlePath.Level;

namespace TurtlePath.UI
{
    public class LevelSelectUI : MonoBehaviour
    {
        public Button[] levelButtons;
        public TextMeshProUGUI[] levelTexts;
        public Button backButton;

        private UIManager uiManager;
        private System.Action<int> onLevelSelected;

        private static readonly Color StarGold = new Color(1f, 0.843f, 0f); // #FFD700
        private static readonly Color LockedColor = new Color(0.6f, 0.6f, 0.6f);
        private static readonly Color UnlockedColor = new Color(0.59f, 0.81f, 0.71f); // Ocean Teal

        public void Initialize(UIManager uiManager, System.Action<int> onLevelSelected)
        {
            this.uiManager = uiManager;
            this.onLevelSelected = onLevelSelected;

            for (int i = 0; i < levelButtons.Length; i++)
            {
                int levelId = i + 1;
                levelButtons[i].onClick.AddListener(() => OnLevelPressed(levelId));
            }

            backButton.onClick.AddListener(OnBackPressed);
        }

        private void OnLevelPressed(int levelId)
        {
            if (SaveManager.IsUnlocked(levelId))
            {
                onLevelSelected?.Invoke(levelId);
            }
        }

        private void OnBackPressed()
        {
            uiManager.ShowPanel("MainMenu");
            // Refresh main menu turtle count
            MainMenuUI mainMenu = uiManager.mainMenuPanel.GetComponent<MainMenuUI>();
            if (mainMenu != null) mainMenu.Refresh();
        }

        public void RefreshButtons()
        {
            int totalLevels = LevelLoader.GetTotalLevels();

            for (int i = 0; i < levelButtons.Length; i++)
            {
                int levelId = i + 1;
                bool unlocked = SaveManager.IsUnlocked(levelId);
                int stars = SaveManager.GetStars(levelId);

                string starText = "";
                for (int s = 0; s < stars; s++) starText += "\u2605";

                if (unlocked)
                {
                    levelTexts[i].text = $"{levelId}\n<color=#FFD700>{starText}</color>";
                    levelButtons[i].interactable = true;
                    ColorBlock cb = levelButtons[i].colors;
                    cb.normalColor = UnlockedColor;
                    levelButtons[i].colors = cb;
                }
                else
                {
                    levelTexts[i].text = $"{levelId}\n<color=#999999>Bloccato</color>";
                    levelButtons[i].interactable = false;
                    ColorBlock cb = levelButtons[i].colors;
                    cb.disabledColor = LockedColor;
                    levelButtons[i].colors = cb;
                }
            }
        }
    }
}
