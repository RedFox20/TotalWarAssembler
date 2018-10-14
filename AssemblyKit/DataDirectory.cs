using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyKit
{
    public class DataDirectory
    {
        public DataDirectory Parent { get; }
        public string Name { get; }
        public List<DataDirectory> Subdirs { get; } = new List<DataDirectory>();
        public List<DataTable> Tables { get; } = new List<DataTable>();
        public bool ReadOnly { get; }

        /// <summary>
        /// Relative path, eg: "db"
        /// </summary>
        public string RelativePath { get; }

        public override string ToString() => $"dir {RelativePath}";

        public DataDirectory(DataDirectory parent, string name, bool readOnly)
        {
            Name = name;
            Parent = parent;
            RelativePath = parent == null ? "" : Path.Combine(parent.RelativePath, Name);
            parent?.Subdirs.Add(this);
            ReadOnly = readOnly;
        }
    }
}
