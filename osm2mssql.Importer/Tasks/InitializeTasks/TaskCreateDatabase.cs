using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.InitializeTasks
{
    public class TaskCreateDatabase : TaskBase
    {
        private string _database;
        private string _osmFile;
        public TaskCreateDatabase(string name) : base(TaskType.InitializeTask, name)
        {
        }

        protected override async Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            _database = Connection.InitialCatalog;
            _osmFile = osmFile;
            Connection.InitialCatalog = string.Empty;

            CreateDatabase();
            CreateTables();

            Connection.InitialCatalog = _database;
        }

        internal void CreateDatabase()
        {
            StepDescription = "Creating database...";
            var createSql = App.GetResourceFileText("osm2mssql.Importer.SQL.CreateDatabase.sql");
            createSql = createSql.Replace("[OSM]", _database);
            ExecuteSqlCmd(createSql);            
        }

        internal void CreateTables()
        {
            var createTables = App.GetResourceFileText("osm2mssql.Importer.SQL.CreateTables.sql");
            StepDescription = "Creating tables...";
            createTables = createTables.Replace("[OSM]", _database);
            ExecuteSqlCmd("ALTER DATABASE " + _database + " SET trustworthy ON");
            ExecuteSqlCmd(createTables);
        }
    }
}
