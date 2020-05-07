using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace ArchiCAD.DB.WebAPI.ServicesExt
{
    public interface IArchiCADWebApiService
    {
        Task<Models.External.AC_APIHealth> CheckHealth();
    }
    public class ArchiCADWebApiService : IArchiCADWebApiService
    {
        private IConfiguration _configuration;
        public HttpClient Client { get; }
        public ArchiCADWebApiService(IConfiguration configuration, HttpClient client)
        {
            _configuration = configuration;
            client.BaseAddress = new Uri("http://localhost:" +  _configuration["ArchiCAD:Program:Port"]);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            Client = client;
        }

        public async Task<Models.External.AC_APIHealth> CheckHealth()
        {
            string sRequestCommand = "{\"command\": \"API.IsAlive\"}";
            StringContent queryString = new StringContent(sRequestCommand);
            var response = await Client.PostAsync("", queryString);

            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Models.External.AC_APIHealth>(responseStream);
        }
    }
}
