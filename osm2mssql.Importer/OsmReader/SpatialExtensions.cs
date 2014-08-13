using System.Data.SqlTypes;
using System.Globalization;
using Microsoft.SqlServer.Types;
using osm2mssql.Importer.OpenStreetMapTypes;

namespace osm2mssql.Importer.OsmReader
{
    public static class SpatialExtensions
    {
        public static SqlGeography ToSqlGeographyPoint(this Node node)
        {
            var point = string.Format(CultureInfo.InvariantCulture, "POINT({1} {0})", node.Latitude, node.Longitude);
            return SqlGeography.STPointFromText(new SqlChars(point), 4326);
        }
    }
}
