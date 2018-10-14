using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TotalWarAssembler
{
    public class DataGridSearchBox : TextBox
    {
        public DataTableGrid Grid;

        public DataGridSearchBox()
        {
            Text = "Filter...";
            MaxLines = 1;
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Text == "Filter...")
                Text = "";
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Text == "")
                Text = "Filter...";
            base.OnLostFocus(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (Grid != null)
            {
                FilterGrid(Text == "Filter..." ? "" : Text);
            }
            base.OnTextChanged(e);
        }

        private void FilterGrid(string text)
        {
            DataTable table = Grid.Table;

            if (text == "")
            {
                foreach (DataRow row in table.Rows)
                {
                }
                return;
            }

            foreach (DataRow row in table.Rows)
            {
                
            }
            Console.WriteLine();
        }

        private static bool RowMatchesText(DataRow row, string text)
        {
            foreach (object column in row.ItemArray)
                if (column is string s && s.Contains(text))
                    return true;
            return false;
        }
    }
}
