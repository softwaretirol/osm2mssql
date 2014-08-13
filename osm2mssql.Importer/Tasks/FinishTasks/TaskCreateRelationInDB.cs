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
            var sql = @"TRUNCATE TABLE tRelation";
            ExecuteSqlCmd(sql);

            sql = @"INSERT INTO tRelation(id, geo, role)
                        SELECT rel.RelationId, dbo.GeographyUnion(x.geo) as geo, rel.role FROM tRelationCreation rel LEFT JOIN
                        (SELECT Id, Location as geo, (SELECT id FROM tMemberType Where Name = 'node') as [Type] FROM tNode
                        UNION ALL
                        SELECT Id, Line as geo, (SELECT id FROM tMemberType Where Name = 'way') as [Type] FROM tWay) x
                        on rel.ref = x.Id and rel.type = x.Type
                        group by RelationId, rel.role";
            ExecuteSqlCmd(sql);

            sql = @"UPDATE tRelation set geo = dbo.ConvertToPolygon(geo)
                    where tRelation.id IN (SELECT RelationId FROM tRelationTag tag where tag.Info = 'multipolygon')";
            ExecuteSqlCmd(sql);
        }
    }
}
