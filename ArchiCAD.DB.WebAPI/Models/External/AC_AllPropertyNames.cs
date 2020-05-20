using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.External
{
    public class AC_PropertyNames_Response
    {
        public bool succeeded { get; set; }
        public AC_PropertyNames_Result result { get; set; }
    }
    public class AC_PropertyNames_Result
    {
        public IEnumerable<AC_Property> properties { get; set; }
    }
}