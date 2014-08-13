using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace osm2mssql.InfoDAL
{
    public class RoadInformation
    {
        /// <summary>
        /// ID of way in tWay
        /// </summary>
        public long WayId { get; set; }
        /// <summary>
        /// Name of the way
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Max. allowed Speed - http://wiki.openstreetmap.org/wiki/Key:maxspeed
        /// </summary>
        public string MaxSpeed { get; set; }
        /// <summary>
        /// Type of road - http://wiki.openstreetmap.org/wiki/Key:highway
        /// </summary>
        public string HighWayType { get; set; }
    }
}
