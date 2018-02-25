using SQLite;

namespace RadarBaykusu.Core.Model
{
    public class Area
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int RoadID { get; set; }
        public double areaID { get; set; }
        public string areaName { get; set; }
        public double areaLatitude { get; set; }
        public double areaLongitude { get; set; }
        public bool isBooth { get; set; }
        public override string ToString()
        {
            return "\nTollAreaName:" + areaName;
        }
    }
}