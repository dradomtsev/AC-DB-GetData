using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Hosting;
using ArchiCAD.DB.WebAPI.Models.External;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArchiCAD.DB.WebAPI.Controllers
{
    public interface IArchiCADController
    {
        Task<Models.External.AC_APIHealth_Response> GetStatus();
        Task<List<Models.Internal.Element>> GetElementsnProps();
        Task<Models.External.AC_PropertyNames_Response> GetAllPropertyNames();
        Task<Models.External.AC_PropertyGuids_Response> GetAllPropertyGuids();
        Task<Models.External.AC_ElementsAndProperties_Response> GetAllElementsAndAllProperties();
    }

    [ApiController]
    [Route("[controller]")]
    public class ArchiCADController : ControllerBase, IArchiCADController
    {
        IHostApplicationLifetime applicationLifetime;
        public static HttpClient Client { get; private set; }
        private static Lazy<HttpClient> lazyClient = new Lazy<HttpClient>(InitializeHttpClient);
        private static HttpClient Clientloc => lazyClient.Value;
        private static List<Models.Internal.Element> ACElements = new List<Models.Internal.Element>();
        private static Models.External.AC_Elements_Response objACElements;
        private static Models.External.AC_PropertyNames_Response objACPropertyNames;
        private static Models.External.AC_PropertyGuids_Response objACPropertyGuids;
        private static Models.External.AC_ElementsAndProperties_Response objACElementsProperties;

        JsonSerializerOptions jsonOpts = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
        JsonSerializerSettings jsonSerialSets = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented
        };
        JsonMergeSettings jsonMergeSets = new JsonMergeSettings
        {

            MergeArrayHandling = MergeArrayHandling.Concat
        };
    public ArchiCADController(IHostApplicationLifetime appLifetime)
        {
            applicationLifetime = appLifetime;
            Client = ServicesExt.ArchiCADWebApiService.Client;
        }

        private static HttpClient InitializeHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:" + "5001");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        [HttpGet("/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Models.External.AC_APIHealth_Response> GetStatus()
        {
            //StringContent queryString = new StringContent(JsonSerializer.Serialize(new Models.External.AC_APICommand_Request("API.IsAlive"), jsonOpts));
            StringContent queryString = new StringContent(JsonConvert.SerializeObject(new Models.External.AC_APICommand_Request("API.IsAlive"),jsonSerialSets));
            var response = await Client.PostAsync("", queryString);

            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();
            //return await JsonSerializer.DeserializeAsync<Models.External.AC_APIHealth_Response>(responseStream);
            string responseString;
            using (StreamReader reader = new StreamReader(responseStream))
            {
                responseString = reader.ReadToEnd();
            }
            return JsonConvert.DeserializeObject<Models.External.AC_APIHealth_Response>(responseString);
        }

        [HttpGet("/GetAllElementsGuid")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<List<Models.Internal.Element>> GetElementsnProps()
        {
            //StringContent queryString = new StringContent(JsonSerializer.Serialize(new Models.External.AC_APICommand_Request("API.GetAllElements"), jsonOpts));
            StringContent queryString = new StringContent(JsonConvert.SerializeObject(new Models.External.AC_APICommand_Request("API.GetAllElements"), jsonSerialSets));
            var response = await Client.PostAsync("", queryString);

            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();
            //objACElements = JsonSerializer.DeserializeAsync<Models.External.AC_Elements_Response>(responseStream).Result;
            string responseString;
            using (StreamReader reader = new StreamReader(responseStream))
            {
                responseString = reader.ReadToEnd();
            }
            objACElements = JsonConvert.DeserializeObject<Models.External.AC_Elements_Response>(responseString);

            foreach (var elem in objACElements.result.elements)
            {
                Models.Internal.Element elem_ = new Models.Internal.Element
                {
                    gGuid = elem.elementId.guid,
                    sName = "ACelement"
                };
                ACElements.Add(elem_);
            }
            return ACElements;
        }

        [HttpGet("/GetAllPropertyNames")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Models.External.AC_PropertyNames_Response> GetAllPropertyNames()
        {
            //StringContent queryString = new StringContent(JsonSerializer.Serialize(new Models.External.AC_APICommand_Request("API.GetAllPropertyNames"), jsonOpts));
            StringContent queryString = new StringContent(JsonConvert.SerializeObject(new Models.External.AC_APICommand_Request("API.GetAllPropertyNames"), jsonSerialSets));
            var response = await Client.PostAsync("", queryString);

            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();

            //ValueTask<Models.External.AC_PropertyNames_Response> vtResult = JsonSerializer.DeserializeAsync<Models.External.AC_PropertyNames_Response>(responseStream);
            string responseString;
            using (StreamReader reader = new StreamReader(responseStream))
            {
                responseString = reader.ReadToEnd();
            }
            objACPropertyNames = JsonConvert.DeserializeObject<Models.External.AC_PropertyNames_Response>(responseString);
            return objACPropertyNames;
        }

        [HttpGet("/GetAllPropertyGuids")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Models.External.AC_PropertyGuids_Response> GetAllPropertyGuids()
        {
            await Clientloc.GetStringAsync("/GetAllPropertyNames");
            //StringContent queryString = new StringContent(JsonSerializer.Serialize(new Models.External.AC_APICommand_Request("API.GetPropertyIds", objACPropertyNames.result), jsonOpts));
            StringContent queryString = new StringContent(JsonConvert.SerializeObject(new Models.External.AC_APICommand_Request("API.GetPropertyIds", objACPropertyNames.result), jsonSerialSets));
            var response = await Client.PostAsync("", queryString);

            response.EnsureSuccessStatusCode();
            using var responseStream = await response.Content.ReadAsStreamAsync();

            //ValueTask<Models.External.AC_PropertyGuids_Response> vtResult = JsonSerializer.DeserializeAsync<Models.External.AC_PropertyGuids_Response>(responseStream);
            string responseString;
            using (StreamReader reader = new StreamReader(responseStream))
            {
                responseString = reader.ReadToEnd();
            }
            objACPropertyGuids = JsonConvert.DeserializeObject<Models.External.AC_PropertyGuids_Response>(responseString);
            return objACPropertyGuids;
        }

        [HttpGet("/GetAllElementsAndAllProperties")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Models.External.AC_ElementsAndProperties_Response> GetAllElementsAndAllProperties()
        {
            await Clientloc.GetStringAsync("/GetAllElementsGuid");
            await Clientloc.GetStringAsync("/GetAllPropertyGuids");
            AC_PropertyGuids_Request objACPropertyGuidsLoc = new AC_PropertyGuids_Request();
            objACPropertyGuidsLoc.result = new AC_PropertyGuids_Request_Result();
            objACPropertyGuidsLoc.result.properties = new List<AC_PropertyGuid>();
            int iCounter = 0;
            foreach (var item in objACPropertyGuids.result.properties)
            {
                objACPropertyGuidsLoc.result.properties.Add(new AC_PropertyGuid() { propertyId = new Propertyid() { guid = "0"} }); ;
                objACPropertyGuidsLoc.result.properties[iCounter].propertyId.guid = item.propertyId.guid;
                iCounter++;
            }
            //StringContent queryString = new StringContent(JsonSerializer.Serialize(new Models.External.AC_APICommand_Request("API.GetPropertyIds", objACElements.result), jsonOpts));
            JObject jobjElements = JObject.FromObject(objACElements.result);
            JObject jobjProperties = JObject.FromObject(objACPropertyGuidsLoc.result);
            jobjElements.Merge(jobjProperties, jsonMergeSets);
            string sdf = JsonConvert.SerializeObject(new Models.External.AC_APICommand_Request("API.GetPropertyValuesOfElements", jobjElements), jsonSerialSets);

            StringContent queryString = new StringContent(sdf);
            var response = await Client.PostAsync("", queryString);
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            string responseString;
            using (StreamReader reader = new StreamReader(responseStream))
            {
                responseString = reader.ReadToEnd();
            }
            //ValueTask<Models.External.AC_ElementsAndProperties_Response> vtResult = JsonSerializer.DeserializeAsync<Models.External.AC_ElementsAndProperties_Response>(responseStream);
            objACElementsProperties = JsonConvert.DeserializeObject<Models.External.AC_ElementsAndProperties_Response>(responseString);
            return objACElementsProperties;
        }
        //General_ElementID, General_HotlinkAndElementID, General_LastIssueID, General_LastIssueName, General_UniqueID, General_HotlinkMasterID
        //    IdAndCategories_ElementID, IdAndCategories_HotlinkAndElementID, IdAndCategories_UniqueID, IdAndCategories_ParentId, IdAndCategories_HotlinkMasterID, IdAndCategories_Name
        //    General_GrossVolume

        [HttpGet("/Exit")]
        public IActionResult StopApplication()
        {
            applicationLifetime.StopApplication();
            return new EmptyResult();
        }
    }
}
