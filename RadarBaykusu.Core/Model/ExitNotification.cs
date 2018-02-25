using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarBaykusu.Core.Model
{
    public class ExitNotification
    {
        public double areaID { get; set; }
        public int ContainerID { get; set; }
        public int RightUpperID { get; set; }
        public int LeftID { get; set; }
        public int CenterID { get; set; }
        public int RightID { get; set; }
    }

    public class ExitNotificationIOS
    {
        public double areaID { get; set; }
        public int ContainerID { get; set; }
        public int RightUpperID { get; set; }
        public int LeftID { get; set; }
        public int CenterID { get; set; }
        public int RightID { get; set; }
        public int BGID { get; set; }
    }

}
