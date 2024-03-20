namespace BLMMX.MatchTable;
/// <summary>
/// 这个 MatchTable 用于 JSON解析的模板
/// </summary>
internal class MatchTable
{
    public List<string> TotalPlayerIds;
    public List<string> AttackPlayerIds;
    public List<string> DefendPlayerIds;
}
/// <summary>
/// MatchManager 用于快速判定，也就是和网页端通信时直接搞定
/// </summary>
public class MatchManager
{
    public static List<string> TotalPlayerIds;
    public static List<string> AttackPlayerIds;
    public static List<string> DefendPlayerIds;
}
