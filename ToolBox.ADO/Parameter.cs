using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolBox.ADO
{
    public class Parameter
    {
        public string ParameterName { get; set; }
        public object Value { get; set; }
        public System.Data.ParameterDirection Direction { get; set; }
        public int Size { get; set; }
    }
}
