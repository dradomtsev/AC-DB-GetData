using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.External
{
    public class AC_APIHealth
    {
        public bool succeeded { get; set; }
        public Result result { get; set; }
    }

    public class Result
    {
        public bool isAlive { get; set; }
    }
}
