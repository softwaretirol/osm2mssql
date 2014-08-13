using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.ParallelTask
{
    public class TaskRelationReader : TaskBulkInsertBase
    {
        private double _countOfInsertedRelations;
        public TaskRelationReader(string name) : base(TaskType.ParallelTask, name)
        {
        }

        internal override void DurationRefresh()
        {
            base.DurationRefresh();
            AdditionalInfos = (int)(_countOfInsertedRelations / Duration.TotalSeconds) + " Relations / sec.";
        }

        protected override Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            var reader = osmFile.EndsWith(".pbf") ?
                                                    (IOsmReader)new PbfOsmReader() :
                                                    (IOsmReader)new XmlOsmReader();

            ExecuteSqlCmd("TRUNCATE TABLE [tRelationCreation]");
            ExecuteSqlCmd("TRUNCATE TABLE [tRelationTag]");

            var dtRelationCreation = new DataTable { MinimumCapacity = MaxRowCountInMemory };
            dtRelationCreation.TableName = "tRelationCreation";
            dtRelationCreation.Columns.Add("relationId", typeof(long));
            dtRelationCreation.Columns.Add("ref", typeof(long));
            dtRelationCreation.Columns.Add("type");
            dtRelationCreation.Columns.Add("role");
            dtRelationCreation.Columns.Add("sort");

            var dtRelationTags = new DataTable { MinimumCapacity = MaxRowCountInMemory };
            dtRelationTags.TableName = "tRelationTag";
            dtRelationTags.Columns.Add("relationId", typeof(long));
            dtRelationTags.Columns.Add("Typ");
            dtRelationTags.Columns.Add("Info");

            var insertingTask = Task.Factory.StartNew(() => StartInserting());

            foreach (var relation in reader.ReadRelations(osmFile, attributeRegistry))
            {
                _countOfInsertedRelations++;
                var sort = 0;
                foreach (var member in relation.Members)
                {
                    dtRelationCreation = AddToCollection(dtRelationCreation, relation.RelationId, member.Ref, member.Type, member.Role, sort);
                    sort += 100000;
                }

                foreach (var tag in relation.Tags)
                    dtRelationTags = AddToCollection(dtRelationTags, relation.RelationId, tag.Typ, tag.Value);
            }

            DataTableCollection.Add(dtRelationCreation);
            DataTableCollection.Add(dtRelationTags);
            DataTableCollection.CompleteAdding();

            Trace.WriteLine(string.Format("Inserted {0} relations", _countOfInsertedRelations));
            return insertingTask.Result;
        }
    }
}
