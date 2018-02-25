using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadarBaykusu.Core.Model
{
    public class Configuration
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string ParamName { get; set; }
        public string ParamValue { get; set; }

    }
}