using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;





namespace Test2.Controllers
{
    public class CaptchaResponse
    {
        [JsonProperty("thành công")]
        public string Success { get; set; }
        [JsonProperty("Lỗi")]
        public List<string> ErrorCode { get; set; }
    }
}