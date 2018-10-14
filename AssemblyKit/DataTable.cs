using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyKit
{
    public class DataColumn
    {
        public FieldDescr Id;
        public object Value;

        public override string ToString() => $"{Id} | {Value}";
    }

    public class DataRow
    {
        public DataColumn[] Columns { get; }
        private readonly Dictionary<string, DataColumn> Unordered;

        public DataRow(Schema schema)
        {
            int count = schema.Fields.Count;
            Columns = new DataColumn[count];
            Unordered = new Dictionary<string, DataColumn>(count);

            for (int i = 0; i < count; ++i)
            {
                FieldDescr descr = schema.Fields[i];
                var column = new DataColumn
                {
                    Id = descr,
                    Value = descr.DefaultValue
                };
                Columns[i] = column;
                Unordered.Add(descr.Name, column);
            }
        }

        public void Set(string columnId, string value)
        {
            DataColumn column = Unordered[columnId];
            column.Value = column.Id.ParseValue(value);
        }
    }

    public partial class DataTable
    {
        public DataDirectory Parent { get; }
        public Schema Schema { get; }
        public string Name { get; }
        public string RelativePath { get; }

        public List<DataRow> Entries { get; private set; }

        public override string ToString() => $"table {RelativePath}";

        public DataTable(DataDirectory parent, Schema schema)
        {
            Parent = parent;
            Schema = schema;
            Name = schema.Name;
            RelativePath = Path.Combine(parent.RelativePath, Name);
        }

        public void LoadXml(string xmlFile)
        {
            Entries = new List<DataRow>();
            using (var parser = new XmlParser(xmlFile))
            {
                // @note This is performance critical,
                //       so a custom XmlReader is the best option
                while (parser.ReadToElement(Name))
                {
                    var row = new DataRow(Schema);
                    Entries.Add(row);

                    int depth = parser.Depth;
                    while (parser.ReadToNextElement(depth))
                    {
                        string name = parser.Name;
                        string value = parser.ReadValue(parser.Depth);
                        row.Set(name, value);
                    }
                }
            }
        }
    }
}
