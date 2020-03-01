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

namespace Inventory_Management.Windows
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow 
        : Window, IDependent
    {
        private Database database;

        public HistoryWindow()
        {
            InitializeComponent();

            this.Loaded += HistoryWindow_Loaded;
        }
        public HistoryWindow(Database database)
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
        public void refresh(){}

        private List<OrderDetail> getDetails(DateTime start, DateTime end, OrderDirection direction)
        {
            return database.orderDetails.Where ( detail => 
                detail.Order.Date <= end && 
                detail.Order.Date >= start && 
                detail.Order.Direction == direction
            ).ToList();
        }

        private void reload(List<OrderDetail> details)
        {
            // Refresh the data grid
            Binder.bind(dg_details, details);

            // Refresh the counters
            lbl_product_counter.Text = details.GroupBy(detail => detail.Product).Count().ToString() + " منتوج";
            lbl_quantity_counter.Text = details.Sum(detail => detail.Quantity).ToString() + " وحدة";
            lbl_total.Text = details.Sum(detail => detail.Total).ToString() + " درهم";
        }

        void HistoryWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Configure event
                btn_search.MouseDown += btn_search_MouseDown;

                // Initial tweaks
                cb_direction.SelectedIndex = 1;
                dt_end.SelectedDate = DateTime.Today.AddDays(1);
                dt_start.SelectedDate = dt_end.SelectedDate.Value.AddDays(-1);

                // Refresh info
                btn_search_MouseDown(null, null);
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
                if (!dt_start.SelectedDate.HasValue || !dt_end.SelectedDate.HasValue)
                    throw new Exception("لم تقم بتحديد التواريخ");

                DateTime start = dt_start.SelectedDate.Value;
                DateTime end = dt_end.SelectedDate.Value;
                OrderDirection direction = cb_direction.SelectedIndex == 0 ? OrderDirection.Sell : OrderDirection.Purchase;

                // Refresh
                reload(getDetails(start, end, direction));
            }
            catch (Exception x)
            {
                Message.show(x.Message);
            }
        }
    }
}
