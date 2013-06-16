using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XChat2.Server.Database
{
    public abstract class BaseRepository<T> : IDisposable
    {
        private IDataSource<T> _dataSource;

        protected IDataSource<T> DataSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        public BaseRepository(IDataSource<T> dataSource)
        {
            _dataSource = dataSource;
            _dataSource.Open();
        }

        public T GetByID(uint id)
        {
            return _dataSource.GetByID(id);
        }

        public IEnumerable<T> GetAll()
        {
            return _dataSource.GetAll();
        }

        public bool Exists(uint id)
        {
            return _dataSource.Exists(id);
        }

        public void Update(T data)
        {
            _dataSource.Update(data);
        }

        public void Dispose()
        {
            _dataSource.Close();
        }
    }
}
