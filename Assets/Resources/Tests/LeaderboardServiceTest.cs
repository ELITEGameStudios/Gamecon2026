using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class TestIfValidNameIsFlagged
{
    LeaderboardService service;
    List<LevelAttempt> attempts;
    [SetUp]
    public void SetUp()
    {
        service = new();
        attempts = new();
    }

    [Test]
    public void TryValidName()
    {
        Assert.IsTrue(service.IsNameAllowed("Name", attempts));
    }

    [TearDown]
    public void TearDown()
    {
        service = null;
        attempts = null;
    }
}
