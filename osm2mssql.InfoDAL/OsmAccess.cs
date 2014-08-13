using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osm2mssql.InfoDAL
{
    public class OsmAccess : IDisposable
    {
        private SqlConnection _conn;
        private SqlCommand _command;

        public OsmAccess(string connectionString)
        {
            _conn = new SqlConnection(connectionString);
        }

        /// <summary>
        /// Determines the exact city of a specified OsmPoint and all administrative Borders around it
        /// (e.g.: Town: Schäftlarn (Level 8) and Regierungsbezirk Oberbayern (Level 5)
        /// http://wiki.openstreetmap.org/wiki/Key:admin_level#admin_level</param>
        /// </summary>
        /// <param name="point">A geopoint with Latitude/Longitude</param>
        /// <param name="adminLevel">If you want to specifie a adminlevel</param>
        /// <returns>List of cities - take care of the admin level</returns>
        public List<CityInformation> GetCityInformation(OsmPoint point, int? adminLevel = null)
        {
            OpenConnection();

            var str = @"SELECT RelationId,Name,PostalCode,Place, AdminLevel
                        FROM info.AdminLevels WITH(nolock, index(idx_AdminLevelsSpatial)) 
                        WHERE geo.STIntersects(geography::STPointFromText('{0}',4326)) = 1";
            if (adminLevel.HasValue)
                str += "and AdminLevel = " + adminLevel.Value;
            else
                str += " and  AdminLevel between 4 and 10";

            _command.CommandText = string.Format(str, GeneratePointString(point));
            using (var reader = _command.ExecuteReader())
            {
                var res = new List<CityInformation>();
                while(reader.Read())
                {
                    var info = new CityInformation
                    {
                        RelationId = reader.GetInt64(0),
                        Name = reader[1] as string,
                        PostalCode = reader[2] as string,
                        Place = reader[3] as string,
                        AdminLevel = reader.GetInt32(4)
                    };
                    res.Add(info);
                }
                return res;
            }
        }

        public RoadInformation GetRoadInformation(OsmPoint point)
        {
            OpenConnection();
            var str = @"SELECT TOP(1) Id, HighWayType, Name, MaxSpeed from Info.Roads WITH(INDEX(idxInfoRoad))
                        WHERE street.STDistance(geography::STPointFromText('{0}',4326))  < 100
                        order by street.STDistance(geography::STPointFromText('{0}',4326)) ";

            _command.CommandText = string.Format(str, GeneratePointString(point));
            using (var reader = _command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var info = new RoadInformation
                    {
                        WayId = reader.GetInt64(0),
                        HighWayType = reader[1] as string,
                        Name = reader[2] as string,
                        MaxSpeed = reader[3] as string
                    };
                    return info;
                }
                return null;
            }
        }

        public CityNearby GetCitiesNearby(OsmPoint point)
        {
            return new CityNearby
            {
                Nearest = ReadCity(point),
                NearestTown = ReadCity(point, "town"),
                NearestCity = ReadCity(point, "city")
            };
        }

        private CityInformation ReadCity(OsmPoint point, string placeTag = null)
        {
            OpenConnection();
            var str = @"SELECT TOP(1) Id, Name, Place, Latitude, Longitude, location.STDistance(geography::STPointFromText('POINT(12.100067 47.772099)',4326))  as Distance
                        from Info.Cities WITH(INDEX(idxCities)) 
                        WHERE location.STDistance(geography::STPointFromText('{0}',4326)) < 100000 {1} 
                        order by location.STDistance(geography::STPointFromText('{0}',4326)) ";
            if (!string.IsNullOrEmpty(placeTag))
                placeTag = "and Place = '" + placeTag + "'";
            _command.CommandText = string.Format(str, GeneratePointString(point), placeTag);
            using (var reader = _command.ExecuteReader())
            {
                if (reader.Read())
                {
                    var info = new CityInformation
                    {
                        RelationId = reader.GetInt64(0),
                        Name = reader[1] as string,
                        Place = reader[2] as string,
                        Location = new OsmPoint{
                            Latitude = (double)reader[3],
                            Longitude = (double)reader[4]
                        },
                        Distance = (double)reader[5]
                    };
                    return info;
                }
                return null;
            }
        }

        private static string GeneratePointString(OsmPoint p)
        {
            return string.Format(CultureInfo.InvariantCulture, "POINT({0} {1})", p.Longitude, p.Latitude);
        }

        private void OpenConnection()
        {
            if (_conn.State == System.Data.ConnectionState.Closed)
            {
                _conn.Open();
                _command = _conn.CreateCommand();
                _command.Prepare();
            }
        }

        public void Dispose()
        {
            if (_conn != null)
            {
                _conn.Dispose();
                _conn = null;
            }
        }
    }
}
