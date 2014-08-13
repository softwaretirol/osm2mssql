using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
// Referencing C:\Program Files (x86)\Microsoft SQL Server\110\SDK\Assemblies\Microsoft.SqlServer.Types.dll


namespace osm2mssql.DbExtensions
{
    [Serializable]
    public class SortedPoint 
    {
        public int Sort { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public SortedPoint(int sort, double longitude, double latitude)
        {
            Sort = sort;
            Longitude = longitude;
            Latitude = latitude;
        }

        public override bool Equals(object obj)
        {
            var point = obj as SortedPoint;
            if (point != null)
            {
                return point.Latitude == Latitude &&
                       point.Longitude == Longitude;
            }
            return base.Equals(obj);
        }
    }

    [Serializable]
    [SqlUserDefinedAggregate(Format.UserDefined, IsInvariantToNulls = true, MaxByteSize = -1)]
    public struct LineStringBuilder : IBinarySerialize
    {

        private List<SortedPoint> points;

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public void Init()
        {
            points = new List<SortedPoint>();
        }

        [SqlMethod(OnNullCall = false, IsDeterministic = true, IsPrecise = true)]
        public void Accumulate(SqlDouble Lat, SqlDouble Lon, SqlInt32 sort)
        {
            if (points == null)
                points = new List<SortedPoint>();
            points.Add(new SortedPoint(sort.Value, Lat.Value, Lon.Value));
        }

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public void Merge(LineStringBuilder Group)
        {
            if (this.points != null)
            {
                this.points.AddRange(Group.points);
            }
            else
            {
                this.points = new List<SortedPoint>(Group.points);
            }
        }

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public SqlGeography Terminate()
        {
            try
            {
                return TryMakeLine();
            }
            catch
            {
                return null;
            }
        }

        private SqlGeography TryMakeLine()
        {
            if (points.Count == 0)
                return null; //No Date
            if (points.Count < 2)
            {
                //Only one Point - Create a POINT for it
                return CreateSinglePoint();
            }
            else
            {
                points.Sort(CompareSortedPoint);

                var res = CreateGeographyLine();
                if (!Functions.IsValid(res))
                {
                    points.RemoveAt(points.Count - 1); //Remove Last Point - try again
                    return TryMakeLine();
                }
                return res;
            }
        }

        private SqlGeography CreateGeographyLine()
        {
            var builder = new SqlGeographyBuilder();

            builder.SetSrid(4326);
            builder.BeginGeography(OpenGisGeographyType.LineString);
            builder.BeginFigure(points[0].Latitude, points[0].Longitude);
            for (var i = 1; i != points.Count; ++i)
            {
                builder.AddLine(points[i].Latitude, points[i].Longitude);
            }
            builder.EndFigure();
            builder.EndGeography();

            var res = builder.ConstructedGeography;
            return res;
        }

        private SqlGeography CreateSinglePoint()
        {
            var builder = new SqlGeographyBuilder();
            builder.SetSrid(4326);
            builder.BeginGeography(OpenGisGeographyType.Point);
            builder.BeginFigure(points[0].Latitude, points[0].Longitude);
            builder.EndFigure();
            builder.EndGeography();
            return builder.ConstructedGeography;
        }

        int CompareSortedPoint(SortedPoint p1, SortedPoint p2)
        {
            return Comparer<int>.Default.Compare(p1.Sort, p2.Sort);
        }

        public void Read(System.IO.BinaryReader r)
        {
            points = new List<SortedPoint>();
            try
            {
                while (true)
                {
                    if (r.BaseStream.Position == r.BaseStream.Length)
                        break;
                    var length = r.BaseStream.Length - r.BaseStream.Position;
                    if (length < sizeof(Int32))
                        break;
                    var sort = r.ReadInt32();
                    if (length < sizeof(double))
                        break;
                    var latitude = r.ReadDouble();
                    if (length < sizeof(double))
                        break;
                    var longitude = r.ReadDouble();
                    points.Add(new SortedPoint(sort, latitude, longitude));

                }
            }
            catch
            {

            }
        }

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public void Write(System.IO.BinaryWriter w)
        {
            try
            {
                foreach (var point in points)
                {
                    w.Write(point.Sort);
                    w.Write(point.Latitude);
                    w.Write(point.Longitude);
                }
            }
            catch
            {

            }
        }
    }
}
