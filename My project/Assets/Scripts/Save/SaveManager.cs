using UnityEngine;

namespace TurtlePath.Save
{
    public static class SaveManager
    {
        public static int GetStars(int levelId)
        {
            return PlayerPrefs.GetInt($"level_{levelId}_stars", 0);
        }

        public static void SetStars(int levelId, int stars)
        {
            int current = GetStars(levelId);
            if (stars > current)
            {
                PlayerPrefs.SetInt($"level_{levelId}_stars", stars);
                PlayerPrefs.Save();
            }
        }

        public static bool IsUnlocked(int levelId)
        {
            if (levelId == 1) return true;
            return PlayerPrefs.GetInt($"level_{levelId}_unlocked", 0) == 1;
        }

        public static void UnlockLevel(int levelId)
        {
            PlayerPrefs.SetInt($"level_{levelId}_unlocked", 1);
            PlayerPrefs.Save();
        }

        public static int GetTotalBabyTurtles()
        {
            return PlayerPrefs.GetInt("total_baby_turtles", 0);
        }

        public static void AddBabyTurtles(int count)
        {
            int current = GetTotalBabyTurtles();
            PlayerPrefs.SetInt("total_baby_turtles", current + count);
            PlayerPrefs.Save();
        }

        public static void ResetAll()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
