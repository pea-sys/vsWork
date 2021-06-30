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
    public class HolidayService : IHolidayService
    {
        private readonly HttpClient httpClient;
        private readonly HolidayRepository holidayRepository;
        private DateTime lastUpdate = new DateTime(2000, 1, 1);

        public HolidayService(HttpClient httpClient,IRepository<Holiday,(int,DateTime)> repository)
        {
            this.httpClient = httpClient;
            this.holidayRepository = (HolidayRepository)repository;

            SetHolidays();
        }
        /// <summary>
        /// 日本の祝日をセットします
        /// </summary>
        /// <param name="year"></param>
        /// <returns>[Key]yyyy-MM-dd [Value]名前</returns>
        public void SetHolidays()
        {
            Dictionary<string, string> result_previous = new Dictionary<string, string>();
            Dictionary<string, string> result = new Dictionary<string, string>();
            Dictionary<string, string> result_next = new Dictionary<string, string>();
            int year = DateTime.Now.Year;
            if (lastUpdate.Date != DateTime.Now.Date)
            {
                // github apiはクライアント毎に1時間に60回までなのでDBに記録しておきたい(起動時に１回取得し、日を跨ぐたびに１回取得する)
                //string url = $"https://holidays-jp.github.io/api/v1/{year-1}/date.json";
                //result_previous = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonSerializer.Serialize(httpClient.GetFromJsonAsync<object?>(url).Result));
                string url = $"https://holidays-jp.github.io/api/v1/{year}/date.json";
                result = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonSerializer.Serialize(httpClient.GetFromJsonAsync<object?>(url).Result));

                if (result.Count > 0)
                {
                    holidayRepository.RemoveByTargetAndYear(ApplyTargetType.Japan, year);
                    foreach (var holiday in result)
                    {
                        holidayRepository.Add(new Holiday {Date = DateTime.Parse(holiday.Key), HolidayType = HolidayType.None, Name = holiday.Value, Target = ApplyTargetType.Japan });
                    }
                    lastUpdate = DateTime.Now;
                }

                url = $"https://holidays-jp.github.io/api/v1/{year+1}/date.json";
                result = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonSerializer.Serialize(httpClient.GetFromJsonAsync<object?>(url).Result));

                if (result.Count > 0)
                {
                    holidayRepository.RemoveByTargetAndYear(ApplyTargetType.Japan, year + 1);
                    foreach (var holiday in result)
                    {
                        holidayRepository.Add(new Holiday { Date = DateTime.Parse(holiday.Key), HolidayType = HolidayType.None, Name = holiday.Value , Target = ApplyTargetType.Japan});
                    }
                    lastUpdate = DateTime.Now;
                }
            }
        }
    }
}
