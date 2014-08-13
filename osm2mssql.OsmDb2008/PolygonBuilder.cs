using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;

namespace osm2mssql.DbExtensions
{
    [Serializable]
    [SqlUserDefinedAggregate(Format.UserDefined, IsInvariantToNulls = true, MaxByteSize = -1)]
    public struct PolygonBuilder : IBinarySerialize
    {
        private long _relationId;
        public struct SortedPoint
        {
            public int Sort;
            public double Longitude;
            public double Latitude;

            public SortedPoint(int sort, double longitude, double latitude)
            {
                Sort = sort;
                Longitude = longitude;
                Latitude = latitude;
            }

            public static int Compare(SortedPoint x, SortedPoint y)
            {
                return x.Sort.CompareTo(y.Sort);
            }
        }

        private List<SortedPoint> points;
        public void Init()
        {
            points = new List<SortedPoint>();
        }

        [SqlMethod(OnNullCall = false)]
        public void Accumulate(SqlDouble Lat, SqlDouble Lon, SqlInt32 sort)
        {
            if (points == null)
                points = new List<SortedPoint>();

            points.Add(new SortedPoint(sort.Value, Lat.Value, Lon.Value));
        }

        public void Merge(PolygonBuilder Group)
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

        public SqlGeography Terminate()
        {
            try
            {
                if (points.Count < 2)
                    return null;

                points.Sort(SortedPoint.Compare);
                var res = CreatePolygon(points);
                return res;
            }
            catch (Exception exception)
            {
                Trace.TraceError(exception.ToString());
                return null;
            }
        }

        private SqlGeography CreatePolygon(List<SortedPoint> points)
        {
            var polygons = CreatePolygons(points);
            var geographyPolygons = CreateGeography(polygons);
            return geographyPolygons;
        }

        private SqlGeography CreateGeography(List<Polygon> polygons)
        {
            List<SqlGeography> geographies = new List<SqlGeography>();
            foreach (var poly in polygons)
            {
                try
                {
                    if (poly.Points.Count < 2)
                        continue;

                    if (poly.Points[0].Latitude != poly.Points[poly.Points.Count - 1].Latitude ||
                       poly.Points[0].Longitude != poly.Points[poly.Points.Count - 1].Longitude)
                        continue;

                    var res = TryMakePolygon(poly.Points);
                    if (!Functions.IsValid(res))
                        poly.ReversePoints();
                    res = TryMakePolygon(poly.Points);
                    if (res != null && !Functions.IsValid(res))
                        res = Functions.TryMakeValid(res);
                    geographies.Add(res);
                }
                catch
                {

                }
            }

            var total = geographies.FirstOrDefault();
            if (total == null)
                return null; //Failed to build one...

            foreach (var geo in geographies.Skip(1).Where(x => x != null))
            {
                total = total.STUnion(geo);
            }
            return total;
        }

        private SqlGeography TryMakePolygon(IList<SortedPoint> list)
        {
            try
            {
                var builder = new SqlGeographyBuilder();
                builder.SetSrid(4326);
                builder.BeginGeography(OpenGisGeographyType.Polygon);
                builder.BeginFigure(list[0].Latitude, list[0].Longitude);
                for (var i = 1; i != list.Count; ++i)
                {
                    builder.AddLine(list[i].Latitude, list[i].Longitude);
                }
                builder.EndFigure();
                builder.EndGeography();
                return builder.ConstructedGeography;
            }
            catch
            {
                return null;
            }
        }

        private List<Polygon> CreatePolygons(List<SortedPoint> points)
        {
            Polygon p = new Polygon();
            var polygons = new List<Polygon>();
            foreach (var point in points)
            {
                if (p.AddPoint(point))
                {
                    //Polygon is now closed --> Add it to the collection and start new one..
                    polygons.Add(p);
                    p = new Polygon();
                    p.AddPoint(point);
                }
            }

            if (!p.IsClosed())
            {
                p.ClosePolygon();
            }
            polygons.Add(p);
            return polygons;
        }

        public class Polygon
        {
            public IList<SortedPoint> Points { get { return _points; } }
            private List<SortedPoint> _points = new List<SortedPoint>();
            private SortedPoint? _startPoint;
            internal bool AddPoint(SortedPoint point)
            {
                if (_startPoint == null)
                {
                    _startPoint = point;
                    _points.Add(point);
                    return false;
                }
                else
                {
                    _points.Add(point);
                    if (IsClosed())
                        return true;
                    return false;
                }
            }

            internal bool IsClosed()
            {
                if (Points.Count > 1 &&
                    Points[Points.Count - 1].Latitude == _startPoint.Value.Latitude &&
                    Points[Points.Count - 1].Longitude == _startPoint.Value.Longitude)
                    return true;

                return false;
            }

            internal bool ClosePolygon()
            {
                if (!IsClosed() && Points.Count > 1)
                {
                    _points.Add(new SortedPoint { Latitude = _startPoint.Value.Latitude, Longitude = _startPoint.Value.Longitude, Sort = int.MaxValue });
                    return true;
                }
                return false;
            }

            internal void ReversePoints()
            {
                _points.Reverse();
            }
        }

        //private static SqlGeography CreatePolygon()
        //{
        //    try
        //    {
        //        var res = CreateGeography(nodes);
        //        if (Functions.IsValid(res))
        //            return res;
        //        res = Functions.TryMakeValid(res);
        //        if (Functions.IsValid(res))
        //            return res;

        //        Array.Reverse(nodes);
        //        res = CreateGeography(nodes);
        //        if (Functions.IsValid(res))
        //            return res;
        //        res = Functions.TryMakeValid(res);
        //        if (Functions.IsValid(res))
        //            return res;
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    return null;
        //}

        public void Read(System.IO.BinaryReader r)
        {
            points = new List<SortedPoint>();
            _relationId = r.ReadInt64();
            while (true)
            {
                if (r.BaseStream.Position == r.BaseStream.Length)
                    break;
                var length = r.BaseStream.Length - r.BaseStream.Position;
                if (length < sizeof(int) + 2 * sizeof(double))
                    break;
                var sort = r.ReadInt32();
                var latitude = r.ReadDouble();
                var longitude = r.ReadDouble();
                points.Add(new SortedPoint(sort, latitude, longitude));
            }
        }
        public void Write(System.IO.BinaryWriter w)
        {
            w.Write(_relationId);
            foreach (var point in points)
            {
                w.Write(point.Sort);
                w.Write(point.Latitude);
                w.Write(point.Longitude);
            }
        }
    }
}
