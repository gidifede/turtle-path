using NUnit.Framework;
using TurtlePath.Core;

namespace TurtlePath.Tests
{
    public class StarCalculationTests
    {
        [Test]
        public void NoCollectibles_Returns3Stars()
        {
            int stars = GameManager.CalculateStars(0, 0, 0, 0);
            Assert.AreEqual(3, stars);
        }

        [Test]
        public void AllCollected_Returns3Stars()
        {
            int stars = GameManager.CalculateStars(1, 1, 1, 1);
            Assert.AreEqual(3, stars);
        }

        [Test]
        public void AllShells_NoBabies_Returns2Stars()
        {
            int stars = GameManager.CalculateStars(1, 1, 0, 1);
            Assert.AreEqual(2, stars);
        }

        [Test]
        public void PartialShells_AllBabies_Returns1Star()
        {
            int stars = GameManager.CalculateStars(0, 1, 1, 1);
            Assert.AreEqual(1, stars);
        }

        [Test]
        public void NothingCollected_WithShells_Returns1Star()
        {
            int stars = GameManager.CalculateStars(0, 2, 0, 0);
            Assert.AreEqual(1, stars);
        }
    }
}
