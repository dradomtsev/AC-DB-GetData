using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.External
{
    public class AC_PropertyGuids_Response
    {
        public bool succeeded { get; set; }
        public AC_PropertyGuids_Result result { get; set; }
    }
    public class AC_PropertyGuids_Result
    {
        public IList<AC_Property> properties { get; set; }
    }
}
