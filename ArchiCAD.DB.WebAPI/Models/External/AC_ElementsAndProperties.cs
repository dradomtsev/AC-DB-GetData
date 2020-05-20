using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiCAD.DB.WebAPI.Models.External
{
    public class AC_ElementsAndProperties_Response
    {
        public bool succeeded { get; set; }
        public AC_ElementsAndProperties_Result result { get; set; }
    }
}

public class AC_ElementsAndProperties_Result
{
    public IList<ElementnProperties> propertyValuesForElements { get; set; }
}

public class ElementnProperties
{
    public ArchiCAD.DB.WebAPI.Models.External.AC_Error error { get; set; }
    public IList<ElementProperty> propertyValues { get; set; }
}

public class ElementProperty
{
    public ElementPropertyValue propertyValue { get; set; }
    public ArchiCAD.DB.WebAPI.Models.External.AC_Error error { get; set; }
}

public class ElementPropertyValue
{
    public string type { get; set; }
    public string status { get; set; }
    public object value { get; set; }
}


//public class Rootobject
//{
//    public bool succeeded { get; set; }
//    public Result result { get; set; }
//}

//public class Result
//{
//    public Propertyvaluesforelement[] propertyValuesForElements { get; set; }
//}

//public class Propertyvaluesforelement
//{
//    public Propertyvalue[] propertyValues { get; set; }
//}

//public class Propertyvalue
//{
//    public Propertyvalue1 propertyValue { get; set; }
//}

//public class Propertyvalue1
//{
//    public string type { get; set; }
//    public string status { get; set; }
//    public object value { get; set; }
//}
