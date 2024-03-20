using Newtonsoft.Json;
using TaleWorlds.MountAndBlade;

namespace BLMMX;

public class PlayerMatchDataContainer
{
    [JsonProperty]
    private static Dictionary<string, PlayerMatchData> _players;
    /// <summary>
    /// Scores 是 每局，总共两局
    /// Round 是 每轮，每局抢3
    /// </summary>
    [JsonProperty]
    private static int AttackScores;

    [JsonProperty]
    private static int DefendScores;

    [JsonProperty]
    private static int AttackRound;

    [JsonProperty]
    private static int DefendRound;

    [JsonProperty] private static HashSet<string> AttackPlayerIds;

    [JsonProperty] private static HashSet<string> DefendPlayerIds;

    [JsonProperty]
    private static string Tag = "Norm";

    public PlayerMatchDataContainer()
    {
        _players ??= new();
        AttackPlayerIds = new HashSet<string>();
        DefendPlayerIds = new HashSet<string>();
    }

    public void SHOW()
    {
        Console.WriteLine("***************");
        Console.WriteLine("当前数据");
        foreach (KeyValuePair<string, PlayerMatchData> item in _players)
        {
            Console.WriteLine($"{item.Value.AttackValue1}|{item.Value.player_id}");
        }
        Console.WriteLine("***************");
    }

    public void AddPlayer(PlayerMatchData player)
    {
        _players.TryAdd(player.player_id, player);
    }

    public static void TurnMatchToTest()
    {
        Tag = "Test";
    }

    public static void TurnMatchToNorm()
    {
        Tag = "Norm";
    }

    public void AddPlayer(string player_id)
    {
        _players.TryAdd(player_id, new(player_id));
        //_players[player_id] = new(player_id);
        //_players.TryAdd(player_id, new(player_id));
    }

    public PlayerMatchData AddPlayerEx(string player_id)
    {
        PlayerMatchData k = new(player_id);
        _players.TryAdd(player_id, k);

        return k;
    }

    private void AddPlayerWithName(string player_id, string player_name)
    {
        bool result = _players.TryAdd(player_id, new(player_id, player_name));

        if (!result)
        {
            PlayerMatchData playerMatchData = _players[player_id];
            if (string.IsNullOrWhiteSpace(playerMatchData.player_name))
            {
                playerMatchData.player_name = player_name;
                _players[player_id] = playerMatchData;
            }
        }
    }

    public void AddPlayerWithName(MissionPeer missionPeer)
    {
        string player_id = missionPeer.GetNetworkPeer().VirtualPlayer.Id.ToString();
        string player_name = missionPeer.DisplayedName;

        AddPlayerWithName(player_id, player_name);
    }

    public void AddPlayerWithName(NetworkCommunicator communicator)
    {
        string player_id = communicator.VirtualPlayer.Id.ToString();
        string player_name = communicator.UserName;

        AddPlayerWithName(player_id, player_name);
    }


    public static void AddPlayerName(string player_id, string player_name)
    {
        if (_players.ContainsKey(player_id))
        {
            PlayerMatchData p = _players[player_id];
            p.player_name = player_name;
            _players[player_id] = p;
        }
    }

    public static void AddAttackPlayer(string playerId)
    {
        AttackPlayerIds.Add(playerId);
    }

    public static void AddAttackPlayers(List<string> strings)
    {
        foreach (string item in strings)
        {
            AttackPlayerIds.Add(item);
        }
    }

    public static void AddDefendPlayer(string playerId)
    {
        DefendPlayerIds.Add(playerId);
    }

    public static void AddDefendPlayers(List<string> strings)
    {
        foreach (string item in strings)
        {
            DefendPlayerIds.Add(item);
        }
    }
    /// <summary>
    /// 添加玩家的 非TK 攻击值
    /// </summary>
    /// <param name="player_id"></param>
    /// <param name="Attack"></param>
    public void AddPlayerAttackValue(string player_id, float Attack)
    {
        AddPlayer(player_id);
        PlayerMatchData playerData = _players[player_id];
        playerData.AddAttack(Attack);
        _players[player_id] = playerData;

        //SHOW();
        //Debug.Print(JsonConvert.SerializeObject(playerData, Formatting.Indented));
        //Debug.Print("[AddPlayerAttackValue|Warning]" + "数据异常");

    }

    public void AddPlayerTKValue(string player_id, int TKValue)
    {
        AddPlayer(player_id);
        // 获取对应的 PlayerMatchData 对象
        PlayerMatchData playerData = _players[player_id];

        // 更新数据
        playerData.AddTK(TKValue);

        // 将更新后的 PlayerMatchData 对象重新存回字典
        _players[player_id] = playerData;

    }

    public void AddKillNumber(string player_id)
    {
        AddPlayer(player_id);
        PlayerMatchData d = _players[player_id];
        d.AddKillNum();
        _players[player_id] = d;
    }

    public void SetKillNumber(string player_id, int KillNum)
    {
        AddPlayer(player_id);
        PlayerMatchData d = _players[player_id];
        d.KillNum1 = KillNum;
        _players[player_id] = d;
    }

    public void AddTKValue(string player_id, float TKValue)
    {
        AddPlayer(player_id);
        PlayerMatchData d = _players[player_id];
        d.AddTK(TKValue);
        _players[player_id] = d;
        //_players[player_id].AddTK(TKValue);
    }

    public void AddAttackHorse(string player_id, int Value)
    {
        AddPlayer(player_id);
        PlayerMatchData d = _players[player_id];
        d.AddAttackHorse(Value);
        _players[player_id] = d;
    }

    public void AddPlayerTKHorse(string player_id, int TKValue)
    {
        AddPlayer(player_id);
        if (_players.ContainsKey(player_id))
        {
            PlayerMatchData playerMatchData = _players[player_id];
            playerMatchData.AddTKHorse(TKValue);
            _players[player_id] = playerMatchData;
        }
    }

    public void AddPlayerDeadNum(string player_id)
    {
        AddPlayer(player_id);
        PlayerMatchData playerMatchData = _players[player_id];
        playerMatchData.AddDeadNum();
        _players[player_id] = playerMatchData;
        //_players[player_id].AddDeadNum();
    }

    public void AddPlayerTK(string player_id)
    {
        AddPlayer(player_id);
        PlayerMatchData playerMatchData = _players[player_id];
        playerMatchData.TKTimes1++;
        _players[player_id] = playerMatchData;
    }

    public void RefreshhAll()
    {
        // 遍历键值对并修改值
        foreach (KeyValuePair<string, PlayerMatchData> pair in _players)
        {
            string key = pair.Key;
            PlayerMatchData value = pair.Value;
            // 使用索引器更新字典中的值
            value.RefreshAll();
            _players[key] = value;
        }

        AttackRound = 0;
        DefendRound = 0;
        AttackScores = 0;
        DefendScores = 0;

        Tag = "Norm";
    }

    public static void AddAttackWinRoundNum()
    {
        AttackRound++;
    }

    public static void AddDefendWinRoundNum()
    {
        DefendRound++;
    }

    public static void AddAttackWinBureauNum()
    {
        AttackScores++;
    }

    public static void AddDefendWinBureauNum()
    {
        DefendScores++;
    }

    public void AddInfantryTimes(string playerId)
    {
        AddPlayer(playerId);

        PlayerMatchData k = _players[playerId];
        k.Infantry++;
        _players[playerId] = k;
    }

    public void AddCalvary(string playerId)
    {
        AddPlayer(playerId);

        PlayerMatchData k = _players[playerId];
        k.Cavalry++;
        _players[playerId] = k;
    }

    internal void AddArcher(string playerId)
    {
        AddPlayer(playerId);

        PlayerMatchData k = _players[playerId];
        k.Archer++;
        _players[playerId] = k;
    }

    internal void SetAttackerSideScores(int sideScore)
    {
        AttackRound = sideScore;
    }

    public void SetDefenderSideScores(int sideScore)
    {
        DefendRound = sideScore;
    }

    public void AddWinRound(string playerId)
    {
        AddPlayer(playerId);

        PlayerMatchData k = _players[playerId];
        k.Win_rounds++;
        _players[playerId] = k;
    }

    public void AddLoseRound(string playerId)
    {
        AddPlayer(playerId);

        PlayerMatchData playerMatchData = _players[playerId];
        playerMatchData.Lose_rounds++;
        _players[playerId] = playerMatchData;
    }

    public void SetDeathNumber(string playerId, int deathCount)
    {
        AddPlayer(playerId);
        PlayerMatchData matchData = _players[playerId];
        matchData.DeadNum1 = deathCount;

        _players[playerId] = matchData;
    }

    internal void SetAssistNumber(string playerId, int assistCount)
    {
        AddPlayer(playerId);

        PlayerMatchData playerMatchData = _players[playerId];
        playerMatchData.AssistNum1 = assistCount;
        _players[playerId] = playerMatchData;
    }

    internal void AddKillNumber2(string PlayerId)
    {

    }
}
public class PlayerMatchData
{
    public string player_id;
    public string player_name;


    private int KillNum = 0;
    private int DeadNum = 0;
    private int AssistNum = 0;
    private int Win = 0;
    private int Lose = 0;
    private int Draw = 0;

    private int win_rounds = 0;
    private int lose_rounds = 0;
    private int draw_rounds = 0;
    private int total_rounds = 0;

    private int infantry = 0; // 步兵
    private int cavalry = 0; //  骑兵
    private int archer = 0; //弓兵

    private float AttackValue = 0;
    private float TKValue = 0;
    private float TKTimes = 0;

    private int AttackHorse = 0;
    private int TKHorse = 0;



    [JsonProperty("damage")]
    public float AttackValue1 { get => AttackValue; set => AttackValue = value; }
    [JsonProperty("team_damage")]
    public float TKValue1 { get => TKValue; set => TKValue = value; }
    [JsonProperty("tk_times")]
    public float TKTimes1 { get => TKTimes; set => TKTimes = value; }
    [JsonProperty("horse_damage")]
    public int AttackHorse1 { get => AttackHorse; set => AttackHorse = value; }
    public int TKHorse1 { get => TKHorse; set => TKHorse = value; }
    public int KillNum1 { get => KillNum; set => KillNum = value; }
    [JsonProperty("death")]
    public int DeadNum1 { get => DeadNum; set => DeadNum = value; }
    [JsonProperty("assist")]
    public int AssistNum1 { get => AssistNum; set => AssistNum = value; }
    [JsonProperty("win")]
    public int Win1 { get => Win; set => Win = value; }
    public int Lose1 { get => Lose; set => Lose = value; }
    public int Draw1 { get => Draw; set => Draw = value; }
    public int Infantry { get => infantry; set => infantry = value; }
    public int Cavalry { get => cavalry; set => cavalry = value; }
    public int Archer { get => archer; set => archer = value; }
    public int Win_rounds { get => win_rounds; set => win_rounds = value; }
    public int Lose_rounds { get => lose_rounds; set => lose_rounds = value; }
    public int Draw_rounds { get => draw_rounds; set => draw_rounds = value; }
    public int Total_rounds { get => total_rounds; set => total_rounds = value; }

    public PlayerMatchData(string PlayerId)
    {
        player_id = PlayerId;
    }

    public PlayerMatchData(string PlayerId, string PlayerName)
    {
        player_id = PlayerId;
        player_name = PlayerName;
    }
    /// <summary>
    /// 重新归零数据
    /// </summary>
    public void RefreshAll()
    {
        KillNum = 0;
        DeadNum = 0;
        AssistNum = 0;
        Win = 0;
        Lose = 0;
        Draw = 0;

        win_rounds = 0;
        lose_rounds = 0;
        draw_rounds = 0;
        total_rounds = 0;

        infantry = 0; // 步兵
        cavalry = 0; // 骑兵
        archer = 0; // 弓兵

        AttackValue = 0;
        TKValue = 0;
        TKTimes = 0;

        AttackHorse = 0;
        TKHorse = 0;

    }

    public void AddAttack(float AttackValue1)
    {
        this.AttackValue1 += AttackValue1;
    }

    public void AddTK(float TKValue1) { this.TKValue1 += TKValue1; }



    public void AddAttackHorse(int AttackHorseValue)
    {
        AttackHorse1 += AttackHorseValue;
    }

    public void AddTKHorse(int TKHorseValue)
    {
        TKHorse1 += TKHorseValue;
    }

    public void AddKillNum() { KillNum1++; }
    public void AddDeadNum() { DeadNum1++; }

    public void AddTotalRounds()
    {
        total_rounds++;
    }

    public void AddWinNum()
    {
        Win++;
    }

    public void AddFailedNum()
    {
        Lose++;
    }


}