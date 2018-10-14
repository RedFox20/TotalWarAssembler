using System;
using System.IO;
using System.Xml;

namespace AssemblyKit
{
    public partial class DataTable
    {
        private class XmlParser : IDisposable
        {
            private readonly XmlReader Reader;
            public int Depth;
            public string Name => Reader.Name;
            public override string ToString() => $"{Reader.NodeType}  {Reader.Name}  {Reader.Value}";

            public XmlParser(string xmlFile)
            {
                var stream = new MemoryStream(File.ReadAllBytes(xmlFile));
                Reader = XmlReader.Create(stream);
            }
            public void Dispose()
            {
                Reader?.Dispose();
            }
            public bool ReadToElement(string name)
            {
                while (Reader.Read()) switch (Reader.NodeType)
                {
                    case XmlNodeType.Element:
                        ++Depth;
                        if (Reader.Name == name)
                            return true;
                        break;
                    case XmlNodeType.EndElement:
                        --Depth;
                        break;
                }
                return false;
            }

            public bool ReadToNextElement(int depth)
            {
                while (Reader.Read()) switch (Reader.NodeType)
                {
                    case XmlNodeType.Element:
                        ++Depth;
                        return true;
                    case XmlNodeType.EndElement:
                        --Depth;
                        if (Depth < depth)
                            return false;
                        break;
                }
                return false;
            }
            public string ReadValue(int depth)
            {
                while (Reader.Read())
                {
                    switch (Reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            ++Depth;
                            break;
                        case XmlNodeType.Text:
                            return Reader.Value;
                        case XmlNodeType.EndElement:
                            --Depth;
                            if (Depth < depth)
                                return "";
                            break;
                    }
                }
                return null;
            }
        }
    }
}