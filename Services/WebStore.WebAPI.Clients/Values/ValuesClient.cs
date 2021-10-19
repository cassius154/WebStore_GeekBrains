using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using WebStore.Interfaces;
using WebStore.Interfaces.TestAPI;
using WebStore.WebAPI.Clients.Base;

namespace WebStore.WebAPI.Clients.Values
{
    public class ValuesClient : ClientBase, IValuesClient
    {
        public ValuesClient(HttpClient client) : base(client, WebAPIAddresses.Values)
        {
        }

        public void Add(string value)
        {
            var response = http.PostAsJsonAsync(address, value).Result;
            response.EnsureSuccessStatusCode(); //проверяет response.IsSuccessStatusCode и выбрасывает exception
        }

        public int Count()
        {
            var response = http.GetAsync($"{address}/count").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadFromJsonAsync<int>().Result;
            }

            return -1;
        }

        public bool Delete(int id)
        {
            var response = http.DeleteAsync($"{address}/{id}").Result;
            return response.IsSuccessStatusCode;
        }

        public void Edit(int id, string value)
        {
            var response = http.PutAsJsonAsync($"{address}/{id}", value).Result;
            response.EnsureSuccessStatusCode();
        }

        public IEnumerable<string> GetAll()
        {
            var response = http.GetAsync(address).Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadFromJsonAsync<IEnumerable<string>>().Result;
            }

            return Enumerable.Empty<string>();
        }

        public string GetById(int id)
        {
            var response = http.GetAsync($"{address}/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadFromJsonAsync<string>().Result;
            }

            return null;
        }
    }
}
