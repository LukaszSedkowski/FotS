using System.Collections.Generic;

public class PlayerStats
{
    public int UserId { get; private set; }
    public int Kills { get; private set; }
    public int Deaths { get; private set; }
    public int TotalScore { get; private set; }

    public PlayerStats(int userId)
    {
        UserId = userId;
        Kills = 0;
        Deaths = 0;
        TotalScore = 0;
    }

    public void AddKill()
    {
        Kills++;
        TotalScore += 50;
    }

    public void AddDeath()
    {
        Deaths++;
        TotalScore -= 30;
    }

    public void WinGame()
    {
        TotalScore += 200;
    }

    public void LoseGame()
    {
        TotalScore -= 150;
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            { "UserId", UserId },
            { "Kills", Kills },
            { "Deaths", Deaths },
            { "TotalScore", TotalScore }
        };
    }

    public void ResetStats()
    {
        Kills = 0;
        Deaths = 0;
        TotalScore = 0;
    }
}
