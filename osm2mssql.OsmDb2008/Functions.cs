using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;

namespace osm2mssql.DbExtensions
{
    public class Functions
    {
        [Microsoft.SqlServer.Server.SqlFunction]
        public static SqlGeography TryCreateGeography(SqlString s, SqlInt32 srid)
        {
            try
            {
                return SqlGeography.STGeomFromText(new SqlChars(s), srid.Value);
            }
            catch
            {
                return null;
            }
        }

        internal const long MaxAreaSize = 400000000000000;
        [Microsoft.SqlServer.Server.SqlFunction]
        public static bool IsValid(SqlGeography geo)
        {
            try
            {
                if (geo == null)
                    return false;
                if (geo.STArea() > MaxAreaSize)
                    return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        [Microsoft.SqlServer.Server.SqlFunction]
        public static SqlGeography TryMakeValid(SqlGeography geo)
        {
            try
            {
                if (IsValid(geo))
                    return geo;
                var validGeo = MakeValid(geo);
                if (IsValid(validGeo))
                    return validGeo;
            }
            catch (Exception)
            {
            }
            
            return null;
        }

        private static SqlGeography MakeValid(SqlGeography geo)
        {
            if (geo == null)
                return null;

            var geometry = SqlGeometry.STGeomFromText(geo.STAsText(), 0).MakeValid();
            return SqlGeography.STGeomFromText(geometry.STAsText(), 4326);
        }


        [Microsoft.SqlServer.Server.SqlFunction]
        public static SqlGeography ConvertToPolygon(SqlGeography geography)
        {
            if (geography == null || geography == SqlGeography.Null)
                return geography;

            var pointCount = geography.STNumPoints();
            List<SortedPoint> points = new List<SortedPoint>();
            for (int i = 1; i < pointCount + 1; i++)
            {
                SqlGeography point = geography.STPointN(i);
                points.Add(new SortedPoint(i, point.Long.Value, point.Lat.Value));
            }

            try
            {
                if (points.Count > 1)
                {
                    if (!points[0].Equals(points[points.Count - 1]))
                        points.Add(points[0]);

                    geography = TryMakeValid(TryCreatePolygon(points));
                    if (geography == null)
                    {
                        points.Reverse();
                        geography = TryMakeValid(TryCreatePolygon(points));
                    }
                }
            }
            catch
            {
            }

            return geography;
        }

        private static SqlGeography TryCreatePolygon(List<SortedPoint> points)
        {
            var builder = new SqlGeographyBuilder();
            builder.SetSrid(4326);
            builder.BeginGeography(OpenGisGeographyType.Polygon);
            builder.BeginFigure(points[0].Latitude, points[0].Longitude);
            for (var i = 1; i != points.Count; ++i)
            {
                builder.AddLine(points[i].Latitude, points[i].Longitude);
            }
            builder.EndFigure();
            builder.EndGeography();
            return builder.ConstructedGeography;
        }
    }
}
