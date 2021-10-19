﻿using System;

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

        static void Main(string[] args)
        {
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