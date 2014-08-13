using System.Collections.Generic;

namespace osm2mssql.Importer.OpenStreetMapTypes
{
    public class Relation 
    {
        public class Member
        {
            public int Type;
            public int Role;
            public long Ref;
        }

        public long RelationId;
        public List<Tag> Tags { get; set; }
        public List<Member> Members { get; set; }
        public Relation()
        {
            Tags = new List<Tag>();
            Members = new List<Member>();
        }
    }
}
