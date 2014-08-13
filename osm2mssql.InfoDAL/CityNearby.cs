using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osm2mssql.InfoDAL
{
    public class CityNearby
    {
        public CityInformation Nearest { get; set; }
        public CityInformation NearestTown { get; set; }
        public CityInformation NearestCity { get; set; }
    }
}
