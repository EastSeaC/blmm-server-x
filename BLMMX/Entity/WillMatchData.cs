using BLMMX.Patch;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.IO;

namespace BLMMX.Entity;

/// <summary>
/// WillMatchData 顾名思义，就是 将要发生的比赛数据，包含了各个成员的名单
/// </summary>
public class WillMatchData
{
    private List<string> _secondTeamPlayerIds;
    private string _firstTeamCultrue;

    [JsonProperty("create_at")]
    public DateTime CreateAt { get; set; }

    [JsonProperty("first_team_player_ids")]
    public List<string> firstTeamPlayerIds { get; set; }

    [JsonProperty("first_team_culture")]
    public string firstTeamCultrue { get => _firstTeamCultrue; set { _firstTeamCultrue = value; } }

    [JsonProperty("second_team_player_ids")]
    public List<string> secondTeamPlayerIds { get => _secondTeamPlayerIds; set { _secondTeamPlayerIds = value; } }

    public ESMatchType MatchType { get; set; }

    public int GetTeamMaxNum()
    {
        return MatchType switch
        {
            ESMatchType.Match33 => 3,
            ESMatchType.Match66 => 6,
            ESMatchType.Match88 => 8,
            _ => 99,
        };
    }

    public int GetTotalNumber()
    {
        return GetTeamMaxNum() * 2;
    }

    private int currentplayerNumber;

    public int CurrentPlayerNumber => currentplayerNumber;

    public void OffSetCurrentPlayerNumber(int off)
    {
        currentplayerNumber += off;
    }

    public EMatchConfig MatchConfig { get; set; }

    public bool isCancel;
    public string matchId;
    public string ServerName;
    public bool isFinished;
    internal object cancelReason;

    public WillMatchData() {
        currentplayerNumber = 0;
    }

    public static WillMatchData GetFake()
    {
        WillMatchData k = new()
        {
            isCancel = true,
            isFinished = true,
            currentplayerNumber = 0,
        };
        return k;
    }

    internal bool isplayerNeedMatch(string playerId)
    {
        return firstTeamPlayerIds.Contains(playerId) || secondTeamPlayerIds.Contains(playerId);
    }

    internal bool isPlayerArrived()
    {
        return currentplayerNumber == GetTotalNumber();
    }
}

public class EMatchConfig
{
    public int TeamNumber;
    public string MapName;
    public List<int> TroopLimits;
}
