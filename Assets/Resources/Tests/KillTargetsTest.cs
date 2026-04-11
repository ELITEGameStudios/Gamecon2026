#if UNITY_EDITOR
using NUnit.Framework;
public class KillTargetsTest
{
    [TestFixture]
    public class TimerTest
    {
        public KillTargets killTargets;

        [SetUp]
        public void StartTimerTest()
        {
            killTargets = new(-1.0f);
        }
        [Test]
        public void GameShouldNotBeConsideredOverIfStartingTimeLessThanZero()
        {
            killTargets.TimerLogic(-1.0f);
            Assert.IsFalse(killTargets.gameOver);
        }
        [TearDown]
        public void TearDown()
        {
            killTargets = null;
        }
    }
}
#endif