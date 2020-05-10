using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace ArchiCAD.DB.WebAPI.Controllers
{
    public interface IArchiCADController
    {
        Task<Models.External.AC_APIHealth> GetStatus();
        //Task<IAsyncEnumerable<Models.External.AC_Element>> GetElementsnProps();
    }

    [ApiController]
    [Route("[controller]")]
    public class ArchiCADController : ControllerBase, IArchiCADController
    {
        IHostApplicationLifetime applicationLifetime;
        private static ServicesExt.IArchiCADWebApiService _ArchiCADClient { get; set; }
        public static HttpClient Client { get; private set; }
        public ArchiCADController(IHostApplicationLifetime appLifetime)
        {
            applicationLifetime = appLifetime;
            Client = ServicesExt.ArchiCADWebApiService.Client;
        }

        [HttpGet("/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Models.External.AC_APIHealth> GetStatus()
        {
            StringContent queryString = new StringContent("{\"command\": \"API.IsAlive\"}");
            var response = await Client.PostAsync("", queryString);

            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<Models.External.AC_APIHealth>(responseStream);
        }

        //[HttpGet("/GetElements")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IAsyncEnumerable<Models.External.AC_Element>> GetElementsnProps()
        //{
        //    StringContent queryString = new StringContent("{\"command\": \"API.GetAllElements\"}");
        //    var response = await Client.PostAsync("", queryString);

        //    response.EnsureSuccessStatusCode();
        //    using var responseStream = await response.Content.ReadAsStreamAsync();
        //    //JsonSerializer.DeserializeAsync<Models.External.AC_APIHealth>(responseStream);

        //    IAsyncEnumerable<Models.External.AC_Element> eACElements = null;
        //    return eACElements;
        //}

        [HttpGet("/Exit")]
        public IActionResult StopApplication()
        {
            applicationLifetime.StopApplication();
            return new EmptyResult();
        }
    }

}
