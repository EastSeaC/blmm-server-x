using Newtonsoft.Json;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Diamond;
using Timer = TaleWorlds.Core.Timer;

namespace BLMMX;

public class BanPlayerBehavior : MultiplayerTeamSelectComponent
{
    private const string IpAddress = "127.0.0.1";
    private const string port = "";

    private const string host = "persistent";

    private static readonly string url = $"http://{IpAddress}:{port}/";
    private static readonly string url2 = $"http://{host}/";

    public static HashSet<string> banPlayers;
    public Timer timer;
    public BanPlayerBehavior()
    {
        InitialBanlist();

    }
    public override void OnBehaviorInitialize()
    {
        base.OnBehaviorInitialize();

        timer = new Timer(Mission.Current.CurrentTime, 10f);
    }

    public static void InitialBanlist()
    {
        banPlayers ??= new HashSet<string>();
    }

    public static async void UpdateBanPlayers()
    {
        InitialBanlist();

        try
        {
            string result = await HttpHelper.DownloadStringTaskAsync(url);
            if (result != null)
            {
                banPlayers = JsonConvert.DeserializeObject<HashSet<string>>(result);
            }
        }
        catch (Exception ex)
        {
            Debug.PrintError(ex.Message);
            Debug.PrintError(ex.StackTrace);
        }
    }

    public override void OnMissionTick(float dt)
    {
        base.OnMissionTick(dt);

        if (timer.Check(Mission.Current.CurrentTime))
        {
            UpdateBanPlayers();
        }
    }

    public override void OnPlayerConnectedToServer(NetworkCommunicator networkPeer)
    {
        base.OnPlayerConnectedToServer(networkPeer);

        string PlayerId = networkPeer.VirtualPlayer.Id.ToString();
        if (banPlayers.Contains(PlayerId))
        {
            DisconnectInfo disconnectInfo = networkPeer.PlayerConnectionInfo.GetParameter<DisconnectInfo>("DisconnectInfo") ?? new DisconnectInfo();
            disconnectInfo.Type = DisconnectType.KickedByHost;
            networkPeer.PlayerConnectionInfo.AddParameter("DisconnectInfo", disconnectInfo);
            GameNetwork.AddNetworkPeerToDisconnectAsServer(networkPeer);
        }
    }
}
