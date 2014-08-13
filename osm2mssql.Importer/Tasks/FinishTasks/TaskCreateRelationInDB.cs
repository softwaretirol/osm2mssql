using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.FinishTasks
{
    public class TaskCreateRelationInDB : TaskBase
    {
        public TaskCreateRelationInDB(string name)
            : base(TaskType.FinishTask, name)
        {
        }

        protected override async Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            var sql = @"TRUNCATE TABLE Relation";
            ExecuteSqlCmd(sql);

            sql = @"INSERT INTO Relation(id, geo, role)
                        SELECT rel.RelationId, dbo.GeographyUnion(x.geo) as geo, rel.role FROM RelationCreation rel LEFT JOIN
                        (SELECT Id, Location as geo, (SELECT id FROM MemberType Where Name = 'node') as [Type] FROM Node
                        UNION ALL
                        SELECT Id, Line as geo, (SELECT id FROM MemberType Where Name = 'way') as [Type] FROM Way) x
                        on rel.ref = x.Id and rel.type = x.Type
                        group by RelationId, rel.role";
            ExecuteSqlCmd(sql);

            sql = @"UPDATE Relation set geo = dbo.ConvertToPolygon(geo)
                    where Relation.id IN (SELECT RelationId FROM RelationTag tag where tag.Info = 'multipolygon')";
            ExecuteSqlCmd(sql);
        }
    }
}
