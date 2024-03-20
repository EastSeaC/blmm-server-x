using Newtonsoft.Json;
using System.Text;

namespace KB5;

public class Program
{
    static void Main(string[] args)
    {
        PlayerMatchDataContainer dataContainer = new();
        dataContainer.AddPlayerWithName("2.0.0.76561199467351625", "sd");
        dataContainer.AddPlayerWithName("2.0.0.76563199467351841", "baga");

        //Console.WriteLine(JsonConvert.SerializeObject(dataContainer));

        string id = "2.0.0.76561199467351625";
        dataContainer.AddPlayerAttackValue("2.0.0.76561199467351625", 300);
        dataContainer.AddPlayerAttackValue("2.0.0.76563199467351841", 300);
        dataContainer.AddPlayerAttackValue("2.0.0.76563199467351841", 300);
        dataContainer.AddPlayerAttackValue("2.0.0.76563199467351841", 300);
        dataContainer.AddAttackHorse(id, 354);
        dataContainer.AddPlayerAttackValue("2.0.0.76563199467351841", 300);

        //Console.WriteLine(JsonConvert.SerializeObject(dataContainer));

        PlayerMatchDataContainer.AddAttackWinRoundNum();
        PlayerMatchDataContainer.AddDefendWinBureauNum();
        PlayerMatchDataContainer.TurnMatchToTest();

        PlayerMatchDataContainer.AddAttackPlayers(new() { "2.0.0.545", "2.0.05" });

        Console.WriteLine(JsonConvert.SerializeObject(dataContainer));

        // 发送数据给
        Kte(JsonConvert.SerializeObject(dataContainer));
    }

    public static void Kte(string data)
    {
        string ip = "http://localhost:14725/UploadMatchData";

        StringContent requestBody = new(data, Encoding.UTF8, "application/json");

        using HttpClient client = new();
        HttpResponseMessage response = client.PostAsync(ip, requestBody).Result;

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        }
        else
        {
            Console.WriteLine($"HTTP GET request failed with status code: {response.StatusCode}");
        }
    }
}
