using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBaultBank.Core.Interfaces;
public interface IRestClient
{
  Task<T> GetAsync<T>(string url, Dictionary<string, string> query);
  Task<T> GetAsync<T>(string url);
  Task<T> PostAsync<T>(string url, string data);
  Task<T> PutAsync<T>(string url, FileStream fs);
}
