using Inventory_Management.Models;
using Inventory_Management.Utilities;
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
using System.Windows.Shapes;

namespace Inventory_Management.Windows
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window, IDependent
    {
        private Database database;

        public NotificationWindow()
        {
            InitializeComponent();

            this.Loaded += NotificationWindow_Loaded;
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
            // Refresh data
            Binder.bind(dg_products, database.products.Where(p => p.Quantity <= database.settings.notifyOn).ToList());
        }

        void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Numeric textbox
                txt_quantity.PreviewTextInput += Binder.allow_numeric_only;

                // Refill button
                btn_refill.MouseDown += btn_refill_MouseDown;

                // Load data
                refresh();
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

        void btn_refill_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (txt_quantity.Text.Length == 0)
                    throw new Exception(Message.Exceptions.FORM_INCOMPLETE);

                if (dg_products.SelectedItem == null)
                    throw new Exception(Message.Exceptions.SELECTED_NONE);

                // Get quantity
                int q = int.Parse(txt_quantity.Text);

                if (q == 0)
                    throw new Exception("لا يمكن لعدد التعبئة ان يكون صفرا");

                Product p = dg_products.SelectedItem as Product;

                // Add this number
                p.Quantity += q;

                // Archive it
                Order o = new Order { 
                    Direction=OrderDirection.Purchase,
                    Date=DateTime.Today
                };

                database.add(new OrderDetail {
                    Order=o,
                    Product=p,
                    Quantity=q,
                    Price=p.PriceIn
                });

                database.add(o);

                // Success
                Message.show(Message.SUCCESS_INSERT, MessageBoxImage.Information);

                // Refresh
                refresh();
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }
    }
}
