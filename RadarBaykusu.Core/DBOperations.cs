using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLitePCL;
using RadarBaykusu.Core.Model;
using RadarBaykusu.Core.HelperClasses;
using System.Collections;
using SQLite;

namespace RadarBaykusu.Core
{
    public class DBOperations
    {
        public string DatabasePath { get; set; }

        public string DatabaseLocation { get; set; }

        public string DatabaseName { get; set; }

        public SQLite.SQLiteConnection SQLConnection { get; set; }

        public DBOperations(string dbLocation)
        {
            DatabaseName = "radarbaykusu.db";
            DatabaseLocation = dbLocation;
            DatabasePath = System.IO.Path.Combine(DatabaseLocation, "radarbaykusu.db");
        }

        public PergaResponse<List<Configuration>> GetConfigurationList()
        {
            var returnResult = new PergaResponse<List<Configuration>>();
            var configList = new List<Configuration>();

            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    configList = SQLConnection.Query<Configuration>("SELECT * FROM Configuration").ToList();
                }

                returnResult.Result = true;
                returnResult.ReturnObject = configList;
            }
            catch (Exception ex)
            {
                returnResult.Result = false;
                returnResult.Message = ex.ToString();
                returnResult.ReturnObject = configList;
            }

            return returnResult;
        }

        public PergaResponse<Configuration> GetConfiguration(string paramName)
        {
            var returnResult = new PergaResponse<Configuration>();
            var configuration = new Configuration();

            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    configuration = SQLConnection.Query<Configuration>("SELECT * FROM Configuration where ParamName = '" + paramName + "'").FirstOrDefault();
                }

                returnResult.Result = true;
                returnResult.ReturnObject = configuration;
            }
            catch (Exception ex)
            {
                returnResult.Result = false;
                returnResult.Message = ex.ToString();
                returnResult.ReturnObject = configuration;
            }

            return returnResult;
        }

        public PergaResponse<Configuration> GetConfiguration(List<Configuration> configurationList, string paramName)
        {
            var returnResult = new PergaResponse<Configuration>();
            var configuration = new Configuration();

            try
            {
                configuration = configurationList.Where(x => x.ParamName == paramName).FirstOrDefault();

                returnResult.Result = true;
                returnResult.ReturnObject = configuration;
            }
            catch (Exception ex)
            {
                returnResult.Result = false;
                returnResult.Message = ex.ToString();
                returnResult.ReturnObject = configuration;
            }

            return returnResult;
        }

        public PergaResponse<List<Area>> GetAreaList()
        {
            var returnResult = new PergaResponse<List<Area>>();
            var areaList = new List<Area>();

            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    areaList = SQLConnection.Query<Area>("SELECT * FROM Area").ToList();
                }

                returnResult.Result = true;
                returnResult.ReturnObject = areaList;
            }
            catch (Exception ex)
            {
                returnResult.Result = false;
                returnResult.Message = ex.ToString();
                returnResult.ReturnObject = areaList;
            }

            return returnResult;
        }

        public PergaResponse<List<Area>> GetAreaListByRoad(int roadID)
        {
            var returnResult = new PergaResponse<List<Area>>();
            var areaList = new List<Area>();

            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    areaList = SQLConnection.Query<Area>("SELECT * FROM Area where RoadID = " + roadID).ToList();
                }

                returnResult.Result = true;
                returnResult.ReturnObject = areaList;
            }
            catch (Exception ex)
            {
                returnResult.Result = false;
                returnResult.Message = ex.ToString();
                returnResult.ReturnObject = areaList;
            }

            return returnResult;
        }

        public PergaResponse<List<AreaWithDistance>> GetAreaPassListWithDisctance(double areaID)
        {
            var returnResult = new PergaResponse<List<AreaWithDistance>>();
            var areaPassList = new List<AreaWithDistance>();

            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    areaPassList = SQLConnection.Query<AreaWithDistance>("select TA.areaID, TA.areaName, TA.areaLatitude, TA.isBooth, TA.areaLongitude, TE.distance from AreaPass TE inner join Area TA on TA.areaID = TE.areaExitID where TE.areaEnterID = " + areaID).ToList();
                }

                //burda gateler ve cıkıslar beraber getırılıyor. Yapılması gereken tanımlarda gatelerın cıkartılması (her seferınde yenıden tanımlanmalarına gerek olmadıgından) ve gateler ıcın ayrı select yapılarak sonuca unıon yapılması

                returnResult.Result = true;
                returnResult.ReturnObject = areaPassList;
            }
            catch (Exception ex)
            {
                returnResult.Result = false;
                returnResult.Message = ex.ToString();
                returnResult.ReturnObject = areaPassList;
            }

            return returnResult;
        }

        public PergaResponse<List<AreaPass>> GetAreaPassList()
        {
            var returnResult = new PergaResponse<List<AreaPass>>();
            var areaPassList = new List<AreaPass>();

            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    areaPassList = SQLConnection.Query<AreaPass>("SELECT * FROM AreaPass").ToList();
                }

                returnResult.Result = true;
                returnResult.ReturnObject = areaPassList;
            }
            catch (Exception ex)
            {
                returnResult.Result = false;
                returnResult.Message = ex.ToString();
                returnResult.ReturnObject = areaPassList;
            }

            return returnResult;
        }
        
        public PergaResponse<List<AreaPass>> GetAreaPassListByEntryID(double enteryID)
        {
            var returnResult = new PergaResponse<List<AreaPass>>();
            var areaPassList = new List<AreaPass>();

            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    areaPassList = SQLConnection.Query<AreaPass>("SELECT * FROM AreaPass where AreaEntryID = " + enteryID).ToList();
                }

                returnResult.Result = true;
                returnResult.ReturnObject = areaPassList;
            }
            catch (Exception ex)
            {
                returnResult.Result = false;
                returnResult.Message = ex.ToString();
                returnResult.ReturnObject = areaPassList;
            }

            return returnResult;
        }

        public PergaResponse<List<CheckTable>> GetTableList()
        {
            var returnResult = new PergaResponse<List<CheckTable>>();
            var tableList = new List<CheckTable>();

            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    tableList = SQLConnection.Query<CheckTable>("SELECT name FROM sqlite_master WHERE type = 'table' AND name in ('Area' , 'AreaPass', 'Configuration', 'Road')").ToList();
                }

                returnResult.Result = true;
                returnResult.ReturnObject = tableList;
            }
            catch (Exception ex)
            {
                returnResult.Result = false;
                returnResult.Message = ex.ToString();
                returnResult.ReturnObject = tableList;
            }

            return returnResult;
        }
        public bool CreateTable(Type objectType)
        {
            bool returnResult = false;
            int executionResult = 0;
            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    executionResult = SQLConnection.CreateTable(objectType);
                }

                if (executionResult == 1)
                    returnResult = true;
            }
            catch (Exception)
            {
                returnResult = false;
            }

            return returnResult;
        }

        public bool InsertAll(IEnumerable objects, Type objectType)
        {
            bool returnResult = false;
            var insertResult = 0;

            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    insertResult = SQLConnection.InsertAll(objects, objectType);
                }

                returnResult = true;
            }
            catch (Exception)
            {
                returnResult = false;
            }

            return returnResult;
        }

        public bool Update(object objectToUpdate, Type objectType)
        {
            bool returnResult = false;
            var updateResult = 0;
            try
            {
                using (SQLConnection = new SQLiteConnection(DatabasePath))
                {
                    updateResult = SQLConnection.Update(objectToUpdate, objectType);
                }
                returnResult = true;
            }
            catch (Exception)
            {
                returnResult = false;
            }

            return returnResult;
        }
    }
}

