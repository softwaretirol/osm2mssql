using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osm2mssql.InfoDAL
{
    public class CityInformation
    {
        /// <summary>
        /// ID of relation in tRelation
        /// </summary>
        public long RelationId { get; set; }
        /// <summary>
        /// Cityname
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Postalcode of City
        /// </summary>
        public string PostalCode { get; set; }
        /// <summary>
        /// Place-Tag: http://wiki.openstreetmap.org/wiki/Key:place
        /// </summary>
        public string Place { get; set; }
        /// <summary>
        /// AdminLevel-Tag: http://wiki.openstreetmap.org/wiki/Key:admin_level#admin_level
        /// </summary>
        public int? AdminLevel { get; set; }
        public double? Distance { get; set; }
        public OsmPoint Location { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
