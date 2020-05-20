using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.Internal
{
    public class Property
    {
        public string sGuid { get; set; }
        public string sType { get; set; }
        public List<string> sNonLocalizedName { get; set; }
        public List<string> sLocalizedName { get; set; }
    }
}
