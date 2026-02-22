using UnityEngine;

namespace TurtlePath.UI
{
    public class UIManager : MonoBehaviour
    {
        public GameObject mainMenuPanel;
        public GameObject levelSelectPanel;
        public GameObject gameplayPanel;
        public GameObject resultPanel;
        public GameObject creditsPanel;

        private GameObject[] allPanels;

        private void Awake()
        {
            allPanels = new GameObject[]
            {
                mainMenuPanel, levelSelectPanel, gameplayPanel, resultPanel, creditsPanel
            };
        }

        public void ShowPanel(string name)
        {
            HideAll();
            GameObject panel = GetPanel(name);
            if (panel != null)
                panel.SetActive(true);
        }

        public void HideAll()
        {
            if (allPanels == null) return;
            for (int i = 0; i < allPanels.Length; i++)
            {
                if (allPanels[i] != null)
                    allPanels[i].SetActive(false);
            }
        }

        private GameObject GetPanel(string name)
        {
            switch (name)
            {
                case "MainMenu": return mainMenuPanel;
                case "LevelSelect": return levelSelectPanel;
                case "Gameplay": return gameplayPanel;
                case "Result": return resultPanel;
                case "Credits": return creditsPanel;
                default:
                    Debug.LogWarning($"Unknown panel: {name}");
                    return null;
            }
        }
    }
}
