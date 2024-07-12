namespace BLMMX;

public class DoFConfigOptions
{
    public static DoFConfigOptions Instance { get; } = new DoFConfigOptions();

    public string CommandPrefix { get; set; } = "!";
    
    public string BanListFileName { get; set; } = "banlist.txt";

    public bool ShowJoinLeaveMessages { get; set; } = true;
}