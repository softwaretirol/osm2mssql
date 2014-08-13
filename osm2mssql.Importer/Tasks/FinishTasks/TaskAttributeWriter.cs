using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Documents;
using osm2mssql.Importer.OpenStreetMapTypes;
using osm2mssql.Importer.OsmReader;
using osm2mssql.Importer.Tasks.ParallelFinishTask;
using osm2mssql.Importer.Tasks.ParallelTask;

namespace osm2mssql.Importer.Tasks.FinishTasks
{
    public class TaskAttributeWriter : TaskBulkInsertBase
    {
        public TaskAttributeWriter(string name)
            : base(TaskType.FinishTask, name)
        {
        }

        protected override Task DoTaskWork(string osmFile, AttributeRegistry attributeRegistry)
        {
            var runningTasks = new List<Task>();
            var tagTypes = attributeRegistry.GetAttributeValues(OsmAttribute.TagType);
            var memberRoles = attributeRegistry.GetAttributeValues(OsmAttribute.MemberRole);

            var memberTypes = attributeRegistry.GetAttributeValues(OsmAttribute.MemberType);

            var tagTypeDataTable = new DataTable("TagType");
            tagTypeDataTable.Columns.Add("Typ", typeof(int));
            tagTypeDataTable.Columns.Add("Name", typeof(string));
            foreach (var tagType in tagTypes)
                tagTypeDataTable.Rows.Add(tagType.Key, tagType.Value);
            runningTasks.Add(WriteToDbAsync(tagTypeDataTable.TableName, tagTypeDataTable));

            var memberRoleDataTable = new DataTable("MemberRole");
            memberRoleDataTable.Columns.Add("id", typeof(int));
            memberRoleDataTable.Columns.Add("name", typeof(string));
            foreach (var tagType in memberRoles)
                memberRoleDataTable.Rows.Add(tagType.Key, tagType.Value);
            runningTasks.Add(WriteToDbAsync(memberRoleDataTable.TableName, memberRoleDataTable));

            var memberTypesDataTable = new DataTable("MemberType");
            memberTypesDataTable.Columns.Add("id", typeof(int));
            memberTypesDataTable.Columns.Add("name", typeof(string));
            foreach (var tagType in memberTypes)
                memberTypesDataTable.Rows.Add(tagType.Key, tagType.Value);
            runningTasks.Add(WriteToDbAsync(memberTypesDataTable.TableName, memberTypesDataTable));

            return Task.WhenAll(runningTasks.ToArray());
        }
    }
}
