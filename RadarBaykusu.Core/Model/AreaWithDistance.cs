using SQLite;

namespace RadarBaykusu.Core.Model
{
    public class AreaWithDistance
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public double areaID { get; set; }
        public string areaName { get; set; }
        public double areaLatitude { get; set; }
        public double areaLongitude { get; set; }
        public double distance { get; set; }
        public bool isBooth { get; set; }
    }
}
