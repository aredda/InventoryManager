using Inventory_Management.Models;
using Inventory_Management.Utilities;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Inventory_Management.Windows
{
    /// <summary>
    /// Interaction logic for StoreWindow.xaml
    /// </summary>
    public partial class StoreWindow 
        : Window, IDependent
    {
        delegate List<Product> Filter(List<Product> products, object criteria);

        private Database database;

        // Filtering system
        private Dictionary<CheckBox, Control> assoc_enable;
        private Dictionary<CheckBox, Filter> assoc_filter;

        public StoreWindow()
        {
            InitializeComponent();

            this.Loaded += StoreWindow_Loaded;
        }

        public StoreWindow(Database database)
            : this()
        {
            this.database = database;
        }

        // Defining IDependent methods
        public void setDatabase(Database database)
        {
            this.database = database;
        }
        public Database getDatabase()
        {
            return this.database;
        }
        public void refresh()
        {
            // Deactivate some checkboxes
            foreach (KeyValuePair<CheckBox, Control> row in assoc_enable)
                row.Key.IsChecked = row.Value.IsEnabled = false;

            // Refreshing
            Binder.bind(cb_category, database.categories, "Name", "Number");
            Binder.bind(dg_products, database.products);
        }

        #region Filtering delegates

        private List<Product> filterByLabel(List<Product> products, object label)
        {
            return products.Where(p => p.Label.ToLower().Contains(Binder.purifyString(label.ToString()))).ToList();
        }

        private List<Product> filterByCode(List<Product> products, object code)
        {
            return products.Where(p => p.Code.ToLower().Contains(Binder.purifyString(code.ToString()))).ToList();
        }

        private List<Product> filterByCategory(List<Product> products, object category)
        {
            return products.Where(p => p.Category.Equals(category)).ToList();
        }

        private List<Product> orderBy(List<Product> products, object direction)
        {
            if (direction.ToString().Contains(Message.ORDER_ASC))
                return products.OrderBy(p => p.Quantity).ToList();

            return products.OrderByDescending(p => p.Quantity).ToList();
        }

        #endregion

        void StoreWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // CheckBoxes with their respective controls
                assoc_enable = new Dictionary<CheckBox, Control>();
                assoc_enable.Add(chb_code, txt_code);
                assoc_enable.Add(chb_name, txt_name);
                assoc_enable.Add(chb_category, cb_category);
                assoc_enable.Add(chb_order, cb_order);

                foreach (KeyValuePair<CheckBox, Control> row in assoc_enable)
                {
                    // Cofigure check event
                    row.Key.Click += delegate(object s, RoutedEventArgs se) 
                    {
                        row.Value.IsEnabled = row.Key.IsChecked.Value;
                    };
                }

                // Preparing delegates
                assoc_filter = new Dictionary<CheckBox, Filter>();
                assoc_filter.Add(chb_code, new Filter(filterByCode));
                assoc_filter.Add(chb_name, new Filter(filterByLabel));
                assoc_filter.Add(chb_category, new Filter(filterByCategory));
                assoc_filter.Add(chb_order, new Filter(orderBy));

                // Configuring Events
                btn_add.MouseDown += btn_add_MouseDown;
                btn_reset.MouseDown += btn_reset_MouseDown;
                btn_search.MouseDown += btn_search_MouseDown;
                btn_edit.MouseDown += btn_edit_MouseDown;

                // Reset
                refresh();
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_edit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (dg_products.SelectedItem == null)
                    throw new Exception(Message.Exceptions.SELECTED_NONE);

                // Get selected product
                Product p = dg_products.SelectedItem as Product;

                // Open "ProductWindow" but in edit mode
                ProductWindow editWindow = new ProductWindow(p);

                Linker.run(editWindow);
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_search_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                // Prepare the original list
                List<Product> list = database.products;

                // Foreach enabled filter, invoke the filtering delegate
                foreach (KeyValuePair<CheckBox, Control> row in assoc_enable)
                    if (row.Key.IsChecked.Value)
                        list = assoc_filter[row.Key] (list, (row.Value is TextBox) ? ((TextBox) row.Value).Text : ((ComboBox) row.Value).SelectedItem);

                // Display filtering result
                Binder.bind(dg_products, list);
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_reset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            refresh();
        }

        void btn_add_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Open "ProductWindow"
            Linker.run(new ProductWindow());
        }
    }
}
