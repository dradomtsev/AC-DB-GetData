using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Microsoft.Extensions.Configuration;

namespace ArchiCAD.DB.WebAPI.ServicesExt
{
    public interface IArchiCADWebApiService
    {
        Task StartArchiCAD();
        Task GetArchiCADPort();
    }
    public class ArchiCADWebApiService : IArchiCADWebApiService, IDisposable
    {
        private IConfiguration _configuration;
        private readonly Process ACProcess = new Process();
        public static HttpClient Client = new HttpClient();
        private bool disposedValue;

        public ArchiCADWebApiService(IConfiguration configuration)
        {
            _configuration = configuration;
            StartArchiCAD();
            Client.BaseAddress = new Uri("http://localhost:" + _configuration["ArchiCAD:Program:Port"]);
            Client.DefaultRequestHeaders.Add("Accept", "application/json");
        }
        public async Task StartArchiCAD()
        {
            ACProcess.StartInfo.FileName = _configuration["ArchiCAD:Program:Path"];
            ACProcess.StartInfo.Arguments = _configuration["ArchiCAD:Workfile:Path"];
            ACProcess.StartInfo.RedirectStandardOutput = true;
            ACProcess.StartInfo.RedirectStandardError = true;

            ACProcess.OutputDataReceived += (sender, data) => Console.WriteLine(data.Data);
            ACProcess.ErrorDataReceived += (sender, data) => Console.WriteLine(data.Data);

            ACProcess.Start();
            ACProcess.WaitForInputIdle();
            Thread.Sleep(45 * 1000);
            if (ACProcess.Id != 0 && !ACProcess.HasExited)
               await GetArchiCADPort();
        }

        public async Task GetArchiCADPort()
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

                string content = stdOutput.ReadToEnd();
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
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                ACProcess.CloseMainWindow();
                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ArchiCADWebApiService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
