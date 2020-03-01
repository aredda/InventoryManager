using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Inventory_Management.Utilities
{
    public static class Binder
    {
        public static void bind(Selector control, IEnumerable data, string displayMember = null, string valueMember = null)
        {
            control.ItemsSource = data;
            control.DisplayMemberPath = displayMember;
            control.SelectedValuePath = valueMember;
            
            control.Items.Refresh();
        }

        public static string purifyString(string target)
        {
            return target.ToLower().Trim();
        }

        public static void allow_numeric_only(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.,]+");

            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
