using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.External
{
    public class AC_Error
    {
        public int code { get; set; }
        public string message { get; set; }
    }
}
