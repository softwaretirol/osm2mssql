using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Properties;
using osm2mssql.Importer.Tasks.ParallelFinishTask;

namespace osm2mssql.Importer.Tasks.ParallelTask
{
    public abstract class TaskBulkInsertBase : TaskBase
    {
        protected readonly BlockingCollection<DataTable> DataTableCollection = new BlockingCollection<DataTable>(Settings.Default.MaxPendingOperations);
        protected TaskBulkInsertBase(TaskType taskType, string name) : base(taskType, name)
        {
        }

        protected DataTable AddToCollection(DataTable loadingNodeTable, params object[] values)
        {
            loadingNodeTable.Rows.Add(values);

            if (loadingNodeTable.Rows.Count < MaxRowCountInMemory)
                return loadingNodeTable;

            var tmp = loadingNodeTable.Clone();
            DataTableCollection.Add(loadingNodeTable);
            loadingNodeTable = tmp;

            return loadingNodeTable;
        }

        protected async Task StartInserting()
        {
            var bulkTasks = new Task[Settings.Default.MaxPendingOperations];
            for (int i = 0; i < bulkTasks.Length; i++)
            {
                bulkTasks[i] = Task.Factory.StartNew(() =>
                {
                    foreach (var table in DataTableCollection.GetConsumingEnumerable())
                    {
                        WriteToDbAsync(table.TableName, table).Wait();
                        if (DataTableCollection.Count == 0 && DataTableCollection.IsAddingCompleted)
                            break;
                    }
                }, TaskCreationOptions.LongRunning);
            }

            await Task.WhenAll(bulkTasks);

        }

        protected async Task WriteToDbAsync(string tableName, DataTable dt)
        {
            var connBuilder = new SqlConnectionStringBuilder(Connection.ToString());
            var options = SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction; //Is Faster with TableLock - also with parallel Imports
            using (var s = new SqlBulkCopy(connBuilder.ToString(), options))
            {
                s.BulkCopyTimeout = int.MaxValue;
                s.BatchSize = 0;
                s.DestinationTableName = tableName;
                await s.WriteToServerAsync(dt);
                s.Close();
            }
        }
    }
}
