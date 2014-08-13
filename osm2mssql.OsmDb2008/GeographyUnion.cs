using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osm2mssql.DbExtensions
{
    [Serializable]
    [SqlUserDefinedAggregate(Format.UserDefined, IsInvariantToNulls = true, MaxByteSize = -1)]
    public struct GeographyUnion : IBinarySerialize
    {
        private SqlGeography _geography;

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public void Init()
        {
        }

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public void Accumulate(SqlGeography geography)
        {
            if (geography == null || geography.IsNull)
                return; //Ignore NULL Values

            if (_geography == null)
                _geography = geography;
            else
            {
                _geography = _geography.STUnion(geography);
            }
        }

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public void Merge(GeographyUnion geography)
        {
            Accumulate(geography._geography);
        }

        [SqlMethod(IsDeterministic = true, IsPrecise = true)]
        public SqlGeography Terminate()
        {
            return _geography;
        }

        public void Read(System.IO.BinaryReader r)
        {
            if (r.BaseStream.Length > 0)
            {
                _geography = new SqlGeography();
                _geography.Read(r);
            }
        }

        public void Write(System.IO.BinaryWriter w)
        {
            if (_geography != null)
                _geography.Write(w);
        }
    }
}
