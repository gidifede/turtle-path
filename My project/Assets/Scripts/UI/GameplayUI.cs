using UnityEngine;
using TMPro;

namespace TurtlePath.UI
{
    public class GameplayUI : MonoBehaviour
    {
        public TextMeshProUGUI shellCounterText;
        public TextMeshProUGUI babyCounterText;

        public void UpdateCounters(int shells, int totalShells, int babies, int totalBabies)
        {
            shellCounterText.text = $"Conchiglie: {shells}/{totalShells}";
            babyCounterText.text = $"Baby: {babies}/{totalBabies}";
        }

        public void ResetCounters(int totalShells, int totalBabies)
        {
            UpdateCounters(0, totalShells, 0, totalBabies);
        }
    }
}
