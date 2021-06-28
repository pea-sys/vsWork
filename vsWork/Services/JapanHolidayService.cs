using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using vsWork.Data;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace vsWork.Services
{
    public class JapanHolidayService : IHolidayService
    {
        private readonly HttpClient httpClient;

        public JapanHolidayService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        /// <summary>
        /// 日本の祝日を取得
        /// </summary>
        /// <param name="year"></param>
        /// <returns>[Key]yyyy-MM-dd [Value]名前</returns>
        public Dictionary<string, string> GetHolidays(int year)
        {
            // github apiはクライアント毎に1時間に60回までなのでDBに記録しておきたい
            string url = $"https://holidays-jp.github.io/api/v1/{year}/date.json";
            Dictionary<string, string> result = JsonConvert.DeserializeObject < Dictionary<string, string> > (JsonSerializer.Serialize(httpClient.GetFromJsonAsync<object?>(url).Result));
            return result;
        }
    }
}
