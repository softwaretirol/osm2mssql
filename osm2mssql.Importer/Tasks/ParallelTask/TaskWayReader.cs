using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.ParallelTask
{
    public class TaskWayReader : TaskBulkInsertBase
    {
        private int _countOfInsertedWays;
        private double? _timeOffset = null;
        public TaskWayReader(string name) : base(TaskType.ParallelTask, name)
        {
        }

        internal override void DurationRefresh()
        {
            base.DurationRefresh();
            if (_timeOffset.HasValue)
                AdditionalInfos = (int)(_countOfInsertedWays / (Duration.TotalSeconds - _timeOffset)) + " Ways / sec.";
        }

        protected override Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            var watch = Stopwatch.StartNew();
            ExecuteSqlCmd("TRUNCATE TABLE tWayCreation");
            ExecuteSqlCmd("TRUNCATE TABLE tWayTag");

            var dtWays = new DataTable();
            dtWays.TableName = "tWayCreation";
            dtWays.MinimumCapacity = MaxRowCountInMemory;
            dtWays.Columns.Add("wayId", typeof(long));
            dtWays.Columns.Add("nodeId", typeof(long));
            dtWays.Columns.Add("sort");

            var dtWayTags = new DataTable();
            dtWayTags.TableName = "tWayTag";
            dtWayTags.MinimumCapacity = MaxRowCountInMemory;
            dtWayTags.Columns.Add("WayId", typeof(long));
            dtWayTags.Columns.Add("Typ", typeof(int));
            dtWayTags.Columns.Add("Info", typeof(string));

            var insertingTask = Task.Factory.StartNew(() => StartInserting());
            var reader = osmFile.EndsWith(".pbf") ?
                                                    (IOsmReader)new PbfOsmReader() :
                                                    (IOsmReader)new XmlOsmReader();

            foreach (var way in reader.ReadWays(osmFile, attributeRegistry))
            {
                if (!_timeOffset.HasValue)
                {
                    watch.Stop();
                    _timeOffset = watch.Elapsed.TotalSeconds;
                }
                _countOfInsertedWays++;
                var sort = 0;

                foreach (var node in way.NodeRefs)
                    dtWays = AddToCollection(dtWays, way.WayId, node, sort++);

                foreach (var tag in way.Tags)
                    dtWayTags = AddToCollection(dtWayTags, way.WayId, tag.Typ, tag.Value);
            }

            DataTableCollection.Add(dtWays);
            DataTableCollection.Add(dtWayTags);
            DataTableCollection.CompleteAdding();

            Trace.WriteLine(string.Format("Inserted {0} ways", _countOfInsertedWays));
            return insertingTask.Result;
        }
    }
}
