using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
// Referencing C:\Program Files (x86)\Microsoft SQL Server\110\SDK\Assemblies\Microsoft.SqlServer.Types.dll


namespace osm2mssql.DbExtensions
{
    [Serializable]
    [SqlUserDefinedAggregate(Format.UserDefined, IsInvariantToNulls = true, MaxByteSize = -1)]
    public struct LineStringBuilder : IBinarySerialize
    {
        private List<WayCreationPoint> _points;

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public void Init()
        {
            _points = new List<WayCreationPoint>();
        }

        [SqlMethod(OnNullCall = false, IsDeterministic = true, IsPrecise = true)]
        public void Accumulate(SqlDouble latitute, SqlDouble longitude, SqlInt32 sort)
        {
            if (_points == null)
                _points = new List<WayCreationPoint>();
            _points.Add(new WayCreationPoint(sort.Value, latitute.Value, longitude.Value));
        }

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public void Merge(LineStringBuilder group)
        {
            if (_points != null)
            {
                _points.AddRange(group._points);
            }
            else
            {
                _points = new List<WayCreationPoint>(group._points);
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
            if (_points.Count == 0)
                return null; 

            if (_points.Count == 1)
            {
                //Only one Point - Create a POINT for it
                return CreateSinglePoint();
            }

            _points = _points.OrderBy(x => x.Sort).ToList();

            var res = CreateGeographyLine();
            if (!Functions.IsValid(res))
            {
                _points.RemoveAt(_points.Count - 1); //Remove Last Point - try again
                return TryMakeLine();
            }
            return res;
        }

        private SqlGeography CreateGeographyLine()
        {
            var builder = new SqlGeographyBuilder();

            builder.SetSrid(4326);
            builder.BeginGeography(OpenGisGeographyType.LineString);
            builder.BeginFigure(_points[0].Latitude, _points[0].Longitude);
            for (var i = 1; i != _points.Count; ++i)
            {
                builder.AddLine(_points[i].Latitude, _points[i].Longitude);
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
            builder.BeginFigure(_points[0].Latitude, _points[0].Longitude);
            builder.EndFigure();
            builder.EndGeography();
            return builder.ConstructedGeography;
        }

        public void Read(System.IO.BinaryReader r)
        {
            _points = new List<WayCreationPoint>();
            while (r.BaseStream.Position < r.BaseStream.Length)
            {
                var sort = r.ReadInt32();
                var latitude = r.ReadDouble();
                var longitude = r.ReadDouble();
                _points.Add(new WayCreationPoint(sort, latitude, longitude));
            }
        }

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public void Write(System.IO.BinaryWriter w)
        {
            foreach (var point in _points)
            {
                w.Write(point.Sort);
                w.Write(point.Latitude);
                w.Write(point.Longitude);
            }
        }
    }
}
