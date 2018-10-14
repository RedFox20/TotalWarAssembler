using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace TotalWarAssembler
{
    public class DataTableGrid : DataGrid
    {
        //static DataTableGrid()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(DataTableGrid), new FrameworkPropertyMetadata(typeof(DataTableGrid)));
        //}

        private readonly DataTable TableRepresentation = new System.Data.DataTable();
        public DataTable Table => TableRepresentation;

        public DataTableGrid()
        {
            CanUserSortColumns = true;
            CanUserAddRows = false;
            CanUserResizeRows = false;
            AutoGenerateColumns = false;
            EnableColumnVirtualization = false;
            EnableRowVirtualization = true;
        }

        public void LoadTable(AssemblyKit.DataTable table)
        {
            Columns.Clear();
            TableRepresentation.Clear();
            TableRepresentation.Columns.Clear();

            AssemblyKit.Schema schema = table.Schema;
            foreach (AssemblyKit.FieldDescr descr in schema.Fields)
            {
                Columns.Add(CreateTextBoxColumn(descr.Name));
                var dataColumn = new DataColumn(descr.Name);
                TableRepresentation.Columns.Add(dataColumn);
            }

            foreach (AssemblyKit.DataRow row in table.Rows)
            {
                System.Data.DataRow dr = TableRepresentation.NewRow();
                for (int i = 0; i < schema.Fields.Length; ++i)
                {
                    dr[i] = row.Columns[i];
                }

                TableRepresentation.Rows.Add(dr);
            }

            DataContext = TableRepresentation.DefaultView;
        }

        private DataGridTemplateColumn CreateTextBoxColumn(string name)
        {
            //create a template column
            var templateColumn = new DataGridTemplateColumn {Header = name};
            //set title of column
            //non editing cell template.. will be used for viweing data
            var textBlockTemplate = new DataTemplate();
            var textBlockElement = new FrameworkElementFactory(typeof(TextBlock));
            var textBlockBinding = new Binding(name);
            textBlockElement.SetBinding(TextBlock.TextProperty, textBlockBinding);
            textBlockTemplate.VisualTree = textBlockElement;
            templateColumn.CellTemplate = textBlockTemplate;
             
            //editing cell template ... will be used when user will edit the data
            var textBoxTemplate = new DataTemplate();
            var textboxElement = new FrameworkElementFactory(typeof(TextBox));
            var textboxBinding = new Binding(name);
            textboxElement.SetBinding(TextBox.TextProperty, textboxBinding);
            textBoxTemplate.VisualTree = textboxElement;
            templateColumn.CellEditingTemplate = textBoxTemplate;
            return templateColumn;
        }

        private DataGridTemplateColumn CreateComboBoxColumn(string name)
        {
            //create a template column
            var templateColumn = new DataGridTemplateColumn {Header = name};
            //set title of column
            //non editing cell template.. will be used for viweing data
            var textBlockTemplate = new DataTemplate();
            var textBlockElement = new FrameworkElementFactory(typeof(TextBlock));
            var textBlockBinding = new Binding(name);
            textBlockElement.SetBinding(TextBlock.TextProperty, textBlockBinding);
            textBlockTemplate.VisualTree = textBlockElement;
            templateColumn.CellTemplate = textBlockTemplate;
          
            //editing cell template ... will be used when user will edit the data
            var comboboxTemplate = new DataTemplate();
            var comboboxElement = new FrameworkElementFactory(typeof(ComboBox));
            var comboboxBinding = new Binding(name);
            comboboxElement.SetBinding(ComboBox.TextProperty, comboboxBinding);
            
            //combo box will show these options to select from
            comboboxElement.SetValue(ComboBox.ItemsSourceProperty, new List<string> { "Value1", "Value2" ,"Value3", "Value4" });
            comboboxTemplate.VisualTree = comboboxElement;
            templateColumn.CellEditingTemplate = comboboxTemplate;
            return templateColumn;
        }
    }
}
