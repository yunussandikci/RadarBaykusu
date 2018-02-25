using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarBaykusu.Core.HelperClasses
{
    public class PergaResponse<T> where T : class
    {
        public bool Result { get; set; }
        public string Message { get; set; }
        public T ReturnObject { get; set; }
    }
}
