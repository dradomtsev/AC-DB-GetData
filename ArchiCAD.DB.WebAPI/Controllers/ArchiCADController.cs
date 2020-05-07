using System;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace ArchiCAD.DB.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArchiCADController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly ServicesExt.IArchiCADWebApiService _ArchiCADClient;
        private readonly Process ACProcess = new Process();

        public ArchiCADController(ServicesExt.IArchiCADWebApiService ArchiCADClient, IConfiguration configuration)
        {
            _configuration = configuration;
            _ArchiCADClient = ArchiCADClient;
            StartArchiCAD();
            if (ACProcess.Id != 0 && !ACProcess.HasExited)
                GetArchiCADPort();
        }

        private void StartArchiCAD()
        {
            ACProcess.StartInfo.FileName = _configuration["ArchiCAD:Program:Path"];
            ACProcess.StartInfo.Arguments = _configuration["ArchiCAD:Workfile:Path"];
            ACProcess.StartInfo.RedirectStandardOutput = true;
            ACProcess.StartInfo.RedirectStandardError = true;

            ACProcess.OutputDataReceived += (sender, data) => Console.WriteLine(data.Data);
            ACProcess.ErrorDataReceived += (sender, data) => Console.WriteLine(data.Data);

            ACProcess.Start();
            ACProcess.WaitForInputIdle();
            Thread.Sleep(60*1000);
            //ACProcess.WaitForInputIdle();
            //ACProcess.BeginOutputReadLine();
            //ACProcess.BeginErrorReadLine();
        }
        private void GetArchiCADPort()
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = @"C:\Windows\System32\NETSTAT.EXE";
                process.StartInfo.Arguments = "-a -n -o";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.Start();
                StreamReader stdOutput = process.StandardOutput;
                StreamReader stdError = process.StandardError;

                string content = stdOutput.ReadToEnd();// + stdError.ReadToEnd();
                string exitStatus = process.ExitCode.ToString();

                string[] rows = Regex.Split(content, "\r\n");
                var PortRange = Enumerable.Range(19723, 20);
                foreach (string row in rows)
                {
                    string[] tokens = Regex.Split(row, "\\s+");
                    if (tokens.Length > 4 && tokens[1].Equals("TCP"))
                    {
                        string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                        if (tokens[5].Equals(ACProcess.Id.ToString()) && PortRange.Contains(Int32.Parse(localAddress.Split(':')[1])))
                            _configuration["ArchiCAD:Program:Port"] = localAddress.Split(':')[1];
                        //string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                        //string port_number = localAddress.Split(':')[1];
                    }
                }
            }
        }
        //[HttpGet("/GetStatus")]
        [HttpGet("/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Models.External.AC_APIHealth> GetStatus()
        {
            return await _ArchiCADClient.CheckHealth();
        }
    }

}
