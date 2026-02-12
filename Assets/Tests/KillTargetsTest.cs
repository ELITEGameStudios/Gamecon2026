public class KillTargetsTest
{
    public class TimerTest
    {
        public KillTargets killTargets;

        public void StartTimerTest()
        {
            killTargets = new(-1.0f);
        }
        public void GameShouldNotBeConsideredOverIfStartingTimeLessThanZero()
        {
            killTargets.TimerLogic(-1.0f);
            // Assert.IsFalse(killTargets.gameOver);
        }
        public void TearDown()
        {
            killTargets = null;
        }
    }
}
