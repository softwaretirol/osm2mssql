using System.Collections.Generic;
using System.Linq;

namespace osm2mssql.Importer.OpenStreetMapTypes
{
    public struct Node 
    {
        public long NodeId
        {
            get { return _nodeId; }
        }

        public double Latitude
        {
            get { return _latitude; }
        }

        public double Longitude
        {
            get { return _longitude; }
        }

        public List<Tag> Tags
        {
            get { return _tags; }
        }

        private readonly long _nodeId;
        private readonly double _latitude;
        private readonly double _longitude;
        private readonly List<Tag> _tags;
         
        public Node(long nodeId, double latitude, double longitude, IEnumerable<Tag> tags)
        {
            _nodeId = nodeId;
            _latitude = latitude;
            _longitude = longitude;
            _tags = tags as List<Tag> ?? tags.ToList();
        }
    }
}
