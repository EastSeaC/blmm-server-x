using BLMMX;
using BLMMX.Entity;
using BLMMX.Helpers;
using BLMMX.Util;
using Newtonsoft.Json;

public class Program
{
    public static async Task Main(string[] args)
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