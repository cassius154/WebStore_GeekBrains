using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStore.Interfaces.TestAPI
{
    public interface IValuesClient
    {
        IEnumerable<string> GetAll();

        int Count();

        string GetById(int id);

        void Add(string value);

        void Edit(int i, string value);

        bool Delete(int id);
    }
}
