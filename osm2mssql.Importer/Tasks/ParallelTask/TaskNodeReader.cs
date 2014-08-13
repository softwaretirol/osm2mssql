using System;
using System.Collections.Concurrent;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SqlServer.Types;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.ParallelTask
{
    public class TaskNodeReader : TaskBulkInsertBase
    {
        private int _countOfInsertedNodes;
        public TaskNodeReader(string name) : base(TaskType.ParallelTask, name)
        {
        }

        internal override void DurationRefresh()
        {
            base.DurationRefresh();
            AdditionalInfos = (int)(_countOfInsertedNodes / Duration.TotalSeconds) + " Nodes / sec.";
        }

        protected override Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            ExecuteSqlCmd("TRUNCATE TABLE [Node]");
            ExecuteSqlCmd("TRUNCATE TABLE [NodeTag]");

            var loadingNodeTable = new DataTable();
            loadingNodeTable.TableName = "Node";
            loadingNodeTable.MinimumCapacity = MaxRowCountInMemory;
            loadingNodeTable.Columns.Add("NodeId", typeof(long));
            loadingNodeTable.Columns.Add("location", typeof(SqlGeography));
            loadingNodeTable.Columns.Add("Latitude", typeof(double));
            loadingNodeTable.Columns.Add("Longitude", typeof(double));

            var dNodeTags = new DataTable();
            dNodeTags.TableName = "NodeTag";
            dNodeTags.MinimumCapacity = MaxRowCountInMemory;
            dNodeTags.Columns.Add("NodeId", typeof(long));
            dNodeTags.Columns.Add("Typ");
            dNodeTags.Columns.Add("Info");

            IOsmReader reader = osmFile.EndsWith(".pbf") ? (IOsmReader)new PbfOsmReader() : (IOsmReader)new XmlOsmReader();

            var insertingTask = Task.Factory.StartNew(() => StartInserting());

            foreach (var node in reader.ReadNodes(osmFile, attributeRegistry))
            {
                _countOfInsertedNodes++;

                loadingNodeTable = AddToCollection(loadingNodeTable, node.NodeId, node.ToSqlGeographyPoint(), node.Latitude, node.Longitude);
                foreach (var tag in node.Tags)
                {
                    dNodeTags = AddToCollection(dNodeTags, node.NodeId, tag.Typ, tag.Value);
                }
            }

            DataTableCollection.Add(loadingNodeTable);
            DataTableCollection.Add(dNodeTags);
            DataTableCollection.CompleteAdding();

            Trace.WriteLine(string.Format("Inserted {0} nodes", _countOfInsertedNodes));
            return insertingTask.Result;
        }
    }
}
