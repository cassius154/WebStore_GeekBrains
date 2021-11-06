using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using WebStoreClient;

namespace WebStore.TestConsole
{
    class Program
    {
        //тут сразу будут операторы сравнения, спецификаторы на свойствах init, деконструктор
        //
        private record Student(string LastName, string FirstName, int Age)
        {
            public Student() : this(default, default, default)
            {
                this.LastName = "";
                this.FirstName = "FF1";
                this.Age = 123;
            }

            public Student(string LLastName) : this(LLastName, default, default)
            {
                this.FirstName = "New First Name";
                Age = 222;
            }

            //конструктор копирования (по умолчанию protected) - тут не нужно : this(...)
            public Student(Student src)
            {
                this.LastName = "My Last";
                FirstName = src.FirstName;
                Age = src.Age + 5;
            }
        };

        //или классическая запись - в этом случае будут операторы сравнения
        //private record Student
        //{
        //    public string LastName { get; set; }
        //    public string FirstName { get; set; }
        //    public int Age { get; set; }
        //}


        private static void _onMessageFromClient(string Message)
        {
            Console.WriteLine("Message from server: {0}", Message);
        }

        static async Task Main(string[] args)
        {
            var builder = new HubConnectionBuilder();
            var connection = builder
               .WithUrl("http://localhost:5000/chat")
               .Build();

            using var registration = connection.On<string>("MessageFromClient", _onMessageFromClient);

            Console.WriteLine("Готов к подключению.");
            Console.ReadLine();

            await connection.StartAsync();

            Console.WriteLine("Соединение установлено.");

            while (true)
            {
                var message = Console.ReadLine();
                await connection.InvokeAsync("SendMessage", message);
            }

            return;

            //swaggerClient.cs лежит в каталоге obj/
            var client = new HttpClient()
            {
                //BaseAddress = new Uri("http://localhost:5001")
            };

            //namespace и имя класса такие, какие указали при генерации
            //(Dependencies - ПКМ - Manage Connected Services - Service References OpenAPI)
            var api = new WebStoreClient.WebAPIClient("http://localhost:5001", client);

            //тут разобраться - тут возвращается void (Task)
            //var products = 
                //await api.ProductsAsync(new ProductFilter());

            //var employee = await api.Employees4Async(2);


            //======================================================================
            //  records tests
            //======================================================================

            //var s1 = new Student
            //{
            //    LastName = "Last1",
            //    FirstName = "First1",
            //    Age = 1,
            //};
            var s1 = new Student("Last1", "First1", 1);

            //s1.LastName = "LL1";  //ошибка, поскольку там спецификатры init

            var s2 = new Student  //поскольку переопределили конструктор - можно инициализатор
            {
                LastName = "Last1",
                FirstName = "First1",
                Age = 1,
            };

            //св-ва s3 равны св-вам s1 кроме Age
            var s3 = s1 with { Age = 31 };

            //деконструкция record
            var (last, first, age) = s3;
            var l = last;
            var f = first;
            var a = age;
            var fio = $"{last}, {first}"; 

            if (s1 == s2)  //в случае классов - false, поскольку там проверяется равенство ссылок на экземпляры
            {
                //в случае record - true, поскольку проверяются только значения свойств
                Console.WriteLine("==");
            }
        }
    }
}
