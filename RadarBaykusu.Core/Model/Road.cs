using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadarBaykusu.Core.Model
{
    public class Road
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int RoadID { get; set; }
        public string RoadName { get; set; }
    }
}