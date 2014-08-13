using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.InitializeTasks
{
    class TaskInstallDbExtension : TaskBase
    {
        public TaskInstallDbExtension(string name) : base(TaskType.InitializeTask, name)
        {

        }

        protected override async Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            var res = await QuerySqlCmd<string>("SELECT @@VERSION;");
            var createExtensions = App.GetResourceFileText("osm2mssql.Importer.SQL.CreateDbExtension.sql");
            var file = Directory.GetCurrentDirectory() + @"\osm2mssql.OsmDb.dll";
           

            var buffer = File.ReadAllBytes(file);
            var data = "0x" + string.Join("", buffer.Select(x => x.ToString("X2")));
            createExtensions = createExtensions.Replace("[OSM]", Connection.InitialCatalog);
            createExtensions = createExtensions.Replace("[DllExtension]", data);
            ExecuteSqlCmd(createExtensions);
        }
    }
}
