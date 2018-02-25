
using SQLite;

namespace RadarBaykusu.Core.Model
{
    public class AreaPass
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public double areaEnterID { get; set; }
        public double areaExitID { get; set; }
        public double distance { get; set; }
        public double speedLimit { get; set; }
    }
}