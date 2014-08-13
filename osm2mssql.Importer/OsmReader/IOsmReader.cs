using System.Collections.Generic;
using osm2mssql.Importer.OpenStreetMapTypes;

namespace osm2mssql.Importer.OsmReader
{
    public interface IOsmReader
    {
        IEnumerable<Node> ReadNodes(string fileName, AttributeRegistry attributeRegistry);
        IEnumerable<Relation> ReadRelations(string fileName, AttributeRegistry attributeRegistry);
        IEnumerable<Way> ReadWays(string fileName, AttributeRegistry attributeRegistry);
    }
}
