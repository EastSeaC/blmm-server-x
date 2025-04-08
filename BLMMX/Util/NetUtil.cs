using BLMMX.Helpers;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;
using System.Web;

namespace BLMMX.Util;

public class NetUtil
{
    //private static string ServerIP = "110.42.109.140:14725";
    private static string ServerIP = "localhost:14725";


    private static HttpClient sharedClient = new()
    {
        BaseAddress = new Uri($"http://{ServerIP}/"),
    };

    public static async Task GetAsync(string route, Action<string> onResponse, Action<Exception> onError)
    {
        await Task.Run(async () =>
        {
            // 创建 HttpClient 实例
            //using HttpClient client = new();
            try
            {
                // 发起 GET 请求
                //string url = "http://110.42.98.67:14725/get-match-obj/";
                HttpResponseMessage response = await sharedClient.GetAsync(route);

                // 确保响应成功
                response.EnsureSuccessStatusCode();

                // 读取响应内容
                string content = await response.Content.ReadAsStringAsync();

                // 输出响应内容
                //Console.WriteLine(content);
                onResponse(content);
                //EWebResponse? eWebResponse = JsonConvert.DeserializeObject<EWebResponse>(content);
                //if (eWebResponse != null)
                //{
                //    if (eWebResponse.code == -1)
                //    {
                //        Console.WriteLine(eWebResponse.Message);
                //    }
                //    else
                //    {
                //        //Console.WriteLine(eWebResponse.data);
                //        WillMatchData? willMatch = JsonConvert.DeserializeObject<WillMatchData>(eWebResponse.data.ToString());
                //        if (willMatch == null)
                //        {
                //            return;
                //        }

                //        Console.WriteLine(willMatch.isCancel);
                //        Console.WriteLine(willMatch.firstTeamCultrue);
                //        foreach (string item in willMatch.secondTeamPlayerIds)
                //        {
                //            Console.WriteLine(item);
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                // 捕获并处理异常
                //Console.WriteLine($"Error: {ex.Message}");
                onError(ex);
            }
        });
    }

    public static Task Get(string route, JsonObject json, Action<string> onResponse, Action<Exception> onError)
    {
        Task task = new(() =>
        {
            try
            {
                string url = $"http://{ServerIP}/" + route;

                List<string> param = new();

                if (json.Count > 0)
                {
                    foreach (KeyValuePair<string, JsonNode?> kv in json)
                    {
                        param.Add(HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value.ToString()));
                    }
                    if (param.Count > 0)
                        url += "?" + string.Join("&", param);
                }
                Helper.Print(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36";
                //request.Headers.Add("Authorization", GetAuthorization(""));

                request.Timeout = 12000;
                request.ReadWriteTimeout = 12000;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        onResponse(reader.ReadToEnd());
                    }
                }
            }
            catch (Exception exception)
            {
                onError(exception);
            }
        });
        task.Start();
        return task;
    }

    public static async Task<Task> Post(string route, JsonObject json, Action<string> onResponse, Action<Exception> onError)
    {
        Task task = new(async () =>
        {
            try
            {
                using StringContent jsonContent = new(
                    System.Text.Json.JsonSerializer.Serialize(json),
                    Encoding.UTF8,
                    "application/json");

                using HttpResponseMessage response = await sharedClient.PostAsync(route, jsonContent);
                string jsonResponse = await response.Content.ReadAsStringAsync();
                onResponse(jsonResponse);
            }
            catch (Exception exception)
            {
                onError(exception);
            }
        });
        task.Start();
        return task;
    }
}