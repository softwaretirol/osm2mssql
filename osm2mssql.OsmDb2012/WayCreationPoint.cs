using System;

namespace osm2mssql.DbExtensions
{
    [Serializable]
    public class WayCreationPoint 
    {
        public int Sort { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public WayCreationPoint(int sort, double longitude, double latitude)
        {
            Sort = sort;
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}