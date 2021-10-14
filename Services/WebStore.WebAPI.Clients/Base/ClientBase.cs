using System.Net.Http;

namespace WebStore.WebAPI.Clients.Base
{
    public abstract class ClientBase
    {
        protected HttpClient http { get; }

        protected string address { get; }

        public ClientBase(HttpClient client, string address)
        {
            http = client;
            this.address = address;
        }
    }
}
