using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Database
{
    public interface IDataSource<T>
    {
        void Open();
        void Close();

        bool Exists(uint id);
        T GetByID(uint id);
        IEnumerable<T> GetAll();

        void Update(T data);
    }
}
