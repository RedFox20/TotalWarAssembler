using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace TotalWarAssembler
{
    public class TreeSearchBox : TextBox
    {
        //static TreeSearchBox()
        //{
        //    DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeSearchBox), new FrameworkPropertyMetadata(typeof(TreeSearchBox)));
        //}

        public TreeView Tree;

        public TreeSearchBox()
        {
            MaxLines = 1;
            Text = "Search...";
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            if (Text == "Search...")
                Text = "";
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (Text == "")
                Text = "Search...";
            base.OnLostFocus(e);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (Tree != null)
            {
                FilterTree(Tree, Text == "Search..." ? "" : Text);
            }
            base.OnTextChanged(e);
        }

        private static void FilterTree(TreeView tree, string text)
        {
            if (tree.Items.Count == 0)
                return;

            bool FilterRecursive(TreeViewItem item)
            {
                bool visible = ((string) item.Header).Contains(text);
                foreach (object subitem in item.Items)
                {
                    if (subitem is TreeViewItem treeItem)
                        visible |= FilterRecursive(treeItem);
                }
                item.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                return visible;
            }

            FilterRecursive(tree.Items[0] as TreeViewItem);
        }
    }
}
