using NUnit.Framework;
using TurtlePath.Save;

namespace TurtlePath.Tests
{
    public class SaveManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            SaveManager.ResetAll();
        }

        [Test]
        public void SetStars_SavesOnlyIfBetter()
        {
            SaveManager.SetStars(1, 2);
            Assert.AreEqual(2, SaveManager.GetStars(1));

            SaveManager.SetStars(1, 1);
            Assert.AreEqual(2, SaveManager.GetStars(1), "Should keep higher star count");

            SaveManager.SetStars(1, 3);
            Assert.AreEqual(3, SaveManager.GetStars(1), "Should update to higher star count");
        }

        [Test]
        public void GetStars_DefaultIsZero()
        {
            Assert.AreEqual(0, SaveManager.GetStars(99));
        }

        [Test]
        public void IsUnlocked_Level1_AlwaysTrue()
        {
            Assert.IsTrue(SaveManager.IsUnlocked(1));
        }

        [Test]
        public void IsUnlocked_OtherLevels_DefaultFalse()
        {
            Assert.IsFalse(SaveManager.IsUnlocked(2));
            Assert.IsFalse(SaveManager.IsUnlocked(3));
        }

        [Test]
        public void UnlockLevel_MakesIsUnlockedTrue()
        {
            SaveManager.UnlockLevel(2);
            Assert.IsTrue(SaveManager.IsUnlocked(2));
        }

        [Test]
        public void AddBabyTurtles_Accumulates()
        {
            SaveManager.AddBabyTurtles(3);
            Assert.AreEqual(3, SaveManager.GetTotalBabyTurtles());

            SaveManager.AddBabyTurtles(2);
            Assert.AreEqual(5, SaveManager.GetTotalBabyTurtles());
        }

        [Test]
        public void ResetAll_ClearsEverything()
        {
            SaveManager.SetStars(1, 3);
            SaveManager.UnlockLevel(2);
            SaveManager.AddBabyTurtles(5);

            SaveManager.ResetAll();

            Assert.AreEqual(0, SaveManager.GetStars(1));
            Assert.IsFalse(SaveManager.IsUnlocked(2));
            Assert.AreEqual(0, SaveManager.GetTotalBabyTurtles());
        }
    }
}
