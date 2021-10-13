using System.Collections.Generic;

namespace WebStore.Interfaces.TestAPI
{
    public interface IValuesClient
    {
        IEnumerable<string> GetAll();

        int Count();

        string GetById(int id);

        void Add(string value);

        void Edit(int id, string value);

        bool Delete(int id);
    }
}
