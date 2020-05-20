using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.External
{
    public class AC_Elements_Response
    {
        public bool succeeded { get; set; }
        public AC_Elements_Result result { get; set; }
    }
    public class AC_Elements_Result
    {
        public IEnumerable<Element> elements { get; set; }
    }

    public class Element
    {
        public Elementid elementId { get; set; }
    }

    public class Elementid
    {
        public string guid { get; set; }
    }
}