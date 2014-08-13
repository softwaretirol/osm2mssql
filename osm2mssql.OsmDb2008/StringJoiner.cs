using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Server;


namespace osm2mssql.DbExtensions
{
    [Serializable]
    [SqlUserDefinedAggregate(Format.UserDefined, IsInvariantToNulls = true, MaxByteSize = -1)]
    public struct StringJoiner : IBinarySerialize
    {
        public class Item
        {
            public int Sort { get; set; }
            public string Part { get; set; }

            public Item(int sort, string part)
            {
                Sort = sort;
                Part = part;
            }
        }

        private List<Item> _items;
        public void Init()
        {
            _items = new List<Item>();
        }

        [SqlMethod(OnNullCall = false)]
        public void Accumulate(SqlInt32 sort, SqlString part)
        {
            if (_items == null)
                _items = new List<Item>();
            _items.Add(new Item(sort.Value, part.Value));
        }
        public void Merge(StringJoiner @group)
        {
            if (_items != null)
            {
                _items.AddRange(@group._items);
            }
            else
            {
                _items = new List<Item>(@group._items);
            }
        }
        public SqlString Terminate()
        {
            try
            {
                _items.Sort(CompareSortedPoint);

                var sb = new StringBuilder();

                var firstItem = true;
                foreach (var item in _items)
                {
                    if (firstItem)
                        firstItem = false;
                    else
                        sb.Append(' ');

                    sb.Append(item.Part);
                }

                return new SqlString(sb.ToString());
            }
            catch
            {
                return null;
            }
        }

        int CompareSortedPoint(Item p1, Item p2)
        {
            return Comparer<int>.Default.Compare(p1.Sort, p2.Sort);
        }

        public void Read(System.IO.BinaryReader r)
        {
            _items = new List<Item>();
            try
            {
                while (true)
                {
                    if (r.BaseStream.Position == r.BaseStream.Length)
                        break;
                    var length = r.BaseStream.Length - r.BaseStream.Position;
                    if (length < sizeof(Int32))
                        break;
                    var sort = r.ReadInt32();
                    var part = r.ReadString();

                    _items.Add(new Item(sort, part));

                }
            }
            catch
            {

            }
        }
        public void Write(System.IO.BinaryWriter w)
        {
            try
            {
                foreach (var point in _items)
                {
                    w.Write(point.Sort);
                    w.Write(point.Part);
                }
            }
            catch
            {

            }
        }
    }
}
