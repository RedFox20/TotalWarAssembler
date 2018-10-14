using System;
using System.Collections.Generic;
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
using AssemblyKit;

namespace TotalWarAssembler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game Game;

        public MainWindow()
        {
            InitializeComponent();

            Game = new Game("Attila", @"\Games\steamapps\common\Total War Attila\");
            Game.LoadVanillaSchemas();
            Game.LoadMod();

            VanillaSearch.Tree = VanillaTree;
            ModSearch.Tree = ModTree;

            InitializeTree(VanillaTree, $"raw_data ({Game.Name})", Game.Data);
            InitializeTree(ModTree, "New Mod*", Game.Mod);
        }

        private static void InitializeTree(TreeView tree, string title, DataDirectory data)
        {
            TreeViewItem root = CreateTree(title, data);

            root.IsExpanded = true;
            if (root.Items[0] is TreeViewItem db)
                db.IsExpanded = true;

            tree.Items.Add(root);
        }

        private static TreeViewItem CreateTree(string title, DataDirectory dir)
        {
            var tree = new TreeViewItem
            {
                Header = title,
                DataContext = dir,
            };

            foreach (DataDirectory subdir in dir.Subdirs)
            {
                tree.Items.Add(CreateTree(subdir.Name, subdir));
            }

            foreach (DataTable table in dir.Tables)
            {
                tree.Items.Add(new TreeViewItem
                {
                    Header = table.Name,
                    DataContext = table,
                });
            }

            return tree;
        }

        private void VanillaTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            HandleTreeSelection(VanillaTree);
        }

        private void ModTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            HandleTreeSelection(ModTree);
        }

        private void HandleTreeSelection(TreeView tree)
        {
            var item = tree.SelectedItem as TreeViewItem;
            if (item?.DataContext is DataDirectory)
            {
                item.IsExpanded = true;
            }
            else if (item?.DataContext is DataTable table)
            {
                DataGrid.LoadTable(table);
            }
        }
    }
}
