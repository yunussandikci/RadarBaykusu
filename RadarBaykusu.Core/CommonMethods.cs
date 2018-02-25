using RadarBaykusu.Core.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarBaykusu.Core
{
    public class CommonMethods
    {
        public static DateTime GetExitTime(double distance, double carMaxSpeed, DateTime tollAreaEntryTime)
        {
            double hour = distance / carMaxSpeed;
            return tollAreaEntryTime.AddHours(hour);
        }

        public static TimeSpan GetMinutesLeft(double distance, double carMaxSpeed, DateTime tollAreaEntryTime)
        {
            double hour = distance / carMaxSpeed;
            DateTime left = tollAreaEntryTime.AddHours(distance / carMaxSpeed);
            if (left - DateTime.Now < TimeSpan.Zero)
                return TimeSpan.Zero;
            else
                return left - DateTime.Now;
        }

        public static double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(lat1 * Math.PI / 180.0) * Math.Sin(lat2 * Math.PI / 180.0) + Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) * Math.Cos(theta * Math.PI / 180.0);
            dist = Math.Acos(dist);
            dist = dist / Math.PI * 180.0;
            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;
            return (dist);
        }

        public static bool DBCheck(string dbLocation)
        {
            bool returnResult = false;
            var tableNameList = new List<CheckTable>();
            var dBOperations = new DBOperations(dbLocation);
            SQLiteConnection SQLConnection = new SQLiteConnection(System.IO.Path.Combine(dbLocation, "radarbaykusu.db"));

            try
            {
                //sqlite dan yaratilmis tablo isimlerini al
                var getTableListResult = dBOperations.GetTableList();

                //sonuc true ise
                if (getTableListResult.Result)
                {
                    //tablo listesini doldur
                    tableNameList = dBOperations.GetTableList().ReturnObject;

                    //liste null degilse
                    if (tableNameList != null)
                    {
                        //Road tabolosu yoksa
                        if (!tableNameList.Any(x => x.Name == "Road"))
                        {
                            //SQLConnection.DropTable<Area>();
                            //SQLConnection.DropTable<AreaPass>();
                            //dBOperations.CreateTable(typeof(Area));
                            //FillTollAreaTable(dbLocation);
                            //dBOperations.CreateTable(typeof(AreaPass));
                            //FillTollAreaExitTable(dbLocation);

                            //Road tabolosunu yarat
                            dBOperations.CreateTable(typeof(Road));
                            //Road verilerini gir
                            FillRoadTable(dbLocation);
                            //tableNameList = dBOperations.GetTableList().ReturnObject;
                        }
                        //configuration tabolosu yoksa
                        if (!tableNameList.Any(x => x.Name == "Configuration"))
                        {
                            //configuration tablosunu yarat
                            dBOperations.CreateTable(typeof(Configuration));
                            //config parametrelerini gir
                            FillConfiguration(dbLocation);
                        }

                        //Area tabolosu yoksa
                        if (!tableNameList.Any(x => x.Name == "Area"))
                        {
                            //Area tabolosunu yarat
                            dBOperations.CreateTable(typeof(Area));
                            //exit bilgilerini gir
                            FillTollAreaTable(dbLocation);
                        }

                        //AreaPass tabolosu yoksa
                        if (!tableNameList.Any(x => x.Name == "AreaPass"))
                        {
                            //AreaPass tabolosunu yarat
                            dBOperations.CreateTable(typeof(AreaPass));
                            //Mapping verilerini gir
                            FillTollAreaExitTable(dbLocation);
                        }
                        returnResult = true;
                    }
                }
                else
                {
                    returnResult = false;
                }
            }
            catch (Exception ex)
            {
                //hata mesaji loglansin TODO
                returnResult = false;
            }

            return returnResult;
        }
        private static void FillConfiguration(string dbLocation)
        {
            var dBOperations = new DBOperations(dbLocation);
            var configurationList = new List<Configuration>()
                {
                    new Configuration() {ParamName = "isTermsAccepted",ParamValue = "false"},
                    new Configuration() {ParamName = "hizLimiti_Binek", ParamValue = "120"},
                    new Configuration() {ParamName = "hizLimiti_Ticari",ParamValue = "90"}
                };

            dBOperations.InsertAll(configurationList, typeof(Configuration));
        }
        private static void FillRoadTable(string dbLocation)
        {
            var dBOperations = new DBOperations(dbLocation);

            var roadList = new List<Road>()
                {
                    new Road() {RoadID = 1, RoadName = "Izmir - Aydın Otoyolu"},
                    new Road() {RoadID = 2, RoadName = "Izmir - Çeşme Otoyolu"},
                    new Road() {RoadID = 3, RoadName = "Avrupa Otoyolu"}
                };

            dBOperations.InsertAll(roadList, typeof(Road));
        }

        private static void FillTollAreaTable(string dbLocation)
        {
            var dBOperations = new DBOperations(dbLocation);
            List<Area> tollAreaList = new List<Area>();
            #region Izmir - Aydin

            Area newTollArea = new Area { areaID = 1, areaName = "Işıkkent", areaLatitude = 38.349817, areaLongitude = 27.233494, isBooth = true };
            tollAreaList.Add(newTollArea);
            Area newTollArea2 = new Area { areaID = 1.1, areaName = "Işıkkent Gate", areaLatitude = 38.348549, areaLongitude = 27.233526, isBooth = false };
            tollAreaList.Add(newTollArea2);
            Area newTollArea3 = new Area { areaID = 1.2, areaName = "Tahtalıçay Gate 1", areaLatitude = 38.280364, areaLongitude = 27.224707, isBooth = false };
            tollAreaList.Add(newTollArea3);
            Area newTollArea4 = new Area { areaID = 2, areaName = "Tahtalıçay", areaLatitude = 38.272664, areaLongitude = 27.217156, isBooth = true };
            tollAreaList.Add(newTollArea4);
            Area newTollArea5 = new Area { areaID = 2.1, areaName = "Tahtalıçay Gate 2", areaLatitude = 38.262033, areaLongitude = 27.221531, isBooth = false };
            tollAreaList.Add(newTollArea5);
            Area newTollArea6 = new Area { areaID = 2.2, areaName = "Torbalı Gate 1", areaLatitude = 38.181769, areaLongitude = 27.311198, isBooth = false };
            tollAreaList.Add(newTollArea6);
            Area newTollArea7 = new Area { areaID = 3, areaName = "Torbalı", areaLatitude = 38.194008, areaLongitude = 27.337692, isBooth = true };
            tollAreaList.Add(newTollArea7);
            Area newTollArea8 = new Area { areaID = 3.1, areaName = "Torbalı Gate 2", areaLatitude = 38.169250, areaLongitude = 27.314788, isBooth = false };
            tollAreaList.Add(newTollArea8);
            Area newTollArea9 = new Area { areaID = 3.2, areaName = "Belevi Gate 1", areaLatitude = 38.026104, areaLongitude = 27.440117, isBooth = false };
            tollAreaList.Add(newTollArea9);
            Area newTollArea10 = new Area { areaID = 4, areaName = "Belevi", areaLatitude = 38.029083, areaLongitude = 27.449236, isBooth = true };
            tollAreaList.Add(newTollArea10);
            Area newTollArea11 = new Area { areaID = 4.1, areaName = "Belevi Gate 2", areaLatitude = 38.018769, areaLongitude = 27.454438, isBooth = false };
            tollAreaList.Add(newTollArea11);
            Area newTollArea12 = new Area { areaID = 4.2, areaName = "Germencik Gate 1", areaLatitude = 37.885426, areaLongitude = 27.574535, isBooth = false };
            tollAreaList.Add(newTollArea12);
            Area newTollArea13 = new Area { areaID = 5, areaName = "Germencik", areaLatitude = 37.878217, areaLongitude = 27.578042, isBooth = true };
            tollAreaList.Add(newTollArea13);
            Area newTollArea14 = new Area { areaID = 5.1, areaName = "Germencik Gate 2", areaLatitude = 37.881230, areaLongitude = 27.590210, isBooth = false };
            tollAreaList.Add(newTollArea14);
            Area newTollArea15 = new Area { areaID = 5.2, areaName = "Aydın Kuzey Gate", areaLatitude = 37.862656, areaLongitude = 27.777258, isBooth = false };
            tollAreaList.Add(newTollArea15);
            Area newTollArea16 = new Area { areaID = 6, areaName = "Aydın Kuzey", areaLatitude = 37.860439, areaLongitude = 27.788492, isBooth = true };
            tollAreaList.Add(newTollArea16);

            #endregion
            #region İzmir - Çeşme
            Area newTollArea18 = new Area { areaID = 7, areaName = "Seferihisar", areaLatitude = 38.363192, areaLongitude = 26.864297, isBooth = true };
            tollAreaList.Add(newTollArea18);
            Area newTollArea19 = new Area { areaID = 7.2, areaName = "Seferihisar Gate 1", areaLatitude = 38.365347, areaLongitude = 26.866310, isBooth = false };
            tollAreaList.Add(newTollArea19);
            Area newTollArea20 = new Area { areaID = 7.3, areaName = "Urla Gate 1", areaLatitude = 38.319070, areaLongitude = 26.789697, isBooth = false };
            tollAreaList.Add(newTollArea20);
            Area newTollArea21 = new Area { areaID = 8, areaName = "Urla", areaLatitude = 38.322789, areaLongitude = 26.781961, isBooth = true };
            tollAreaList.Add(newTollArea21);
            Area newTollArea22 = new Area { areaID = 8.1, areaName = "Urla Gate 2", areaLatitude = 38.316125, areaLongitude = 26.778991, isBooth = false };
            tollAreaList.Add(newTollArea22);
            Area newTollArea23 = new Area { areaID = 8.2, areaName = "Karaburun Gate 1", areaLatitude = 38.295224, areaLongitude = 26.696702, isBooth = false };
            tollAreaList.Add(newTollArea23);
            Area newTollArea24 = new Area { areaID = 9, areaName = "Karaburun", areaLatitude = 38.303045, areaLongitude = 26.686012, isBooth = true };
            tollAreaList.Add(newTollArea24);
            Area newTollArea25 = new Area { areaID = 9.1, areaName = "Karaburun Gate 2", areaLatitude = 38.296799, areaLongitude = 26.682553, isBooth = false };
            tollAreaList.Add(newTollArea25);
            Area newTollArea26 = new Area { areaID = 9.2, areaName = "Zeytinler Gate 1", areaLatitude = 38.280887, areaLongitude = 26.570717, isBooth = false };
            tollAreaList.Add(newTollArea26);
            Area newTollArea27 = new Area { areaID = 10, areaName = "Zeytinler", areaLatitude = 38.285034, areaLongitude = 26.564107, isBooth = true };
            tollAreaList.Add(newTollArea27);
            Area newTollArea28 = new Area { areaID = 10.1, areaName = "Zeytinler Gate 2", areaLatitude = 38.280462, areaLongitude = 26.558759, isBooth = false };
            tollAreaList.Add(newTollArea28);
            Area newTollArea29 = new Area { areaID = 10.2, areaName = "Alaçatı Gate 1", areaLatitude = 38.278265, areaLongitude = 26.389298, isBooth = false };
            tollAreaList.Add(newTollArea29);
            Area newTollArea30 = new Area { areaID = 11, areaName = "Alaçatı", areaLatitude = 38.280511, areaLongitude = 26.382368, isBooth = true };
            tollAreaList.Add(newTollArea30);
            Area newTollArea31 = new Area { areaID = 11.1, areaName = "Alaçatı Gate 2", areaLatitude = 38.276998, areaLongitude = 26.377074, isBooth = false };
            tollAreaList.Add(newTollArea31);
            Area newTollArea32 = new Area { areaID = 11.2, areaName = "Çeşme Gate", areaLatitude = 38.294980, areaLongitude = 26.310653, isBooth = false };
            tollAreaList.Add(newTollArea32);
            Area newTollArea33 = new Area { areaID = 12, areaName = "Çeşme", areaLatitude = 38.295994, areaLongitude = 26.310317, isBooth = true };
            tollAreaList.Add(newTollArea33);
            #endregion
            dBOperations.InsertAll(tollAreaList, typeof(Area));

        }

        private static void FillTollAreaExitTable(string dbLocation)
        {

            var dBOperations = new DBOperations(dbLocation);

            #region Izmir - Aydin

            List<AreaPass> tollAreaExitList = new List<AreaPass>();
            AreaPass newexit = new AreaPass { areaEnterID = 2, areaExitID = 1, distance = 10.800, speedLimit = 120 };
            tollAreaExitList.Add(newexit);
            AreaPass newexit2 = new AreaPass { areaEnterID = 3, areaExitID = 1, distance = 26.000, speedLimit = 120 };
            tollAreaExitList.Add(newexit2);
            AreaPass newexit3 = new AreaPass { areaEnterID = 3, areaExitID = 2, distance = 18.200, speedLimit = 120 };
            tollAreaExitList.Add(newexit3);
            AreaPass newexit4 = new AreaPass { areaEnterID = 4, areaExitID = 1, distance = 45.100, speedLimit = 120 };
            tollAreaExitList.Add(newexit4);
            AreaPass newexit5 = new AreaPass { areaEnterID = 4, areaExitID = 2, distance = 37.300, speedLimit = 120 };
            tollAreaExitList.Add(newexit5);
            AreaPass newexit6 = new AreaPass { areaEnterID = 4, areaExitID = 3, distance = 25.000, speedLimit = 120 };
            tollAreaExitList.Add(newexit6);
            AreaPass newexit7 = new AreaPass { areaEnterID = 5, areaExitID = 1, distance = 67.300, speedLimit = 120 };
            tollAreaExitList.Add(newexit7);
            AreaPass newexit8 = new AreaPass { areaEnterID = 5, areaExitID = 2, distance = 59.500, speedLimit = 120 };
            tollAreaExitList.Add(newexit8);
            AreaPass newexit9 = new AreaPass { areaEnterID = 5, areaExitID = 3, distance = 47.300, speedLimit = 120 };
            tollAreaExitList.Add(newexit9);
            AreaPass newexit10 = new AreaPass { areaEnterID = 5, areaExitID = 4, distance = 23.500, speedLimit = 120 };
            tollAreaExitList.Add(newexit10);
            AreaPass newexit11 = new AreaPass { areaEnterID = 6, areaExitID = 1, distance = 99.600, speedLimit = 120 };
            tollAreaExitList.Add(newexit11);
            AreaPass newexit12 = new AreaPass { areaEnterID = 6, areaExitID = 2, distance = 76.600, speedLimit = 120 };
            tollAreaExitList.Add(newexit12);
            AreaPass newexit13 = new AreaPass { areaEnterID = 6, areaExitID = 3, distance = 65.000, speedLimit = 120 };
            tollAreaExitList.Add(newexit13);
            AreaPass newexit14 = new AreaPass { areaEnterID = 6, areaExitID = 4, distance = 41.300, speedLimit = 120 };
            tollAreaExitList.Add(newexit14);
            AreaPass newexit15 = new AreaPass { areaEnterID = 6, areaExitID = 5, distance = 19.400, speedLimit = 120 };
            tollAreaExitList.Add(newexit15);
            AreaPass newexit16 = new AreaPass { areaEnterID = 1, areaExitID = 2, distance = 10.800, speedLimit = 120 };
            tollAreaExitList.Add(newexit16);
            AreaPass newexit17 = new AreaPass { areaEnterID = 1, areaExitID = 3, distance = 26.000, speedLimit = 120 }; 
            tollAreaExitList.Add(newexit17);
            AreaPass newexit18 = new AreaPass { areaEnterID = 1, areaExitID = 4, distance = 45.100, speedLimit = 120 };
            tollAreaExitList.Add(newexit18);
            AreaPass newexit19 = new AreaPass { areaEnterID = 1, areaExitID = 5, distance = 67.300, speedLimit = 120 };
            tollAreaExitList.Add(newexit19);
            AreaPass newexit20 = new AreaPass { areaEnterID = 1, areaExitID = 6, distance = 99.600, speedLimit = 120 };
            tollAreaExitList.Add(newexit20);
            AreaPass newexit21 = new AreaPass { areaEnterID = 2, areaExitID = 3, distance = 18.200, speedLimit = 120 };
            tollAreaExitList.Add(newexit21);
            AreaPass newexit22 = new AreaPass { areaEnterID = 2, areaExitID = 4, distance = 37.300, speedLimit = 120 };
            tollAreaExitList.Add(newexit22);
            AreaPass newexit23 = new AreaPass { areaEnterID = 2, areaExitID = 5, distance = 59.500, speedLimit = 120 };
            tollAreaExitList.Add(newexit23);
            AreaPass newexit24 = new AreaPass { areaEnterID = 2, areaExitID = 6, distance = 76.600, speedLimit = 120 };
            tollAreaExitList.Add(newexit24);
            AreaPass newexit25 = new AreaPass { areaEnterID = 3, areaExitID = 4, distance = 25.000, speedLimit = 120 };
            tollAreaExitList.Add(newexit25);
            AreaPass newexit26 = new AreaPass { areaEnterID = 3, areaExitID = 5, distance = 47.300, speedLimit = 120 };
            tollAreaExitList.Add(newexit26);
            AreaPass newexit27 = new AreaPass { areaEnterID = 3, areaExitID = 6, distance = 65.000, speedLimit = 120 };
            tollAreaExitList.Add(newexit27);
            AreaPass newexit28 = new AreaPass { areaEnterID = 4, areaExitID = 5, distance = 23.500, speedLimit = 120 };
            tollAreaExitList.Add(newexit28);
            AreaPass newexit29 = new AreaPass { areaEnterID = 4, areaExitID = 6, distance = 41.300, speedLimit = 120 };
            tollAreaExitList.Add(newexit29);
            AreaPass newexit30 = new AreaPass { areaEnterID = 5, areaExitID = 6, distance = 19.400, speedLimit = 120 };
            tollAreaExitList.Add(newexit30);
            AreaPass newexit31 = new AreaPass { areaEnterID = 1, areaExitID = 1.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit31);
            AreaPass newexit32 = new AreaPass { areaEnterID = 1, areaExitID = 1.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit32);
            AreaPass newexit33 = new AreaPass { areaEnterID = 1, areaExitID = 2.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit33);
            AreaPass newexit34 = new AreaPass { areaEnterID = 1, areaExitID = 2.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit34);
            AreaPass newexit35 = new AreaPass { areaEnterID = 1, areaExitID = 3.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit35);
            AreaPass newexit36 = new AreaPass { areaEnterID = 1, areaExitID = 3.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit36);
            AreaPass newexit37 = new AreaPass { areaEnterID = 1, areaExitID = 4.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit37);
            AreaPass newexit38 = new AreaPass { areaEnterID = 1, areaExitID = 4.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit38);
            AreaPass newexit39 = new AreaPass { areaEnterID = 1, areaExitID = 5.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit39);
            AreaPass newexit40 = new AreaPass { areaEnterID = 1, areaExitID = 5.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit40);
            AreaPass newexit41 = new AreaPass { areaEnterID = 2, areaExitID = 1.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit41);
            AreaPass newexit42 = new AreaPass { areaEnterID = 2, areaExitID = 1.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit42);
            AreaPass newexit43 = new AreaPass { areaEnterID = 2, areaExitID = 2.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit43);
            AreaPass newexit44 = new AreaPass { areaEnterID = 2, areaExitID = 2.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit44);
            AreaPass newexit45 = new AreaPass { areaEnterID = 2, areaExitID = 3.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit45);
            AreaPass newexit46 = new AreaPass { areaEnterID = 2, areaExitID = 3.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit46);
            AreaPass newexit47 = new AreaPass { areaEnterID = 2, areaExitID = 4.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit47);
            AreaPass newexit48 = new AreaPass { areaEnterID = 2, areaExitID = 4.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit48);
            AreaPass newexit49 = new AreaPass { areaEnterID = 2, areaExitID = 5.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit49);
            AreaPass newexit50 = new AreaPass { areaEnterID = 2, areaExitID = 5.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit50);
            AreaPass newexit51 = new AreaPass { areaEnterID = 3, areaExitID = 1.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit51);
            AreaPass newexit52 = new AreaPass { areaEnterID = 3, areaExitID = 1.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit52);
            AreaPass newexit53 = new AreaPass { areaEnterID = 3, areaExitID = 2.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit53);
            AreaPass newexit54 = new AreaPass { areaEnterID = 3, areaExitID = 2.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit54);
            AreaPass newexit55 = new AreaPass { areaEnterID = 3, areaExitID = 3.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit55);
            AreaPass newexit56 = new AreaPass { areaEnterID = 3, areaExitID = 3.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit56);
            AreaPass newexit57 = new AreaPass { areaEnterID = 3, areaExitID = 4.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit57);
            AreaPass newexit58 = new AreaPass { areaEnterID = 3, areaExitID = 4.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit58);
            AreaPass newexit59 = new AreaPass { areaEnterID = 3, areaExitID = 5.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit59);
            AreaPass newexit60 = new AreaPass { areaEnterID = 3, areaExitID = 5.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit60);
            AreaPass newexit61 = new AreaPass { areaEnterID = 4, areaExitID = 1.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit61);
            AreaPass newexit62 = new AreaPass { areaEnterID = 4, areaExitID = 1.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit62);
            AreaPass newexit63 = new AreaPass { areaEnterID = 4, areaExitID = 2.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit63);
            AreaPass newexit64 = new AreaPass { areaEnterID = 4, areaExitID = 2.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit64);
            AreaPass newexit65 = new AreaPass { areaEnterID = 4, areaExitID = 3.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit65);
            AreaPass newexit66 = new AreaPass { areaEnterID = 4, areaExitID = 3.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit66);
            AreaPass newexit67 = new AreaPass { areaEnterID = 4, areaExitID = 4.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit67);
            AreaPass newexit68 = new AreaPass { areaEnterID = 4, areaExitID = 4.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit68);
            AreaPass newexit69 = new AreaPass { areaEnterID = 4, areaExitID = 5.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit69);
            AreaPass newexit70 = new AreaPass { areaEnterID = 4, areaExitID = 5.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit70);
            AreaPass newexit71 = new AreaPass { areaEnterID = 5, areaExitID = 1.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit71);
            AreaPass newexit72 = new AreaPass { areaEnterID = 5, areaExitID = 1.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit72);
            AreaPass newexit73 = new AreaPass { areaEnterID = 5, areaExitID = 2.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit73);
            AreaPass newexit74 = new AreaPass { areaEnterID = 5, areaExitID = 2.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit74);
            AreaPass newexit75 = new AreaPass { areaEnterID = 5, areaExitID = 3.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit75);
            AreaPass newexit76 = new AreaPass { areaEnterID = 5, areaExitID = 3.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit76);
            AreaPass newexit77 = new AreaPass { areaEnterID = 5, areaExitID = 4.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit77);
            AreaPass newexit78 = new AreaPass { areaEnterID = 5, areaExitID = 4.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit78);
            AreaPass newexit79 = new AreaPass { areaEnterID = 5, areaExitID = 5.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit79);
            AreaPass newexit80 = new AreaPass { areaEnterID = 5, areaExitID = 5.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit80);
            AreaPass newexit81 = new AreaPass { areaEnterID = 6, areaExitID = 1.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit81);
            AreaPass newexit82 = new AreaPass { areaEnterID = 6, areaExitID = 1.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit82);
            AreaPass newexit83 = new AreaPass { areaEnterID = 6, areaExitID = 2.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit83);
            AreaPass newexit84 = new AreaPass { areaEnterID = 6, areaExitID = 2.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit84);
            AreaPass newexit85 = new AreaPass { areaEnterID = 6, areaExitID = 3.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit85);
            AreaPass newexit86 = new AreaPass { areaEnterID = 6, areaExitID = 3.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit86);
            AreaPass newexit87 = new AreaPass { areaEnterID = 6, areaExitID = 4.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit87);
            AreaPass newexit88 = new AreaPass { areaEnterID = 6, areaExitID = 4.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit88);
            AreaPass newexit89 = new AreaPass { areaEnterID = 6, areaExitID = 5.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit89);
            AreaPass newexit90 = new AreaPass { areaEnterID = 6, areaExitID = 5.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit90);

            #endregion
            #region Izmir - Cesme
            AreaPass newexit91 = new AreaPass { areaEnterID = 8, areaExitID = 7, distance = 9.700, speedLimit = 120 };
            tollAreaExitList.Add(newexit91);
            AreaPass newexit92 = new AreaPass { areaEnterID = 9, areaExitID = 7, distance = 19.000, speedLimit = 120 };
            tollAreaExitList.Add(newexit92);
            AreaPass newexit93 = new AreaPass { areaEnterID = 9, areaExitID = 8, distance = 12.900, speedLimit = 120 };
            tollAreaExitList.Add(newexit93);
            AreaPass newexit94 = new AreaPass { areaEnterID = 10, areaExitID = 7, distance = 31.100, speedLimit = 120 };
            tollAreaExitList.Add(newexit94);
            AreaPass newexit95 = new AreaPass { areaEnterID = 10, areaExitID = 8, distance = 23.400, speedLimit = 120 };
            tollAreaExitList.Add(newexit95);
            AreaPass newexit96 = new AreaPass { areaEnterID = 10, areaExitID = 9, distance = 14.600, speedLimit = 120 };
            tollAreaExitList.Add(newexit96);
            AreaPass newexit97 = new AreaPass { areaEnterID = 11, areaExitID = 7, distance = 47.900, speedLimit = 120 };
            tollAreaExitList.Add(newexit97);
            AreaPass newexit98 = new AreaPass { areaEnterID = 11, areaExitID = 8, distance = 40.200, speedLimit = 120 };
            tollAreaExitList.Add(newexit98);
            AreaPass newexit99 = new AreaPass { areaEnterID = 11, areaExitID = 9, distance = 31.400, speedLimit = 120 };
            tollAreaExitList.Add(newexit99);
            AreaPass newexit100 = new AreaPass { areaEnterID = 11, areaExitID = 10, distance = 18.600, speedLimit = 120 };
            tollAreaExitList.Add(newexit100);
            AreaPass newexit101 = new AreaPass { areaEnterID = 12, areaExitID = 7, distance = 57.600, speedLimit = 120 };
            tollAreaExitList.Add(newexit101);
            AreaPass newexit102 = new AreaPass { areaEnterID = 12, areaExitID = 8, distance = 48.400, speedLimit = 120 };
            tollAreaExitList.Add(newexit102);
            AreaPass newexit103 = new AreaPass { areaEnterID = 12, areaExitID = 9, distance = 39.600, speedLimit = 120 };
            tollAreaExitList.Add(newexit103);
            AreaPass newexit104 = new AreaPass { areaEnterID = 12, areaExitID = 10, distance = 26.900, speedLimit = 120 };
            tollAreaExitList.Add(newexit104);
            AreaPass newexit105 = new AreaPass { areaEnterID = 12, areaExitID = 11, distance = 9.800, speedLimit = 120 };
            tollAreaExitList.Add(newexit105);
            AreaPass newexit106 = new AreaPass { areaEnterID = 7, areaExitID = 8, distance = 10.400, speedLimit = 120 };
            tollAreaExitList.Add(newexit106);
            AreaPass newexit107 = new AreaPass { areaEnterID = 7, areaExitID = 9, distance = 20.800, speedLimit = 120 };
            tollAreaExitList.Add(newexit107);
            AreaPass newexit108 = new AreaPass { areaEnterID = 7, areaExitID = 10, distance = 31.100, speedLimit = 120 };
            tollAreaExitList.Add(newexit108);
            AreaPass newexit109 = new AreaPass { areaEnterID = 7, areaExitID = 11, distance = 49.000, speedLimit = 120 };
            tollAreaExitList.Add(newexit109);
            AreaPass newexit110 = new AreaPass { areaEnterID = 7, areaExitID = 12, distance = 58.800, speedLimit = 120 };
            tollAreaExitList.Add(newexit110);
            AreaPass newexit111 = new AreaPass { areaEnterID = 8, areaExitID = 9, distance = 10.800, speedLimit = 120 };
            tollAreaExitList.Add(newexit111);
            AreaPass newexit112 = new AreaPass { areaEnterID = 8, areaExitID = 10, distance = 23.000, speedLimit = 120 };
            tollAreaExitList.Add(newexit112);
            AreaPass newexit113 = new AreaPass { areaEnterID = 8, areaExitID = 11, distance = 39.700, speedLimit = 120 };
            tollAreaExitList.Add(newexit113);
            AreaPass newexit114 = new AreaPass { areaEnterID = 8, areaExitID = 12, distance = 49.500, speedLimit = 120 };
            tollAreaExitList.Add(newexit114);
            AreaPass newexit115 = new AreaPass { areaEnterID = 9, areaExitID = 10, distance = 15.100, speedLimit = 120 };
            tollAreaExitList.Add(newexit115);
            AreaPass newexit116 = new AreaPass { areaEnterID = 9, areaExitID = 11, distance = 31.800, speedLimit = 120 };
            tollAreaExitList.Add(newexit116);
            AreaPass newexit117 = new AreaPass { areaEnterID = 9, areaExitID = 12, distance = 41.600, speedLimit = 120 };
            tollAreaExitList.Add(newexit117);
            AreaPass newexit118 = new AreaPass { areaEnterID = 10, areaExitID = 11, distance = 17.500, speedLimit = 120 };
            tollAreaExitList.Add(newexit118);
            AreaPass newexit119 = new AreaPass { areaEnterID = 10, areaExitID = 12, distance = 27.300, speedLimit = 120 };
            tollAreaExitList.Add(newexit119);
            AreaPass newexit120 = new AreaPass { areaEnterID = 11, areaExitID = 12, distance = 10.100, speedLimit = 120 };
            tollAreaExitList.Add(newexit120);
            AreaPass newexit122 = new AreaPass { areaEnterID = 7, areaExitID = 7.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit122);
            AreaPass newexit123 = new AreaPass { areaEnterID = 7, areaExitID = 7.3, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit123);
            AreaPass newexit124 = new AreaPass { areaEnterID = 7, areaExitID = 8.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit124);
            AreaPass newexit125 = new AreaPass { areaEnterID = 7, areaExitID = 8.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit125);
            AreaPass newexit126 = new AreaPass { areaEnterID = 7, areaExitID = 9.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit126);
            AreaPass newexit127 = new AreaPass { areaEnterID = 7, areaExitID = 9.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit127);
            AreaPass newexit128 = new AreaPass { areaEnterID = 7, areaExitID = 10.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit128);
            AreaPass newexit129 = new AreaPass { areaEnterID = 7, areaExitID = 10.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit129);
            AreaPass newexit130 = new AreaPass { areaEnterID = 7, areaExitID = 11.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit130);
            AreaPass newexit131 = new AreaPass { areaEnterID = 7, areaExitID = 11.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit131);
            AreaPass newexit133 = new AreaPass { areaEnterID = 8, areaExitID = 7.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit133);
            AreaPass newexit134 = new AreaPass { areaEnterID = 8, areaExitID = 7.3, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit134);
            AreaPass newexit135 = new AreaPass { areaEnterID = 8, areaExitID = 8.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit135);
            AreaPass newexit136 = new AreaPass { areaEnterID = 8, areaExitID = 8.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit136);
            AreaPass newexit137 = new AreaPass { areaEnterID = 8, areaExitID = 9.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit137);
            AreaPass newexit138 = new AreaPass { areaEnterID = 8, areaExitID = 9.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit138);
            AreaPass newexit139 = new AreaPass { areaEnterID = 8, areaExitID = 10.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit139);
            AreaPass newexit140 = new AreaPass { areaEnterID = 8, areaExitID = 10.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit140);
            AreaPass newexit141 = new AreaPass { areaEnterID = 8, areaExitID = 11.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit141);
            AreaPass newexit142 = new AreaPass { areaEnterID = 8, areaExitID = 11.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit142);
            AreaPass newexit144 = new AreaPass { areaEnterID = 9, areaExitID = 7.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit144);
            AreaPass newexit145 = new AreaPass { areaEnterID = 9, areaExitID = 7.3, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit145);
            AreaPass newexit146 = new AreaPass { areaEnterID = 9, areaExitID = 8.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit146);
            AreaPass newexit147 = new AreaPass { areaEnterID = 9, areaExitID = 8.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit147);
            AreaPass newexit148 = new AreaPass { areaEnterID = 9, areaExitID = 9.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit148);
            AreaPass newexit149 = new AreaPass { areaEnterID = 9, areaExitID = 9.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit149);
            AreaPass newexit150 = new AreaPass { areaEnterID = 9, areaExitID = 10.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit150);
            AreaPass newexit151 = new AreaPass { areaEnterID = 9, areaExitID = 10.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit151);
            AreaPass newexit152 = new AreaPass { areaEnterID = 9, areaExitID = 11.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit152);
            AreaPass newexit153 = new AreaPass { areaEnterID = 9, areaExitID = 11.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit153);
            AreaPass newexit155 = new AreaPass { areaEnterID = 10, areaExitID = 7.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit155);
            AreaPass newexit156 = new AreaPass { areaEnterID = 10, areaExitID = 7.3, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit156);
            AreaPass newexit157 = new AreaPass { areaEnterID = 10, areaExitID = 8.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit157);
            AreaPass newexit158 = new AreaPass { areaEnterID = 10, areaExitID = 8.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit158);
            AreaPass newexit159 = new AreaPass { areaEnterID = 10, areaExitID = 9.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit159);
            AreaPass newexit160 = new AreaPass { areaEnterID = 10, areaExitID = 9.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit160);
            AreaPass newexit161 = new AreaPass { areaEnterID = 10, areaExitID = 10.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit161);
            AreaPass newexit162 = new AreaPass { areaEnterID = 10, areaExitID = 10.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit162);
            AreaPass newexit163 = new AreaPass { areaEnterID = 10, areaExitID = 11.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit163);
            AreaPass newexit164 = new AreaPass { areaEnterID = 10, areaExitID = 11.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit164);
            AreaPass newexit166 = new AreaPass { areaEnterID = 11, areaExitID = 7.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit166);
            AreaPass newexit167 = new AreaPass { areaEnterID = 11, areaExitID = 7.3, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit167);
            AreaPass newexit168 = new AreaPass { areaEnterID = 11, areaExitID = 8.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit168);
            AreaPass newexit169 = new AreaPass { areaEnterID = 11, areaExitID = 8.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit169);
            AreaPass newexit170 = new AreaPass { areaEnterID = 11, areaExitID = 9.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit170);
            AreaPass newexit171 = new AreaPass { areaEnterID = 11, areaExitID = 9.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit171);
            AreaPass newexit172 = new AreaPass { areaEnterID = 11, areaExitID = 10.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit172);
            AreaPass newexit173 = new AreaPass { areaEnterID = 11, areaExitID = 10.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit173);
            AreaPass newexit174 = new AreaPass { areaEnterID = 11, areaExitID = 11.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit174);
            AreaPass newexit175 = new AreaPass { areaEnterID = 11, areaExitID = 11.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit175);
            AreaPass newexit177 = new AreaPass { areaEnterID = 12, areaExitID = 7.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit177);
            AreaPass newexit178 = new AreaPass { areaEnterID = 12, areaExitID = 7.3, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit178);
            AreaPass newexit179 = new AreaPass { areaEnterID = 12, areaExitID = 8.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit179);
            AreaPass newexit180 = new AreaPass { areaEnterID = 12, areaExitID = 8.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit180);
            AreaPass newexit181 = new AreaPass { areaEnterID = 12, areaExitID = 9.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit181);
            AreaPass newexit182 = new AreaPass { areaEnterID = 12, areaExitID = 9.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit182);
            AreaPass newexit183 = new AreaPass { areaEnterID = 12, areaExitID = 10.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit183);
            AreaPass newexit184 = new AreaPass { areaEnterID = 12, areaExitID = 10.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit184);
            AreaPass newexit185 = new AreaPass { areaEnterID = 12, areaExitID = 11.1, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit185);
            AreaPass newexit186 = new AreaPass { areaEnterID = 12, areaExitID = 11.2, distance = 0, speedLimit = 120 };
            tollAreaExitList.Add(newexit186);

            #endregion

            dBOperations.InsertAll(tollAreaExitList, typeof(AreaPass));

        }


    }
}
