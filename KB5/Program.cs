using BLMMX.Entity;
using Newtonsoft.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        await Task.Run(async () =>
        {
            // 创建 HttpClient 实例
            using HttpClient client = new();
            try
            {
                // 发起 GET 请求
                string url = "http://localhost:14725/get-match-obj/CN_BTL_NINGBO_1";
                HttpResponseMessage response = await client.GetAsync(url);

                // 确保响应成功
                response.EnsureSuccessStatusCode();

                // 读取响应内容
                string content = await response.Content.ReadAsStringAsync();

                // 输出响应内容
                //Console.WriteLine(content);
                EWebResponse? eWebResponse = JsonConvert.DeserializeObject<EWebResponse>(content);
                if (eWebResponse != null)
                {
                    if (eWebResponse.code == -1)
                    {
                        Console.WriteLine(eWebResponse.Message);
                    }
                    else
                    {
                        //Console.WriteLine(eWebResponse.data);
                        WillMatchData? willMatch = JsonConvert.DeserializeObject<WillMatchData>(eWebResponse.data.ToString());
                        if (willMatch == null)
                        {
                            return;
                        }

                        Console.WriteLine(willMatch.isCancel);
                        Console.WriteLine(willMatch.firstTeamCultrue);
                        foreach (string item in willMatch.secondTeamPlayerIds)
                        {
                            Console.WriteLine(item);
                        }
                        Console.WriteLine(willMatch.MatchType);
                    }
                }
            }
            catch (Exception ex)
            {
                // 捕获并处理异常
                Console.WriteLine($"Error: {ex.Message}");
            }
        });
    }
}