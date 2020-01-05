using IdentityModel.Client;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Console
{
    class Program
    {
        static async Task Main(string[] args)
        {

            // discover endpoints from metadata
            var client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError)
            {
                System.Console.WriteLine(disco.Error);
                return;
            }
        }
    }
}
