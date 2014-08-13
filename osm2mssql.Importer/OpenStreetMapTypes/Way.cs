using System.Collections.Generic;

namespace osm2mssql.Importer.OpenStreetMapTypes
{
    public class Way
    {
        public long WayId;
        public List<Tag> Tags { get; set; }
        public List<long> NodeRefs { get; set; }
        public Way()
        {
            Tags = new List<Tag>();
            NodeRefs = new List<long>();
        }
    }
}
