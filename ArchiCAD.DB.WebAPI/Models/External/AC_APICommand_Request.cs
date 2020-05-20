using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.External
{
    public class AC_APICommand_Request
    {
        public string command { get; set; }
        public object parameters { get; set; }
        public AC_APICommand_Request(string sCommand, [System.Runtime.InteropServices.OptionalAttribute] object objParameters)
        {
            command = sCommand;
            parameters = objParameters;
        }
    }
}
