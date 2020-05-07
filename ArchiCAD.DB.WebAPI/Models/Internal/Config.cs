using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.Internal
{

    public class ConfigOptions
    {
        public Archicad ArchiCAD { get; set; }
    }

    public class Archicad
    {
        public Program Program { get; set; }
        public Workfile WorkFile { get; set; }
    }

    public class Program
    {
        public string Path { get; set; }
        public int Port { get; set; }
    }

    public class Workfile
    {
        public string Path { get; set; }
    }
}