using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerVisionQuickstart.Models
{
    public class ResponseModel
    {
        public string FileName { get; set; }
        public string PathFile { get; set; }
        public object ResponseComputerVision { get; set; }
    }
}
