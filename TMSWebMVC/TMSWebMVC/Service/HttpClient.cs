using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace TMSWebMVC.Service
{
   
    public class ClientService : HttpClient, IDisposable
    {
        private static HttpClient _client = new HttpClient();
      
        public Uri _clientUrl { get; set; }
        public ClientService(HttpClient client)
        {
            _client = client;
            _clientUrl = client.BaseAddress;
        }

    }
}
