using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osm2mssql.Importer.Enums
{
    public enum ConnectionResult
    {
        Unknown,
        Successful,
        Error,
        DbAlreadyExists
    }
}
