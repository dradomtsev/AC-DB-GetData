using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.External
{
    public class AC_PropertyGuids_Request
    {
        public bool succeeded { get; set; }
        public AC_PropertyGuids_Request_Result result { get; set; }
    }
    public class AC_PropertyGuids_Request_Result
    {
        public IList<AC_PropertyGuid> properties { get; set; }
    }
}
