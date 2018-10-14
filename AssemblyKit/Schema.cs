﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AssemblyKit
{
    public enum FieldType
    {
        Unknown,
        Bool,
        Int32,
        Int64,
        Single,
        Double,
        Decimal,
        Text,
        AutoNumber,
    }

    public class FieldDescr
    {
        public string Uuid { get; }
        public string Name { get; }
        public FieldType Type { get; }
        public bool PrimaryKey { get; }
        public bool Required { get; }
        public int MaxLength { get; }
        public object DefaultValue { get; }
        public string FieldDescription { get; }
        public bool EncylopediaExport { get; }
        public bool IsFileName { get; }

        private static readonly Dictionary<string, string> EmptyDict = new Dictionary<string, string>();

        private Dictionary<string, string> SchemaStrings;
        public IReadOnlyDictionary<string, string> Strings => SchemaStrings;

        public override string ToString() => $"{Name}:{Type}";

        public FieldDescr(XmlNode field)
        {
            for (XmlNode e = field.FirstChild; e != null; e = e?.NextSibling)
            {
                XmlNode valueNode = e.FirstChild;
                if (valueNode == null)
                    continue;
                string value = valueNode.Value;
                switch (e.Name)
                {
                    case "field_uuid":  Uuid = value; break;
                    case "field_type":  Type = ParseType(value); break;
                    case "name":        Name = value; break;
                    case "primary_key": PrimaryKey = ParseBool(value); break;
                    case "required":    Required   = ParseBool(value); break;
                    case "max_length":  MaxLength  = int.Parse(value); break;
                    case "default_value":          DefaultValue       = ParseValue(value); break;
                    case "field_description":      FieldDescription   = value; break;
                    case "encyclopaedia_export":   EncylopediaExport  = ParseBool(value); break;
                    case "is_filename":            IsFileName         = ParseBool(value); break;
                    case "column_source_column":   AddString(e.Name, value); break;
                    case "column_source_table":    AddString(e.Name, value); break;
                    case "filename_relative_path": AddString(e.Name, value); break;
                    case "fragment_path":          AddString(e.Name, value); break;
                    case "table_name":             AddString(e.Name, value); break;
                    case "foreign_table_name":     AddString(e.Name, value); break;
                    case "foreign_column_name":    AddString(e.Name, value); break;
                    case "column_source_fixed_value": AddString(e.Name, value); break;
                    default:
                        Console.WriteLine($"Unknown schema field: {e.OuterXml} ");
                        break;
                }
            }
            if (SchemaStrings == null)
                SchemaStrings = EmptyDict;
            if (DefaultValue == null)
                DefaultValue = CreateDummyDefaultValue();
        }

        private void AddString(string name, string value)
        {
            if (SchemaStrings == null)
                SchemaStrings = new Dictionary<string, string>();

            if (SchemaStrings.TryGetValue(name, out string existingValue))
                SchemaStrings[name] = existingValue + ":" + name;
            else
                SchemaStrings.Add(name, value);
        }

        private static bool ParseBool(string boolean)
        {
            return boolean == "1" || boolean == "true";
        }

        private static FieldType ParseType(string type)
        {
            switch (type)
            {
                case "integer": return FieldType.Int32;
                case "longinteger": return FieldType.Int64;
                case "yesno": return FieldType.Bool;
                case "text": return FieldType.Text;
                case "single": return FieldType.Single;
                case "double": return FieldType.Double;
                case "decimal": return FieldType.Decimal;
                case "autonumber": return FieldType.AutoNumber;
            }
            Console.WriteLine($"Unknown schema field type: {type}");
            return FieldType.Unknown;
        }

        public object ParseValue(string value)
        {
            switch (Type)
            {
                default:
                case FieldType.Unknown: return value;
                case FieldType.Bool:    return ParseBool(value);
                case FieldType.Int32:   return value == "" ? 0    : int.Parse(value);
                case FieldType.Int64:   return value == "" ? 0L   : long.Parse(value);
                case FieldType.Single:  return value == "" ? 0.0f : float.Parse(value, CultureInfo.InvariantCulture);
                case FieldType.Double:  return value == "" ? 0.0  : double.Parse(value, CultureInfo.InvariantCulture);
                case FieldType.Decimal: return value == "" ? 0.0m : decimal.Parse(value, CultureInfo.InvariantCulture);
                case FieldType.Text:    return value;
                case FieldType.AutoNumber: return long.Parse(value);
            }
        }

        private object CreateDummyDefaultValue()
        {
            switch (Type)
            {
                default:
                case FieldType.Unknown: return null;
                case FieldType.Bool: return false;
                case FieldType.Int32: return 0;
                case FieldType.Int64: return 0L;
                case FieldType.Single: return 0.0f;
                case FieldType.Double: return 0.0;
                case FieldType.Decimal: return decimal.Zero;
                case FieldType.Text: return "";
                case FieldType.AutoNumber: return 0;
            }
        }
    }

    public class Schema
    {
        public string Name { get; }
        public List<FieldDescr> Fields { get; } = new List<FieldDescr>();
        
        public Schema(string name, string schemaFile)
        {
            Name = name;
            using (XmlReader reader = XmlReader.Create(schemaFile, new XmlReaderSettings()
            {
                DtdProcessing = DtdProcessing.Parse,
            }))
            {
                var doc = new XmlDocument();
                doc.Load(reader);

                XmlNode root = doc.LastChild;
                Debug.Assert(root.Name == "root");

                XmlNode field = root.FirstChild;
                while ((field = field?.NextSibling) != null)
                {
                    Fields.Add(new FieldDescr(field));
                }
            }
        }
    }
}