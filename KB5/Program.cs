using BLMMX;
using BLMMX.Entity;
using BLMMX.Helpers;
using BLMMX.Util;
using Newtonsoft.Json;
using System.Net.NetworkInformation;

public class Program
{
    public static void Main(string[] args)
    {
        // 你想要 ping 的 IP 地址
        string ipAddress = "47.251.43.74"; // 例：Google的公共DNS服务器

        Ping pingSender = new Ping(); // 创建一个 Ping 对象
        try
        {
            PingReply reply = pingSender.Send(ipAddress); // 发送 ping 请求
            if (reply.Status == IPStatus.Success) // 如果成功
            {
                Console.WriteLine($"Ping 成功: {reply.Address} - RTT = {reply.RoundtripTime}ms");
            }
            else
            {
                Console.WriteLine($"Ping 失败: {reply.Status}");
            }
        }
        catch (PingException ex)
        {
            Console.WriteLine($"Ping 过程中出现错误: {ex.Message}");
        }
    }
    public static async Task Mainex3(string[] args)
    {
        WillMatchData? willMatchData = null;
        string serverName = "CN_BTL_NINGBO_1";
        await NetUtil.GetAsync("/get-match-obj/" + serverName,
                (res) =>
                {
                    EWebResponse? eWebResponse = JsonConvert.DeserializeObject<EWebResponse>(res);
                    if (eWebResponse != null)
                    {
                        if (eWebResponse.code == -1)
                        {
                            Console.WriteLine($"[es|get-match-obj] {eWebResponse.Message}");
                        }
                        else
                        {
                            //Console.WriteLine(eWebResponse.data);
                            string? value = eWebResponse.data.ToString();
                            Console.WriteLine(value);
                            WillMatchData? willMatch = JsonConvert.DeserializeObject<WillMatchData>(value);
                            if (willMatch == null)
                            {
                                Console.WriteLine("[es|get-match-obj] data is null");
                                return;
                            }

                            willMatchData = willMatch;
                            Console.WriteLine($"[es|get-match-obj-info] {willMatch.MatchType}");
                            //Helper.SendMessageToAllPeers("成功读取匹配数据");
                            //Console.WriteLine(willMatch.isCancel);
                            //Console.WriteLine(willMatch.firstTeamCultrue);
                            //foreach (string item in willMatch.secondTeamPlayerIds)
                            //{
                            //    Console.WriteLine(item);
                            //}
                        }
                    }
                }, (e) =>
                {
                    Console.WriteLine("[es|get-match-obj] error");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                });
    }
    public static async Task Main2(string[] args)
    {
        PlayerMatchDataContainer playerMatchDataContainer = new PlayerMatchDataContainer();
        KeyValuePair<bool, int> k = playerMatchDataContainer.AddKillRecord("1231", "123");
        Console.WriteLine(k);
        await Task.Delay(1000);
        k = playerMatchDataContainer.AddKillRecord("1231", "12x3");
        Console.WriteLine($"{k}");

        await Task.Delay(1000);
        k = playerMatchDataContainer.AddKillRecord("1231", "12x3");
        Console.WriteLine($"{k}");

        await Task.Delay(8000);
        k = playerMatchDataContainer.AddKillRecord("1231", "12x3");
        Console.WriteLine($"{k}");

        k = playerMatchDataContainer.AddKillRecord("C231", "12x3");
        Console.WriteLine($"{k}");
        //await Task.Run(async () =>
        //{
        //    // 创建 HttpClient 实例
        //    using HttpClient client = new();
        //    try
        //    {
        //        // 发起 GET 请求
        //        string url = "http://localhost:14725/get-match-obj/CN_BTL_NINGBO_1";
        //        HttpResponseMessage response = await client.GetAsync(url);

        //        // 确保响应成功
        //        response.EnsureSuccessStatusCode();

        //        // 读取响应内容
        //        string content = await response.Content.ReadAsStringAsync();

        //        // 输出响应内容
        //        //Console.WriteLine(content);
        //        EWebResponse? eWebResponse = JsonConvert.DeserializeObject<EWebResponse>(content);
        //        if (eWebResponse != null)
        //        {
        //            if (eWebResponse.code == -1)
        //            {
        //                Console.WriteLine(eWebResponse.Message);
        //            }
        //            else
        //            {
        //                //Console.WriteLine(eWebResponse.data);
        //                WillMatchData? willMatch = JsonConvert.DeserializeObject<WillMatchData>(eWebResponse.data.ToString());
        //                if (willMatch == null)
        //                {
        //                    return;
        //                }

        //                Console.WriteLine(willMatch.isCancel);
        //                Console.WriteLine(willMatch.firstTeamCultrue);
        //                foreach (string item in willMatch.secondTeamPlayerIds)
        //                {
        //                    Console.WriteLine(item);
        //                }
        //                Console.WriteLine(willMatch.MatchType);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // 捕获并处理异常
        //        Console.WriteLine($"Error: {ex.Message}");
        //    }
        //});
    }
}