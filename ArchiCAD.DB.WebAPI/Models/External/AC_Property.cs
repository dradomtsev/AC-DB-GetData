using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.External
{
    public class AC_Property
    {
        public string type { get; set; }
        public string nonLocalizedName { get; set; }
        public List<string> localizedName { get; set; }

        public Propertyid propertyId { get; set; }
        public AC_Error error { get; set; }
    }
    public class Propertyid
    {
        public string guid { get; set; }
    }
}
