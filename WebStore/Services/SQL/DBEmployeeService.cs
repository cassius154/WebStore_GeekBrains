﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WebStore.DAL.Context;
using WebStore.DAL.Models;
using WebStore.Services.Interfaces;

namespace WebStore.Services.SQL
{
    public class DBEmployeeService : IEmployeeService
    {
        private WebStoreDbContext _db;

        public DBEmployeeService(WebStoreDbContext db)
        {
            _db = db;
        }

        public Employee GetEmployee(string id)
        {
            return null;
            //return _db.Employees.Find(id);
        }

        public IEnumerable<Employee> GetEmployeeList(Func<Employee, bool> predicate = null)
        {
            return new List<Employee>();
            //IQueryable<Employee> ret = _db.Employees;
            //if (predicate is not null)
            //{
            //    Expression<Func<Employee, bool>> exp = 
            //        Expression.Lambda<Func<Employee, bool>>(Expression.Call(predicate.Method));
            //    ret = ret.Where(exp);
            //}

            //return ret.AsNoTracking(); //.ToList();
        }

        private int _getMaxId()
        {
            return 0;

            //try
            //{
            //    //IQueryable<Employee> ret = _db.Employees;
            //    var ret = _db.Employees.Select(e => e.Id).DefaultIfEmpty(0); //.Max();
            //    return ret.Max();  //вот это не работает
            //                       //{"The LINQ expression 'DbSet<Employee>().Select(e => e.Id)
            //                       //.DefaultIfEmpty(__p_0)' could not be translated.
            //                       //Either rewrite the query in a form that can be translated,
            //                       //or switch to client evaluation explicitly by inserting a call to
            //                       //'AsEnumerable', 'AsAsyncEnumerable', 'ToList', or 'ToListAsync'."}
            //                       //надо разбираться
            //}
            //catch
            //{
            //    return 0;
            //}
        }

        private void _save()
        {
            _db.SaveChanges();
        }

        public Employee Add(Employee emp)
        {
            if (emp is null)
            {
                throw new ArgumentNullException(nameof(emp));
            }

            //emp.Id = (TestData.Employees.OrderByDescending(e => e.Id).FirstOrDefault()?.Id).GetValueOrDefault() + 1;
            //var id = _getMaxId() + 1;

            //_db.Employees.Add(emp);
            //_save();

            return emp;
        }

        public bool Delete(string id)
        {
            return true;

            var e = GetEmployee(id);
            if (e is not null)
            {
                //_db.Employees.Remove(e);
                //_save();

                return true;
            }

            return false;
        }

        public Employee Edit(Employee emp)
        {
            return emp;

            if (emp is null)
            {
                throw new ArgumentNullException(nameof(emp));
            }

            var e = GetEmployee(emp.Id);
            if (e is not null)
            {
                e.FirstName = emp.FirstName;
                e.LastName = emp.LastName;
                e.Patronymic = emp.Patronymic;
                e.BirthDate = emp.BirthDate;
            }
            _save();

            return e;
        }
    }
}
