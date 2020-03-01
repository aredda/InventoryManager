using Inventory_Management.Utilities;
using Inventory_Management.Windows;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace Inventory_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDependent
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private Database database;

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
            // Loading datagrids
            Binder.bind(dg_purchase, database.orderDetails.Where(d => d.Order.Direction == Models.OrderDirection.Purchase).ToList().OrderByDescending(d => d.Order.Date));
            Binder.bind(dg_sell, database.orderDetails.Where(d => d.Order.Direction == Models.OrderDirection.Sell).ToList().OrderByDescending(d => d.Order.Date));
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Configuring routing
                Dictionary<Control, Type> routes = new Dictionary<Control, Type>();

                routes.Add(btn_add_product, typeof(ProductWindow));
                routes.Add(btn_add_category, typeof(CategoryWindow));
                routes.Add(btn_sell, typeof(SellWindow));
                routes.Add(btn_store, typeof(StoreWindow));
                routes.Add(btn_history, typeof(HistoryWindow));

                foreach (KeyValuePair<Control, Type> item in routes)
                    item.Key.MouseDown += delegate(object s, MouseButtonEventArgs se) 
                    {
                        Linker.run((IDependent) Activator.CreateInstance(item.Value));
                    };

                // Initial data grid loading
                refresh();
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }

    }
}
