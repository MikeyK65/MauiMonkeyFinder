using MonkeyFinder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonkeyFinder.Services
{
    public class MonkeyService
    {
        HttpClient httpClient;
        public MonkeyService()
        {
            httpClient = new HttpClient();
        }

        List<Monkey> monkeyList = new();

        public async Task<List<Monkey>> GetMonkeys()
        {
            if (monkeyList?.Count > 0) return monkeyList;

            var url = "https://montemagno.com/monkeys.json";
            var response = await httpClient.GetAsync(url);

            // If no internet - cando it from a local file
            /*
            using var stream = await FileSystem.OpenAppPackageFileAsync("MonkeyData.json");
            var contents = new StreamReader(stream);
            monkeyList = JsonSerializer.Deserialize<List<Monkey>>(contents);
            */

            if (response.IsSuccessStatusCode) 
            {
                monkeyList = await response.Content.ReadFromJsonAsync<List<Monkey>>();
            }

            return monkeyList;
        }
    }
}
