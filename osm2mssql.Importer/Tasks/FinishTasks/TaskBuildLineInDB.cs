using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.FinishTasks
{
    public class TaskCreateLineInDB : TaskBase
    {
        public TaskCreateLineInDB(string name) : base(TaskType.FinishTask, name)
        {
        }

        protected override async Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            ExecuteSqlCmd("INSERT INTO dbo.Way(Id, line) " +
                          "SELECT wayId, dbo.CreateLineString(Latitude, Longitude, sort) " +
                          "from WayCreation INNER JOIN " +
                          "Node on Node.Id = WayCreation.nodeId group by wayId");
        }
    }
}
