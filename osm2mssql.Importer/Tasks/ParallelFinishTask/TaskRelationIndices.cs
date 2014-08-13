using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;

namespace osm2mssql.Importer.Tasks.ParallelFinishTask
{
    class TaskRelationIndices : TaskBase
    {
        public TaskRelationIndices(string name)
            : base(TaskType.ParallelFinishTask, name)
        {

        }

        protected override async Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            ExecuteSqlCmd("ALTER TABLE Relation ADD CONSTRAINT PK_Relation PRIMARY KEY CLUSTERED (id, role) " +
                          "WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY];");

            ExecuteSqlCmd("ALTER TABLE [RelationCreation] ADD CONSTRAINT PK_RelationCreation PRIMARY KEY CLUSTERED (relationId, sort) " +
                          "WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]");
        }
    }
}
