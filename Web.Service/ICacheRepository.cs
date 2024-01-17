using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Repository
{
    public interface ICacheRepository
    {
        T Get<T>(string key);
        bool Set<T>(T data, string key);
        bool Update<T>(T data, string key);
        void Remove(string key);
    }
}
