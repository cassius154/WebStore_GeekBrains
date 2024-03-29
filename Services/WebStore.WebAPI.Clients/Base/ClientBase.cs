﻿using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WebStore.WebAPI.Clients.Base
{
    public abstract class ClientBase : IDisposable
    {
        protected HttpClient http { get; }

        protected string address { get; }

        protected ClientBase(HttpClient client, string address)
        {
            http = client;
            this.address = address;
        }

        protected T Get<T>(string url) => GetAsync<T>(url).Result;

        protected async Task<T> GetAsync<T>(string url, CancellationToken cancel = default)
        {
            //если мы пришли сюда в потоке thread.id = 7
            var response = await http.GetAsync(url)  //без ConfigureAwait(false) будем дожидаться thread,
                                                     //который нас сюда привел (id = 7)
                .ConfigureAwait(false);  //с ConfigureAwait(false) будем брать любой thread

            if (response.StatusCode == HttpStatusCode.NoContent)
            {
                return default;
            }
            return await response
                .EnsureSuccessStatusCode()
                .Content
                .ReadFromJsonAsync<T>()
                //пишут по разному - или что достаточно только один раз применить в начале,
                //или что в каждом await
                .ConfigureAwait(false);
        }

        protected HttpResponseMessage Post<T>(string url, T item) => PostAsync<T>(url, item).Result;

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, T item, CancellationToken cancel = default)
        {
            var response = await http.PostAsJsonAsync<T>(url, item).ConfigureAwait(false);
            return response.EnsureSuccessStatusCode();
        }

        protected HttpResponseMessage Put<T>(string url, T item) => PutAsync<T>(url, item).Result;

        protected async Task<HttpResponseMessage> PutAsync<T>(string url, T item, CancellationToken cancel = default)
        {
            var response = await http.PutAsJsonAsync<T>(url, item).ConfigureAwait(false);
            return response.EnsureSuccessStatusCode();
        }

        protected HttpResponseMessage Delete(string url) => DeleteAsync(url).Result;

        protected async Task<HttpResponseMessage> DeleteAsync(string url, CancellationToken cancel = default)
        {
            var response = await http.DeleteAsync(url).ConfigureAwait(false);
            //return response.EnsureSuccessStatusCode();
            //почему-то тут не проверяем через EnsureSuccessStatusCode
            //можно проверить вручную через response.IsSuccessStatusCode
            return response;
        }


        //можно написать Dispose() как виртуальный, но тогда потомки могут разрушить логику базового класса
        //такой шаблон обычно применяется для класса с наследниками
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);  - указание GC, чо при сборке мусора этот Dispose вызывать не надо
        }

        //если бы писали деструктор (финализатор)
        //можно было бы сделать так + GC.SuppressFinalize(this)
        //но без лишней нужды писать деструктор не надо - замедляет работу класса и сборку мусора
        //~BaseClient()
        //{
        //    Dispose(false);
        //}

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }
            _disposed = true;

            if (disposing)
            {
                // должны освободить управляемые ресурсы - т.е. вызвать Dispose() у всех IDisposable
                //Http.Dispose(); // не мы создали, не нам и освобождать!
            }

            // должны освободить неуправляемые ресурсы
        }
    }
}
