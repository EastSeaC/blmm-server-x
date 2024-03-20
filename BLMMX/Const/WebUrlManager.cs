using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLMMX.Const
{
    public class WebUrlManager
    {
        private const string ip = "localhost";
        private const string port = "14725";

        private const string base_url = $"http://{ip}:{port}/";

        private const string _uploadMatchData = "UploadMatchData";
        private const string _getMatchList = "GetMatchList";

        private const string _getVerifyCode = "GetVerifyCode";

        public const string UploadMatchData = base_url + _uploadMatchData;
        public const string GetMatchList = base_url + _getMatchList;

        public const string GetVerifyCode = base_url + _getVerifyCode;
    }
}
