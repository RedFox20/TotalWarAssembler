using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyKit
{
    public class DataRow
    {
        public readonly object[] Columns;

        public DataRow(Schema schema)
        {
            int length = schema.Fields.Length;
            Columns = new object[length];

            for (int i = 0; i < length; ++i)
            {
                FieldDescr field = schema.Fields[i];
                Columns[i] = field.DefaultValue;
            }
        }

        public void Set(Schema schema, string columnId, string value)
        {
            int id = schema.IndexOf(columnId);
            FieldDescr field = schema.Fields[id];
            Columns[id] = field.ParseValue(value);
        }
    }

    public class DataTable
    {
        public DataDirectory Parent { get; }
        public Schema Schema { get; }
        public string Name   { get; }
        public string RelativePath  { get; }
        public bool ReadOnly { get; }

        // Load tables lazily, since total amount of Total War data is huge
        public List<DataRow> Rows
        {
            get
            {
                if (DataRows == null)
                {
                    DataRows = new List<DataRow>();
                    if (XmlSource != null)
                        LoadXml(Schema, XmlSource);
                }
                return DataRows;
            }
        }

        private List<DataRow> DataRows;
        private readonly string XmlSource;

        public override string ToString() => $"table {RelativePath}";

        public DataTable(DataDirectory parent, Schema schema, string xmlFile, bool readOnly)
        {
            Parent = parent;
            Schema = schema;
            Name = schema.Name;
            RelativePath = Path.Combine(parent.RelativePath, Name);
            XmlSource = xmlFile;
            ReadOnly = readOnly;
            parent.Tables.Add(this);
        }

        private void LoadXml(Schema schema, string xmlFile)
        {
            // @note This is performance critical,
            //       so a custom XmlReader is the best option
            using (var parser = new XmlParser(xmlFile))
            {
                while (parser.ReadToElement(Name))
                {
                    var row = new DataRow(Schema);
                    Rows.Add(row);

                    int depth = parser.Depth;
                    while (parser.ReadToNextElement(depth))
                    {
                        string name = parser.Name;
                        string value = parser.ReadValue(parser.Depth);
                        row.Set(schema, name, value);
                    }
                }
            }
        }
    }
}
