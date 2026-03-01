using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace TurtlePath.UI
{
    public class PauseMenuUI : MonoBehaviour
    {
        public Button resumeButton;
        public Button restartButton;
        public Button menuButton;
        public Button audioToggleButton;
        public TextMeshProUGUI audioToggleText;

        private bool audioMuted;

        public void Initialize(System.Action onResume, System.Action onRestart, System.Action onMenu)
        {
            resumeButton.onClick.AddListener(() => onResume?.Invoke());
            restartButton.onClick.AddListener(() => onRestart?.Invoke());
            menuButton.onClick.AddListener(() => onMenu?.Invoke());
            audioToggleButton.onClick.AddListener(ToggleAudio);

            // Load saved audio state
            audioMuted = PlayerPrefs.GetInt("audio_muted", 0) == 1;
            UpdateAudioText();
        }

        private void ToggleAudio()
        {
            audioMuted = !audioMuted;
            PlayerPrefs.SetInt("audio_muted", audioMuted ? 1 : 0);
            PlayerPrefs.Save();
            UpdateAudioText();
        }

        private void UpdateAudioText()
        {
            if (audioToggleText != null)
                audioToggleText.text = audioMuted ? "Audio: OFF" : "Audio: ON";
        }
    }
}
