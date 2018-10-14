using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AssemblyKit
{
    public class Game
    {
        /// <summary>Eg: Attila</summary>
        public string Name { get; }

        /// <summary>Root folder of the game</summary>
        public string Root { get; }

        public string AssemblyKit { get; }
        public string RawData { get; }
        public DataDirectory Data { get; private set; }
        public DataDirectory Mod { get; private set; }

        public Game(string name, string root)
        {
            Name = name;
            Root = root;
            AssemblyKit = Path.Combine(Root, "assembly_kit");
            RawData = Path.Combine(AssemblyKit, "raw_data");
            if (!Directory.Exists(root))
                throw new DirectoryNotFoundException($"Directory does not exist: {Root}");
            if (!Directory.Exists(AssemblyKit))
                throw new DirectoryNotFoundException($"Directory does not exist: {AssemblyKit}. Please install {Name} Assembly Kit via Steam");
            if (!Directory.Exists(RawData))
                throw new DirectoryNotFoundException($"Directory does not exist: {RawData}. Missing raw_data XML's. Please verify {Name} Assembly Kit integrity via Steam");
        }

        public void LoadVanillaSchemas()
        {
            Data = new DataDirectory(null, "", readOnly:true);
            var db = new DataDirectory(Data, "db", readOnly:true);

            FileInfo[] schemas = new DirectoryInfo($"{RawData}\\db").GetFiles("TWaD_*.xml");

            foreach (FileInfo schemaFile in schemas)
            {
                string tableName = schemaFile.Name.Substring(5); // skip "TwaD_"
                if (tableName == "validation.xml") // ignore the index file
                    continue;
                
                string tableXmlPath = Path.Combine(RawData, "db", tableName);
                if (!File.Exists(tableXmlPath))
                    continue;

                var schema = new Schema(Path.GetFileNameWithoutExtension(tableName), schemaFile.FullName);
                var table = new DataTable(db, schema, tableXmlPath, readOnly:true);
            }
        }

        public void LoadMod()
        {
            Mod = new DataDirectory(null, "", readOnly:false);
            var db = new DataDirectory(Mod, "db", readOnly:false);

        }
    }
}
