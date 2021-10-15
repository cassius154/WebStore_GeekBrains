using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebStore.WebAPI.Clients.Base
{
    public abstract class ClientBase
    {
        protected HttpClient http { get; }

        protected string address { get; }

        protected ClientBase(HttpClient client, string address)
        {
            http = client;
            this.address = address;
        }

        protected T Get<T>(string url) => GetAsync<T>(url).Result;

        protected async Task<T> GetAsync<T>(string url)
        {
            //если мы пришли сюда в потоке thread.id = 7
            var response = await http.GetAsync(url)  //без ConfigureAwait(false) будем дожидаться thread,
                                                     //который нас сюда привел (id = 7)
                .ConfigureAwait(false);  //с ConfigureAwait(false) будем брать любой thread

            return await response
                .EnsureSuccessStatusCode()
                .Content
                .ReadFromJsonAsync<T>()
                //пишут по разному - или что достаточно только один раз применить в начале,
                //или что в каждом await
                .ConfigureAwait(false);
        }

        protected HttpResponseMessage Post<T>(string url, T item) => PostAsync<T>(url, item).Result;

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, T item)
        {
            var response = await http.PostAsJsonAsync<T>(url, item).ConfigureAwait(false);
            return response.EnsureSuccessStatusCode();
        }

        protected HttpResponseMessage Put<T>(string url, T item) => PutAsync<T>(url, item).Result;

        protected async Task<HttpResponseMessage> PutAsync<T>(string url, T item)
        {
            var response = await http.PutAsJsonAsync<T>(url, item).ConfigureAwait(false);
            return response.EnsureSuccessStatusCode();
        }

        protected HttpResponseMessage Delete(string url) => DeleteAsync(url).Result;

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            var response = await http.DeleteAsync(url).ConfigureAwait(false);
            //return response.EnsureSuccessStatusCode();
            //почему-то тут не проверяем через EnsureSuccessStatusCode
            //можно проверить вручную через response.IsSuccessStatusCode
            return response;
        }
    }
}
