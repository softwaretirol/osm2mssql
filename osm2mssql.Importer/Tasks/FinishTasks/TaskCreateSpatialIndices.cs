using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.FinishTasks
{
    public class TaskCreateSpatialIndices : TaskBase
    {
        public TaskCreateSpatialIndices(string name) : base(TaskType.FinishTask, name)
        {
        }

        protected override Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            var task1 = Task.Factory.StartNew(() => ExecuteSqlCmd("CREATE SPATIAL INDEX idx ON Way(line) USING GEOGRAPHY_AUTO_GRID"));
            var task2 = Task.Factory.StartNew(() => ExecuteSqlCmd("CREATE SPATIAL INDEX idx ON Node(location) USING GEOGRAPHY_AUTO_GRID"));

            return Task.WhenAll(new[] { task1, task2 });
        }
    }
}
